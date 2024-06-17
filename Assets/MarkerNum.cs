using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerNum : MonoBehaviour
{

    public int MarkerLineNum;
    [SerializeField]
    private BasicHandPlayback playback;
    // Start is called before the first frame update
    void Start()
    {
        playback = GameObject.Find("ReplayerManager").GetComponent<BasicHandPlayback>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMarkerPos(){
        playback.UpdateMarkerTrans(MarkerLineNum, this.transform.position, this.transform.rotation);
    }

    public void DeleteMarker(){
        playback.DeleteMarker(MarkerLineNum);

    }
}
