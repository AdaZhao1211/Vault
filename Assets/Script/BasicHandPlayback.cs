using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Basic method to read joint positions from a log file and apply them to the children of the game object on which this script is attached.
/// </summary>
public class BasicHandPlayback : MonoBehaviour
{
    [SerializeField]
    [Header("Parent of hand joints")] // Should have 21 children.
    public GameObject _playbackObject;

    [SerializeField]
    [Header("Left Hand IK")]
    public GameObject _leftHand;

    [SerializeField]
    [Header("Right Hand IK")]
    public GameObject _rightHand;
   
    [SerializeField]
    [Header("Location of Log File")]
    private string _logFilePath;

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

    void Start()
    {

        
    }

    void Update()
    {
        if(RecordingMode.Mode == 3){
            //replaying
            if(!_readFile){
                // need to read file
                string path = Application.persistentDataPath + "/test.txt";
                //Read the text from directly from the test.txt file
                StreamReader reader = new StreamReader(path);
                lines = reader.ReadToEnd().Split("\n");
                reader.Close();
                _readFile = true;
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
                }

                // Read line from file, and apply transforms to joints
                ParseLineAndApplyTransform(lines[_currentIndex]);
                // Debug.Log("replaying: " + lines[_currentIndex]);
                
                // Set the index of the next line to read
                _currentIndex++;
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


        // Cycle for each position value obtained (value at index 0 is timestamp - useful for syncing two hands/multiple recordings from the same session)
        // for(int i = 1; i< 25; i++)
        // {
        //     Matrix4x4 HandMatrix = ConvertStringToMatrix(parts[i]);
        //     _playbackObject.transform.GetChild(i-1).localPosition = new Vector3(HandMatrix[0,3], HandMatrix[1,3], HandMatrix[2,3]);
        // }

        // right hand
        Matrix4x4 HandMatrix1 = ConvertStringToMatrix(parts[1]);
        _rightHandChild.localRotation = ExtractRotation(HandMatrix1);
        _rightHandChild.localPosition = new Vector3(HandMatrix1[0,3], HandMatrix1[1,3], HandMatrix1[2,3]);


        Transform thumb = _rightHandChild.Find("b_r_thumb0");
        Matrix4x4 preM = HandMatrix1;
        for ( int i= 3; i < 7; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;

        }

        thumb = _rightHandChild.Find("b_r_index1");
        preM = HandMatrix1;
        for ( int i= 7; i < 10; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        thumb = _rightHandChild.Find("b_r_middle1");
        preM = HandMatrix1;
        for ( int i= 10; i < 13; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        thumb = _rightHandChild.Find("b_r_ring1");
        preM = HandMatrix1;
        for ( int i= 13; i < 16; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        thumb = _rightHandChild.Find("b_r_pinky0");
        preM = HandMatrix1;
        for ( int i= 16; i < 20; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        // ----------------
        // left hand
        Matrix4x4 HandMatrix25 = ConvertStringToMatrix(parts[25]);
        _leftHandChild.localRotation = ExtractRotation(HandMatrix25);
        _leftHandChild.localPosition = new Vector3(HandMatrix25[0,3], HandMatrix25[1,3], HandMatrix25[2,3]);


        thumb = _leftHandChild.Find("b_l_thumb0");
        preM = HandMatrix25;
        for ( int i= 27; i < 31; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb.localPosition = new Vector3(HandRela[0,3], HandRela[1,3], HandRela[2,3]);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;

        }

        thumb = _leftHandChild.Find("b_l_index1");
        preM = HandMatrix25;
        for ( int i= 31; i < 34; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        thumb = _leftHandChild.Find("b_l_middle1");
        preM = HandMatrix25;
        for ( int i= 34; i < 37; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        thumb = _leftHandChild.Find("b_l_ring1");
        preM = HandMatrix25;
        for ( int i= 37; i < 40; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }

        thumb = _leftHandChild.Find("b_l_pinky0");
        preM = HandMatrix25;
        for ( int i= 40; i < 44; i++){
            Matrix4x4 HandMatrixNow = ConvertStringToMatrix(parts[i]);
            Matrix4x4 HandRela = preM.inverse * HandMatrixNow;
            thumb.localRotation = ExtractRotation(HandRela);
            thumb = thumb.GetChild(0);
            preM = HandMatrixNow;
        }
        
        // head
        Matrix4x4 HeadMatrix = ConvertStringToMatrix(parts[49]);
        _playbackObject.transform.GetChild(0).localRotation = ExtractRotation(HeadMatrix);
        _playbackObject.transform.GetChild(0).localPosition = new Vector3(HeadMatrix[0,3], HeadMatrix[1,3], HeadMatrix[2,3]);

        // marker?
        if(parts.Length > 49){
            // there is marker
            // marker type # matrix 4x4
            if(parts[50] == "turn"){
                Matrix4x4 MarkerMatrix = RecordingMode.AnchorMatrix * ConvertStringToMatrix(parts[51]);
                Debug.Log(MarkerMatrix);
                Instantiate(_turnMarker, new Vector3(MarkerMatrix[0,3], MarkerMatrix[1,3], MarkerMatrix[2,3]), ExtractRotation(MarkerMatrix));
            }
            if(parts[50] == "select"){
                Matrix4x4 MarkerMatrix = RecordingMode.AnchorMatrix * ConvertStringToMatrix(parts[51]);
                // Instantiate(_selectMarker, new Vector3(MarkerMatrix[0,3], MarkerMatrix[1,3], MarkerMatrix[2,3]), ExtractRotation(MarkerMatrix));
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
        string[] components = vectorString.Split(',');

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
}
