using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataform : MonoBehaviour
{
    public Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameInvisible() 
    {
        Destroy(gameObject);
        GameManager.Instance.InstantiatePlataforms();
    }

    private void OnBecameVisible() 
    {

    }
}
