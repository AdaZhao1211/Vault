using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChild : MonoBehaviour
{

    [SerializeField]
    private Transform parent;
    // Start is called before the first frame update
    void Start()
    {

        Vector3 childLocalPosition = parent.InverseTransformPoint(this.transform.position);

        Debug.Log(childLocalPosition);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
