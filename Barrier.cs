using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    public int roomIdActivation;
    public GameObject barrier;

    // Update is called once per frame
    void Update () {
        if (GManager.fighting == true && GManager.roomID == roomIdActivation)
        {
            barrier.SetActive(true);
        }
        else
        {
            barrier.SetActive(false);
        }
	}
}
