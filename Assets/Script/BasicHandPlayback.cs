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

    void Start()
    {
        // if (string.IsNullOrEmpty(_logFilePath))
        // {
        //     Debug.Log("Missing log file.");
        //     return;
        // }

        var textFile = Resources.Load<TextAsset>(_logFilePath);
        string fileLines = textFile.text;
        print("big string = " + fileLines);
        lines = fileLines.Split("\n");
        // print("lines = " + lines);


        // if (File.Exists(_logFilePath))
        // {
        //     try
        //     {
        //         lines = File.ReadAllLines(_logFilePath); // Stores all lines in a string array
        //     }
        //     catch (System.Exception e)
        //     {
        //         Debug.Log($"Error reading the file: {e.Message}");
        //     }
        // }
        // else
        // {
        //     Debug.Log("Log file not found.");
        // }

        // _currentIndex = 0;
    }

    void Update()
    {
        // if (File.Exists(_logFilePath))
        // {
        //     Debug.Log("file exist");
        // }else{
        //     Debug.Log("file doesn't exist");
        // }

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
            
            // Set the index of the next line to read
            _currentIndex++;

            
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
        for(int i = 1; i< parts.Length; i++)
        {
            string tempPosString = parts[i].Replace("(", "").Replace(")", "");
            Vector3 tempPos = ParseVector3(tempPosString);
            _playbackObject.transform.GetChild(i-1).localPosition = tempPos;
        }
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
}
