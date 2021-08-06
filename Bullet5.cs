using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Lanzallamas
public class Bullet5 : MonoBehaviour
{
    [Tooltip("Velocidad con la que la llama avanza:")]
    float bulletSpeed = 10f;
    [Tooltip("Velocidad con la que la llama crece:")]
    float bulletGrowSpeed = 15f;
    [Tooltip("El tamaño máximo que va a tener la llama.")]
    public float maxScale = 3f;
    [Tooltip("La velocidad con la que va a crecerla llama.")]
    public float growSpeed = 80f;
    [Tooltip("Cuánto daño hace la llama.")]
    public float bulletDamage;
    [Tooltip("Cuánto tiempo de inmunidad otorga el impacto.")]
    public float immunityTimeGiven;

    // Update is called once per frame
    void Update()
    {
        //Si el juego no está pausado
        if (GManager.pause == false)
        {
            //La llama avanza
            GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;

            //Se escala la llama según los parametros dados
            transform.localScale += new Vector3(1, 1, 1) * bulletGrowSpeed * Time.deltaTime;
            //Si la escala supera el tamaño máximo del disparo, se destruye
            if (transform.localScale.x > maxScale)
            {
                Destroy(gameObject);
            }
        }
        //Si está pausado, la llama no avanza
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
    }

    //Colisiones
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Layer 14 = Enemigo
        if (other.gameObject.layer == 14)
        {
            //Se frena el retroceso dado en el enemigo.
            if (other.gameObject.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
            {
                other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            }
            //Se le pasa el daño hecho y el tiempo de inmunidad.
            other.gameObject.SendMessage("RecieveDamage", bulletDamage);
            other.gameObject.SendMessage("RecieveImmunityTime", immunityTimeGiven);
        }
    }
}
