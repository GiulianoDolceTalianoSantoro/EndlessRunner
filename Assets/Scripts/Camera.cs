using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    private float offsetz;
    private float offsetx;

    // Start is called before the first frame update
    void Start()
    {
        offsetz = transform.position.z - target.transform.position.z;
        offsetx = transform.position.x - target.transform.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(target.transform.position.x + offsetx, transform.position.y, target.transform.position.z + offsetz);
    }
}
