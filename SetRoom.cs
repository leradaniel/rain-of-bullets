using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRoom : MonoBehaviour
{
    //La iD de la room a la cual entra el jugador
    public int roomIDNow;

    void OnTriggerEnter2D(Collider2D other)
    {
        //10 = Player
        if (other.gameObject.layer == 10)
        {
            GManager.roomID = roomIDNow;
            Destroy(gameObject);
        }
    }
}
