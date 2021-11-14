using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
    public float velocidad;
    public int vida;
    public int bonificacion;
    public float costo;
    public AudioClip[] sonidos;
    public Sprite[] sprites;
    public GameObject[] powerUps;

    bool destruirInvisible = false;
    int ultimoHit;

    public ParticleSystem ps;
    bool playPS = true;
    protected bool atacar = true;

    protected GameController gc;

    private void Awake()
    {
        ps.Stop();
    }

    protected void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    protected void Update()
    {
        transform.position += new Vector3(0, -velocidad * Time.deltaTime, 0);

        if (gc.estadoActual == GameController.Estado.Victoria) vida = 0;

        if (vida <= 0) 
        {
            atacar = false;
            gameObject.tag = "NoEnemy";
            if (playPS) 
            {
                ps.Play();
                SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);
                playPS = false;
            }
            
            GetComponent<SpriteRenderer>().color = new Color(0f,0f,0f,0f);
            StartCoroutine(Destruir(1.2f));
        } 
    }

    protected void OnBecameVisible(){

        destruirInvisible = true;
    }

    protected void OnBecameInvisible(){
        
        if(destruirInvisible) Destroy(gameObject);
    }

    public void RestarVida(int valor, int jugador)
    {
        if (destruirInvisible)
        {
            // valor *= multiplicador
            vida -= valor;
            StartCoroutine(Asistencia(0.1f, jugador));
            
            if(vida <= 0)
            {
                gc.fuerzasRestantes -= costo;
                float nuevaEscala = Mathf.Clamp(gc.fuerzasRestantes / gc.fuerzasTotales, 0, gc.fuerzasRestantes);
                gc.barraFuerzas.transform.localScale = new Vector3(nuevaEscala, 1f, 1f);
                StartCoroutine(ParpadeoBarra(0.05f, 3));

                if (gc.dosJugadores)
                {
                    if (jugador == 1)
                    {
                        gc.p1Score += bonificacion;
                        StartCoroutine(ParpadeoScore(0.05f, 10, jugador));

                        if (ultimoHit == 2)
                        {
                            gc.p2Score += bonificacion;
                            StartCoroutine(ParpadeoScore(0.05f, 10, ultimoHit));
                        }

                    }

                    else if (jugador == 2)
                    {
                        gc.p2Score += bonificacion;
                        StartCoroutine(ParpadeoScore(0.05f, 10, jugador));

                        if (ultimoHit == 1)
                        {
                            gc.p1Score += bonificacion;
                            StartCoroutine(ParpadeoScore(0.05f, 10, ultimoHit));
                        }

                    }
                }
                else if (jugador != 3)
                {
                    gc.p1Score += bonificacion;
                    StartCoroutine(ParpadeoScore(0.05f, 10, 1));
                }

                int p = Random.Range(0, 101);

                if (p < 20) Instantiate(powerUps[Random.Range(0, powerUps.Length)], transform.position, Quaternion.identity);
            }
            StartCoroutine(AnimacionDaño(0.05f));
        }
    }

    IEnumerator Asistencia(float s, int jugador)
    {
        ultimoHit = jugador;
        yield return new WaitForSeconds(s);
        ultimoHit = 0;

    }

    IEnumerator ParpadeoScore(float s, int vueltas, int jugador)
    {
        if (jugador == 1) jugador = 0;
        else if (jugador == 2) jugador = 2;

        for (int i = 0; i < vueltas; i++)
        {
            gc.textosDatos[jugador].color = Color.yellow;
            yield return new WaitForSeconds(s);
            gc.textosDatos[jugador].color = Color.white;
            yield return new WaitForSeconds(s);
        }
    }

    IEnumerator ParpadeoBarra(float s, int vueltas)
    {
        for (int i = 0; i < vueltas; i++)
        {
            gc.barraFuerzas.color = Color.white;
            yield return new WaitForSeconds(s);
            gc.barraFuerzas.color = Color.red;
            yield return new WaitForSeconds(s);
        }
    }

    IEnumerator AnimacionDaño(float s)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[1];
        yield return new WaitForSeconds(s);
        GetComponent<SpriteRenderer>().sprite = sprites[0];
    }

    IEnumerator Destruir(float s)
    {
        yield return new WaitForSeconds(s);
        Destroy(gameObject);
    }
}
