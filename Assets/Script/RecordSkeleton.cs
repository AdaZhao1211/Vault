using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RecordSkeleton : MonoBehaviour
{
    [SerializeField]
    private OVRHand hand;

    [SerializeField]
    private OVRSkeleton handSkeleton;

    [SerializeField]
    private GameObject bonePrefab;

    public OSC osc;

    private bool bonesAdded = false;

    [SerializeField]
    private SpatialAnchorsManager RecordingMode;


    public Vector3 IndexTipPos;

    public Vector3[] HandSkeletonPos;
    public Quaternion[] HandSkeletonQua;

    public Matrix4x4[] HandSkeletonM;



    private void Awake()
    {
        if (!hand) hand = GetComponent<OVRHand>();
        if (!handSkeleton) handSkeleton = GetComponent<OVRSkeleton>();   
    }


    private void Start()
    {
        HandSkeletonPos = new Vector3[24];
        HandSkeletonQua = new Quaternion[24];
        HandSkeletonM = new Matrix4x4[24];


    }


    // Update is called once per frame
    void Update()
    {
        // is recording
        if(RecordingMode.Mode == 1){
            // first save the anchor
            
            if(hand.IsTracked)
            {
                DisplayBoneInfo();
                if(!bonesAdded) CreateBones();
            }

        }

        // on end recording pressed
        
        
    }

    private void DisplayBoneInfo()
    {
        // and record
        string message = "";
        message += handSkeleton.GetSkeletonType();
        message += "  ";
        message += handSkeleton.GetCurrentNumBones();
        message += "  ";
        message += handSkeleton.GetCurrentStartBoneId();
        message += "\n";
        
        OscMessage _oscmessage;
        _oscmessage = new OscMessage();
        _oscmessage.address = "/location";
        _oscmessage.values.Add(System.DateTime.Now.ToString());


        // var theAcnhorMatrix = _recordingMode.GetComponent<SpatialAnchorsManager>().AnchorMatrix;
        // Vector3 anchorP = new Vector3(theAcnhorMatrix[0,3], theAcnhorMatrix[1,3], theAcnhorMatrix[2,3]);
        // Quaternion anchorR = ExtractRotation(theAcnhorMatrix);
        // var constructedAnchorTransform = new GameObject().transform;
        // // Transform constructedAnchorTransform;
        // constructedAnchorTransform.position = anchorP;
        // constructedAnchorTransform.rotation = anchorR;
        // constructedAnchorTransform.localScale = new Vector3(1, 1, 1);


        int i = 0;
        foreach (var bone in handSkeleton.Bones){
            
            
            // Vector3 relativePosition = bone.Transform.position - _recordingMode.GetComponent<SpatialAnchorsManager>().AnchorPosition;
            Vector3 relativePosition = RecordingMode.AnchorTransform.InverseTransformPoint(bone.Transform.position);
            Quaternion relativeQua = Quaternion.Inverse(RecordingMode.AnchorTransform.rotation) * bone.Transform.rotation;
            HandSkeletonPos[i] = relativePosition;
            HandSkeletonQua[i] = relativeQua;

            // Vector3 relativePosition = bone.Transform.position;
            // log bone position to file
            // _oscmessage.values.Add(relativePosition.ToString());
            _oscmessage.values.Add(relativePosition.ToString());
            if(i == 20){
                IndexTipPos = bone.Transform.position;
                // Debug.Log(HandSkeletonM[i]);
            }
            i ++;
            
        }
        // message += i.ToString();
        // message += "\n";
        // message += IndexTipPos.ToString();
        // message += "\n";
        // message += constructedAnchorTransform.position.ToString();
        // osc.Send(_oscmessage);
        // Debug.Log(message);
    }

    private void CreateBones()
    {
        int i = 0;
        foreach (var bone in handSkeleton.Bones)
        {
            Instantiate(bonePrefab, bone.Transform)
                .GetComponent<BoneInfo>()
                .AddBone(bone, i);

            i++;
        }

        bonesAdded = true;
    }


    private Quaternion ExtractRotation(Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;
 
        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;
 
        return Quaternion.LookRotation(forward, upwards);
    }

}
