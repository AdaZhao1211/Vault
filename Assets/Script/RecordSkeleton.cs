using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RecordSkeleton : MonoBehaviour
{
    [SerializeField]
    private OVRHand _ovrHand;
    [SerializeField]
    private OVRSkeleton _ovrSkeleton;



    private void Awake()
    {
        if(_ovrHand == null) _ovrHand = GetComponent<OVRHand>();
        if(_ovrSkeleton == null) _ovrSkeleton = GetComponent<OVRSkeleton>();
        
        
    }


    private void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        // Debug.Log("updating");
        if(_ovrHand.IsTracked){
            Debug.Log("tracking");
            DisplayBoneInfo();
        }





        // if (_ovrSkeleton.IsInitialized)
        // {
        //     Debug.Log("is initialized");
        //     for (int i = 0; i < _ovrSkeleton.Bones.Count; i++)
        //     {
            
        //             //Debug.Log(_ovrSkeleton.Bones[i].Transform);
        //     }

        //     if (_renderPhysicsCapsules && _ovrSkeleton.Capsules != null)
        //     {
        //         for (int i = 0; i < _ovrSkeleton.Capsules.Count; i++)
        //         {
        //             var capsuleVis = new CapsuleVisualization(
        //                 _skeletonGO,
        //                 _capsuleMaterial,
        //                 _systemGestureMaterial,
        //                 _scale,
        //                 _ovrSkeleton.Capsules[i]);

        //             _capsuleVisualizations.Add(capsuleVis);
        //         }
        //     }

        // }
        
    }

    private void DisplayBoneInfo()
    {
        string message = "";
        message += _ovrSkeleton.GetCurrentNumBones();
        message += " Num of Bones\n";
        int i = 0;
        foreach (var bone in _ovrSkeleton.Bones){
            message += _ovrSkeleton.GetSkeletonType
            message += bone.Id;
            message += "\n";
            
            // i ++;
            // if (i > 7){
            //     message += _ovrSkeleton.GetSkeletonType();
            //     message += " = Skeleton Type\n";
            //     message += bone.Id;
            //     message += " = Bone ID\n";
            //     message += bone.Transform.position;
            //     message += " = Transform position\n";

            //     if( i > 9) break
            // }
        }
        message += "\n" + i + "\n";
        Debug.Log(message);
        // foreach (var bone in _ovrSkeleton.Bones)
        // {
        //     Logger.Instance.LogInfo($"{_ovrSkeleton.GetSkeletonType()}: boneId -> {bone.Id} pos -> {bone.Transform.position}");
        // }

        // Logger.Instance.LogInfo($"{_ovrSkeleton.GetSkeletonType()} num of bones: {_ovrSkeleton.GetCurrentNumBones()}");
        // Logger.Instance.LogInfo($"{_ovrSkeleton.GetSkeletonType()} num of skinnable bones: {_ovrSkeleton.GetCurrentNumSkinnableBones()}");
        // Logger.Instance.LogInfo($"{_ovrSkeleton.GetSkeletonType()} start bone id: {_ovrSkeleton.GetCurrentStartBoneId()}");
        // Logger.Instance.LogInfo($"{_ovrSkeleton.GetSkeletonType()} end bone id: {_ovrSkeleton.GetCurrentEndBoneId()}");
    }
}
