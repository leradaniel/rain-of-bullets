using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAnimation : MonoBehaviour
{

    //La velocidad de la animación
    float animatorSpeed;
    [HideInInspector]
    public bool pausedSound = false;

    // Use this for initialization
    void Start()
    {
        //Se adquiere la velocidad de animación del animator
        if (GetComponent<Animator>())
        {
            animatorSpeed = GetComponent<Animator>().speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Si el juego está pausado, el objeto se anima correctamente. De lo contrario, se pausa.
        if (GManager.pause == false)
        {
            if (GetComponent<Animator>())
            {
                GetComponent<Animator>().speed = animatorSpeed;
            }
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().UnPause();
                pausedSound = false;
            }
        }
        else
        {
            if (GetComponent<Animator>())
            {
                GetComponent<Animator>().speed = 0;
            }
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().Pause();
                pausedSound = true;
            }
        }
    }
}
