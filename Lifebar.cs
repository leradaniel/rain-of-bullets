using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifebar : MonoBehaviour {
    [Tooltip("GameObject de la barra verde de vida.")]
    public GameObject lifebarGreen;
    public float hpMax = 0;
    public float hpNow = 0;

	// Update is called once per frame
	void Update ()
    {
        float lifebarScaleX = hpNow / hpMax;
        lifebarGreen.transform.localScale = new Vector2(lifebarScaleX, lifebarGreen.transform.localScale.y);
    }
}
