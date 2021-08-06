using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet6 : MonoBehaviour {

    [Tooltip("Cuánto daño hace la bala.")]
    public float bulletDamage;
    [Tooltip("Cuánto tiempo de inmunidad otorga el impacto.")]
    public float immunityTimeGiven;
    
    [HideInInspector]
    public GameObject center;//El objeto al cual va a estar pegado el laser.

    // Update is called once per frame
    void Update()
    {
        //Se deja pegado el rayo al centro
        transform.position = center.transform.position;
        CheckDestroy();
    }

    void CheckDestroy()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            if (!GetComponent<AudioSource>().isPlaying)
            {
                Destroy(gameObject);
            }
        }   
    }

    //Colisiones
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Layer 14 = Enemigo
        if (other.gameObject.layer == 14)
        {
            //Se frena el retroceso dado en el enemigo.
            other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            //Se le pasa el daño hecho y el tiempo de inmunidad.
            other.gameObject.SendMessage("RecieveDamage", bulletDamage);
            other.gameObject.SendMessage("RecieveImmunityTime", immunityTimeGiven);
        }

        if (other.gameObject.layer == 15)
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
