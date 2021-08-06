using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public int roomID;
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> barriers = new List<GameObject>();
    public GameObject activationZone;
    public GameObject cameraPosition;

    // Update is called once per frame
    void Awake()
    {
        //if (enemies != null)
        //{
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<SpawnPoint>().roomActivationID = roomID;
        }

        //}
        //if (barriers != null)
        //{
            for (int i = 0; i < barriers.Count; i++)
            {
                barriers[i].GetComponent<Barrier>().roomIdActivation = roomID;
            }
        //}
        activationZone.GetComponent<SetRoom>().roomIDNow = roomID;
        if (cameraPosition != null)
        {
            cameraPosition.GetComponent<RoomCamera>().myRoomID = roomID;
        }
    }
}
