using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Text;

/// <summary>
/// Specific functionality for spawned anchors
/// </summary>
[RequireComponent(typeof(OVRSpatialAnchor))]

public class Anchor : MonoBehaviour
{
    private OVRSpatialAnchor _spatialAnchor;

    [SerializeField, FormerlySerializedAs("saveIcon_")]
    private GameObject _saveIcon;

    public const string NumUuidsPlayerPref = "numUuids";

    float timeRemaining = 3;
    float flashInterval = 0.1f;
    float timeForFlash = 3;

    private void Awake()
    {
        _spatialAnchor = GetComponent<OVRSpatialAnchor>();
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if(_spatialAnchor.Created){
            if(timeRemaining > 0){
            timeRemaining -= Time.deltaTime;
            if(timeForFlash - timeRemaining > flashInterval){
                timeForFlash = timeRemaining;
                FlashSaveIcon();
                }
            }else{
                // OnSaveLocalButtonPressed();
                // OnHideButtonPressed();
            }
        }
        
        
    }

        /// <summary>
    /// UI callback for the anchor menu's Save button
    /// </summary>
    public void OnSaveLocalButtonPressed()
    {
        if (!_spatialAnchor) return;

        _spatialAnchor.Save((anchor, success) =>
        {
            if (!success) return;

            // Enables save icon on the menu
            ShowSaveIcon = true;

            SaveUuidToPlayerPrefs(anchor.Uuid);
        });
    }

    void SaveUuidToPlayerPrefs(Guid uuid)
    {
        // Write uuid of saved anchor to file
        if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
        }

        int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
        PlayerPrefs.SetString("uuid" + playerNumUuids, uuid.ToString());
        PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);
    }

    public bool ShowSaveIcon
    {
        set => _saveIcon.SetActive(value);
    }

    public void FlashSaveIcon()
    {
        _saveIcon.SetActive(!_saveIcon.activeSelf);
        
    }

    public void OnHideButtonPressed()
    {
        Destroy(gameObject);
    }
}
