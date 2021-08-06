using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFunctions : MonoBehaviour
{


    #region DECLARACIÓN DE VARIABLES 
    [Header("Configuración del disparo:")]
    [Tooltip("Prefab de la bala")]
    public GameObject bulletPrefab;
    [Tooltip("Tamaño agregadoa a la bala")]
    public float bulletScale;
    //Escala normal de las balas
    float bulletScaleRegular = 2f;
    [Tooltip("Cuántas balas dispara por tanda")]
    public int shotsAmmountMax = 5;
    [Tooltip("Tipo de disparo que va a tener.\nTipo 1: Disparo simple\nTipo 2: Disparo rafaga\nTipo 3: Muchos disparos rectos, una linea vertical, en ambas direcciones\nTipo 4: Disparo enorme\nTipo 5: Metralleta recta, que no sigue al jugador\nTipo 6: Espiral\nTipo 7: Circular\nTipo 8: En forma de V")]
    public int shootID = 1;
    [Tooltip("Cada cuánto dispara cada tanda")]
    public float shootCooldownMax = 2f;
    [Tooltip("Cada cuánto dispara los disparos dentro de una ")]
    public float shootWaveCooldown = 0.5f;
    [Tooltip("Velocidad con la que la bala avanza:")]
    public float bulletSpeed = 5f;
    [Tooltip("GameObject del efecto que se instancia al ser impactar la bala:")]
    public int bulletDamage = 1;
    [Tooltip("Si la bala se mueve con patrones:")]
    public bool patternMovement = false;
    [Tooltip("Amplitud a la que se llega a oscilar del centro a los costados.")]
    public float widthAmplitude;
    [Tooltip("Amplitud a la que se llega a oscilar del centro hacia arriba y abajo.")]
    public float heightAmplitude;
    [Tooltip("Velocidad de oscilación.")]
    public float cicleSpeed;

    [Header("Configuración del enemigo:")]
    [Tooltip("Cada cuánto dispara")]
    public bool doesItMove = true;
    [HideInInspector]
    public float shootCooldownNow;//Contador
    [HideInInspector]
    public bool canShoot;//Contador

    CharacterBody characterBody;

    public bool isEnabled;

    //Variables nuevas
    int shotsAmmount = 0;
    Quaternion lineRotation;
    Vector3 tempPosition;
    GameObject leadBullet;
    //GameObject bulletProperties;
    float tempFloat;

    [Tooltip("Sonido de movimiento")]
    public AudioClip shootSound;


    #endregion

    public void GetBody(CharacterBody body)
    {
        characterBody = body;
        isEnabled = true;
    }

    void Start()
    {
        GetComponent<AudioSource>().clip = shootSound;
        shootCooldownNow = shootCooldownMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (GManager.pause == false)
        {
            ActivateShoot();
            if (doesItMove == false)
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
        }
    }

    void ActivateShoot()
    {
        if (shootCooldownNow < 0)
        {
            canShoot = true;
        }
        else
        {
            shootCooldownNow -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        /*
        if (characterBody.isBoss == true)
        {
            Quaternion tempRotation = characterBody.shotPoint.transform.rotation;
            float rotationAngle = 20f;
            GameObject bullet = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
            ApplyBulletProperties(bullet);
            for (int i = 0; i < 18; i++)
            {
                tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                GameObject bullet2 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                ApplyBulletProperties(bullet2);
            }
            canShoot = false;
            shootCooldownNow = shootCooldownMax;

        }
        else
        {
        */
            float rotationAngle;
            Quaternion tempRotation;
            switch (shootID)
            {
                case 1:
                    //Tipo 1: Disparo simple
                    GameObject bullet = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                    ApplyBulletProperties(bullet);
                    canShoot = false;
                    shootCooldownNow = shootCooldownMax;
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    break;
                case 2:
                    //Tipo 2: Disparo rafaga
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    tempRotation = characterBody.shotPoint.transform.rotation;
                    rotationAngle = 20f;
                    GameObject bullet2 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                    ApplyBulletProperties(bullet2);
                    bullet2 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    tempRotation *= Quaternion.Euler(0f, 0f, -rotationAngle * 2);
                    ApplyBulletProperties(bullet2);
                    bullet2 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle * 3);
                    ApplyBulletProperties(bullet2);
                    bullet2 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    tempRotation *= Quaternion.Euler(0f, 0f, -rotationAngle * 4);
                    ApplyBulletProperties(bullet2);
                    bullet2 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    ApplyBulletProperties(bullet2);
                    shotsAmmount++;
                    if (shotsAmmount > shotsAmmountMax)
                    {
                        shotsAmmount = 0;
                        canShoot = false;
                        shootCooldownNow = shootCooldownMax;
                    }
                    else
                    {
                        shootCooldownNow = shootWaveCooldown;
                        canShoot = false;
                    }
                    break;
                case 3:
                    //Tipo 3: Muchos disparos rectos, una linea vertical, en ambas direcciones
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    tempRotation = new Quaternion(0f, 0f, 0f, 1f);
                    rotationAngle = 180f;
                    GameObject bullet3 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    tempRotation *= Quaternion.Euler(0f, 0f, rotationAngle);
                    ApplyBulletProperties(bullet3);
                    bullet3 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, tempRotation);
                    ApplyBulletProperties(bullet3);
                    shotsAmmount++;
                    if (shotsAmmount > shotsAmmountMax)
                    {
                        shotsAmmount = 0;
                        canShoot = false;
                        shootCooldownNow = shootCooldownMax;
                    }
                    else
                    {
                    canShoot = false;
                    shootCooldownNow = shootWaveCooldown;
                    }
                    break;
                case 4:
                    //Tipo 4: Disparo enorme
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    GameObject bigBullet = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, characterBody.shotPoint.transform.rotation);
                    bigBullet.transform.localScale += new Vector3(10f, 10f, 10f);
                    canShoot = false;
                    ApplyBulletProperties(bigBullet);
                    shootCooldownNow = shootCooldownMax;
                    break;
                case 5:
                //Tipo 5: Metralleta recta, que no sigue al jugador
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    if (shotsAmmount > shotsAmmountMax)
                    {
                        shotsAmmount = 0;
                        canShoot = false;
                        shootCooldownNow = shootCooldownMax;
                    }
                    else if (shotsAmmount < 1)
                    {
                        lineRotation = characterBody.shotPoint.transform.rotation;
                        GameObject bullet4 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                        ApplyBulletProperties(bullet4);
                        shootCooldownNow = shootWaveCooldown;
                    canShoot = false;
                    shotsAmmount++;
                    }
                    else
                    {
                        GameObject bullet5  = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                        ApplyBulletProperties(bullet5);
                        shootCooldownNow = shootWaveCooldown;
                    canShoot = false;
                    shotsAmmount++;
                    }
                    break;
                case 6:
                    //Tipo 6: Espiral
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    if (shotsAmmount > shotsAmmountMax)
                    {
                        shotsAmmount = 0;
                        canShoot = false;
                        shootCooldownNow = shootCooldownMax;
                    }
                    else if (shotsAmmount < 1)
                    {
                        lineRotation = characterBody.shotPoint.transform.rotation;
                        GameObject bullet6 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                        ApplyBulletProperties(bullet6);
                        shootCooldownNow = shootCooldownMax;
                        shotsAmmount++;
                    }
                    else
                    {
                        rotationAngle = 20f;
                        lineRotation *= Quaternion.Euler(0f, 0f, -rotationAngle);
                        GameObject bullet7 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                        ApplyBulletProperties(bullet7);
                        shotsAmmount++;
                        shootCooldownNow = shootWaveCooldown;
                        canShoot = false;
                    }
                    break;
                case 7:
                    //Tipo 7: Circular
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    if (shotsAmmount > shotsAmmountMax)
                    {
                        shotsAmmount = 0;
                        canShoot = false;
                        shootCooldownNow = shootCooldownMax;
                    }
                    else if (shotsAmmount < 1)
                    {
                        lineRotation = characterBody.shotPoint.transform.rotation;
                        GameObject bullet8 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                        ApplyBulletProperties(bullet8);
                        shootCooldownNow = shootCooldownMax;
                        shotsAmmount++;
                    }
                    else
                    {
                        rotationAngle = 20f;
                        for (int i = 0; i < 18; i++)
                        {
                            lineRotation *= Quaternion.Euler(0f, 0f, -rotationAngle);
                            GameObject bullet9 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                            ApplyBulletProperties(bullet9);
                        }
                        shotsAmmount++;
                        shootCooldownNow = shootWaveCooldown;
                        canShoot = false;
                    }
                    break;
                case 8:
                    //Tipo 8: En forma de V
                    GetComponent<AudioSource>().clip = shootSound;
                    GetComponent<AudioSource>().Play();
                    if (shotsAmmount > shotsAmmountMax)
                    {
                        shotsAmmount = 0;
                        canShoot = false;
                        shootCooldownNow = shootCooldownMax;
                    }
                    else if (shotsAmmount < 1)
                    {
                        lineRotation = characterBody.shotPoint.transform.rotation;
                        leadBullet = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position, lineRotation);
                        tempPosition = leadBullet.transform.right;
                        tempFloat = leadBullet.GetComponent<SpriteRenderer>().bounds.size.y;
                        tempPosition *= tempFloat;
                        ApplyBulletProperties(leadBullet);
                        shootCooldownNow = shootWaveCooldown;
                        canShoot = false;
                        shotsAmmount++;
                    }
                    else
                    {
                        GameObject bullet9 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position + tempPosition * shotsAmmount / 2, lineRotation);
                        ApplyBulletProperties(bullet9);
                        bullet9 = Instantiate(bulletPrefab, characterBody.shotPoint.transform.position - tempPosition * shotsAmmount / 2, lineRotation);
                        ApplyBulletProperties(bullet9);
                        shotsAmmount++;
                        shootCooldownNow = shootWaveCooldown;
                        canShoot = false;
                    }
                    break;
                default:
                    break;
            }

        //}
    }

    void ApplyBulletProperties(GameObject bulletProperties)
    {
        bulletProperties.GetComponent<BulletEnemy>().bulletSpeed = bulletSpeed;
        bulletProperties.GetComponent<BulletEnemy>().bulletDamage = bulletDamage;
        bulletProperties.GetComponent<BulletEnemy>().patternMovement = patternMovement;
        bulletProperties.GetComponent<BulletEnemy>().widthAmplitude = widthAmplitude;
        bulletProperties.GetComponent<BulletEnemy>().heightAmplitude = heightAmplitude;
        bulletProperties.GetComponent<BulletEnemy>().cicleSpeed = cicleSpeed;
        bulletProperties.transform.localScale = new Vector3(bulletScale + bulletScaleRegular, bulletScale + bulletScaleRegular, bulletScale + bulletScaleRegular);
    }
}
