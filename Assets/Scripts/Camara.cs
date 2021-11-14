using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    void Start()
    {
        if (Screen.height == 768) GetComponent<Camera>().orthographicSize = 8f;
        else GetComponent<Camera>().orthographicSize = 6.75f;
    }

}
