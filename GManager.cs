using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    //Variable estatica para que se pueda acceder de varios lados. Todos los GO chequean si el juego está en pausa
    public static bool pause;
    //Chequea en qué sala está el jugador. Todos los enemigos lo chequean para activarse.
    public static int roomID;
    //Chequea en qué sala está el jugador. Todos los enemigos lo chequean para activarse.
    public static bool victory = false;
    //Chequea en qué sala está el jugador. Todos los enemigos lo chequean para activarse.
    public static bool fighting = false;
    //Chequea en qué sala está el jugador. Todos los enemigos lo chequean para activarse.
    public static int enemiesToKill = 0;
    //Variable estatica para que se pueda acceder de varios lados. Todos los GO chequean si el juego está en pausa
    public static int stage = 1;

    [Header("Personaje principal")]
    [Tooltip("Punto donde va a spawnear el personaje.")]
    public GameObject characterSpawn;

    [Header("Configuración de la UI")]
    [Tooltip("Imagen en canvas del gráfico del puntero que va a reemplazar al puntero del mouse.")]
    public Image crosshair;
    [Tooltip("Puntero para el menú")]
    public Image crosshairMenu;



    [Tooltip("GameObject que contiene el Menú de Pausa.")]
    public GameObject pauseMenu;

    //El objeto al cual la cámara va a seguir:
    public GameObject objectToFollow;
    //Personaje principal:
    GameObject hero;

    void Awake()
    {
        //Reseteo del juego
        roomID = 0;
        victory = false;
        enemiesToKill = 0;
    }


    void Start()
    {
        //Se guarda al jugador en la variable
        hero = characterSpawn.GetComponent<SpawnPoint>().spawnedCharacter;
        //Se establece que la cámara va a seguir al jugador
        objectToFollow = hero;
        //Se desactiva al jugador para que tenga su introducción
        hero.SetActive(false);

        //Se hace al puntero del mouse invisible.
        Cursor.visible = false;
        //Se establece la pausa del juego como false.
        pauseMenu.SetActive(false);
        pause = false;
    }

    // Update frame a frame
    void Update()
    {
        print(enemiesToKill);
        //Se guarda en un Vector3 (mousePos) las posiciones del mouse en X, Y y Z (menos la distancia con la cámara).
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - Camera.main.transform.position.z));
        UpdateCursorPosition();
        PauseGame();
        PauseSound();
        if (enemiesToKill <= 0)
        {
            enemiesToKill = 0;
            fighting = false;
            objectToFollow = hero;
        }
        else
        {
            fighting = true;
        }

        switch(stage)
        {
            case 1:
                hero.GetComponent<HeroFunctions>().weaponIDMax = 2;
                break;
            case 2:
                hero.GetComponent<HeroFunctions>().weaponIDMax = 4;
                break;
            case 3:
                hero.GetComponent<HeroFunctions>().weaponIDMax = 5;
                break;
        }
    }

    void UpdateCursorPosition()
    {
        //Se iguala la posición de la instancia del prefab de la mira y la posición del mouse.
        if (pause == true)
        {
            crosshair.enabled = false;
            crosshairMenu.enabled = true;
            crosshairMenu.transform.position = Input.mousePosition;
        }
        else
        {
            crosshair.enabled = true;
            crosshairMenu.enabled = false;
            crosshair.transform.position = Input.mousePosition;
        }
    }

    void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GManager.pause == false)
        {
            pauseMenu.SetActive(true);
            pause = true;
        }
    }
    

    // LateUpdate, ejecutado después de todos los demás updates.
    void LateUpdate()
    {
        //Se igualan las posiciones en X y en Y del objeto a seguir y la cámara
        transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z);
    }

    void PauseSound()
    {
        if (pause == true)
        {
            GetComponent<AudioSource>().Pause();
        }
        else
        {
            GetComponent<AudioSource>().UnPause();
        }
    }


    //Función que devuelve cuántas vidas tiene el héroe, usado por el HUD.
    public int GetHeroLifes()
    {
        int heroLifes = hero.GetComponent<HeroFunctions>().lifesNow;
        return heroLifes;
    }

    //Función que devuelve cuántas vidas tiene el héroe, usado por el HUD.
    public int GetWeaponID()
    {
        int weapID = hero.GetComponent<HeroFunctions>().weaponID;
        return weapID;
    }

    //Función que devuelve cuántas vidas tiene el héroe, usado por el HUD.
    public int GetWeaponIDMax()
    {
        int weaponIDMax = hero.GetComponent<HeroFunctions>().weaponIDMax;
        return weaponIDMax;
    }

    //Función que devuelve cuántas vidas tiene el héroe, usado por el HUD.
    public List<float> GetWeaponsAmmo()
    {
        List<float> weapAmmo = hero.GetComponent<HeroFunctions>().ammo;
        return weapAmmo;
    }

    public List<float> GetWeaponsAmmoMax()
    {
        List<float> weapAmmoMax = hero.GetComponent<HeroFunctions>().ammoMax;
        return weapAmmoMax;
    }


}
