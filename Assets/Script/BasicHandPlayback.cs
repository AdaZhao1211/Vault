using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;

/// <summary>
/// Basic method to read joint positions from a log file and apply them to the children of the game object on which this script is attached.
/// </summary>
public class BasicHandPlayback : MonoBehaviour
{
    [SerializeField]
    public GameObject _headplayback;

    [SerializeField]
    public GameObject _leftHand;

    [SerializeField]
    public GameObject _rightHand;

    [SerializeField]
    [Header("Sampling rate")]
    private float _readInterval = 0.03f;
 
    [SerializeField]
    [Header("Debug Scrubber")] // Allows to move to different point in playback - modify the range based on the number of sampled points.
    [Range(0, 600)]
    private int _currentIndex = 0;

    private float timer = 0f;
    string[] lines;

    private bool _readFile = false;

    [SerializeField]
    private SpatialAnchorsManager RecordingMode;

    [SerializeField]
    private GameObject _selectMarker;

    [SerializeField]
    private GameObject _turnMarker;

    [SerializeField]
    private GameObject _selectMarkerWT;

    [SerializeField]
    private GameObject _turnMarkerWT;

    [SerializeField]
    private GameObject _slider;

    [SerializeField]
    private GameObject _KeyEventText;

    [SerializeField]
    private Transform _canvas;

    private bool firstTime = true;
    private bool guidebook = true;



    void Start()
    {

        
    }

    void Update()
    {
        if(RecordingMode.Mode == 3){
            //replaying
            if(!_readFile){
                // need to read file
                string path = Path.Combine(Application.persistentDataPath, RecordingMode.fname);
                path += ".txt";

                //Read the text from directly from the test.txt file
                StreamReader reader = new StreamReader(path);
                lines = reader.ReadToEnd().Split("\n");
                reader.Close();
                _readFile = true;

                // create timeline
                _slider.SetActive(true);

                for (int i = 0; i < lines.Length-1; i++){
                    var singleline = lines[i];
                    var singleinfo = singleline.Split('#');
                    if (singleinfo.Length > 49){
                        
                            GameObject turnText = Instantiate(_KeyEventText,_canvas);
                            turnText.GetComponent<RectTransform>().localPosition = new Vector3(i/(float)(lines.Length-1)*3-1.5f,-0.05f,0);
                            turnText.GetComponent<TextMeshProUGUI>().text = singleinfo[50];
                    }
                }
                

            }
            timer += Time.deltaTime;

            // Execute at sampling rate
            if (timer >= _readInterval)
            {
                
                timer = 0f;

                // Check for index out of bounds
                if (_currentIndex >= lines.Length-1)
                {
                    _currentIndex = 0;
                    firstTime = false;
                }

                // Read line from file, and apply transforms to joints
                ParseLineAndApplyTransform(lines[_currentIndex]);
                _slider.GetComponent<Slider>().value = (float)_currentIndex / (float)(lines.Length-1);
                Debug.Log(_currentIndex / (lines.Length-1));
                
                // Set the index of the next line to read
                _currentIndex++;
            }


        }
        if(RecordingMode.Mode == 4){
            Debug.Log("spatial guid mode");
            if (_readFile && guidebook){
                int markerN = 1;
                for (int i = 0; i < lines.Length-1; i++){
                    var singleline = lines[i];
                    var singleinfo = singleline.Split('#');
                    
                    if (singleinfo.Length > 51){
                        if(singleinfo[50] == "turn"){
                            var MInfo = ConvertStringToInfo(singleinfo[51]);
                            Vector3 MPos = RecordingMode.AnchorTransform.TransformPoint(MInfo.Item1);
                            Quaternion MQua = RecordingMode.AnchorTransform.rotation * MInfo.Item2;
                            GameObject marker = (GameObject)Instantiate(_turnMarkerWT, MPos, MQua);
                            marker.transform.Find("T").gameObject.GetComponent<TextMeshPro>().text = markerN.ToString();

                        }
                        if(singleinfo[50] == "select"){
                            var MInfo = ConvertStringToInfo(singleinfo[51]);
                            Vector3 MPos = RecordingMode.AnchorTransform.TransformPoint(MInfo.Item1);
                            Quaternion MQua = RecordingMode.AnchorTransform.rotation * MInfo.Item2;
                            GameObject marker = (GameObject)Instantiate(_selectMarkerWT, MPos, MQua);
                            marker.transform.Find("T").gameObject.GetComponent<TextMeshPro>().text = markerN.ToString();
                        }
                        
                        markerN ++;
                            
                    }
                    Debug.Log("here:" + singleinfo.Length);
                }
                guidebook = false;

            }
        }


        
    }

    /// <summary>
    /// Reads one line from the file, parses it, and applies transforms to joints
    /// </summary>
    /// <param name="line"></param>
    void ParseLineAndApplyTransform(string line)
    {
        // New line format: "092443800#(0.00, 0.00, 0.00)#(0.00, 0.00, 0.00)#..." - Timestamp + 21 position values
        string[] parts = line.Split('#');
        Transform _leftHandChild = _leftHand.transform.GetChild(0).GetChild(0);
        Transform _rightHandChild = _rightHand.transform.GetChild(0).GetChild(0);

        // right hand

        var handInfo = ConvertStringToInfo(parts[1]);
        _rightHandChild.transform.position = RecordingMode.AnchorTransform.TransformPoint(handInfo.Item1);
        _rightHandChild.transform.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;


        Transform thumb = _rightHandChild.Find("b_r_thumb0");
        // Matrix4x4 preM = HandMatrix1;
        for ( int i= 3; i < 7; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _rightHandChild.Find("b_r_index1");
        for ( int i= 7; i < 10; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _rightHandChild.Find("b_r_middle1");
        for ( int i= 10; i < 13; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _rightHandChild.Find("b_r_ring1");
        for ( int i= 13; i < 16; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _rightHandChild.Find("b_r_pinky0");
        for ( int i= 16; i < 20; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        // ----------------
        // left hand
        handInfo = ConvertStringToInfo(parts[25]);
        _leftHandChild.transform.position = RecordingMode.AnchorTransform.TransformPoint(handInfo.Item1);
        _leftHandChild.transform.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;

        thumb = _leftHandChild.Find("b_l_thumb0");
        // Matrix4x4 preM = HandMatrix1;
        for ( int i= 27; i < 31; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _leftHandChild.Find("b_l_index1");
        for ( int i= 31; i < 34; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _leftHandChild.Find("b_l_middle1");
        for ( int i= 34; i < 37; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _leftHandChild.Find("b_l_ring1");
        for ( int i= 37; i < 40; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        thumb = _leftHandChild.Find("b_l_pinky0");
        for ( int i= 40; i < 44; i++){
            handInfo = ConvertStringToInfo(parts[i]);
            thumb.rotation = RecordingMode.AnchorTransform.rotation * handInfo.Item2;
            thumb = thumb.GetChild(0);
        }

        
        // head
        var headInfo = ConvertStringToInfo(parts[49]);
        _headplayback.transform.position = RecordingMode.AnchorTransform.TransformPoint(headInfo.Item1);
        _headplayback.transform.rotation = RecordingMode.AnchorTransform.rotation * headInfo.Item2;

        // marker?
        if(parts.Length > 49 && firstTime){
            // there is marker
            // marker type # matrix 4x4
            if(parts[50] == "turn"){
                var MInfo = ConvertStringToInfo(parts[51]);
                Vector3 MPos = RecordingMode.AnchorTransform.TransformPoint(MInfo.Item1);
                Quaternion MQua = RecordingMode.AnchorTransform.rotation * MInfo.Item2;
                Instantiate(_turnMarker, MPos, MQua);
            }
            if(parts[50] == "select"){
                var MInfo = ConvertStringToInfo(parts[51]);
                Vector3 MPos = RecordingMode.AnchorTransform.TransformPoint(MInfo.Item1);
                Quaternion MQua = RecordingMode.AnchorTransform.rotation * MInfo.Item2;
                Instantiate(_selectMarker, MPos, MQua);
            }
        }


    }

    private Matrix4x4 ConvertStringToMatrix(string line)
    {

        string[] parts = line.Split('$');
        Vector4[] m = new Vector4[4];
        for(int i = 0; i< parts.Length; i++){
            string tempPosString = parts[i].Replace("(", "").Replace(")", "");
            Vector4 vv = ParseVector4(i, tempPosString);
            m[i] = vv;

        }
        
        Matrix4x4 tempMatrix = new Matrix4x4();
        for (int i = 0; i < 4; i++){
            tempMatrix.SetRow(i, m[i]);

        }
        
        return tempMatrix;
    }

    private (Vector3, Quaternion) ConvertStringToInfo(string line)
    {

        string[] parts = line.Split('$');
        Vector3 pos = ParseVector3(parts[0]);
        Quaternion qua = ParseQua(parts[1]);
        
        
        return (pos, qua);
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

    /// <summary>
    /// Takes a string and returns a Vector3 
    /// </summary>
    /// <param name="vectorString"></param>
    /// <returns></returns>
    Vector3 ParseVector3(string vectorString)
    {

        string tempPosString = vectorString.Replace("(", "").Replace(")", "");
        string[] components = tempPosString.Split(',');

        float x = float.Parse(components[0]);
        float y = float.Parse(components[1]);
        float z = float.Parse(components[2]);

        return new Vector3(x, y, z);
    }


    Vector4 ParseVector4(int i, string vectorString)
    {
        string[] components = vectorString.Split(",");
        
        float x = float.Parse(components[0]);
        float y = float.Parse(components[1]);
        float z = float.Parse(components[2]);
        float w = float.Parse(components[3]);
        return new Vector4(x, y, z, w);
    }

    Quaternion ParseQua(string quaString)
    {
        string tempPosString = quaString.Replace("(", "").Replace(")", "");

        string[] components = tempPosString.Split(',');

        float x = float.Parse(components[0]);
        float y = float.Parse(components[1]);
        float z = float.Parse(components[2]);
        float w = float.Parse(components[3]);

        return new Quaternion(x, y, z, w);
    }
}
