using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BoneInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro boneText;

    private OVRBone bone;

    public void AddBone(OVRBone bone) => this.bone = bone;

    void Update()
    {
        if (bone == null) return;
        boneText.text = $"{bone.Id}";
        transform.position = bone.Transform.position;
        transform.rotation = bone.Transform.rotation;
    }
}
