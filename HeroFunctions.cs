using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroFunctions : MonoBehaviour
{
    #region DECLARACIÓN DE VARIABLES 
    [Header("CHEATS: Teclas [I] y [O] los activan. [P] los desactiva")]
    [Tooltip("No se pierden vidas")]
    public bool noLifeLose = false;
    [Tooltip("Munición infinita")]
    public bool maxAmmo = false;

    [Header("Configuración de estadísticas propias del jugador")]
    [Tooltip("Vidas máximas del character. Los enemigos no tienen.")]
    public int lifesMax = 3;
    [Tooltip("Vidas actuales del character.")]
    public int lifesNow = 3;
    [Tooltip("Velocidad de movimiento cuando se hace un dash.")]
    public float dashSpeed = 16f;
    [Tooltip("Tiempo durante el cual se hace un dash.")]
    public float dashTimeMax = 0.1f;
    [HideInInspector]
    public float dashTimeNow = 0f;//Contador
    [HideInInspector]
    public bool isDashing = false; //Indica si el personaje realiza un dash.

    //Cuánto tiempo tarda el personaje muerto en revivir.
    float deadTimeMax = 2f;
    //Contador
    float deadTimeNow = 0f;
    [HideInInspector]
    public bool exploded = false;//Indica si ya la animación de muerte ya se instanció
    [HideInInspector]
    public bool isReviving = false; //Indica si el personaje está reviviendo.

    [Header("Configuración de las armas:")]
    [Tooltip("Prefab del arma 1 (Pistola)")]
    public GameObject bulletGunPrefab;
    [Tooltip("Prefab del arma 2 (Escopeta)")]
    public GameObject bulletSpreadPrefab;
    [Tooltip("Prefab del arma 3 (Granada)")]
    public GameObject bulletAOEPrefab;
    [Tooltip("Prefab de la explosión del arma 3.")]
    public GameObject bulletAOEExplosionPrefab;
    [Tooltip("Prefab del arma 4 (Laser.)")]
    public GameObject bulletLaserPrefab;
    [Tooltip("Prefab del arma 5 (Lanza llamas.)")]
    public GameObject bulletFlamethrowerPrefab;
    [Tooltip("Prefab del arma 6 (Escudo eléctrico.)")]
    public GameObject bulletShieldPrefab;

    //[HideInInspector]
    public int weaponIDMax = 5; //Cantidad máxima de armas.
    public int weaponID = 1; //Arma activada actualmente.

    //Munición, Tiempo de recarga y Cadencia de disparo de cada arma
    float gunMaxAmmo = 50f;
    float gunReloadTime = 10f;
    float gunCooldown = 0.2f;
    float gunCooldownNow;

    float spreadMaxAmmo = 15f;
    float spreadReloadTime = 15f;
    float spreadCooldown = 0.6f;
    float spreadCooldownNow;

    float aoeMaxAmmo = 10f;
    float aoeReloadTime = 20f;
    float aoeCooldown = 1f;
    float aoeCooldownNow;

    float laserMaxAmmoTime = 4f;
    float laserReloadTime = 40f;

    float flamethrowerMaxAmmo = 200f;
    float flamethrowerReloadTime = 30f;
    float flamethrowerCooldown = 0.03f;
    float flamethrowerCooldownNow;

    float shieldMaxAmmo = 1f;
    float shieldReloadTime = 1f;

    [HideInInspector]
    public GameObject shieldBullet; //Disparo del arma 6. Cuando desaparece, se establece que el jugador ya no está disparando.
    GameObject laserBullet; //Disparo del arma 5. No se destruye solo, se destruye manualmente.

    //Listas para las armas
    [HideInInspector]
    public List<float> ammo = new List<float>();
    [HideInInspector]
    public List<float> ammoMax = new List<float>();
    [HideInInspector]
    public List<float> reloadTime = new List<float>();
    [HideInInspector]
    public List<float> bulletCooldown = new List<float>();
    [HideInInspector]
    public List<float> bulletCooldownNow = new List<float>();

    CharacterBody characterBody;

    public bool isEnabled;

    //Sonidos a reproducir, seteados en el inspector
    public AudioClip laserShootSound;
    public AudioClip flamethrowerShootSound;
    public AudioClip characterRevive;

    bool revivedSoundPlayed = false;
    #endregion

    public void GetBody(CharacterBody body)
    {
        characterBody = body;
        isEnabled = true;
    }

    // Use this for initialization
    void Start()
    {
        lifesNow = lifesMax;
        InitializeGunInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (GManager.pause == false)
        {
            LoseLife();
            ReloadAmmo();
            ClampQuantities();
            BulletsCooldown();
            HeroAnimation();
            CheatsInput();
        }
        PauseSound();
    }

    private void BulletsCooldown()
    {
        for (int i = 0; i <= weaponIDMax; i++)
        {
            //Ni laser ni parry
            if (i != 4 && i != 0 && bulletCooldownNow[i] > 0)
            {
                bulletCooldownNow[i] -= Time.deltaTime;
            }
        }
    }

    void ClampQuantities()
    {
        for (int i = 0; i < ammo.Count; i++)
        {
            ammo[i] = Mathf.Clamp(ammo[i], 0, ammoMax[i]);
        }
    }

    void ReloadAmmo()
    {
        for (int i = 0; i <= weaponIDMax; i++)
        {
            if (i != weaponID || i == 0)
            {
                ammo[i] += ammoMax[i] / reloadTime[i] * Time.deltaTime;
            }
        }
    }

    void InitializeGunInfo()
    {
        ammoMax.Add(shieldMaxAmmo);
        ammoMax.Add(gunMaxAmmo);
        ammoMax.Add(spreadMaxAmmo);
        ammoMax.Add(aoeMaxAmmo);
        ammoMax.Add(laserMaxAmmoTime);
        ammoMax.Add(flamethrowerMaxAmmo);

        for (int i = 0; i < ammoMax.Count; i++)
        {
            ammo.Add(ammoMax[i]);
        }

        reloadTime.Add(shieldReloadTime);
        reloadTime.Add(gunReloadTime);
        reloadTime.Add(spreadReloadTime);
        reloadTime.Add(aoeReloadTime);
        reloadTime.Add(laserReloadTime);
        reloadTime.Add(flamethrowerReloadTime);

        bulletCooldown.Add(0);
        bulletCooldown.Add(gunCooldown);
        bulletCooldown.Add(spreadCooldown);
        bulletCooldown.Add(aoeCooldown);
        bulletCooldown.Add(0);
        bulletCooldown.Add(flamethrowerCooldown);

        for (int i = 0; i < bulletCooldown.Count; i++)
        {
            bulletCooldownNow.Add(bulletCooldown[i]);
        }

    }


    public void CharacterShoot(bool parry)
    {
        int auxWeaponID = weaponID;
        if (parry == true)
        {
            weaponID = 0;
        }
        switch (weaponID)
        {
            case 0: //Parry
                if (ammo[0] == ammoMax[0])
                {
                    shieldBullet = Instantiate(bulletShieldPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                    shieldBullet.GetComponent<Bullet6>().center = characterBody.shotPoint;
                    //Power up
                    /*
                    shieldBullet.transform.localScale *= 1.5f;
                    shieldBullet.GetComponent<Animator>().speed = 0.5f;


                    Quaternion tempRotation = characterBody.shotPoint.transform.rotation;
                    //float rotationAngle = 45f;
                    //for (int i = 0; i < 7; i++)
                    float rotationAngle = 22.5f;
                    for (int i = 0; i < 15; i++)
                    {
                        tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                        shieldBullet = Instantiate(bulletShieldPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        shieldBullet.GetComponent<Bullet6>().center = characterBody.shotPoint;
                        shieldBullet.transform.localScale *= 1.5f;
                        shieldBullet.GetComponent<Animator>().speed = 0.5f;
                    }
                    */
                    //

                    if (maxAmmo == false)
                    {
                        ammo[0]--;
                    }
                }
                else
                {
                    characterBody.isShooting = false;
                }

                break;
            case 1://Pistola
                if (bulletCooldownNow[1] <= 0)
                {
                    if (ammo[1] > 0)
                    {
                        if (maxAmmo == false)
                        {
                            ammo[1]--;
                        }
                        Instantiate(bulletGunPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                        bulletCooldownNow[1] = bulletCooldown[1];
                    }
                    else
                    {
                        characterBody.isShooting = false;
                    }
                }
                break;
            case 2://Escopeta
                if (bulletCooldownNow[2] <= 0)
                {
                    if (ammo[2] > 0)
                    {
                        Quaternion tempRotation = characterBody.shotPoint.transform.rotation;
                        float rotationAngle = 20f;
                        Instantiate(bulletSpreadPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                        Instantiate(bulletSpreadPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        tempRotation *= Quaternion.Euler(0f, 0f, -rotationAngle * 2);
                        Instantiate(bulletSpreadPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle * 3);
                        Instantiate(bulletSpreadPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        tempRotation *= Quaternion.Euler(0f, 0f, -rotationAngle * 4);
                        Instantiate(bulletSpreadPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        bulletCooldownNow[2] = bulletCooldown[2];
                        if (maxAmmo == false)
                        {
                            ammo[2]--;
                        }
                    }
                    else
                    {
                        characterBody.isShooting = false;
                    }

                }
                break;
            case 3://Granada
                if (bulletCooldownNow[3] <= 0)
                {
                    if (ammo[3] > 0)
                    {
                        GameObject bullet = Instantiate(bulletAOEPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                        bullet.GetComponent<Bullet3>().explosion = bulletAOEExplosionPrefab;
                        bulletCooldownNow[3] = bulletCooldown[3];
                        if (maxAmmo == false)
                        {
                            ammo[3]--;
                        }
                    }
                    else
                    {
                        characterBody.isShooting = false;
                    }
                }
                break;
            case 4://Laser
                if (ammo[4] > 0)
                {
                    if (laserBullet == null)
                    {
                        laserBullet = Instantiate(bulletLaserPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                        laserBullet.GetComponent<Bullet4>().center = characterBody.shotPoint;
                    }
                    else
                    {
                        if (!characterBody.shotPoint.GetComponent<AudioSource>().isPlaying)
                        {
                            characterBody.shotPoint.GetComponent<AudioSource>().clip = laserShootSound;
                            characterBody.shotPoint.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            if (characterBody.shotPoint.GetComponent<AudioSource>().clip != null)
                            {
                                float soundTemp = characterBody.shotPoint.GetComponent<AudioSource>().clip.length - characterBody.shotPoint.GetComponent<AudioSource>().time;
                                if (soundTemp < 0.2f)
                                {
                                    characterBody.shotPoint.GetComponent<AudioSource>().clip = laserShootSound;
                                    characterBody.shotPoint.GetComponent<AudioSource>().Play();
                                }
                            }
                        }
                        ammo[4] -= Time.deltaTime;
                    }
                }
                else
                {
                    characterBody.isShooting = false;
                    DeleteShoot();
                }
                break;
            case 5: //Lanzallamas

                if (bulletCooldownNow[5] <= 0)
                {
                    if (ammo[5] > 0)
                    {
                        Instantiate(bulletFlamethrowerPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                        //PowerUp
                        /*
                        Quaternion tempRotation = characterBody.shotPoint.transform.rotation;
                        float rotationAngle = 90f;
                        tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                        Instantiate(bulletFlamethrowerPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                        Instantiate(bulletFlamethrowerPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                        Instantiate(bulletFlamethrowerPrefab, characterBody.shotPoint.transform.position, tempRotation);
                        */
                        bulletCooldownNow[5] = bulletCooldown[5];
                        if (maxAmmo == false)
                        {


                            if (!characterBody.shotPoint.GetComponent<AudioSource>().isPlaying)
                            {
                                characterBody.shotPoint.GetComponent<AudioSource>().clip = flamethrowerShootSound;
                                characterBody.shotPoint.GetComponent<AudioSource>().Play();
                                //characterBody.shotPoint.GetComponent<AudioSource>().PlayOneShot(laserShootSound);
                            }
                            else
                            {
                                if (characterBody.shotPoint.GetComponent<AudioSource>().clip != null)
                                {
                                    float soundTemp = characterBody.shotPoint.GetComponent<AudioSource>().clip.length - characterBody.shotPoint.GetComponent<AudioSource>().time;
                                    if (soundTemp < 0.2f)
                                    {
                                        characterBody.shotPoint.GetComponent<AudioSource>().clip = flamethrowerShootSound;
                                        characterBody.shotPoint.GetComponent<AudioSource>().Play();
                                    }
                                }
                            }

                            ammo[5]--;
                        }
                    }
                    else
                    {
                        characterBody.isShooting = false;
                    }
                }
                break;
            default:
                break;
        }
        weaponID = auxWeaponID;
    }

    public void DeleteShoot()
    {
        Destroy(laserBullet);
        characterBody.shotPoint.GetComponent<AudioSource>().clip = null;
    }

    public void changeWeaponMouse(int goesUpOrDown)
    {
        weaponID += goesUpOrDown;
        Destroy(laserBullet);
        if (characterBody.isShooting == true)
        {
            characterBody.shotPoint.GetComponent<AudioSource>().Stop();
        }
        if (weaponID > weaponIDMax)
        {
            weaponID = 1;
        }
        else if (weaponID <= 0)
        {
            weaponID = weaponIDMax;
        }
    }

    public void changeWeaponKey(int keyPressed)
    {
        if (keyPressed <= weaponIDMax)
        {
            Destroy(laserBullet);
            weaponID = keyPressed;
        }
    }

    void LoseLife()
    {
        if (exploded == true)
        {
            deadTimeNow += Time.deltaTime;
            if (deadTimeNow > deadTimeMax)
            {
                isReviving = true;
                characterBody.isHit = false;
                if (revivedSoundPlayed == false)
                {
                    GetComponent<AudioSource>().clip = characterRevive;
                    GetComponent<AudioSource>().Play();
                    revivedSoundPlayed = true;
                }
                if (lifesNow > 0)
                {
                    characterBody.sr.enabled = true;
                    if (characterBody.characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        characterBody.isDead = false;
                        characterBody.deadSoundPlayed = false;
                        isReviving = false;
                        characterBody.hpNow = characterBody.hpMax;
                        deadTimeNow = 0f;
                        characterBody.deadExplotionTimeNow = 0f;
                        exploded = false;
                        revivedSoundPlayed = false;
                        if (noLifeLose == false)
                        {
                            lifesNow--;
                        }

                        characterBody.immunityTimeNow = 0f;
                        characterBody.realImmunity = true;
                        characterBody.realImmunityTimeNow = 0f;
                        characterBody.GetComponent<Collider2D>().enabled = true;
                        characterBody.heroBrain.isEnabled = true;
                    }

                }
                else
                {
                    SceneManager.LoadScene("EndSCreen");
                }
            }
        }
    }

    void PauseSound()
    {
        if (GManager.pause == true)
        {
            GetComponent<AudioSource>().Pause();
            characterBody.shotPoint.GetComponent<AudioSource>().Pause();
        }
        else
        {
            GetComponent<AudioSource>().UnPause();
            characterBody.shotPoint.GetComponent<AudioSource>().UnPause();
        }
    }

    void HeroAnimation()
    {
        characterBody.characterAnimator.SetBool("animIsDashing", isDashing);
        characterBody.characterAnimator.SetBool("animIsReviving", isReviving);
    }

    //Función encargada de cambiar de escenas, según el string que le pasamos:
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void CheatsInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            noLifeLose = true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            maxAmmo = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            noLifeLose = false;
            maxAmmo = false;
        }

    }
}