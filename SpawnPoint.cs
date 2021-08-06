using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Configuración:")]
    [Tooltip("Prefab del character a spawnear.")]
    public GameObject characterToSpawn;
    [Tooltip("Animación de entrada que se va a instanciar al aparecer el character.")]
    public GameObject appearAnimation;
    [Tooltip("Tiempo que se retrasa el inicio del spawneo del character, después de spawnear su animación de entrada.")]
    public float characterSpawnDelay = 0.5f;
    [Tooltip("Si el character es un enemigo, spawnea según la variable 'roomActivationID'. Si no lo es, spawnea inmediatamente.")]
    public bool isAnEnemy = false;
    [Tooltip("En el caso de ser un enemigo, elegir en qué habitación se encuentra. Cuando el jugador entre en la misma, spawnea.")]
    public int roomActivationID = 0;
    //Bool indicando si la animación de entrada ya se instanció.
    bool animIsSpawned = false;
    [Tooltip("Prefab del character a spawnear.")]
    public GameObject spawnedCharacter;

    void Awake()
    {
        //Si no es un enemigo, se spawnea de antemano para que la cámara se posicione junto al héroe.
        //Una vez hecho, la cámara misma lo desaactiva
        if (isAnEnemy == false)
        {
            spawnedCharacter = Instantiate(characterToSpawn, transform.position, transform.rotation);
            spawnedCharacter.name = "Character";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Si el charcter a spawnear es un enemigo y el jugador se encuentra en la misma habitación
        if (isAnEnemy == true && GManager.roomID == roomActivationID)
        {
            SpawnCharacterNow();
        }
        else if (isAnEnemy == false)
        {
            SpawnCharacterNow();
        }
    }

    void SpawnCharacterNow()
    {
        //Si la animación de entrada todavía no fue instanciada.
        if (animIsSpawned == false)
        {
            //Se instancia la animación de entrada en la posición del spawn.
            Instantiate(appearAnimation, transform.position, Quaternion.identity);
            //Se establece que la animación ya fue instanciada.
            animIsSpawned = true;
            if (isAnEnemy == true)
            {
                GManager.enemiesToKill += 1;
            }
        }
        //Se resta tiempo al delay que hay entre el spawn de la animación de entrada y la del hero.
        characterSpawnDelay -= Time.deltaTime;
        //Si el tiempo se cumple:
        if (characterSpawnDelay <= 0)
        {
            //Se reactiva nuevamente al jugador, si no es un enemigo.
            if (isAnEnemy == false)
            {
                spawnedCharacter.SetActive(true);
            }
            //Si es un enemigo, se lo instancia.
            else
            {
                spawnedCharacter = characterToSpawn;
                spawnedCharacter.SetActive(true);
                GetComponent<Transform>().DetachChildren();
            }
            //Se destruye este gameobject.
            Destroy(gameObject);
        }
    }
}