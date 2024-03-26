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
     }

     private void Start()
     {
         _voiceExperience = GetComponent<AppVoiceExperience>();
     }

     private void Update()
     {
        if(!turnonWit){

        
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("*** Pressed Space bar ***");
                ActivateWit();
            }

            if (_recordManager.GetComponent<SpatialAnchorsManager>().Mode == 1)
            {
                Debug.Log("*** hand recording & listening ***");
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
         _voiceExperience.Activate();
     }
 }
