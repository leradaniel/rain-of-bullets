using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esta clase es para evitar que un gameobject rote, independientemente de la rotación del parent.
public class FreezeChildRotation : MonoBehaviour {
    
    //En LateUpdate para que se actualice después de la rotación del parent (así se evita un tambaleo en el próximo frame).
    void LateUpdate()
    {
        //Se congela su rotación.
        transform.rotation = new Quaternion(0, 0, 0, 1);
    }
}
