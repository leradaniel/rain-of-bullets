using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterBody : MonoBehaviour
{
    #region DECLARACIÓN DE VARIABLES

    [Header("Configuración:")]
    [Tooltip("Indica si recibe un cerebro enemigo o si es controlado por el jugador.")]
    public bool isEnemy = false;
    [Tooltip("Indica si es el boss.")]
    public bool isBoss = false;
    [Tooltip("Indica de qué nivel es el boss.")]
    public int stageBossID = 0;
    [Tooltip("Animación que deja al morir.")]
    public GameObject deathExplotion;
    [Tooltip("Prefab de la Barra de vida.")]
    public GameObject prefabLifeBar;
    [Tooltip("GameObject con la posición donde va a instanciarse la barra de vida.")]
    public GameObject lifebarPosition;

    [Header("Estadísticas:")]
    [Tooltip("Movimiento del character.")]
    public float movementSpeed = 4;
    [Tooltip("Salud máxima del character.")]
    public int hpMax = 3;
    [Tooltip("Salud actual del character.")]
    public int hpNow = 3;
    [Tooltip("GameObject que apunta para atacar.")]
    public GameObject shotPoint;

    [HideInInspector]
    public Animator characterAnimator;//Animator del character
    //La velocidad del animator
    float animatorSpeed;
    [HideInInspector]
    public bool isSpawning = false;//Indica si el character está spawneando
    [HideInInspector]
    public bool isShooting = false; //Indica si el character está atacando
    [HideInInspector]
    public bool isHit = false; //Indica si el character recibió un golpe
    [HideInInspector]
    public bool isDead = false; //Indica si el character recibió un golpe letal
    [Tooltip("Cuando recibe un golpe, cuánto tiempo queda en estado de recibir un golpe")]
    public float immunityTimeMax = 0.5f;
    [HideInInspector]
    public float immunityTimeNow = 0f;//Contador
    [Tooltip("Cuando recibe un golpe, cuánto tiempo de inmunidad tiene. Solamente para el jugador.")]
    public float realImmunityTimeMax = 0.5f;
    [HideInInspector]
    public float realImmunityTimeNow = 0f;//Contador
    [Tooltip("Cuánto tiempo tarda el character muerto en dejar su animación de muerte.")]
    public float deadExplotionTimeMax = 2f;
    [HideInInspector]
    public float deadExplotionTimeNow = 0f;//Contador
    //Rigidbody del character
    Rigidbody2D rb;
    [Tooltip("GameObject del child que contiene el gráfico del jugador.")]
    public SpriteRenderer sr;//Gráfico del character
    //GameObject de la barra de vida
    GameObject lifeBar;

    [HideInInspector]
    public HeroBrain heroBrain;//Cerebro con los inputs del jugador
    [HideInInspector]
    public HeroFunctions hero;//Funciones especiales del jugador
    [HideInInspector]
    public EnemyBrain enemyBrain;//Cerebro con los inputs del enemigo
    [HideInInspector]
    public EnemyFunctions enemy;//Cerebro con los inputs del enemigo

    float timeToEndGame = 3f;
    float timeToEndGameNow = 0f;

    [Header("Sonido:")]
    [Tooltip("Sonido de movimiento")]
    public AudioClip movementSound;
    [Tooltip("Sonido cuando es golpeado")]
    public AudioClip getHit;
    [Tooltip("Sonido de muerte")]
    public AudioClip deathSound;
    [Tooltip("Sonido cuando se esquiva (solamente para el jugador)")]
    public AudioClip dashSound;

    [HideInInspector]
    public bool deadSoundPlayed = false;

    bool dashSoundPlaying = false;

    [HideInInspector]
    public bool realImmunity = false;
    #endregion

    // Use this for initialization
    void Awake()
    {
        if (isEnemy)
        {
            realImmunityTimeMax = immunityTimeMax;
        }
        else
        {
            realImmunityTimeMax += immunityTimeMax;
        }
        //Se establece que el character está spawneando
        isSpawning = true;
        //Se iguala la salud actual a la máxima
        hpNow = hpMax;
        //Si es un enemigo, se le otorga un cerebro del tal.
        if (isEnemy == true)
        {
            enemyBrain = new EnemyBrain();
            enemyBrain.GetBody(this);
            enemy = GetComponent<EnemyFunctions>();
            enemy.GetBody(this);
            enemyBrain.GetEnemyFunctions(enemy);
        }
        //De lo contrario, se le da control al jugador
        else
        {
            //Se le da un cerebro de jugador.
            heroBrain = new HeroBrain();
            //Se le pasa el cuerpo al cerebro.
            heroBrain.GetBody(this);
            //Se une el cuerpo a las funciones especiales del jugador.
            hero = GetComponent<HeroFunctions>();
            hero.GetBody(this);
            //También las funciones especiales de un jugador.
            heroBrain.GetHeroFunctions(hero);
        }
        //Se instancia la barra de vida en posición.
        lifeBar = Instantiate(prefabLifeBar, lifebarPosition.transform.position, Quaternion.identity);
        //Se la agrega como child, para que se mueva con el personaje.
        lifeBar.transform.parent = gameObject.transform;
        //Se desactiva
        //lifeBar.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        //Se guarda el animator del character
        characterAnimator = GetComponentInChildren<Animator>();
        //Se guarda la velocidad el animator
        animatorSpeed = characterAnimator.speed;
        //Se guarda el rigid body
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GManager.pause == false)
        {
            if (isEnemy == true)
            {
                enemyBrain.InputListener();
            }
            else
            {
                heroBrain.InputListener();
            }
            characterAnimator.speed = animatorSpeed;
            ActivateHitImmunity();
            UpdateHealthbar();
            Animation();
            Die();
            Spawn();
            UpdateRealImmunity();
        }
        else
        {
            characterAnimator.speed = 0;
            isShooting = false;
            rb.velocity = new Vector2(0f, 0f);
        }
    }

    void UpdateRealImmunity()
    {
        if (realImmunity == true)
        {
            realImmunityTimeNow += Time.deltaTime;
            if (realImmunityTimeNow > realImmunityTimeMax)
            {
                realImmunityTimeNow = 0f;
                realImmunity = false;
            }
        }
    }

    private void UpdateHealthbar()
    {
        //Según la barra de vida que tenga el character, se actualizan los datos
        if (lifeBar.GetComponent<Lifebar>())
        {
            lifeBar.GetComponent<Lifebar>().hpNow = hpNow;
            lifeBar.GetComponent<Lifebar>().hpMax = hpMax;
        }
        else if (lifeBar.GetComponent<LifeBarHero>())
        {
            lifeBar.GetComponent<LifeBarHero>().hpNow = hpNow;
            lifeBar.GetComponent<LifeBarHero>().hpMax = hpMax;
        }
    }

    //Función que indica si el gráfico del character tiene que girar o no
    public void CharacterFlip(bool isFlip)
    {
        sr.flipX = isFlip;
    }

    public void CharacterAiming(Vector3 targetPosition)
    {
        //aim.transform.rotation = Quaternion.LookRotation(Vector3.forward, targetPosition - shotPoint.transform.position);
        shotPoint.transform.rotation = Quaternion.LookRotation(Vector3.forward, targetPosition - shotPoint.transform.position);
    }

    public void CharacterMovement(Vector2 direction, float speed)
    {
        if (GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
        {

            rb.velocity = direction * speed;

            if (isEnemy == false)
            {
                if (hero.isDashing == true && dashSoundPlaying == false)
                {
                    dashSoundPlaying = true;
                    GetComponent<AudioSource>().PlayOneShot(dashSound);
                }
                else if (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) > 0 && hero.isDashing == false)
                {
                    dashSoundPlaying = false;
                    if (!GetComponent<AudioSource>().isPlaying)
                    {
                        GetComponent<AudioSource>().PlayOneShot(movementSound);
                    }
                }
                else if (direction == new Vector2(0f, 0f))
                {
                    dashSoundPlaying = false;
                    if (isHit == false && isDead == false && hero.isReviving == false)
                    {
                        GetComponent<AudioSource>().Stop();
                    }
                }
            }

        }

    }

    void RecieveDamage(int damage)
    {
        if (realImmunity == false)
        {
            if (isHit == false)
            {
                realImmunity = true;
                isHit = true;
                hpNow -= (damage);
                hpNow = Mathf.Clamp(hpNow, 0, hpMax);
                GetComponent<AudioSource>().clip = getHit;
                GetComponent<AudioSource>().Play();
            }
        }
        
    }

    void RecieveImmunityTime(float immunityTime)
    {
        if (immunityTime > 0)
        {
            immunityTimeMax = immunityTime;
        }
    }

    void Spawn()
    {
        if (isSpawning == true)
        {
            GetComponent<Collider2D>().enabled = false;
            CharacterMovement(new Vector2(0f, 0f), 0f);
            if (isEnemy == true)
            {
                enemyBrain.isEnabled = false;
            }
            else
            {
                heroBrain.isEnabled = false;
            }
            if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                isSpawning = false;
                GetComponent<Collider2D>().enabled = true;
                if (isEnemy == true)
                {
                    enemyBrain.isEnabled = true;
                }
                else
                {
                    heroBrain.isEnabled = true;
                }
            }
        }

    }

    void ActivateHitImmunity()
    {
        if (isHit)
        {
            if (isEnemy == true)
            {
                enemyBrain.isEnabled = false;
            }
            else
            {
                heroBrain.isEnabled = false;
                if (isShooting)
                {
                    hero.DeleteShoot();
                }
            }
            GetComponent<Collider2D>().enabled = false;
            CharacterMovement(new Vector2(0f, 0f), 0f);


            if (hpNow > 0)
            {
                immunityTimeNow += Time.deltaTime;
                if (immunityTimeNow > immunityTimeMax)
                {
                    isHit = false;
                    immunityTimeNow = 0f;
                    GetComponent<Collider2D>().enabled = true;
                    if (isEnemy == true)
                    {
                        enemyBrain.isEnabled = true;
                    }
                    else
                    {
                        heroBrain.isEnabled = true;
                    }
                }
            }
            else
            {
                isDead = true;
                if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dead") && deadSoundPlayed == false)
                {
                    GetComponent<AudioSource>().clip = deathSound;
                    GetComponent<AudioSource>().Play();
                    deadSoundPlayed = true;
                }
            }
        }
    }

    void Die()
    {
        if (isDead == true)
        {
            deadExplotionTimeNow += Time.deltaTime;
            if (deadExplotionTimeNow > deadExplotionTimeMax)
            {
                if (isEnemy == true)
                {
                    Instantiate(deathExplotion, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                    if (isBoss == true)
                    {
                        timeToEndGameNow++;
                        if (timeToEndGameNow > timeToEndGame)
                        {
                            if (GManager.stage == 1)
                            {
                                GManager.stage = 2;
                                SceneChange("Stage 2");
                                Destroy(gameObject);
                            }
                            else if (GManager.stage == 2)
                            {
                                GManager.stage = 3;
                                SceneChange("Stage 3");
                                Destroy(gameObject);
                            }
                            else if (GManager.stage == 3)
                            {
                                GManager.victory = true;
                                SceneChange("EndScreen");
                                Destroy(gameObject);
                            }
                            
                        }
                    }
                    else
                    {
                        GManager.enemiesToKill -= 1;
                        Destroy(gameObject);
                    }
                }
                else if (isEnemy == false && hero.exploded == false)
                {
                    Instantiate(deathExplotion, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                    sr.enabled = false;
                    hero.exploded = true;
                }
            }
        }
    }

    void Animation()
    {
        characterAnimator.SetFloat("animSpeed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
        characterAnimator.SetBool("animIsHit", isHit);
        characterAnimator.SetBool("animIsDead", isDead);
        characterAnimator.SetBool("animIsShooting", isShooting);
    }

    //Función encargada de cambiar de escenas, según el string que le pasamos:
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}