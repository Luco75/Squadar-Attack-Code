using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo2 : Enemigos
{
    public Transform armaDerecha, armaIzquierda;
    public GameObject disparo;

    new void Start()
    {
        base.Start();
        StartCoroutine(CargarDisparo(1.5f));
    }

    new void Update()
    {
        base.Update();
    }

    IEnumerator CargarDisparo(float s){
        yield return new WaitForSeconds(s);
        if (vida > 0) Disparar();
    }

    void Disparar(){
        Instantiate(disparo, armaDerecha.position, Quaternion.identity);
        Instantiate(disparo, armaIzquierda.position, Quaternion.identity);
    }

    
}
