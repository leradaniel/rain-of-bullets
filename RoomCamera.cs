using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamera : MonoBehaviour {

    public int myRoomID;
	
	// Update is called once per frame
	void Update () {
        if (GManager.fighting == true && GManager.roomID == myRoomID)
        {
            Camera.main.GetComponent<GManager>().objectToFollow = this.gameObject;
        }
	}
}
