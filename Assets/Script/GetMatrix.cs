using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMatrix : MonoBehaviour
{
    [SerializeField]
    private GameObject CubeA;

    [SerializeField]
    private GameObject MarkerB;

    // Start is called before the first frame update

    private Vector3 RelativePos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("d"))
        {
            Debug.Log("*** Pressed D ***");
            RelativePos = transform.InverseTransformPoint(CubeA.transform.position);
            Debug.Log(RelativePos);
        }

        if (Input.GetKeyDown("m"))
        {
            Vector3 pos = MarkerB.transform.TransformPoint(RelativePos);
            // _recordMarkers.Add(Instantiate(_selectMarker, _handTracking.GetComponent<RecordSkeleton>().IndexTipPos, Quaternion.LookRotation(forward, upwards)));

        }
        
    }
}
