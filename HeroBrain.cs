using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBrain
{
    CharacterBody characterBody;
    HeroFunctions hero;
    Vector2 dir;
    float dashDir;
    public bool isEnabled;

    public void GetBody(CharacterBody body)
    {
        characterBody = body;
        isEnabled = true;
    }

    public void GetHeroFunctions(HeroFunctions functions)
    {
        hero = functions;
        isEnabled = true;
    }

    public void InputListener()
    {
        if (isEnabled)
        {
            Movement();
            Dash();
            InputShoot();
            InputChangeWeapon();
            Aim();
            Flip();
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

    void Movement()
    {
        if (hero.isDashing == false)
        {
            dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            characterBody.CharacterMovement(dir, characterBody.movementSpeed);
        }
        else
        {
            characterBody.CharacterMovement(dir, hero.dashSpeed);
        }
    }

    void Dash()
    {
        if (hero.isDashing == false)
        {
            if (characterBody.shotPoint.transform.eulerAngles.z > 135 && characterBody.shotPoint.transform.eulerAngles.z < 235)
            {
                dashDir = 1f;
            }
            else
            {
                dashDir = -1f;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                dir = new Vector2(dashDir * characterBody.shotPoint.transform.right.x, dashDir * characterBody.shotPoint.transform.right.y);
                hero.isDashing = true;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                dashDir *= -1f;
                dir = new Vector2(dashDir * characterBody.shotPoint.transform.right.x, dashDir * characterBody.shotPoint.transform.right.y);
                hero.isDashing = true;
            }
        }
        else
        {
            hero.dashTimeNow += Time.deltaTime;
            if (hero.dashTimeNow > hero.dashTimeMax || characterBody.isHit)
            {
                hero.isDashing = false;
                hero.dashTimeNow = 0;
            }
        }
    }

    void InputShoot()
    {
        if (Input.GetMouseButton(0) && hero.shieldBullet == null)
        {
            characterBody.isShooting = true;
            //False para indicar que no es parry
            hero.CharacterShoot(false);
        }
        else
        {
            characterBody.isShooting = false;
            hero.DeleteShoot();
        }

        if (Input.GetMouseButtonDown(1) )// && characterBody.isShooting == false)
        {
            characterBody.isShooting = true;
            //True para indicar que es parry
            hero.CharacterShoot(true);
        }
    }

    void InputChangeWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            hero.changeWeaponMouse(1);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            hero.changeWeaponMouse(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hero.changeWeaponKey(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hero.changeWeaponKey(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hero.changeWeaponKey(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            hero.changeWeaponKey(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            hero.changeWeaponKey(5);
        }
    }

    void Aim()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        float z = Input.mousePosition.z;
        float cameraZ = Camera.main.transform.position.z;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, z - cameraZ));
        characterBody.CharacterAiming(mousePos);
    }

}
