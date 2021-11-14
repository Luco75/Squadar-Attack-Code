using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo3 : Enemigos
{
    public Transform arma;
    public GameObject disparo;
    float cadenciaMin;
    float cadenciaMax;

    new void Start()
    {
        base.Start();
        cadenciaMin = 1.5f;
        cadenciaMax = 3f;
        StartCoroutine(CargarDisparo(Random.Range(cadenciaMin, cadenciaMax)));
    }

    new void Update()
    {
        base.Update();
        if (gc.fuerzasRestantes < gc.fuerzasTotales / 2) cadenciaMax = 2f;
    }

    IEnumerator CargarDisparo(float s)
    {
        yield return new WaitForSeconds(s);
        if (atacar) Disparar();
    }

    void Disparar()
    {
        if(gc.jugadoresVivos > 0) Instantiate(disparo, arma.position, Quaternion.identity);
        StartCoroutine(CargarDisparo(Random.Range(cadenciaMin, cadenciaMax)));
    }
}
