using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain
{
    [HideInInspector]
    public bool isEnabled;
    //Tiempo que tarda en activarse el enemigo
    float brainActiveDelay = 1f;
    bool brainIsActive = false;
    //Dirección de movimiento
    Vector2 dir;
    //Gameobject al cual va a apuntar
    GameObject target;

    CharacterBody characterBody;
    EnemyFunctions enemy;

    public void GetBody(CharacterBody body)
    {
        characterBody = body;
        isEnabled = true;
    }

    public void GetEnemyFunctions(EnemyFunctions functions)
    {
        enemy = functions;
        isEnabled = true;
    }

    public void InputListener()
    {
        if (isEnabled == true)
        {
            Activate();
            Movement();
            Shoot();
            Aim();
            Flip();
        }
    }

    void Activate()
    {
        if (characterBody.isSpawning == false && brainIsActive == false)
        {
            if (brainActiveDelay <= 0)
            {
                brainIsActive = true;
            }
            else
            {
                brainActiveDelay -= Time.deltaTime;
            }
        }
    }

    void Flip()
    {
        if (characterBody.shotPoint.transform.eulerAngles.z < 180)
        {
            characterBody.CharacterFlip(true);
        }
        else
        {
            characterBody.CharacterFlip(false);
        }
    }

    void Aim()
    {
        target = GameObject.Find("Character");
        if (target != null)
        {
            float x = target.transform.position.x;
            float y = target.transform.position.y;
            float z = target.transform.position.z;
            Vector3 targetPos = new Vector3(x, y, z);
            characterBody.CharacterAiming(targetPos);
        }
    }

    void Shoot()
    {
        if (enemy.canShoot == true)
        {
            characterBody.isShooting = true;
            enemy.Shoot();
        }
        else
        {
            characterBody.isShooting = false;
        }
    }

    void Movement()
    {
        if (characterBody.characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            dir = new Vector2(characterBody.shotPoint.transform.up.x, characterBody.shotPoint.transform.up.y);
            characterBody.CharacterMovement(dir, characterBody.movementSpeed);
        }
        else if(characterBody.characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shooting"))
        {
            characterBody.CharacterMovement(new Vector2(0f, 0f), 0f);
        }

    }
}
