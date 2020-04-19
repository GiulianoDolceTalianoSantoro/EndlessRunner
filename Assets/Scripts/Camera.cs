using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position.z - target.transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, target.transform.position.z + offset);
    }
}
