using Oculus.Voice;
 using UnityEngine;

 public class WitActivation : MonoBehaviour
 {
    [SerializeField]
    private GameObject _recordManager;
     private AppVoiceExperience _voiceExperience;
     private bool turnonWit = false;
     private void OnValidate()
     {
         if (!_voiceExperience) _voiceExperience = GetComponent<AppVoiceExperience>();


        _voiceExperience.VoiceEvents.OnRequestCompleted.AddListener(() =>
        {
            Debug.Log("request completed");
        });

        _voiceExperience.AudioEvents.OnMicStartedListening.AddListener(() =>
        {
            Debug.Log("start");
        });

        _voiceExperience.AudioEvents.OnMicStoppedListening.AddListener(() =>
        {
            Debug.Log("stoppppp");
            // _voiceExperience.Activate();
        });
     }

     private void Start()
     {
        
     }

     private void Update()
     {

        if (Input.GetKeyDown(KeyCode.Space))
        {
                Debug.Log("*** Pressed Space bar ***");
                ActivateWit();
        }
        if(!turnonWit){

        
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("*** Pressed Space bar ***");
                ActivateWit();
            }

            if (_recordManager.GetComponent<SpatialAnchorsManager>().Mode == 1)
            {
                ActivateWit();
                turnonWit = true;
            }
        }

        if(turnonWit){
            if (_recordManager.GetComponent<SpatialAnchorsManager>().Mode == 2)
            {
                // _voiceExperience.Deactivate();
                turnonWit = false;
            }

        }


     }

     /// <summary>
     /// Activates Wit i.e. start listening to the user.
     /// </summary>
     public void ActivateWit()
     {
        Debug.Log("activate");
        _voiceExperience.Activate();
     }
 }
