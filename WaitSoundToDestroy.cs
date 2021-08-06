using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaitSoundToDestroy : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
        {
            if (GetComponent<Collider2D>())
            {
                GetComponent<Collider2D>().enabled = false;
            }
            if (GetComponent<SpriteRenderer>())
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
            if (!GetComponent<AudioSource>().isPlaying)
            {
                if (GetComponent<PauseAnimation>())
                {
                    if (GetComponent<PauseAnimation>().pausedSound == false)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
