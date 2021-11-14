using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public enum Tipos {Daño, Armas, Escudo, Misiles, Bomba, Vida}
    public Tipos esteTipo;
    public Sprite[] sprites;
    int esteSprite;
    public float velocidad = 0.25f;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector3(0f, -velocidad, 0f);

        switch (esteTipo)
        {
            case Tipos.Daño:
                gameObject.tag = "PowerUpDaño";
                GetComponent<SpriteRenderer>().sprite = sprites[0];
                esteSprite = 0;
                break;
            case Tipos.Armas:
                gameObject.tag = "PowerUpArmas";
                GetComponent<SpriteRenderer>().sprite = sprites[1];
                esteSprite = 1;
                break;
            case Tipos.Escudo:
                gameObject.tag = "PowerUpEscudo";
                GetComponent<SpriteRenderer>().sprite = sprites[2];
                esteSprite = 2;
                break;
            case Tipos.Misiles:
                gameObject.tag = "PowerUpMisiles";
                GetComponent<SpriteRenderer>().sprite = sprites[3];
                esteSprite = 3;
                break;
            case Tipos.Bomba:
                gameObject.tag = "PowerUpBomba";
                GetComponent<SpriteRenderer>().sprite = sprites[4];
                esteSprite = 4;
                break;
            case Tipos.Vida:
                gameObject.tag = "PowerUpVida";
                GetComponent<SpriteRenderer>().sprite = sprites[5];
                esteSprite = 5;
                break;
        }

        StartCoroutine(Animacion(0.05f, 1f));
    }

    private void Update()
    {
        
    }

    IEnumerator Animacion(float s, float s1)
    {
        yield return new WaitForSeconds(s1);
        GetComponent<SpriteRenderer>().sprite = sprites[6];
        yield return new WaitForSeconds(s);
        GetComponent<SpriteRenderer>().sprite = sprites[esteSprite];
        StartCoroutine(Animacion(0.05f, 1f));
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
