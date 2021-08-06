using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet3Explosion : MonoBehaviour
{
    [Tooltip("Cuánto daño hace la explosión.")]
    public float explotionDamage;
    [Tooltip("Cuánto tiempo de inmunidad otorga el impacto.")]
    public float immunityTimeGiven;

    //Colisiones
    private void OnCollisionStay2D(Collision2D other)
    {
        //Layer 14 = Enemigo
        if (other.gameObject.layer == 14)
        {
            //Se le pasa el daño hecho y el tiempo de inmunidad.
            other.gameObject.SendMessage("RecieveDamage", explotionDamage);
            other.gameObject.SendMessage("RecieveImmunityTime", immunityTimeGiven);
        }
    }
}
