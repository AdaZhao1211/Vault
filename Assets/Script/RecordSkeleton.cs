using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RecordSkeleton : MonoBehaviour
{
    [SerializeField]
    private OVRHand hand;

    [SerializeField]
    private OVRSkeleton handSkeleton;

    [SerializeField]
    private GameObject bonePrefab;

    private bool bonesAdded = false;



    private void Awake()
    {
        if (!hand) hand = GetComponent<OVRHand>();
        if (!handSkeleton) handSkeleton = GetComponent<OVRSkeleton>();   
    }


    private void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if(hand.IsTracked)
        {
            DisplayBoneInfo();
            if(!bonesAdded) CreateBones();
        }
        
    }

    private void DisplayBoneInfo()
    {
        string message = "";
        message += handSkeleton.GetSkeletonType();
        message += "  ";
        message += handSkeleton.GetCurrentNumBones();
        message += "  ";
        message += handSkeleton.GetCurrentStartBoneId();
        message += "\n";
        
        // int i = 0;
        foreach (var bone in handSkeleton.Bones){
            message += bone.Id;
            message += ": ";
            message += bone.Transform.position;
            message += "\n";
        }
        Debug.Log(message);
    }

    private void CreateBones()
    {
        foreach (var bone in handSkeleton.Bones)
        {
            Instantiate(bonePrefab, bone.Transform)
                .GetComponent<BoneInfo>()
                .AddBone(bone);
        }

        bonesAdded = true;
    }

}
