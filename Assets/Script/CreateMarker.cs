using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;


public class CreateMarker : MonoBehaviour
{
    [SerializeField]
    private SpatialAnchorsManager RecordingMode;

    [SerializeField]
    private GameObject _statusText;


    [SerializeField]
    private GameObject _handTracking;

    [SerializeField]
    private GameObject _turnMarker;

    [SerializeField]
    private GameObject _selectMarker;

    [SerializeField]
    private GameObject _camera;

    [SerializeField]
    private AppVoiceExperience WitExp;

    private List<GameObject> _recordMarkers;

    public string MarkerType;
    public Matrix4x4 MarkerMatrix;
    public Vector3 MarkerPos;
    public Quaternion MarkerQua;
    public bool NeedtoRecord;
    public bool NeedtoActivate;



    private float _recordingInterval = 0.03f;
    private float _timer = 0f;

    private bool _findAnchor = false;

    private Transform constructedAnchorTransform;


    // Start is called before the first frame update
    void Start()
    {
        _recordMarkers = new List<GameObject>();
        NeedtoRecord = false;
        NeedtoActivate = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        // if(RecordingMode.Mode == 1){
        //     // first save the anchor
        //     if (!_findAnchor){
        //         var theAcnhorMatrix = RecordingMode.AnchorMatrix;
        //         Vector3 anchorP = new Vector3(theAcnhorMatrix[0,3], theAcnhorMatrix[1,3], theAcnhorMatrix[2,3]);
        //         Quaternion anchorR = ExtractRotation(theAcnhorMatrix);
        //         constructedAnchorTransform = new GameObject().transform;
        //         // Transform constructedAnchorTransform;
        //         constructedAnchorTransform.position = anchorP;
        //         constructedAnchorTransform.rotation = anchorR;
        //         constructedAnchorTransform.localScale = new Vector3(1, 1, 1);
        //         _findAnchor = true;
        //     }            

        // }
        
        
    }

    public void CreateTurnMarker(){
        _statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "Create Turn Marker";
        Vector3 upwards = _camera.transform.position - _handTracking.GetComponent<RecordSkeleton>().IndexTipPos;
        Vector3 forward = -_camera.transform.up;
        _recordMarkers.Add(Instantiate(_turnMarker, _handTracking.GetComponent<RecordSkeleton>().IndexTipPos, Quaternion.LookRotation(forward, upwards)));
        // assign matrix here for recording
        MarkerType = "turn";
        // MarkerMatrix = RecordingMode.AnchorMatrix.inverse * _recordMarkers[^1].transform.localToWorldMatrix;
        MarkerPos = RecordingMode.AnchorTransform.InverseTransformPoint(_recordMarkers[^1].transform.position);
        MarkerQua = Quaternion.Inverse(RecordingMode.AnchorTransform.rotation) * _recordMarkers[^1].transform.rotation;
        Debug.Log("creating turn marker");
        NeedtoRecord = true;
    }

    public void CreateSelectMarker(){
        _statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "Create Select Marker";
        Vector3 forward = _camera.transform.position - _handTracking.GetComponent<RecordSkeleton>().IndexTipPos;
        Vector3 upwards = _camera.transform.up;
        _recordMarkers.Add(Instantiate(_selectMarker, _handTracking.GetComponent<RecordSkeleton>().IndexTipPos, Quaternion.LookRotation(forward, upwards)));
        MarkerType = "select";
        MarkerPos = RecordingMode.AnchorTransform.InverseTransformPoint(_recordMarkers[^1].transform.position);
        MarkerQua = Quaternion.Inverse(RecordingMode.AnchorTransform.rotation) * _recordMarkers[^1].transform.rotation;
        Debug.Log("creating select marker");
        NeedtoRecord = true;
        // WitExp.Activate();

    }

    public void NeedToActivate(){
        Debug.Log("trigger from the need to activate function");
        NeedtoActivate = true;

    }


    public void DestroyAllMarker(){
        foreach(var marker in _recordMarkers){
            Destroy(marker);
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
