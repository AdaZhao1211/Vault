using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Networking;
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

    [SerializeField]
    private bool onlyReplay;

    private GameObject _recordAnchor;
    private GameObject _replayAnchor;

    public String fname;

    public string serverUrl = "http://192.168.0.230:8000";  // Change to your server's IP address and port
    private AudioSource audioSource;
    private bool getAudio;







    

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

        if (!onlyReplay){
             fname = System.DateTime.Now.ToString("MM-dd-HH-mm-ss");
            string path = Path.Combine(Application.persistentDataPath, fname);
            path += ".txt";
            //Write some text to the test.txt file
            writer = new StreamWriter(path, false);   
        }

        

        audioSource = GetComponent<AudioSource>();
        getAudio = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(Mode == 0){
            // to record
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)){
                PlaceAnchor();
                AnchorTransform = _recordAnchor.transform;
                // create a new empty file
                // send osc signal to start audio recording
                OscMessage _oscmessage = new OscMessage();
                _oscmessage.address = "/startRecording";
                osc.Send(_oscmessage);
                Mode = 1; // recording
            }
        }

        if (Mode == 1){
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
            
            //end recording
            if (OVRInput.GetUp(OVRInput.Button.One)){
                Debug.Log("endRecording");

                // osc to end recording
                OscMessage _oscmessage;
                _oscmessage = new OscMessage();
                _oscmessage.address = "/endRecording";
                _oscmessage.values.Add(fname);
                osc.Send(_oscmessage);

                writer.Close();
                Mode = 2; // to replay
                Destroy(_recordAnchor);
                Marker.GetComponent<CreateMarker>().DestroyAllMarker();


                // print the recorded txt file

                // string path = Application.persistentDataPath + "/test.txt";
                // //Read the text from directly from the test.txt file
                // StreamReader reader = new StreamReader(path);
                // Debug.Log(reader.ReadToEnd());
                // reader.Close();
  
            }
        }

        if(Mode == 2){

            // get audio from local server
            if(!getAudio){
                string audioUrl = $"{serverUrl}/{fname}";
                audioUrl += ".wav";
                Debug.Log(audioUrl);
                StartCoroutine(DownloadAndPlayAudio(audioUrl));
                getAudio = true;
            }

            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) && getAudio){
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

        if (OVRInput.GetUp(OVRInput.Button.Two)){
            if (Mode == 4){
                Mode = 3;
            }else if(Mode == 3){
                Mode = 4;
            }
        }

        if(Mode == 3){
            if (OVRInput.GetUp(OVRInput.Button.One)){
                Mode = 5;
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


    public IEnumerator DownloadAndPlayAudio(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                Debug.Log("playing audio file");
            }
        }
    }


    // /// <summary>
    // /// Load anchors button pressed UI callback. Referenced by the Load Anchors button in the menu.
    // /// </summary>
    // public void OnLoadAnchorsButtonPressed()
    // {
    //     GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
    // }
}
