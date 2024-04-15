using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracking : MonoBehaviour
{
    public Matrix4x4 HeadMatrix;

    // to send out head transformation
    public Vector3 HeadPos;
    public Quaternion HeadQua;

    // access recording mode
    [SerializeField]
    private SpatialAnchorsManager RecordingMode;

    // private variable for internal use
    private bool _findAnchor = false;
    private Transform constructedAnchorTransform;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(RecordingMode.Mode == 1){
            // first save the anchor
            if (!_findAnchor){
                var theAcnhorMatrix = RecordingMode.AnchorMatrix;
                Vector3 anchorP = new Vector3(theAcnhorMatrix[0,3], theAcnhorMatrix[1,3], theAcnhorMatrix[2,3]);
                Quaternion anchorR = ExtractRotation(theAcnhorMatrix);
                constructedAnchorTransform = new GameObject().transform;
                // Transform constructedAnchorTransform;
                constructedAnchorTransform.position = anchorP;
                constructedAnchorTransform.rotation = anchorR;
                constructedAnchorTransform.localScale = new Vector3(1, 1, 1);
                _findAnchor = true;
            }

            HeadPos = RecordingMode.AnchorTransform.InverseTransformPoint(this.transform.position);

            HeadQua = Quaternion.Inverse(RecordingMode.AnchorTransform.rotation) * this.transform.rotation;

            
            // HeadMatrix = RecordingMode.AnchorMatrix.inverse * transform.localToWorldMatrix;

        }
        
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
