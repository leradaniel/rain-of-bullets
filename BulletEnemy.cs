using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{

    [Tooltip("Velocidad con la que la bala avanza:")]
    public float bulletSpeed = 5f;
    [Tooltip("GameObject del efecto que se instancia al ser impactar la bala:")]
    public GameObject explotion;
    [Tooltip("GameObject del efecto que se instancia al ser impactar la bala:")]
    public int bulletDamage = 1;

    [Tooltip("Si la bala se mueve con patrones:")]
    public bool patternMovement = false;

    [Tooltip("Amplitud a la que se llega a oscilar del centro a los costados.")]
    public float widthAmplitude;
    [Tooltip("Amplitud a la que se llega a oscilar del centro hacia arriba y abajo.")]
    public float heightAmplitude;
    [Tooltip("Velocidad de oscilación.")]
    public float cicleSpeed;
    //Variable para almacenar los grados
    float degrees;
    //Variable para almacenar los grados convertidos a radianes
    float radians;
    //El punto central de la oscilación
    Vector3 center;

    bool canDestroy;

    void Start()
    {
        //Al momento de instanciarse, se guarda la posición central de la bala
        center = transform.position;
    }

    // Update frame a frame
    void Update()
    {
        if (GManager.pause == false)
        {
            MoveBullet();
        }

        else
        {
            //Si el juego está en pausa, no se mueve
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
        GetComponent<Animator>().SetBool("animCanDestroy", canDestroy);
    }

    private void MoveBullet()
    {
        if (patternMovement == true)
        {
            //A la variable de los grados se le suma la velocidad con la cual queremos que se oscile.
            degrees += cicleSpeed * Time.deltaTime;
            //Esos grados se los multiplica por (pi*2)/360 para convertirlos a radianes y se los guarda en otra variable
            radians = degrees * Mathf.Deg2Rad;

            //Se posiciona el centro de la oscilación de la bala, según su up
            center += transform.up * bulletSpeed * Time.deltaTime;

            //Se posiciona el game object de la bala, según su oscilación
            Vector3 realPosition = Vector3.zero;
            //Se utiliza el coseno del ángulo en radianes para el movimiento en X y el seno para el movimiento en Y
            //Además se lo multiplica por la amplitu a la cual se quiere llegar en al oscilación
            realPosition.x = Mathf.Cos(radians) * widthAmplitude;
            realPosition.y = Mathf.Sin(radians) * heightAmplitude;

            //Movimiento de la bala, basado en la posición del centro de la oscilacón y ela posición del game object
            transform.position = center + realPosition;
        }
        else
        {
            //La bala avanza con su up según la velocidad dada por la variable bulletSpeed.
            GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
        }
    }

    // Colisiones
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (canDestroy == false)
        {
            //Si se colisiona con objetos en el Layer 9 (Paredes) o en el layer 10 (Jugador)
            if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
            {
                //Instancia una animación (game object) en la misma posición de la bala.
                //El quaternion de identidad indica que no tiene rotación.
                Instantiate(explotion, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                //Se destruye el objeto inmediatamente.
                canDestroy = true;
                if (other.gameObject.layer == 10)
                {
                    other.gameObject.SendMessage("RecieveDamage", bulletDamage);
                }
            }
            //Si se colisiona con objetos en el Layer 15 (Parry)
            if (other.gameObject.layer == 15)
            {
                GetComponent<AudioSource>().Play();
                //Instancia una animación (game object) en la misma posición de la bala.
                //El quaternion de identidad indica que no tiene rotación.
                Instantiate(explotion, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                //Se destruye el objeto inmediatamente.
                canDestroy = true;
            }

        }
    }
}
