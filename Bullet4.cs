using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet4 : MonoBehaviour {

    [HideInInspector]
    public GameObject center;//El objeto al cual va a estar pegado el laser.
    [Tooltip("Cuánto daño hace el laser.")]
    public float bulletDamage;
    [Tooltip("Cuánto tiempo de inmunidad otorga el impacto.")]
    public float immunityTimeGiven;

    // Update is called once per frame
    void Update()
    {
        //Si el juego no está pausado
        if (GManager.pause == false)
        {
            BulletMovement();
        }
    }

    //Función encargada de que el laser quede pegado al centro y rote según la mira
    void BulletMovement()
    {
        transform.position = center.transform.position;
        transform.rotation = center.transform.rotation;
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
