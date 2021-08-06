using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSet : MonoBehaviour {

    public int stageNow = 0;
	// Use this for initialization
	void Start () {
        GManager.stage = stageNow;
        Destroy(gameObject);
	}
}
