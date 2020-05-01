using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataform : MonoBehaviour
{
    Instantiator instantiator;

    public Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        instantiator = FindObjectOfType<Instantiator>();

        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameInvisible() 
    {
        Destroy(gameObject);
        instantiator.InstantiatePlataforms();
    }

    private void OnBecameVisible() 
    {

    }
}
