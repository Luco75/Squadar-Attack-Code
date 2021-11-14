using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generador : MonoBehaviour
{
    public GameObject[] naves;
    public float cadencia;
    public float xmin169, xmax169, xmin43, xmax43, xmax219, xmin219;
    float xmax, xmin;
    public Vector3 startPosition;
    GameController gc;

    void Start()
    {
        if (Camera.main.aspect >= 1.3f && Camera.main.aspect < 1.7f)//4:3
        {
            xmax = xmax43;
            xmin = xmin43;
        }
        else if (Camera.main.aspect >= 1.7f && Camera.main.aspect < 2.36f) //16:9 
        {
            if(Camera.main.orthographicSize == 8)
            {
                xmax = xmax169 + 2f;
                xmin = xmin169 + 2f;
            }
            else
            {
                xmax = xmax169;
                xmin = xmin169;
            }
            
        }
        else
        {
            xmax = xmax219;
            xmin = xmin219;
        }

        gc = GameObject.Find("GameController").GetComponent<GameController>();
        transform.position = startPosition;
        StartCoroutine(Espera(3f));
    }

    void Update()
    {
        if (gc.estadoActual == GameController.Estado.Victoria) Destroy(gameObject);
    }

    IEnumerator Espera(float s){
        yield return new WaitForSeconds(s);
        Generar();
    }

    void Generar(){
        int r;

        if (gc.fuerzasRestantes > gc.fuerzasTotales - 20) r = Random.Range(0, 2);
        else if(gc.fuerzasRestantes > 50) r = Random.Range(0, 3);
        else r = Random.Range(1, 3);
        
        GameObject g = naves[r];
        Vector3 v = new Vector3(Random.Range(xmin,xmax), transform.position.y, 0);

        Instantiate(g, v, Quaternion.identity);
        StartCoroutine(Espera(cadencia));
    }
}
