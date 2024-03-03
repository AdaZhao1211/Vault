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
    private GameObject _recordingMode;


    private void Awake()
    {
        if (!hand) hand = GetComponent<OVRHand>();
        if (!handSkeleton) handSkeleton = GetComponent<OVRSkeleton>();   
    }


    private void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if(_recordingMode.GetComponent<SpatialAnchorsManager>().HandRecording){
            if(hand.IsTracked)
            {
                DisplayBoneInfo();
                if(!bonesAdded) CreateBones();
            }

        }
        

        if (OVRInput.GetUp(OVRInput.Button.One)){
            Debug.Log("end");
            OscMessage _oscmessage;
            _oscmessage = new OscMessage();
            _oscmessage.address = "/end";
            osc.Send(_oscmessage);
            _recordingMode.GetComponent<SpatialAnchorsManager>().HandRecording = false;
        }
        
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

        // int i = 0;
        foreach (var bone in handSkeleton.Bones){
            
            Vector3 relativePosition = bone.Transform.position - _recordingMode.GetComponent<SpatialAnchorsManager>().AnchorPosition;
            // log bone position to file
            // _oscmessage.values.Add(relativePosition.ToString());
            _oscmessage.values.Add(relativePosition.ToString());
            message += relativePosition.ToString();
            message += "\n";

            
        }
        osc.Send(_oscmessage);
        Debug.Log(message);
    }

    private void CreateBones()
    {
        foreach (var bone in handSkeleton.Bones)
        {
            Instantiate(bonePrefab, bone.Transform)
                .GetComponent<BoneInfo>()
                .AddBone(bone);
        }

        bonesAdded = true;
    }

}
