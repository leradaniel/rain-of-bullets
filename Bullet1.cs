using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    [Tooltip("Velocidad con la que la bala avanza.")]
    public float bulletSpeed = 30f;
    [Tooltip("GameObject del efecto que se instancia al ser impactar la bala.")]
    public GameObject explotion;
    [Tooltip("Cuánto recorre la bala antes de destruirse, en caso de no impactar nada. [Si es 0, no se lo hace]")]
    public int spaceToDestroy;
    [Tooltip("Cuánto daño hace la bala.")]
    public float bulletDamage;
    [Tooltip("Cuánto tiempo de inmunidad otorga el impacto.")]
    public float immunityTimeGiven;

    //Posición inicial de la bala
    Vector2 initialPosition;
    //Posición actual de la bala 
    Vector2 actualPosition;

    bool canDestroy;

    // Start
    void Start()
    {
        //Se establece la posición inicial de la bala.
        initialPosition = transform.localPosition;
    }

    // Update frame a frame
    void Update()
    {
        if (GManager.pause == false)
        {
            //La bala avanza con su up según la velocidad dada por la variable bulletSpeed.
            //Al momento de ser instanciada en la clase Character, se orienta su up con el del jugador
            GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
            //Si tiene especificado una longitud de destrucción
            if (spaceToDestroy > 0)
            {
                //Se actualiza la información posición actual
                actualPosition = transform.localPosition;
                //Si la distancia entre la posición actual y la posición inicial super a la establecida, se destruye la bala.
                if (Vector2.Distance(actualPosition, initialPosition) > spaceToDestroy)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            //Si el juego está en pausa, no se mueve
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
        GetComponent<Animator>().SetBool("animCanDestroy", canDestroy);
    }

    // Colisiones
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (canDestroy == false)
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

            //Si colisiona con cualquier cosa, instancia una animación (game object) en la misma posición.
            //El quaternion de identidad indica que no tiene rotación.
            Instantiate(explotion, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            //Se destruye el objeto inmediatamente
            canDestroy = true;
        }
    }
}