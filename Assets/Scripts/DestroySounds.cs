using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Este script se le añade a los sonidos cuando son creados
El componente AudioSource se añade previamente, por lo que siempre se podra crear una instancia del mismo 
Mediante este script se elimina al sonido de la escena una vez que completo la reproduccion del clip de audio
*/

public class DestroySound : MonoBehaviour
{
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(DestruirSonido(audioSource.clip.length));
    }

    IEnumerator DestruirSonido(float s)
    {
        yield return new WaitForSeconds(s);
        Destroy(gameObject);
    }
}
