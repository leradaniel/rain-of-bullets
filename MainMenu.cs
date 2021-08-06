using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    #region DECLARACIÓN DE VARIABLES
    [Header("Configuración de imágenes.")]
    [Tooltip("Imagen de fondo para el menú:")]
    public Image backgroundMenu;
    [Tooltip("Imagen frontal para el menú con layout de botones:")]
    public Image frontMenu;
    [Tooltip("Imagen 1 a la cual transicionar (Tutorial):")]
    public Image tutorialPicture;
    [Tooltip("Imagen 2 a la cual transicionar:")]
    public Image creditsPicture;
    [Tooltip("Imagen de fondo para el botón seleciconable 1:")]
    public Image buttonBackground1;
    [Tooltip("Imagen de fondo para el botón seleciconable 2:")]
    public Image buttonBackground2;
    [Tooltip("Imagen de fondo para el botón seleciconable 3:")]
    public Image buttonBackground3;
    [Tooltip("Imagen de fondo para el botón seleciconable 4:")]
    public Image buttonBackground4;
    [Tooltip("Imagen de fondo para el botón seleciconable 5:")]
    public Image buttonBackground5;
    [Tooltip("Imagen superior para la transición entre escena:")]
    public Image fadePicture;
    [Tooltip("Imagen de detalle por encima de las demás:")]
    public Image detailMenu;
    [Tooltip("Imagen que se superpone cuando se elige 'Empezar':")]
    public Image overlayPicture;
    [Tooltip("Transparencia para los botones inactivos (0 transparente - 255 visible):")]
    [Range(0, 255)]
    public int inactiveAlpha;
    //inactiveAlpha sirve para el inspector, el real es el float que se usa para el codigo.
    float inactiveRealAlpha;

    [Header("Configuración de Transiciones.")]
    [Tooltip("Color del cual se viene de la escena anterior:")]
    public Color fadeInColor;
    [Tooltip("Tiempo en ms que tarda en mostrarse la pantalla:")]
    public float fadeInTime;
    [Tooltip("Tiempo en ms que tarda en mostrarse el menú por primera vez y activarlo:")]
    public float fadeInMenuTime;
    [Tooltip("Color al cual se va a la próxima escena:")]
    public Color fadeOutColor;
    [Tooltip("Tiempo en ms que tarda en esconderse el menú y pasar a la próxima escena:")]
    public float fadeOutTime;
    [Tooltip("Tiempo en ms que tarda la transición entre selecciones:")]
    public float transitionTime;

    [Header("Configuración del cursor")]
    [Tooltip("Imagen en canvas del puntero que va a reemplazar al puntero del mouse.")]
    public Image crosshair;
    //public SpriteRenderer crosshairPrefab;
    //Variable que va a almacenar la instancia de la mira.
    //Image crosshair;

    //Configuración de Inputs
    KeyCode acceptKey = KeyCode.Space; //Botón para aceptar
    KeyCode acceptKey2 = KeyCode.Return; //Botón para aceptar
    KeyCode cancelKey = KeyCode.Escape; //Botón para cancelar

    //Variables auxiliares para colores y opacidad
    Color opacity; //Color auxiliar para modificar la transparencia de colores elegidos en el inspector
    Color opacity2; //Color auxiliar para modificar la transparencia de colores elegidos en el inspector
    float alpha = 1; //Transparencia de las imagenes del menú para los efectos fade in/out
                     //float fadeTime = 0.75f; //A menor número, más tarda
    Color opacityButton; //Color auxiliar para modificar la transparencia de los botones
    Color opacityButtonCancel; //Color auxiliar para modificar la transparencia del boton de cancelar
    float positiveNegative = 1f; //Variable que es 1 o -1 para ser multiplicada y variar
    //entre un resultado positivo o negativo. Principalmente para saber si se entra o sale de un menú.

    //Las instancias de trabajo del menú se dividen en pasos, indicadas por esta variable:
    int step = 0;
    //Qué opción del menú está seleccionada actualmente
    int selectionID = 1;
    //Lista con las imágenes de los botones
    List<Image> buttonList = new List<Image>();
    //Este booleano indica si el jugador puede manejar los inputs o no.
    bool actionsEnabled = false;

    //Sonidos a reproducir, seteados en el inspector
    public AudioClip cursorSound;
    public AudioClip selectSound;
    public AudioClip cancelSound;
    public AudioClip startSound;
    public GameObject bgmContainer;
    #endregion

    void Awake()
    {
        //Se activan las imagenes y se les asignan sus colores:
        Cursor.visible = true;
        backgroundMenu.enabled = true;
        frontMenu.enabled = true;
        tutorialPicture.enabled = true;
        tutorialPicture.color = new Color(1f, 1f, 1f, 0f);
        creditsPicture.enabled = true;
        creditsPicture.color = new Color(1f, 1f, 1f, 0f);
        buttonBackground1.enabled = true;
        buttonBackground2.enabled = true;
        buttonBackground3.enabled = true;
        buttonBackground4.enabled = true;
        overlayPicture.enabled = false;
        fadePicture.enabled = true;
        detailMenu.enabled = true;
        fadePicture.color = fadeInColor;
        //Al empezar, se establece que el menú se enceuntra en el primer paso:
        step = 1;

        //Se agregan los botones a la lista:
        buttonList.Add(buttonBackground1);
        buttonList.Add(buttonBackground2);
        buttonList.Add(buttonBackground3);
        buttonList.Add(buttonBackground4);
    }

    // Use this for initialization
    void Start()
    {
        // Se hace al puntero del mouse invisible.
        Cursor.visible = false;
        //Se instancia el prefab de la mira.
        //crosshair = Instantiate(crosshairPrefab);

        //Se pasan los milisegundos establecidos en el inspector a segundos:
        fadeInTime /= 1000;
        fadeInMenuTime /= 1000;
        transitionTime /= 1000;
        fadeOutTime /= 1000;
        //Se pasa el alpha a segundos:
        inactiveRealAlpha = inactiveAlpha / 255f;
    }

    // Update frame a frame
    void Update()
    {
        UpdateCursorPosition();
        //Según el paso en el cual se encuentra el menú, se realizan las respectivas acciones.
        //Cada paso tiene su descripción para entender qué hacen.
        switch (step)
        {
            #region STEP 1: FADE IN / FADE OUT / SALIR DEL JUEGO
            case 1:
                //El juego entra desde una pantalla negra para revelar el menú.
                //Se adquieren las opacidades y colores:
                opacity = fadePicture.color;
                opacity2 = frontMenu.color;
                opacityButton = buttonList[0].color;
                opacityButtonCancel = buttonBackground5.color;

                //La opacidad de la pantalla negra es mayor a 1 cuando se sale del juego:
                if (opacity.a > 1)
                {
                    //Se cierra el juego
                    print("JUEGO CERRADO");
                    Application.Quit();
                }
                //Si la opacidad de la pantalla negra no es mayor a 1, pero es mayor a 0:
                else if (opacity.a >= 0)
                {
                    //Se le resta opacidad a la pantalla negra para revelar el menú:
                    opacity.a -= positiveNegative / fadeInTime * Time.deltaTime;
                    fadePicture.color = opacity;

                }
                //Si la opacidad de la pantalla negra es menor a 0 (se ve el menú),
                //y además los botones se iluminaron (ver el else paso para entender esto):
                else if (opacityButton.a >= inactiveRealAlpha)
                {
                    //Se le permite al jugador manejar al menú y se activa el paso 2:
                    actionsEnabled = true;
                    step = 2;
                }
                //Se entra en el else si la pantalla negra ya reveló el menú, pero los botones todavía
                //no se iluminaron.
                else
                {
                    //La opacidad de los botones empieza a aumentar.
                    opacityButton.a += inactiveRealAlpha / fadeInMenuTime * Time.deltaTime;
                    //A cada botón se le asigna la opacidad nueva:
                    for (int i = 0; i < buttonList.Count; i++)
                    {
                        buttonList[i].color = opacityButton;
                    }
                    //Al mismo tiempo, se empieza a mostrar el layout del menú:
                    opacity2.a += positiveNegative / fadeInMenuTime * Time.deltaTime;
                    frontMenu.color = opacity2;
                }
                break;
            #endregion
            #region STEP 2: ACTIVAR GRAFICOS DE LOS BOTONES / INPUTS
            case 2:
                //Si se oprimen la flecha de abajo o la S del teclado:
                if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    //La ID que indica qué opción del menú está seleccionado aumenta:
                    selectionID++;
                    //Se reproduce el sonido:
                    GetComponent<AudioSource>().clip = cursorSound;
                    GetComponent<AudioSource>().Play();
                    //En el caso de superar la cantidad máxima de opciones, se vuelve al valor más bajo:
                    if (selectionID >= 5)
                    {
                        selectionID = 1;
                    }
                }
                //En el caso de apretar arriba o la W, ocurre lo mismo pero al revés:
                else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    //La ID disminuye:
                    selectionID--;
                    //Se reproduce el sonido:
                    GetComponent<AudioSource>().clip = cursorSound;
                    GetComponent<AudioSource>().Play();
                    //Y si está por debajo de la cantidad mínima de opciones, se asigna el balor más alto:
                    if (selectionID <= 0)
                    {
                        selectionID = 4;
                    }
                }
                //Si se oprime la tecla de aceptar asignada previamente:
                else if (Input.GetKeyDown(acceptKey) || Input.GetKeyDown(acceptKey2))
                {
                    //Se llama a la función que maneja las selecciones del menú.
                    //Dicha función establece cuál va a ser el próximo paso, según la ID que
                    //se le pasa como parámetro
                    ButtonSelection(selectionID);
                }
                //Se le asigna una opacidad baja a todos los botones.
                opacityButton.a = inactiveRealAlpha;
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].color = opacityButton;
                }
                //Y según la ID actual, se le asigna una opacidad alta al botón correspondiente:
                switch (selectionID)
                {
                    case 1:
                        opacityButton.a = 1;
                        buttonBackground1.color = opacityButton;
                        break;
                    case 2:
                        opacityButton.a = 1;
                        buttonBackground2.color = opacityButton;
                        break;
                    case 3:
                        opacityButton.a = 1;
                        buttonBackground3.color = opacityButton;
                        break;
                    case 4:
                        opacityButton.a = 1;
                        buttonBackground4.color = opacityButton;
                        break;
                }
                break;
            #endregion
            #region STEP 3: TRANSICIONAR ENTRE MENU PRINCIPAL Y EL ELEGIDO (Y VICEVERSA)
            case 3:
                //Se le baja la opacidad la layout del menu
                alpha -= positiveNegative / transitionTime * Time.deltaTime;
                frontMenu.color = new Color(1f, 1f, 1f, alpha);
                //En el caso de elegir la opción 2, la imagen a mostrar es la del tutorial:
                if (selectionID == 2)
                {
                    //La opacidad es total, restándole el alpha (que va baajndo de 0 a 1)
                    tutorialPicture.color = new Color(1f, 1f, 1f, 1f - alpha);
                }
                //En el caso de elegir la opción 3, la imagen a mostrar es la de los créditos:
                else if (selectionID == 3)
                {
                    //La opacidad es total, restándole el alpha (que va baajndo de 0 a 1)
                    creditsPicture.color = new Color(1f, 1f, 1f, 1f - alpha);
                }

                //Se establece una especie de clamp para que la variable no se exceda
                if (opacityButton.a > inactiveRealAlpha)
                {
                    opacityButton.a = inactiveRealAlpha;
                }

                //Se actualiza la variable de la opacidad que se le asigna a los botones:
                opacityButton.a -= positiveNegative * inactiveRealAlpha / transitionTime * Time.deltaTime;
                opacityButtonCancel.a -= -positiveNegative * inactiveRealAlpha / transitionTime * Time.deltaTime;
                //Se asigna la opacidad a los botones, que van desvaneciendo:
                for (int i = 0; i < buttonList.Count; i++)
                {
                    buttonList[i].color = opacityButton;
                }
                buttonBackground5.color = opacityButtonCancel;


                //Si el alpha es menor a 0, se pasa al paso 4
                if (alpha < 0)
                {
                    step = 4;
                }
                //Si aumenta, se vuelve al paso 2 y se activan las acciones
                else if (alpha > 1)
                {
                    actionsEnabled = true;
                    step = 2;
                }
                break;
            #endregion
            #region STEP 4: INPUT DE ESCAPE
            case 4:
                //Si se está mostrando al iamgen de tutorial/créditos y se oprime Escape
                if (Input.GetKeyDown(cancelKey))
                {
                    CancelCall();
                }
                break;
            #endregion
            #region STEP 5: CARGAR LA PRÓXIMA ESCENA (START)
            case 5:
                //Se congela la animación del fondo:
                backgroundMenu.GetComponent<Animator>().speed = 0;
                //Se muestra una imagen como overlay:
                overlayPicture.enabled = true;
                //Se actualiza la opacidad de la imagen frontal,
                //para que haga un flash para resaltar más todavía la selección.
                opacity = fadePicture.color;
                opacity.a -= positiveNegative / fadeOutTime * 2 * Time.deltaTime;
                fadePicture.color = opacity;

                //Si el alpha llega menor a 0:
                if (opacity.a <= 0)
                {
                    //Se le cambia el color a negro para prepararlo para la transición
                    fadeInColor.a = 0f;
                    fadePicture.color = fadeInColor;
                    //Se establece la variable auxiliar para que empiece a aumentar el fade:
                    positiveNegative = -1f;
                }
                //Si el alpha llega mayor a 1:
                else if (opacity.a > 1)
                {
                    //Se carga el nivel 1:
                    SceneChange("Stage 1");
                }
                break;
            #endregion
            default:
                break;
        }

    }

    public void CancelCall()
    {
        if (step == 4)
        {
            //Se reproduce el sonido
            GetComponent<AudioSource>().clip = cancelSound;
            GetComponent<AudioSource>().Play();
            //Se indica con la variable auxiliar:
            positiveNegative = -1f;
            //Se veulve al paso 3:
            step = 3;
        }
    }

    public void HighlightCancelButton(bool highlight)
    {
        if (step == 4)
        {
            Color opacityAux; //Color auxiliar para modificar la transparencia de los botones
            opacityAux = buttonBackground5.color;
            if (highlight == true && opacityAux.a != 1f)
            {
                //Se reproduce el sonido
                GetComponent<AudioSource>().clip = cursorSound;
                GetComponent<AudioSource>().Play();
                //Se asigna
                opacityAux.a = 1f;
                buttonBackground5.color = opacityAux;
            }
            else if (highlight == false)
            {
                opacityAux.a = inactiveRealAlpha;
                buttonBackground5.color = opacityAux;
            }

        }
    }

    //Función encargada de establece la ID del menú
    public void SetMenuId(int value)
    {
        //Se hace si el input está activado:
        if (actionsEnabled)
        {
            if (value != selectionID)
            {
                GetComponent<AudioSource>().clip = cursorSound;
                GetComponent<AudioSource>().Play();
            }
            selectionID = value;
        }
    }

    //Función encargada de manejar el próximo paso al hacer una selección del menú
    public void ButtonSelection(int selectionMade)
    {
        //Si es que el jugador todavía puede manejar los inputs:
        if (actionsEnabled)
        {
            //Se toma la ID de la opción seleccionada:
            selectionID = selectionMade;
            //Se desactivan los inputs:
            actionsEnabled = false;
            //Se actúa según la ID tomada:
            switch (selectionMade)
            {
                //Caso 1: Empezar el juego
                case 1:
                    //Se reproduce el sonido:
                    GetComponent<AudioSource>().clip = startSound;
                    GetComponent<AudioSource>().Play();
                    //Se detiene la musica de fondo:
                    bgmContainer.GetComponent<AudioSource>().Stop();
                    //Se establece la variable auxiliar:
                    positiveNegative = 1f;
                    //El color de fade toma el color establecido (blanco):
                    fadePicture.color = fadeOutColor;
                    //Se pasa al paso 5:
                    step = 5;
                    break;
                //Caso 2: Tutorial
                case 2:
                    //Se reproduce el sonido:
                    GetComponent<AudioSource>().clip = selectSound;
                    GetComponent<AudioSource>().Play();
                    //Se establece la variable auxiliar:
                    positiveNegative = 1f;
                    //Se pasa al paso 3:
                    step = 3;
                    break;
                //Caso 3: Créditos
                case 3:
                    //Se reproduce el sonido:
                    GetComponent<AudioSource>().clip = selectSound;
                    GetComponent<AudioSource>().Play();
                    //Se establece la variable auxiliar:
                    positiveNegative = 1f;
                    //Se pasa al paso 3:
                    step = 3;
                    break;
                //Caso 3: Salir
                case 4:
                    //Se reproduce el sonido:
                    GetComponent<AudioSource>().clip = selectSound;
                    GetComponent<AudioSource>().Play();
                    //Se establece la variable auxiliar:
                    positiveNegative = -1f;
                    //Se establecen las variables para la transición fade
                    opacity = fadePicture.color;
                    opacity.a = 0;
                    fadePicture.color = opacity;
                    //Se vuelve al paso 1, pero con positiveNegative indicando que aumenta la opacidad:
                    step = 1;
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateCursorPosition()
    {
        //Se iguala la posición de la instancia del prefab de la mira y la posición del mouse.
        crosshair.transform.position = Input.mousePosition;
    }

    //Función encargada de cambiar de escenas, según el string que le pasamos:
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
