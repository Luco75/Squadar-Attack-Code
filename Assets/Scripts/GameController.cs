using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum Estado {Principal, Seleccion, Opciones, Controles, Sonido, Juego, Derrota, Victoria, Pausa1, Pausa2, Creditos}
    public Estado estadoActual;

    [Header("INTERFAZ")] //corresponden a los distintos menus del juego y elementos de la interfaz
    public GameObject menuPrincipal; //menu principal
    public GameObject menuSeleccion; //seleccion de naves
    public GameObject menuOpciones; //opciones del juego
    public GameObject listaDeControles; // muestra la lista de comandos de teclado y joystick
    public GameObject menuSonidos; //configuracion del volumen de la musica y efectos de sonido
    public GameObject menuPausa; //aparece al pausar el juego, permite cerrarlo o volver al menu principal
    public GameObject mensajeVictoria;  //texto de victoria
    public GameObject mensajeDerrota; //texto de mision fallida
    public GameObject creditos; //muestra los creditos
    public GameObject datosJugadores; // menu con la informacion de los jugadores y el nivel en general (vidas, score, misiles y fuerzas enemigas)
    

    // Las siguientes declaraciones corresponden a los textos y elementos secundarios de los menus anteriores
    public Text[] textosPrincipal;
    public Text[] textosPausa;
    public Text[] textosDatos; 
    public Text[] textosOpciones;
    public Text[] textosSonidos;
    public Text[] valoresSonidos;
    public Image[] pilotosSeleccion;
    public GameObject[] contadores;
    public GameObject[] cursoresSeleccion;
    public GameObject flechas;
    public GameObject flechasB;
    public GameObject datosJugador1; 
    public GameObject datosJugador2; 
    public Image barraFuerzas; 

    [Header("OBJETOS DE LA ESCENA")]
    public GameObject[] pilotos; //contiene prefabs de naves del jugador
    public GameObject generador; //contiene prefab del generador de enemigos
    

    [Header("VARIABLES DE JUGADOR")]
    public int p1Piloto; //indica cual es la nave del jugador 1
    public int p2Piloto; //indica cual es la nave del jugador 2
    public int p1Vidas; //numero de vidas del jugador 1
    public int p2Vidas; //numero de vidas del jugador 2
    public int p1Misiles; //numero de misiles del jugador 1
    public int p2Misiles; //numero de vidas del jugador 1
    public int p1Score; //puntaje del jugador 1
    public int p2Score; //puntaje del jugador 2
    public float fuerzasTotales; //fuerzas totales de enemigos que deben eliminarse
    public float fuerzasRestantes; //fuerzas restantes de enemigos
    public int jugadoresVivos = 0; //cada vez que se instancia un jugador se incrementa en 1 y cada vez que un jugador muere se disminuye en 1


    [HideInInspector]
    public bool p1Start, p1Up, p1Down, p1Left, p1Right, p1Ok, p1Back, p1Fire, p1Misile, p1CancelFire, p1Special, p1CC, p1MoveUp, p1MoveDown, p1MoveRight, p1MoveLeft;
    //guardan todas las posibles acciones del jugador 1

    [HideInInspector]
    public bool p2Start, p2Up, p2Down, p2Left, p2Right, p2Ok, p2Back, p2Fire, p2Misile, p2CancelFire, p2Special, p2CC, p2MoveUp, p2MoveDown, p2MoveRight, p2MoveLeft;
    //guardan todas las posibles acciones del jugador 2
    float lastYpos, lastYneg, lastXpos, lastXneg, lastYpos2, lastYneg2, lastXpos2, lastXneg2;
    int joystickOK; //guarda el numero de joystick conectados
    
    [HideInInspector]
    public bool dosJugadores;// indica si hay 2 jugadores jugando
    
    int indice, indiceB; //indices que se utilizan para los menus
    
    AudioSource audioSource; //contiene la musica del juego
    public AudioClip[] sonidos; //contiene los sonidos de la interfaz
    float volMusica; //volumen de la musica (lineal)
    float volEfectos; //volumen de los efectos (lineal)
    [HideInInspector]
    public float rectVolMusica; //volumen de la musica (no lineal)
    [HideInInspector]
    public float rectVolEfectos; //volumen de los efectos corregido (no lineal)


    void Start()
    {
        Cursor.visible = false;
        jugadoresVivos = 0;
        volEfectos = 1;
        volMusica = 1;
        rectVolEfectos = volEfectos * volEfectos;
        rectVolMusica = volMusica * volMusica;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = rectVolMusica;

        menuPrincipal.SetActive(true);
        menuSeleccion.SetActive(false);
        menuOpciones.SetActive(false);
        menuPausa.SetActive(false);
        mensajeVictoria.SetActive(false);
        mensajeDerrota.SetActive(false);
        datosJugadores.SetActive(false);
        listaDeControles.SetActive(false);
        menuSonidos.SetActive(false);
        flechas.SetActive(false);
        creditos.SetActive(false);

        for (int i = 0; i < contadores.Length; i++)
        {
            contadores[i].SetActive(false);
        }

        indice = 0;
        indiceB = 1;

        p1Piloto = -1;
        p2Piloto = -1;

        p1Vidas = 3;
        p2Vidas = 3;

        fuerzasRestantes = fuerzasTotales;
    }


    void Update()
    {
        Controles();

        Time.timeScale = (estadoActual == Estado.Pausa1 || estadoActual == Estado.Pausa2) ? 0f : 1f;

        audioSource.volume = rectVolMusica;

        switch (estadoActual)
        {
            case Estado.Principal:

                if (p1Down) 
                {
                    indice++;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                }
                if (p1Up) 
                {
                    indice--;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                } 

                if (indice == textosPrincipal.Length) indice = 0;
                if (indice < 0) indice = textosPrincipal.Length - 1;

                for (int i = 0; i < textosPrincipal.Length; i++)
                {
                    if (i == indice) textosPrincipal[i].color = Color.green;
                    else textosPrincipal[i].color = Color.white;
                }

                if (p1Ok)
                {
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);

                    switch (indice)
                    {
                        case 0:
                            menuPrincipal.SetActive(false);
                            menuSeleccion.SetActive(true);
                            dosJugadores = false;
                            indice = 0;
                            estadoActual = Estado.Seleccion;
                            break;
                        case 1:
                            menuPrincipal.SetActive(false);
                            menuSeleccion.SetActive(true);
                            dosJugadores = true;
                            indice = 0;
                            estadoActual = Estado.Seleccion;
                            break;
                        case 2:
                            indice = 0;
                            menuPrincipal.SetActive(false);
                            menuOpciones.SetActive(true);
                            estadoActual = Estado.Opciones;
                            break;
                        case 3:
                            menuPrincipal.SetActive(false);
                            creditos.SetActive(true);
                            estadoActual = Estado.Creditos;
                            break;
                        case 4:
                            Application.Quit();
                            break;
                    }
                }

            break;

            case Estado.Seleccion:

                if (!dosJugadores) cursoresSeleccion[1].SetActive(false);
                else cursoresSeleccion[1].SetActive(true);

                if(p1Piloto == -1)
                {
                    if (p1Right) 
                    {
                        SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                        indice++;
                    } 
                    if (p1Left)
                    {
                        SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                        indice--;
                    }

                    if (indice == pilotosSeleccion.Length) indice = 0;
                    if (indice < 0) indice = pilotosSeleccion.Length - 1;
                }


                if (p2Piloto == -1)
                {
                    if (p2Right)
                    {
                        SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                        indiceB++;
                    }

                    if (p2Left) 
                    {
                        SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                        indiceB--;
                    } 

                    if (indiceB == pilotosSeleccion.Length) indiceB = 0;
                    if (indiceB < 0) indiceB = pilotosSeleccion.Length - 1;
                }
                
                cursoresSeleccion[0].transform.position = pilotosSeleccion[indice].transform.position;
                cursoresSeleccion[1].transform.position = pilotosSeleccion[indiceB].transform.position - new Vector3(0f,3f,0f);

                if (p1Ok) 
                {
                    p1Piloto = indice;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                }
                if (p2Ok) 
                {
                    p2Piloto = indiceB;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                }
                

                if (p1Piloto == 0 && p1Piloto == p2Piloto)
                {
                    p2Piloto = 2;
                }
                else if (p1Piloto == 1 && p1Piloto == p2Piloto)
                {
                    p2Piloto = 3;
                }

                if (p1Back)
                {
                    p1Piloto = -1;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                }
                
                if (p2Back)
                {
                    p2Piloto = -1;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                }

                if (p1Back && p1Piloto == -1)
                {
                    dosJugadores = false;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                    menuPrincipal.SetActive(true);
                    menuSeleccion.SetActive(false);
                    estadoActual = Estado.Principal;
                    p1Piloto = -1;
                    p2Piloto = -1;
                }

                if (p1Piloto != -1)
                {
                    if (!dosJugadores)
                    {
                        menuSeleccion.SetActive(false);
                        datosJugadores.SetActive(true);
                        datosJugador2.SetActive(false);
                        Instantiate(pilotos[p1Piloto], transform.position, Quaternion.identity);
                        Instantiate(generador, transform.position, Quaternion.identity);

                        estadoActual = Estado.Juego;
                    }
                    else if (p2Piloto != -1)
                    {
                        menuSeleccion.SetActive(false);
                        datosJugadores.SetActive(true);
                        datosJugador2.SetActive(true);
                        Instantiate(pilotos[p1Piloto], transform.position, Quaternion.identity);
                        Instantiate(pilotos[p2Piloto], transform.position, Quaternion.identity);
                        Instantiate(generador, transform.position, Quaternion.identity);

                        estadoActual = Estado.Juego;
                    }
                }

                break;

            case Estado.Opciones:

                if (p1Down)
                {
                    indice++;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                }
                if (p1Up)
                {
                    indice--;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                }

                if (indice == textosOpciones.Length) indice = 0;
                if (indice < 0) indice = textosOpciones.Length - 1;

                for (int i = 0; i < textosOpciones.Length; i++)
                {
                    if (i == indice) textosOpciones[i].color = Color.green;
                    else textosOpciones[i].color = Color.white;
                }

                if (p1Back)
                {
                    menuOpciones.SetActive(false);
                    menuPrincipal.SetActive(true);
                    indice = 3;
                    estadoActual = Estado.Principal;
                }

                if (p1Ok)
                {
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);

                    switch (indice)
                    {
                        case 0:
                            menuOpciones.SetActive(false);
                            listaDeControles.SetActive(true);
                            flechas.SetActive(true);
                            indice = 0;
                            listaDeControles.transform.position = new Vector3(0f, 8f, 0f);
                            estadoActual = Estado.Controles;
                            break;
                        case 1:
                            menuOpciones.SetActive(false);
                            menuSonidos.SetActive(true);
                            indice = 0;
                            estadoActual = Estado.Sonido;
                            break;
                        case 2:
                            menuOpciones.SetActive(false);
                            menuPrincipal.SetActive(true);
                            indice = 2;
                            estadoActual = Estado.Principal;
                            break;
                    }
                }

                break;

            case Estado.Controles:
                float v = Input.GetAxis("Vertical");

                float velocidad = 10f;

                listaDeControles.transform.position += new Vector3(0f, velocidad * -v * Time.deltaTime, 0f);

                float rectX = listaDeControles.transform.position.x;
                float rectY = Mathf.Clamp(listaDeControles.transform.position.y, 8f, 30f);

                listaDeControles.transform.position = new Vector3(rectX, rectY, 0f);

                if (p1Back)
                {
                    listaDeControles.SetActive(false);
                    flechas.SetActive(false);
                    menuOpciones.SetActive(true);
                    indice = 0;
                    estadoActual = Estado.Opciones;
                }

                break;

            case Estado.Sonido:
                if (p1Down)
                {
                    indice++;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                }
                if (p1Up)
                {
                    indice--;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                }

                if (indice == textosSonidos.Length) indice = 0;
                if (indice < 0) indice = textosSonidos.Length - 1;

                for (int i = 0; i < textosSonidos.Length; i++)
                {
                    if (i == indice) textosSonidos[i].color = Color.green;
                    else textosSonidos[i].color = Color.white;
                }

                if (indice != 2) 
                {
                    flechasB.SetActive(true);
                    flechasB.transform.position = valoresSonidos[indice].transform.position;
                }
                else
                {
                    flechasB.SetActive(false);
                }

                if (p1Right) 
                {
                    if (indice == 0) volMusica += 0.1f;
                    else volEfectos += 0.1f;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                } 
                else if (p1Left)
                {
                    if (indice == 0) volMusica -= 0.1f;
                    else volEfectos -= 0.1f;
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                }

                if (volEfectos < 0) volEfectos = 0;
                if (volEfectos > 1) volEfectos = 1;
                
                if (volMusica < 0) volMusica = 0;
                if (volMusica > 1) volMusica = 1;

                valoresSonidos[0].text = ((int)Mathf.Round(volMusica * 100)).ToString();
                valoresSonidos[1].text = ((int)Mathf.Round(volEfectos * 100)).ToString();

                rectVolEfectos = volEfectos * volEfectos;
                rectVolMusica = volMusica * volMusica;

                

                if (p1Back || (p1Ok && indice == 2))
                {
                    menuSonidos.SetActive(false);
                    menuOpciones.SetActive(true);
                    indice = 1;
                    estadoActual = Estado.Opciones;
                }

                break;

            case Estado.Juego:

                textosDatos[0].text = p1Score.ToString();
                textosDatos[1].text = " x " + p1Vidas.ToString();
                textosDatos[2].text = p2Score.ToString();
                textosDatos[3].text = " x " + p2Vidas.ToString();
                textosDatos[4].text = " x " + p1Misiles.ToString();
                textosDatos[5].text = " x " + p2Misiles.ToString();

                if(fuerzasRestantes <= 0) 
                {
                    mensajeVictoria.SetActive(true);
                    datosJugadores.SetActive(false);
                    
                    estadoActual = Estado.Victoria;
                }

                if (p1Start)
                {
                    indice = 0;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                    menuPausa.SetActive(true);
                    audioSource.Pause();
                    estadoActual = Estado.Pausa1;
                }

                if (p2Start)
                {
                    indice = 0;
                    SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                    menuPausa.SetActive(true);
                    audioSource.Pause();
                    estadoActual = Estado.Pausa2;
                }

                break;

            case Estado.Victoria:

                Invoke("Creditos", 5f);

                break;

            case Estado.Derrota:

                Invoke("Recargar", 5f);

                break;

            case Estado.Pausa1:
                if (p1Down) 
                {
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                    indice++;
                } 
                if (p1Up)
                {
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                    indice--;
                }

                if (indice == textosPausa.Length) indice = 0;
                if (indice < 0) indice = textosPausa.Length - 1;

                for (int i = 0; i < textosPausa.Length; i++)
                {
                    if (i == indice) textosPausa[i].color = Color.green;
                    else textosPausa[i].color = Color.white;
                }

                if (p1Ok)
                {
                    switch (indice)
                    {
                        case 0:
                            menuPausa.SetActive(false);
                            SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                            audioSource.Play();
                            estadoActual = Estado.Juego;
                            break;
                        case 1:
                            SceneManager.LoadScene("EscenaPrincipal");
                            break;
                        case 2:
                            Application.Quit();
                            break;
                    }
                }
                break;

            case Estado.Pausa2:
                if (p2Down)
                {
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                    indice++;
                }
                if (p2Up)
                {
                    SoundManager.CrearSonido(sonidos[0], transform.position, false, rectVolEfectos);
                    indice--;
                }

                if (indice == textosPausa.Length) indice = 0;
                if (indice < 0) indice = textosPausa.Length - 1;

                for (int i = 0; i < textosPausa.Length; i++)
                {
                    if (i == indice) textosPausa[i].color = Color.green;
                    else textosPausa[i].color = Color.white;
                }

                if (p2Ok)
                {
                    switch (indice)
                    {
                        case 0:
                            menuPausa.SetActive(false);
                            SoundManager.CrearSonido(sonidos[1], transform.position, false, rectVolEfectos);
                            audioSource.Play();
                            estadoActual = Estado.Juego;
                            break;
                        case 1:
                            SceneManager.LoadScene("EscenaPrincipal");
                            break;
                        case 2:
                            Application.Quit();
                            break;
                    }
                }
                break;
            case Estado.Creditos:
                if (p1Back)
                {
                    creditos.SetActive(false);
                    menuPrincipal.SetActive(true);
                    estadoActual = Estado.Principal;
                }
                break;

        }
    }

    public void ActivarContador(int contador)
    {
        contadores[contador].SetActive(false);
        contadores[contador].SetActive(true);
    }

    void Recargar()
    {
        SceneManager.LoadScene("EscenaPrincipal");
    }

    void Creditos()
    {
        mensajeVictoria.SetActive(false);
        creditos.SetActive(true);
        Invoke("Recargar", 10f);
    }

    public void NuevoJugador(bool player1)
    {
        if (player1)
        {
            p1Vidas--;
            if (p1Vidas >= 0) 
            { 
                Instantiate(pilotos[p1Piloto], transform.position, Quaternion.identity);
                
            } 
            else if (!dosJugadores || (dosJugadores && p2Vidas < 0)) 
            {
                datosJugadores.SetActive(false);
                mensajeDerrota.SetActive(true);
                estadoActual = Estado.Derrota;
            }
        }

        else 
        { 
            p2Vidas--;
            if (p2Vidas >= 0) 
            {
                Instantiate(pilotos[p2Piloto], transform.position, Quaternion.identity);
                
            } 
            else if (p1Vidas < 0) 
            {
                datosJugadores.SetActive(false);
                mensajeDerrota.SetActive(true);
                estadoActual = Estado.Derrota;
            }
        }
    }

    void Controles()
    {
        int joystickOK = 0;

        // Si no hay ningun joystick conectado o se desconecta el unico joystick que habia conectado
        if (Input.GetJoystickNames().Length == 0 || Input.GetJoystickNames()[0] == "") joystickOK = 0;
        // Si hay un joystick conectado y no esta conectado un segundo joystick
        if (Input.GetJoystickNames().Length == 1 && Input.GetJoystickNames()[0] != "") joystickOK = 1;
        //Si tambien esta conectado el segundo joystick
        if (Input.GetJoystickNames().Length == 2) joystickOK = 2;

        switch (joystickOK)
        {
            case 0:

                if (!dosJugadores)
                {
                    p1Start = p1Ok = Input.GetKeyDown(KeyCode.Escape);
                    p1Ok = Input.GetKeyDown(KeyCode.Return);
                    p1Back = Input.GetKeyDown(KeyCode.Escape);
                    p1Fire = Input.GetKey(KeyCode.F);
                    p1CancelFire = Input.GetKeyUp(KeyCode.F);
                    p1Misile = Input.GetKeyDown(KeyCode.G);
                    p1Up = Input.GetKeyDown(KeyCode.UpArrow);
                    p1Down = Input.GetKeyDown(KeyCode.DownArrow);
                    p1Right = Input.GetKeyDown(KeyCode.RightArrow);
                    p1Left = Input.GetKeyDown(KeyCode.LeftArrow);
                    p1MoveUp = Input.GetKey(KeyCode.UpArrow);
                    p1MoveDown = Input.GetKey(KeyCode.DownArrow);
                    p1MoveRight = Input.GetKey(KeyCode.RightArrow);
                    p1MoveLeft = Input.GetKey(KeyCode.LeftArrow);
                    
                }
                else
                {
                    //Ambos jugadores juegan con teclado
                    p1Start = p1Ok = Input.GetKeyDown(KeyCode.Escape);
                    p1Ok = Input.GetKeyDown(KeyCode.Return);
                    p1Back = Input.GetKeyDown(KeyCode.Escape);
                    p1Fire = Input.GetKey(KeyCode.F);
                    p1CancelFire = Input.GetKeyUp(KeyCode.F);
                    p1Misile = Input.GetKeyDown(KeyCode.G);
                    p1Up = Input.GetKeyDown(KeyCode.W);
                    p1Down = Input.GetKeyDown(KeyCode.S);
                    p1Right = Input.GetKeyDown(KeyCode.D);
                    p1Left = Input.GetKeyDown(KeyCode.A);
                    p1MoveUp = Input.GetKey(KeyCode.W);
                    p1MoveDown = Input.GetKey(KeyCode.S);
                    p1MoveRight = Input.GetKey(KeyCode.D);
                    p1MoveLeft = Input.GetKey(KeyCode.A);

                    p2Start = Input.GetKeyDown(KeyCode.Keypad0);
                    p2Ok = Input.GetKeyDown(KeyCode.RightControl);
                    p2Back = Input.GetKeyDown(KeyCode.Backspace);
                    p2Fire = Input.GetKey(KeyCode.RightControl);
                    p2CancelFire = Input.GetKeyUp(KeyCode.RightControl);
                    p2Misile = Input.GetKeyDown(KeyCode.RightShift);
                    p2Up = Input.GetKeyDown(KeyCode.UpArrow);
                    p2Down = Input.GetKeyDown(KeyCode.DownArrow);
                    p2Right = Input.GetKeyDown(KeyCode.RightArrow);
                    p2Left = Input.GetKeyDown(KeyCode.LeftArrow);
                    p2MoveUp = Input.GetKey(KeyCode.UpArrow);
                    p2MoveDown = Input.GetKey(KeyCode.DownArrow);
                    p2MoveRight = Input.GetKey(KeyCode.RightArrow);
                    p2MoveLeft = Input.GetKey(KeyCode.LeftArrow);

                }

                break;

            case 1:
                // Jugador 1 juega con joystick y Jugador 2 con teclado
                p1Start = Input.GetKeyDown(KeyCode.Joystick1Button7);
                p1Ok = Input.GetKeyDown(KeyCode.Joystick1Button0);
                p1Back = Input.GetKeyDown(KeyCode.Joystick1Button1);
                p1Fire = Input.GetKey(KeyCode.Joystick1Button2);
                p1CancelFire = Input.GetKeyUp(KeyCode.Joystick1Button2);
                p1Misile = Input.GetKeyDown(KeyCode.Joystick1Button5);
                p1Up = DpadUp1();
                p1Down = DpadDown1();
                p1Right = DpadRight1();
                p1Left = DpadLeft1();
                p1MoveUp = Input.GetAxis("DpadV") > 0.1f;
                p1MoveDown = Input.GetAxis("DpadV") < -0.1;
                p1MoveRight = Input.GetAxis("DpadH") > 0.1f;
                p1MoveLeft = Input.GetAxis("DpadH") < -0.1;

                p2Start = Input.GetKeyDown(KeyCode.Alpha1);
                p2Ok = Input.GetKeyDown(KeyCode.Return);
                p2Back = Input.GetKeyDown(KeyCode.Escape);
                p2Fire = Input.GetKey(KeyCode.F);
                p2CancelFire = Input.GetKeyUp(KeyCode.F);
                p2Misile = Input.GetKeyDown(KeyCode.G);
                p2Up = Input.GetKeyDown(KeyCode.UpArrow);
                p2Down = Input.GetKeyDown(KeyCode.DownArrow);
                p2Right = Input.GetKeyDown(KeyCode.RightArrow);
                p2Left = Input.GetKeyDown(KeyCode.LeftArrow);
                p2MoveUp = Input.GetKey(KeyCode.UpArrow);
                p2MoveDown = Input.GetKey(KeyCode.DownArrow);
                p2MoveRight = Input.GetKey(KeyCode.RightArrow);
                p2MoveLeft = Input.GetKey(KeyCode.LeftArrow);
                break;

            case 2:
                //Ambos jugadores juegan con joystick
                p1Start = Input.GetKeyDown(KeyCode.Joystick1Button7);
                p1Ok = Input.GetKeyDown(KeyCode.Joystick1Button0);
                p1Back = Input.GetKeyDown(KeyCode.Joystick1Button1);
                p1Fire = Input.GetKey(KeyCode.Joystick1Button2);
                p1CancelFire = Input.GetKeyUp(KeyCode.Joystick1Button2);
                p1Misile = Input.GetKeyDown(KeyCode.Joystick1Button5);
                p1Up = DpadUp1();
                p1Down = DpadDown1();
                p1Right = DpadRight1();
                p1Left = DpadLeft1();
                p1MoveUp = Input.GetAxis("DpadV") > 0.1f;
                p1MoveDown = Input.GetAxis("DpadV") < -0.1;
                p1MoveRight = Input.GetAxis("DpadH") > 0.1f;
                p1MoveLeft = Input.GetAxis("DpadH") < -0.1;

                p2Start = Input.GetKeyDown(KeyCode.Joystick2Button7);
                p2Ok = Input.GetKeyDown(KeyCode.Joystick2Button0);
                p2Back = Input.GetKeyDown(KeyCode.Joystick2Button1);
                p2Fire = Input.GetKey(KeyCode.Joystick2Button2);
                p2CancelFire = Input.GetKeyUp(KeyCode.Joystick2Button2);
                p2Misile = Input.GetKeyDown(KeyCode.Joystick2Button5);
                p2Up = DpadUp2();
                p2Down = DpadDown2();
                p2Right = DpadRight2();
                p2Left = DpadLeft2();
                p2MoveUp = Input.GetAxis("DpadVp2") > 0.1f;
                p2MoveDown = Input.GetAxis("DpadVp2") < -0.1;
                p2MoveRight = Input.GetAxis("DpadHp2") > 0.1f;
                p2MoveLeft = Input.GetAxis("DpadHp2") < -0.1;
                break;
        }
    }

    // Las siguientes funciones hacen que los cursores del joystick se tomen como botones(respondan una unica vez) y no como ejes. Dejar al final
    bool DpadUp1()
    {
        bool push = false;

        float eje = Input.GetAxis("DpadV");

        if (lastYpos != eje)
        {
            if (eje == 1)
            {
                push = true;
            }
        }
        lastYpos = eje;
        return push;
    }

    bool DpadDown1()
    {
        bool push = false;
        float eje = Input.GetAxis("DpadV");

        if (lastYneg != eje)
        {
            if (eje == -1)
            {
                push = true;
            }
        }
        lastYneg = eje;
        return push;
    }

    bool DpadRight1()
    {
        bool push = false;

        float eje = Input.GetAxis("DpadH");

        if (lastXpos != eje)
        {
            if (eje == 1)
            {
                push = true;
            }
        }
        lastXpos = eje;
        return push;
    }

    bool DpadLeft1()
    {
        bool push = false;
        float eje = Input.GetAxis("DpadH");

        if (lastXneg != eje)
        {
            if (eje == -1)
            {
                push = true;
            }
        }
        lastXneg = eje;
        return push;
    }

    bool DpadUp2()
    {
        bool push = false;

        float eje = Input.GetAxis("DpadVp2");

        if (lastYpos2 != eje)
        {
            if (eje == 1)
            {
                push = true;
            }
        }
        lastYpos2 = eje;
        return push;
    }

    bool DpadDown2()
    {
        bool push = false;
        float eje = Input.GetAxis("DpadVp2");

        if (lastYneg2 != eje)
        {
            if (eje == -1)
            {
                push = true;
            }
        }
        lastYneg2 = eje;
        return push;
    }

    bool DpadRight2()
    {
        bool push = false;

        float eje = Input.GetAxis("DpadHp2");

        if (lastXpos2 != eje)
        {
            if (eje == 1)
            {
                push = true;
            }
        }
        lastXpos2 = eje;
        return push;
    }

    bool DpadLeft2()
    {
        bool push = false;
        float eje = Input.GetAxis("DpadHp2");

        if (lastXneg2 != eje)
        {
            if (eje == -1)
            {
                push = true;
            }
        }
        lastXneg2 = eje;
        return push;
    }
}


