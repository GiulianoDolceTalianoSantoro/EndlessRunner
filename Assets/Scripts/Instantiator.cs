using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public Plataform[] plataforms;

    int instantiateTime;

    // Start is called before the first frame update
    void Start()
    {
        instantiateTime = 0;

        InstantiatePlataforms();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InstantiatePlataforms()
    {
        var plataformIndex = Random.Range(0, plataforms.Length);
        Plataform plataform = plataforms[plataformIndex];

        if (instantiateTime == 0)
        {
            Plataform firstPlataform = Instantiate(plataform, new Vector3(0f, 0f, 0f), Quaternion.identity);
            Plataform secondPlataform = Instantiate(plataform, new Vector3(0f, 0f, firstPlataform.transform.localScale.z), Quaternion.identity);
            instantiateTime = 1;
        }
        else
        {
            Plataform newPlataform = Instantiate(plataform, plataform.initialPosition + new Vector3(0f, 0f, instantiateTime * plataform.transform.localScale.z), Quaternion.identity);
        }

        instantiateTime++;
    }
}
