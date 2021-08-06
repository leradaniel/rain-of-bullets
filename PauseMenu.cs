using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    #region DECLARACIÓN DE VARIABLES
    [Header("Configuración de imágenes.")]
    [Tooltip("Imagen de fondo para la pausa:")]
    public Image backgroundMenu;
    [Tooltip("Imagen trasera de la pausa:")]
    public Image backMenu;
    [Tooltip("Imagen frontal de la pausa:")]
    public Image frontMenu;
    [Tooltip("Imagen de fondo para el botón seleciconable 1:")]
    public Image buttonBackground1;
    [Tooltip("Imagen de fondo para el botón seleciconable 2:")]
    public Image buttonBackground2;
    [Tooltip("Imagen de fondo para el botón seleciconable 3:")]
    public Image buttonBackground3;
    [Tooltip("Imagen de fondo para el botón seleciconable 4:")]
    public Image buttonBackground4;
    [Tooltip("Imagen superior para la transición entre escenas:")]
    public Image fadePicture;
    [Tooltip("Transparencia para los botones inactivos (0 transparente - 255 visible):")]
    [Range(0, 255)]
    public int inactiveAlpha;
    //inactiveAlpha sirve para el inspector, el real es el float que se usa para el codigo.
    float inactiveRealAlpha;

    [Header("Configuración de Transiciones.")]
    [Tooltip("Tiempo en ms que tarda en mostrarse el menú por primera vez y activarlo:")]
    public float fadeInMenuTime;
    [Tooltip("Color al cual se va a la próxima escena:")]
    public Color fadeOutColor;
    [Tooltip("Tiempo en ms que tarda en esconderse el menú y pasar a la próxima escena:")]
    public float fadeOutTime;

    //Configuración de Inputs
    KeyCode acceptKey = KeyCode.Space; //Botón para aceptar
    KeyCode acceptKey2 = KeyCode.Return; //Botón para aceptar
    KeyCode cancelKey = KeyCode.Escape; //Botón para cancelar

    //Variables auxiliares para colores y opacidad
    Color opacity; //Color auxiliar para modificar la transparencia de colores elegidos en el inspector
    Color opacity2; //Color auxiliar para modificar la transparencia de colores elegidos en el inspector
    Color opacityButton; //Color auxiliar para modificar la transparencia de los botones

    //Las instancias de trabajo del menú se dividen en pasos, indicadas por esta variable:
    int step = 0;
    //Qué opción del menú está seleccionada actualmente
    int selectionID = 1;
    //Lista con las imágenes de los botones
    List<Image> buttonList = new List<Image>();
    //Este booleano indica si el jugador puede manejar los inputs o no.
    bool actionsEnabled = false;

    //Sonidos a reproducir, seteados en el inspector
    public AudioClip startSound;
    public AudioClip cursorSound;
    public AudioClip selectSound;
    public AudioClip cancelSound;
    #endregion

    void Awake()
    {
        //Se activan las imagenes y se les asignan sus colores:
        backgroundMenu.enabled = true;
        backMenu.enabled = true;
        frontMenu.enabled = true;

        buttonBackground1.enabled = true;
        buttonBackground2.enabled = true;
        buttonBackground3.enabled = true;
        buttonBackground4.enabled = true;
        fadePicture.enabled = true;
        fadePicture.color = fadeOutColor;
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
        
        //Se pasan los milisegundos establecidos en el inspector a segundos:
        fadeInMenuTime /= 1000;
        fadeOutTime /= 1000;
        //Se pasa el alpha a segundos:
        inactiveRealAlpha = inactiveAlpha / 255f;

        opacityButton = buttonList[0].color;
    }

    // Update frame a frame
    void Update()
    {
        //Según el paso en el cual se encuentra el menú, se realizan las respectivas acciones.
        //Cada paso tiene su descripción para entender qué hacen.
        switch (step)
        {
            #region STEP 1: FADE IN
            case 1:
                //GManager.pause = true;
                //El juego entra desde una pantalla negra para revelar el menú.
                //Se adquieren las opacidades y colores:
                if (opacityButton.a >= inactiveRealAlpha)
                {
                    //Se le permite al jugador manejar al menú y se activa el paso 2:
                    actionsEnabled = true;
                    step = 2;
                }
                else
                {

                    opacityButton = buttonList[0].color;
                    //La opacidad de los botones empieza a aumentar.
                    opacityButton.a += inactiveRealAlpha / fadeInMenuTime * Time.deltaTime;
                    //A cada botón se le asigna la opacidad nueva:
                    for (int i = 0; i < buttonList.Count; i++)
                    {
                        buttonList[i].color = opacityButton;
                    }
                    //Al mismo tiempo, se empieza a mostrar el layout del menú:
                    opacity = backMenu.color;
                    opacity.a += 1f / fadeInMenuTime * Time.deltaTime;
                    backMenu.color = opacity;
                    frontMenu.color = opacity;

                    Color temp = backgroundMenu.color;
                    temp.a = 1f;
                    backgroundMenu.color = temp;
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
                else if (Input.GetKeyDown(cancelKey))
                {
                    ButtonSelection(1);
                }
                //Se le asigna una opacidad baja a todos los botones.

                if (actionsEnabled == true)
                {
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
                }


                break;
            #endregion

            #region STEP 3: CARGAR LA PRÓXIMA ESCENA (START)
            case 3:
                opacity = fadePicture.color;
                opacity.a += 1 / fadeOutTime * Time.deltaTime;
                fadePicture.color = opacity;

                if (fadePicture.color.a > 1)
                {
                    switch (selectionID)
                    {
                        case 2:
                            SceneChange("Stage 1");
                            break;
                        case 3:
                            SceneChange("MainMenu");
                            break;
                        case 4:
                            print("JUEGO CERRADO");
                            Application.Quit();
                            break;
                        default:
                            break;
                    }
                }
                break;
            #endregion
            default:
                break;
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
            //Para evitar un error:
            selectionID = selectionMade;
            //Se desactivan los inputs:
            actionsEnabled = false;
            //Se actúa según la ID tomada:
            switch (selectionMade)
            {
                //Caso 1: Continuar el juego
                case 1:
                    //Se hace invisible nuevamente todos los gráficos
                    opacity = backMenu.color;
                    opacity.a = 0;
                    backMenu.color = opacity;
                    frontMenu.color = opacity;
                    backgroundMenu.color = opacity;

                    opacityButton = buttonList[0].color;
                    opacityButton.a = 0f;
                    //A cada botón se le asigna la opacidad nueva:
                    for (int i = 0; i < buttonList.Count; i++)
                    {
                        buttonList[i].color = opacityButton;
                    }
                    //Se prepara el sonido para la proxima vez que se active la pausa
                    GetComponent<AudioSource>().clip = startSound;
                    step = 1;
                    GManager.pause = false;
                    gameObject.SetActive(false);
                    break;
                    //Todos los demas
                default:
                    //Se reproduce el sonido
                    GetComponent<AudioSource>().clip = cancelSound;
                    GetComponent<AudioSource>().Play();
                    step = 3;
                    break;
            }
        }
    }

    //Función encargada de cambiar de escenas, según el string que le pasamos:
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
