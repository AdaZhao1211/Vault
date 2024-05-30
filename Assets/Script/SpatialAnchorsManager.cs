using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.IO;
using Oculus.Voice;

/// <summary>
/// Manages UI of anchor sample.
/// </summary>
// [RequireComponent(typeof(SpatialAnchorLoader))]

public class SpatialAnchorsManager : MonoBehaviour
{

    public OSC osc;

    public int Mode = 0;

    //Anchor UI manager singleton instance
    public static SpatialAnchorsManager Instance;

    // reference to the anchor on controller
    [SerializeField, FormerlySerializedAs("anchorPlacementTransform_")]
    private Transform _anchorPlacementTransform;

    // the one to create
    [SerializeField]
    private GameObject _anchorPrefab;

    // the one to create
    [SerializeField]
    private GameObject _demoAnchorPrefab;

    [SerializeField]
    private GameObject _replayLeftHand;

    [SerializeField]
    private GameObject _replayRightHand;

    public Vector3 AnchorPosition;
    public Transform AnchorTransform;
    public Matrix4x4 AnchorMatrix;

    private float _recordingInterval = 0.03f;

    private StreamWriter writer;

    private float timer = 0f;

   [SerializeField]
    private GameObject RightHand;

    [SerializeField]
    private GameObject LeftHand;

    [SerializeField]
    private GameObject Head;

    [SerializeField]
    private GameObject Marker;

    [SerializeField]
    private GameObject VoiceCommand;

    private GameObject _recordAnchor;
    private GameObject _replayAnchor;

    public String fname;

    private bool recording;

    [SerializeField]

    private GameObject myAudioClip;
    private float startRecordingTime = 0f;







    

    // to make sure there is only one anchor manager running
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        fname = System.DateTime.Now.ToString("MMM-dd-HH-mm-ss");
        string path = Path.Combine(Application.persistentDataPath, fname);
        path += ".txt";
        //Write some text to the test.txt file
        writer = new StreamWriter(path, false);
        recording = false;

    }

    // Update is called once per frame
    void Update()
    {

        // Debug.Log("mode = "+ Mode);

        if(Mode == 0){
            // to record
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)){
            // Debug.Log("button down");
                PlaceAnchor();
                AnchorTransform = _recordAnchor.transform;


                // create a new empty file
                OscMessage _oscmessage = new OscMessage();
                _oscmessage.address = "/startRecording";
                osc.Send(_oscmessage);
                Mode = 1; // recording
            }
        }

        if (Mode == 1){
            // Debug.Log("micro: " + Microphone.devices.Length);
            // record audio for 2 mins
            if(!recording){
                recording = true;
                // var temMessage = "";
                // foreach (var device in Microphone.devices)
                // {
                //     temMessage += device;
                //     temMessage += "\n";
                // }
                // Debug.Log("Name: " + temMessage);

                myAudioClip = Microphone.Start(Microphone.devices[0], false, 120, 44100);
                startRecordingTime = Time.time;
            }

            // record hands & head & markers
            timer += Time.deltaTime;
            if (timer >= _recordingInterval)
            {
                timer = 0f;
                writer.Write(System.DateTime.Now.ToString());
                writer.Write("#");
                // right hand
                for (int i = 0; i < 24; i++){
                    writer.Write(RightHand.GetComponent<RecordSkeleton>().HandSkeletonPos[i]);
                    writer.Write("$");
                    writer.Write(RightHand.GetComponent<RecordSkeleton>().HandSkeletonQua[i]);
                    writer.Write("#");
                }
                // left hand
                for (int i = 0; i < 24; i++){
                    writer.Write(LeftHand.GetComponent<RecordSkeleton>().HandSkeletonPos[i]);
                    writer.Write("$");
                    writer.Write(LeftHand.GetComponent<RecordSkeleton>().HandSkeletonQua[i]);
                    writer.Write("#");
                }
                // head
                writer.Write(Head.GetComponent<HeadTracking>().HeadPos);
                writer.Write("$");
                writer.Write(Head.GetComponent<HeadTracking>().HeadQua);
                writer.Write("#");

                // marker
                if( Marker.GetComponent<CreateMarker>().NeedtoRecord){
                    // if there is marker need to be recorded
                    writer.Write(Marker.GetComponent<CreateMarker>().MarkerType);
                    writer.Write("#");
                    writer.Write(Marker.GetComponent<CreateMarker>().MarkerPos);
                    writer.Write("$");
                    writer.Write(Marker.GetComponent<CreateMarker>().MarkerQua);
                    writer.Write("#");
                    Marker.GetComponent<CreateMarker>().NeedtoRecord = false;
                    VoiceCommand.GetComponent<AppVoiceExperience>().Activate();
                }
                writer.Write("\n");

                if (Marker.GetComponent<CreateMarker>().NeedtoActivate){
                    VoiceCommand.GetComponent<AppVoiceExperience>().Activate();
                    // Debug.Log("activate from anchor manager");
                    Marker.GetComponent<CreateMarker>().NeedtoActivate = false;

                }
            }
            
            // wait for end recording
            if (OVRInput.GetUp(OVRInput.Button.One)){
                Debug.Log("endRecording");

                OscMessage _oscmessage;
                _oscmessage = new OscMessage();
                _oscmessage.address = "/endRecording";
                osc.Send(_oscmessage);


                writer.Close();
                Mode = 2; // to replay
                Destroy(_recordAnchor);
                Marker.GetComponent<CreateMarker>().DestroyAllMarker();

                Microphone.End(Microphone.devices[0]);
                var temMessage = "";
                foreach (var device in Microphone.devices)
                {
                    temMessage += device;
                    temMessage += "\n";
                }

                Debug.Log("Namereplay: " + temMessage);


                // save wav audio file
                float recordingTime = Time.time - startRecordingTime;
                AudioClip trimSilence = SavWav.TrimSilenceByTime(myAudioClip, recordingTime, 120f);
                SavWav.Save(fname, trimSilence);


                

                // Debug.Log("Namereplay");



                // print the recorded txt file

                // string path = Application.persistentDataPath + "/test.txt";
                // //Read the text from directly from the test.txt file
                // StreamReader reader = new StreamReader(path);
                // Debug.Log(reader.ReadToEnd());
                // reader.Close();

                
            }
        }

        if(Mode == 2){
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)){
                // _replayAnchor.transform.position = _anchorPlacementTransform.position;
                _replayLeftHand.transform.position = _anchorPlacementTransform.position;
                _replayRightHand.transform.position = _anchorPlacementTransform.position;
                AnchorMatrix = _anchorPlacementTransform.localToWorldMatrix;
                PlaceReplayAnchor();
                AnchorTransform = _replayAnchor.transform;

                // _replayAnchor.transform.rotation = _anchorPlacementTransform.rotation;
                Mode = 3;
            } 

        }

        if(Mode == 3){
            if (OVRInput.GetUp(OVRInput.Button.One)){
                Mode = 4;
            }
        }
        

        

        // if (OVRInput.GetUp(OVRInput.Button.One)){
        //     OnLoadAnchorsButtonPressed();
        // }
        
    }

    private void PlaceAnchor()
    {
        AnchorMatrix = _anchorPlacementTransform.localToWorldMatrix;
        AnchorPosition = new Vector3(AnchorMatrix[0,3], AnchorMatrix[1,3], AnchorMatrix[2,3]);

        // check once!!!
        _recordAnchor = (GameObject)Instantiate(_anchorPrefab, _anchorPlacementTransform.position, _anchorPlacementTransform.rotation);
    }

    private void PlaceReplayAnchor()
    {

        // check once!!!
        _replayAnchor = (GameObject)Instantiate(_demoAnchorPrefab, _anchorPlacementTransform.position, _anchorPlacementTransform.rotation);
    }


    // /// <summary>
    // /// Load anchors button pressed UI callback. Referenced by the Load Anchors button in the menu.
    // /// </summary>
    // public void OnLoadAnchorsButtonPressed()
    // {
    //     GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
    // }
}
