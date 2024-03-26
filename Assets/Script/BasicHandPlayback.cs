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
    [Header("Location of Log File")]
    private string _logFilePath;

    [SerializeField]
    [Header("Sampling rate")]
    private float _readInterval;
 
    [SerializeField]
    [Header("Debug Scrubber")] // Allows to move to different point in playback - modify the range based on the number of sampled points.
    [Range(0, 600)]
    private int _currentIndex = 0;

    private float timer = 0f;
    string[] lines;

    private bool _readFile = false;

    [SerializeField]
    private SpatialAnchorsManager RecordingMode;

    void Start()
    {

        
    }

    void Update()
    {
        if(RecordingMode.Mode == 3){
            // Debug.Log("replaying");
            //replaying
            if(!_readFile){
                // need to read file
                string path = Application.persistentDataPath + "/test.txt";
                //Read the text from directly from the test.txt file
                StreamReader reader = new StreamReader(path);
                // Debug.Log(reader.ReadToEnd());
                lines = reader.ReadToEnd().Split("\n");
                // Debug.Log(lines);
                reader.Close();
                _readFile = true;
            }
            timer += Time.deltaTime;

            // Execute at sampling rate
            if (timer >= _readInterval)
            {
                
                timer = 0f;

                // Check for index out of bounds
                if (_currentIndex >= lines.Length)
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

        // Cycle for each position value obtained (value at index 0 is timestamp - useful for syncing two hands/multiple recordings from the same session)
        for(int i = 1; i< parts.Length-1; i++)
        {
            string tempPosString = parts[i].Replace("(", "").Replace(")", "");
            Vector3 tempPos = ParseVector3(tempPosString);
            _playbackObject.transform.GetChild(i-1).localPosition = tempPos;
        }
        // Debug.Log(parts[parts.Length-1]);
        Matrix4x4 HeadMatrix = ConvertStringToMatrix(parts[parts.Length-1]);
        _playbackObject.transform.GetChild(parts.Length-2).localPosition = new Vector3(HeadMatrix[0,3], HeadMatrix[1,3], HeadMatrix[2,3]);;
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
        
        Debug.Log(tempMatrix);
        return tempMatrix;
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
