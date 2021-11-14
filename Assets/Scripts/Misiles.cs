using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misiles : MonoBehaviour
{
    public float velocidad;
    public bool derecho;
    public int numJugador;

    void Start()
    {
        if (derecho) GetComponent<Rigidbody2D>().velocity = new Vector3(velocidad, 0f, 0f);
        else 
        {
            GetComponent<SpriteRenderer>().flipX = true;
            GetComponent<Rigidbody2D>().velocity = new Vector3(-velocidad, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemigos>().RestarVida(1000, numJugador);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);        
    }
}
