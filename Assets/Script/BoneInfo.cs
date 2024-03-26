using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BoneInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro boneText;

    private OVRBone bone;
    private int num;

    public void AddBone(OVRBone bone, int num){
        this.bone = bone;
        this.num = num;
    } 

    void Update()
    {
        if (bone == null) return;
        boneText.text = num.ToString();
        transform.position = bone.Transform.position;
        transform.rotation = bone.Transform.rotation;
    }
}
