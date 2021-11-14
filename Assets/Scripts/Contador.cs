using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contador : MonoBehaviour
{
    public void Desactivar()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
