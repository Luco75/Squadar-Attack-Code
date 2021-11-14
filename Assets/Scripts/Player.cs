using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocidad; //velocidad de movimiento
    public float xmin169, xmax169, xmin219, xmax219, xmin43, xmax43; //limites de movimiento para cada resolucion en x
    public float ymin675, ymax675, ymin8, ymax8; //limites de movimiento para cada resolucion en y
    float xmax, xmin, ymax, ymin;//limites de movimiento finales, son los que se utilizan
    public float cadencia; //cadencia de los disparos
    public float tiempoPowerUp = 10f; //tiempo que duran los potenciadores
    public float tiempoMisil = 0.1f; //tiempo entre el disparo de un misil y otro
    public GameObject disparo1; //balas correspondientes al jugador 1
    public GameObject disparo2; //balas correspondientes al jugador 2
    //las balas estan diferenciadas para poder distinguir que jugador fue el ultimo en golpear al enemigo y poder computarlo bien en su score
    public GameObject laser;// es la bala que se utiliza, puede ser disparo 1 o disparo 2
    public GameObject p1MisilD, p1MisilI, p2MisilD, p2MisilI;
    public GameObject armasSecundarias; // contiene el sprite de las armas secundarias
    public GameObject escudo; // contiene el sprite del escudo
    public Transform armaDerecha, armaIzquierda, armaSecundariaDerecha, armaSecundariaIzquierda, misilDerecho, misilIzquierdo;
    // posiciones de donde salen los disparos
    
    public ParticleSystem ps; //hace el efecto de explosion al morir
    public AudioClip[] sonidos; //contiene sonidos propios del jugador como el de la explosion
    bool playPS = true;
    bool controlable = true;
    bool inmune;
    bool secundarias;
    bool primerAtaque = true;
    bool misilCargado = true;

    bool derecha, izquierda, arriba, abajo, dispara, noDispara, disparaMisil, esP1;
    //representan todas las acciones posibles

    public int index;

    GameController gc;

    private void Awake()
    {
        ps.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main.aspect >= 1.3f && Camera.main.aspect < 1.7f)//4:3
        {
            xmax = xmax43;
            xmin = xmin43;
        }
        else if (Camera.main.aspect >= 1.7f && Camera.main.aspect < 2.36f) //16:9 
        {
            if (Camera.main.orthographicSize == 8)
            {
                xmax = xmax169 + 2.2f;
                xmin = xmin169 - 2.2f;
            }
            else
            {
                xmax = xmax169;
                xmin = xmin169;
            }
        }
        else //21:9 
        {
            xmax = xmax219;
            xmin = xmin219;
        }

        if(Camera.main.orthographicSize == 6.75f) //6.75f
        {
            ymax = ymax675;
            ymin = ymin675;
        }
        else //8f
        {
            ymax = ymax8;
            ymin = ymin8;
        }

        armasSecundarias.SetActive(false);
        escudo.SetActive(false);
        inmune = true;
        gameObject.tag = "NoPlayer";
        StartCoroutine(EfectoInmune(0.05f, 25));

        gc = GameObject.Find("GameController").GetComponent<GameController>();
        esP1 = gc.p1Piloto == index;

        if (esP1) 
        { 
            transform.position = new Vector3(-2f, -5f, 0f);
            laser = disparo1;
            gc.p1Misiles = 5;
        } 
        else 
        { 
            transform.position = new Vector3(2f, -5f, 0f);
            laser = disparo2;
            gc.p2Misiles = 5;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(gc.estadoActual == GameController.Estado.Victoria) inmune = true;

        if (esP1)
        {
            arriba = gc.p1MoveUp;
            abajo = gc.p1MoveDown;
            derecha = gc.p1MoveRight;
            izquierda = gc.p1MoveLeft;
            dispara = gc.p1Fire;
            noDispara = gc.p1CancelFire;
            disparaMisil = gc.p1Misile;
        }
        else
        {
            arriba = gc.p2MoveUp;
            abajo = gc.p2MoveDown;
            derecha = gc.p2MoveRight;
            izquierda = gc.p2MoveLeft;
            dispara = gc.p2Fire;
            noDispara = gc.p2CancelFire;
            disparaMisil = gc.p2Misile;
        }

        if (controlable)
        {
            Movimiento();

            if (dispara && primerAtaque)
            {
                InvokeRepeating("GenerarBala", 0.001f, cadencia);
                primerAtaque = false;
            }
            else if (noDispara) 
            {
                CancelInvoke();
                primerAtaque = true;
            }

            bool existeMisil;

            if (esP1) existeMisil = gc.p1Misiles > 0;
            else existeMisil = gc.p2Misiles > 0;

            if (disparaMisil && misilCargado && existeMisil)
            {
                if (esP1)
                {
                    Instantiate(p1MisilD, misilDerecho.position, Quaternion.identity);
                    Instantiate(p1MisilI, misilIzquierdo.position, Quaternion.identity);
                    gc.p1Misiles--;
                    SoundManager.CrearSonido(sonidos[2], transform.position, false, gc.rectVolEfectos);
                }
                else
                {
                    Instantiate(p2MisilD, misilDerecho.position, Quaternion.identity);
                    Instantiate(p2MisilI, misilIzquierdo.position, Quaternion.identity);
                    gc.p2Misiles--;
                    SoundManager.CrearSonido(sonidos[2], transform.position, false, gc.rectVolEfectos);
                }

                misilCargado = false;
                StartCoroutine(CargarMisil(tiempoMisil));
            }
            
        }
          
    }

    void Movimiento(){

        if (arriba) transform.position += new Vector3(0f, velocidad, 0f) * Time.deltaTime;
        if (abajo) transform.position += new Vector3(0f, -velocidad, 0f) * Time.deltaTime;
        if (derecha) transform.position += new Vector3(velocidad, 0f, 0f) * Time.deltaTime;
        if (izquierda) transform.position += new Vector3(-velocidad, 0f, 0f) * Time.deltaTime;

        float rectX = Mathf.Clamp(transform.position.x, xmin, xmax);
        float rectY = Mathf.Clamp(transform.position.y, ymin, ymax);

        transform.position = new Vector3(rectX, rectY, 0);
    }

    void GenerarBala(){

        Instantiate(laser, armaDerecha.position, Quaternion.identity);
        Instantiate(laser, armaIzquierda.position, Quaternion.identity);

        if (secundarias)
        {
            Instantiate(laser, armaSecundariaDerecha.position, Quaternion.identity);
            Instantiate(laser, armaSecundariaIzquierda.position, Quaternion.identity);
        }
    }

    public void Destruir() 
    {
        if (!inmune)
        {
            gc.jugadoresVivos--;
            armasSecundarias.SetActive(false);
            gameObject.tag = "NoPlayer";
            CancelInvoke();
            controlable = false;
            SoundManager.CrearSonido(sonidos[0], transform.position, false, gc.rectVolEfectos);
            if (playPS)
            {
                ps.Play();
                playPS = false;
            }
            GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
            StartCoroutine(ReSpawn(2f));
        }
    }

    IEnumerator CargarMisil(float s)
    {
        yield return new WaitForSeconds(s);
        misilCargado = true;   
    }

    IEnumerator PowerUpArmas(float s)
    {
        if (esP1) gc.ActivarContador(1);
        else gc.ActivarContador(4);
            
        secundarias = true;
        armasSecundarias.SetActive(true);
        yield return new WaitForSeconds(s);
        secundarias = false;
        armasSecundarias.SetActive(false);
    }

    IEnumerator PowerUpEscudo(float s, float s1)
    {
        if (esP1) gc.ActivarContador(2);
        else gc.ActivarContador(3);
        inmune = true;
        escudo.SetActive(true);        
        yield return new WaitForSeconds(s);
        escudo.SetActive(false);
        yield return new WaitForSeconds(s1);
        inmune = false;
       
    }

    IEnumerator PowerUpDaño (float s)
    {
        if (esP1) gc.ActivarContador(0);
        else gc.ActivarContador(5);
        cadencia = 0.080f;
        CancelInvoke();
        primerAtaque = true;
        yield return new WaitForSeconds(s);
        cadencia = 0.15f;
        CancelInvoke();
        primerAtaque = true;
    }

    IEnumerator ReSpawn(float s)
    {
        yield return new WaitForSeconds(s);
        gc.NuevoJugador(esP1);
        Destroy(gameObject);
    }

    IEnumerator EfectoInmune(float s, int vueltas)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        for (int i = 0; i < vueltas; i++)
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(s);
            sr.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(s);
        }

        sr.color = new Color(1f, 1f, 1f, 1f);
        
        if(!escudo.activeSelf) inmune = false;

        if (esP1)
        {
            gameObject.tag = "Player";
            gc.jugadoresVivos++;
        }
        else
        {
            gameObject.tag = "Player2";
            gc.jugadoresVivos++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy" && !inmune && (gameObject.tag == "Player" || gameObject.tag == "Player2"))
        {
            collision.gameObject.GetComponent<Enemigos>().RestarVida(1000, 3);
            Destruir();
        }

        if (collision.gameObject.tag == "PowerUpDaño")
        {
            SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);
            StartCoroutine(PowerUpDaño(tiempoPowerUp));
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "PowerUpArmas")
        {
            SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);
            StartCoroutine(PowerUpArmas(tiempoPowerUp));
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "PowerUpEscudo")
        {
            SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);
            StartCoroutine(PowerUpEscudo(tiempoPowerUp, 0.5f));
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "PowerUpMisiles")
        {
            SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);
            if(esP1) gc.p1Misiles ++;
            else gc.p2Misiles ++;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "PowerUpBomba")
        {
            SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);

            GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject e in enemigos)
            {
                gc.fuerzasRestantes -= e.GetComponent<Enemigos>().costo;
                float nuevaEscala = Mathf.Clamp(gc.fuerzasRestantes / gc.fuerzasTotales, 0, gc.fuerzasRestantes);
                gc.barraFuerzas.transform.localScale = new Vector3(nuevaEscala, 1f, 1f);
                if (esP1) gc.p1Score += e.GetComponent<Enemigos>().bonificacion;
                else gc.p2Score += e.GetComponent<Enemigos>().bonificacion;
                e.GetComponent<Enemigos>().vida = -1;
            }

            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "PowerUpVida")
        {
            SoundManager.CrearSonido(sonidos[1], transform.position, false, gc.rectVolEfectos);
            if (esP1) gc.p1Vidas++;
            else gc.p2Vidas++;
            Destroy(collision.gameObject);
        }
    }
}
