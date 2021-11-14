using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public static void CrearSonido(AudioClip clip, Vector3 posicion, bool tonoVariable, float volumen)
    {
        GameObject sonido = new GameObject("Sonido");
        AudioSource audioSource = sonido.AddComponent<AudioSource>();
        sonido.transform.position = posicion;
        audioSource.clip = clip;
        if (tonoVariable) audioSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
        audioSource.volume = volumen;
        audioSource.Play();
        sonido.AddComponent<DestroySound>();
    }
}
