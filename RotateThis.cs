using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esta clase hace que un game object rote.
public class RotateThis : MonoBehaviour {

    [Tooltip("Velocidad con la que el game object rota:")]
    public float rotationSpeed = 10f;
    
    // Update frame a frame
    void Update () {
        if (GManager.pause == false)
        {
            //Se rota el objeto en Z, multiplicándolo por la velocidad de rotación dada.
            transform.Rotate(new Vector3(0f, 0f, 1f) * rotationSpeed * Time.deltaTime);
        }

    }
}
