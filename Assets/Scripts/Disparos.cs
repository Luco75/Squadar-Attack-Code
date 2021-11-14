using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disparos : MonoBehaviour
{
    public enum Origen{J, E2, E3}
    public Origen origen;
    public int numJugador;
    public float velocidad;
    public AudioClip sonido;
    Vector3 objetivo, direccion;
    string[] tags = new string[2];

    Player jugador;
    GameController gc;


    // Start is called before the first frame update
    void Start()
    {
        tags[0] = "Player";
        tags[1] = "Player2";

        gc = GameObject.Find("GameController").GetComponent<GameController>();

        switch (origen){
            case Origen.J:  
                GetComponent<Rigidbody2D>().velocity = new Vector3(0, velocidad, 0);
                SoundManager.CrearSonido(sonido, transform.position, true, gc.rectVolEfectos);
                break;

            case Origen.E2: 
                GetComponent<Rigidbody2D>().velocity = new Vector3(0, -velocidad, 0); 
                SoundManager.CrearSonido(sonido, transform.position, false, gc.rectVolEfectos);
                break;

            case Origen.E3:
                if (gc.jugadoresVivos <= 0) Destroy(gameObject);

                //Si el modo dos jugadores esta activo y ambos jugadores estan vivos
                if (gc.dosJugadores && gc.p1Vidas >= 0 && gc.p2Vidas >= 0)
                {
                    jugador = GameObject.FindWithTag(tags[Random.Range(0, 2)]).GetComponent<Player>();
                }
                // Si no se dan las 3 condiciones
                else
                {
                    // puede que el jugador 2 haya muerto o que el modo de dos jugadores nunca estuvo activo
                    if ((gc.dosJugadores && gc.p1Vidas >= 0 && gc.p2Vidas < 0) || !gc.dosJugadores)
                    {
                        //en ambos casos solo se debe instanciar al jugador 1 y su posicion sera la objetivo
                        jugador = GameObject.FindWithTag("Player").GetComponent<Player>();
                    }
                    // O puede que el jugador 1 sea el que murio
                    else if (gc.dosJugadores && gc.p1Vidas < 0 && gc.p2Vidas >= 0)
                    {
                        //en este caso solo se debe instanciar al jugador 2 y su posicion sera la objetivo
                        jugador = GameObject.FindWithTag("Player2").GetComponent<Player>();
                    }
                }

                // Finalmente se calcula el trayecto entre la posicion inicial del disparo y la posicion objetivo
                objetivo = jugador.transform.position;
                direccion = (objetivo - transform.position).normalized;
                SoundManager.CrearSonido(sonido, transform.position, false, gc.rectVolEfectos);
                
                break;
        }  
    }

    // Update is called once per frame
    void Update()
    {
        if (origen == Origen.E3) transform.position += direccion * velocidad * Time.deltaTime;
    }

    protected void OnBecameInvisible(){
        
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy" && origen == Origen.J)
        {
            collision.gameObject.GetComponent<Enemigos>().RestarVida(1, numJugador);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Escudo" && (origen == Origen.E2  || origen == Origen.E3))
        {
            Destroy(gameObject);
        }

        if ((collision.gameObject.tag == "Player" || collision.gameObject.tag == "Player2") && origen != Origen.J)
        {
            collision.gameObject.GetComponent<Player>().Destruir();
            Destroy(gameObject);
        }
    }
}
