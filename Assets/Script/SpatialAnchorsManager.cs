using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Manages UI of anchor sample.
/// </summary>
// [RequireComponent(typeof(SpatialAnchorLoader))]

public class SpatialAnchorsManager : MonoBehaviour
{

    public enum AnchorMode {
        Record,
        Replay
    }
    public AnchorMode _mode;

    //Anchor UI manager singleton instance
    public static SpatialAnchorsManager Instance;

    // reference to the anchor on controller
    [SerializeField, FormerlySerializedAs("anchorPlacementTransform_")]
    private Transform _anchorPlacementTransform;

    // the one to create
    [SerializeField]
    private GameObject _anchorPrefab;
    // public Anchor AnchorPrefab => _anchorPrefab;


    // the one to create
    [SerializeField]
    private GameObject _replayAnchor;

    public Vector3 AnchorPosition;

    public bool HandRecording = false;





    

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_mode == AnchorMode.Record){
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)){
            // Debug.Log("button down");
                PlaceAnchor();
            }
        }

        if(_mode == AnchorMode.Replay){
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)){
                _replayAnchor.transform.position = _anchorPlacementTransform.position;
                // _replayAnchor.transform.rotation = _anchorPlacementTransform.rotation;
            }

        }
        

        

        // if (OVRInput.GetUp(OVRInput.Button.One)){
        //     OnLoadAnchorsButtonPressed();
        // }
        
    }

    private void PlaceAnchor()
    {
        AnchorPosition = _anchorPlacementTransform.position;
        // check once!!!
        Instantiate(_anchorPrefab, _anchorPlacementTransform.position, _anchorPlacementTransform.rotation);
        HandRecording = true;
    }


    // /// <summary>
    // /// Load anchors button pressed UI callback. Referenced by the Load Anchors button in the menu.
    // /// </summary>
    // public void OnLoadAnchorsButtonPressed()
    // {
    //     GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
    // }
}
