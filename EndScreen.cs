using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//Clase encargada de las pantallas de Game Over / Victoria
public class EndScreen : MonoBehaviour
{
    [Header("Configuración de imágenes de Victoria.")]
    public Image backgroundVictory;
    public Image selection1Victory;
    public Image selection2Victory;
    public Image frontVictory;

    [Header("Configuración de imágenes de Derrota.")]
    public Image backgroundGameOver;
    public Image selection1GameOver;
    public Image selection2GameOver;
    public Image frontGameOver;

    [Header("Configuración de imágenes de degradado.")]
    public Image frontWhite;

    [Header("Configuración del cursor")]
    [Tooltip("Imagen en canvas del puntero que va a reemplazar al puntero del mouse.")]
    public Image crosshair;

    Image background;
    Image selection1;
    Image selection2;
    Image front;

    //Inputs
    KeyCode accept = KeyCode.Space;
    KeyCode accept2 = KeyCode.Return;

    //Manejo
    int selectionID = 1;
    bool selectionMade = false;
    bool enableActions = false;

    //Colores y opacidad
    Color opacity; //Color auxiliar para modificar la transparencia de los botones
    float alpha; //Transparencia de las imagenes del menú para los efectos fade in/out
    float fadeTime = 0.75f; //A menor número, más tarda

    //Sonidos
    public AudioClip cursorSound;
    public AudioClip selectSound;
    public AudioClip MEGameOver;
    public AudioClip MEVictory;

    // Use this for initialization
    void Start()
    {
        // Se hace al puntero del mouse invisible.
        Cursor.visible = false;

        //Si se establece la victoria en el game manager
        if (GManager.victory == true)
        {
            background = backgroundVictory;
            front = frontVictory;
            selection1 = selection1Victory;
            selection2 = selection2Victory;
            GetComponent<AudioSource>().clip = MEVictory;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            background = backgroundGameOver;
            front = frontGameOver;
            selection1 = selection1GameOver;
            selection2 = selection2GameOver;
            GetComponent<AudioSource>().clip = MEGameOver;
            GetComponent<AudioSource>().Play();
        }
        //Se activan las imagenes
        background.enabled = true;
        front.enabled = true;
        selection1.enabled = true;
        selection2.enabled = true;
        frontWhite.enabled = true;

        //Se resalta la primer opcion
        selectionID = 1;
        opacity = selection1.color;
        alpha = 1f;
        enableActions = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCursorPosition();
        Inputs();
        UpdateAlpha();
        UpdateGraphics();
    }

    private void UpdateCursorPosition()
    {
        
        //Se iguala la posición de la instancia del prefab de la mira y la posición del mouse.
        crosshair.transform.position = Input.mousePosition;
    }

    public virtual void Inputs()
    {
        //Si estan habilitados, se puede elegir entre 2 opciones
        if (!selectionMade && enableActions)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W))
            {
                GetComponent<AudioSource>().PlayOneShot(cursorSound);
                selectionID++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S))
            {
                GetComponent<AudioSource>().PlayOneShot(cursorSound);
                selectionID--;
            }
            else if (Input.GetKeyDown(accept) || Input.GetKeyDown(accept2))
            {
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().PlayOneShot(selectSound);
                selectionMade = true;
                enableActions = false;
                fadeTime = 0.5f;
                alpha = 1f;
            }
        }
    }

    //Actualizacion de la opacidad
    void UpdateAlpha()
    {
        if (!enableActions)
        {
            if (alpha > 0)
            {
                alpha -= fadeTime * Time.deltaTime;
            }
            else
            {
                alpha = 0f;
                enableActions = true;
            }
        }
    }

    //Segun la seleccion, se ilumina cada cosa
    void UpdateGraphics()
    {
        if (selectionMade)
        {
            opacity.a = alpha * 0.3f;
            selection1.color = opacity;
            selection2.color = opacity;
            switch (selectionID)
            {
                case 1:
                    frontWhite.color = new Color(0f, 0f, 0f, 1f - alpha);
                    if (alpha <= 0)
                    {
                        //Al menu
                        SceneChange("MainMenu");
                    }
                    break;
                case 2:
                    frontWhite.color = new Color(0f, 0f, 0f, 1f - alpha);
                    if (alpha <= 0)
                    {
                        //Salir
                        Application.Quit();
                    }
                    break;
            }
        }
        else
        {
            //Si no se pueden usar los controles, se transiciona
            opacity.a = 0.3f - alpha * 0.3f;
            selection1.color = opacity;
            selection2.color = opacity;
            switch (selectionID)
            {
                case 1:
                    if (GManager.victory == true)
                    {
                        frontWhite.color = new Color(1f, 1f, 1f, alpha);
                    }
                    else
                    {
                        frontWhite.color = new Color(0f, 0f, 0f, alpha);
                    }
                    if (enableActions == true)
                    {
                        opacity.a = 0.8f;
                        selection1.color = opacity;
                    }
                    break;
                case 2:
                    if (enableActions == true)
                    {
                        opacity.a = 0.8f;
                        selection2.color = opacity;
                    }
                    break;
                default:
                    //Correcion de IDs
                    if (selectionID <= 0 && alpha == 0)
                    {
                        selectionID = 2;
                    }
                    else if (selectionID >= 3 && alpha == 0)
                    {
                        selectionID = 1;
                    }
                    break;
            }

        }
    }

    //Función encargada de establece la ID del menú
    public void SetMenuId(int value)
    {
        //Se hace si el input está activado:
        if (enableActions)
        {
            if (value != selectionID)
            {
                GetComponent<AudioSource>().PlayOneShot(cursorSound);
            }
            selectionID = value;
        }
    }

    public void ButtonSelection(int selectionOption)
    {
        selectionID = selectionOption;
        //Si es que el jugador todavía puede manejar los inputs:
        if (enableActions)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(selectSound);
            selectionMade = true;
            enableActions = false;
            fadeTime = 0.5f;
            alpha = 1f;
        }
    }

    //Función encargada de cambiar de escenas, según el string que le pasamos:
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
