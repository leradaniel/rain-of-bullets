using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBarHero : MonoBehaviour {
    [Tooltip("GameObject del punto de vida 1 que gira alrededor del character.")]
    public GameObject healthGraphic1;
    [Tooltip("GameObject del punto de vida 2 que gira alrededor del character.")]
    public GameObject healthGraphic2;
    [Tooltip("GameObject del punto de vida 3 que gira alrededor del character.")]
    public GameObject healthGraphic3;
    public int hpMax = 0;
    public int hpNow = 0;

    // Update is called once per frame
    void Update()
    {
        switch (hpNow)
        {
            case 4:
                healthGraphic1.SetActive(true);
                healthGraphic2.SetActive(true);
                healthGraphic3.SetActive(true);
                break;
            case 3:
                healthGraphic1.SetActive(true);
                healthGraphic2.SetActive(true);
                healthGraphic3.SetActive(false);
                break;
            case 2:
                healthGraphic1.SetActive(true);
                healthGraphic2.SetActive(false);
                healthGraphic3.SetActive(false);
                break;
            case 1:
                healthGraphic1.SetActive(false);
                healthGraphic2.SetActive(false);
                healthGraphic3.SetActive(false);
                break;
            default:
                break;
        }
    }
}
