using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet3 : MonoBehaviour
{
    [Tooltip("Velocidad con la que la granada avanza")]
    public float bulletSpeed;
    [Tooltip("GameObject del efecto que se instancia al explotar la granada")]
    public GameObject explosion;
    [Tooltip("Tiempo que tarda en explotar si no choca con nada")]
    public float triggerTime = 0.5f;
    //Contador
    float triggerTimeNow = 0;
    [Tooltip("Cuánto daño hace la granada")]
    public float bulletDamage;
    [Tooltip("Cuánto tiempo de inmunidad otorga el impacto")]
    public float immunityTimeGiven;


    // Update is called once per frame
    void Update()
    {
        //Si el juego no está pausado
        if (GManager.pause == false)
        {
            //Se empieza a sumar el tiempo para explotar
            triggerTimeNow += Time.deltaTime;
            //Si se supera dicho tiempo
            if (triggerTimeNow >= triggerTime)
            {
                //Se instancia la explosión y se borra la granada.
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            //De lo contrario, la granada avanza
            else
            {
                GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
            }
        }
        //Si el juego está pausado, no se mueve
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
    }

    //Colisiones
    void OnCollisionEnter2D(Collision2D other)
    {
        //Si choca con cualquier cosa, explota inmediatamente
        triggerTimeNow = triggerTime;
        
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
