using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [Tooltip("Imágenes de las vidas en el HUD.")]
    public List<Image> lifesInHud = new List<Image>();
    [Tooltip("Contenedores de las armas en el HUD.")]
    public List<GameObject> weaponContainers = new List<GameObject>();
    [Tooltip("Imágenes de las barras de municiones, para agrandar o achicar según la munición que queda.")]
    public List<Image> weapAmmoInHud = new List<Image>();
    [Tooltip("Imágenes de las barras de municiones frontales, para oscurecer la que no esté activa.")]
    public List<Image> weapAmmoInnactive = new List<Image>();
    [Tooltip("Imágenes de fondo de las armas. La elegida se sobresalta.")]
    public List<Image> weapFrameBG = new List<Image>();
    [Tooltip("Imágenes de las vidas en el HUD.")]
    public List<Image> weapFrame = new List<Image>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LifesDisplay();
        AmmoDisplay();
        ParryDisplay();
        WeaponDisplay();
        WeaponHighlight();
        WeaponHUDPosition();
    }

    private void WeaponDisplay()
    {
        int weapIdMax = Camera.main.GetComponent<GManager>().GetWeaponIDMax();
        for (int i = 0; i < weaponContainers.Count; i++)
        {
            if (i <= weapIdMax)
            {
                weaponContainers[i].SetActive(true);
            }
            else
            {
                weaponContainers[i].SetActive(false);
            }
        }
    }

    void WeaponHUDPosition()
    {

        int weapIdMax = Camera.main.GetComponent<GManager>().GetWeaponIDMax();

        float width = weaponContainers[1].GetComponent<RectTransform>().rect.width;
        float posY = weaponContainers[0].transform.localPosition.y;
        float addvalue = weaponContainers[1].GetComponent<RectTransform>().rect.width/6;
        float parryDistance = 1.5f;
        switch (weapIdMax)
        {
            case 1:
                weaponContainers[1].transform.localPosition = new Vector2(0, posY);

                weaponContainers[0].transform.localPosition = new Vector2(weaponContainers[weapIdMax].transform.localPosition.x + width * parryDistance, posY);
                break;
            case 2:
                weaponContainers[1].transform.localPosition = new Vector2(-width/2f + addvalue/2, posY);
                weaponContainers[2].transform.localPosition = new Vector2(width/2f - addvalue/2, posY);

                weaponContainers[0].transform.localPosition = new Vector2(weaponContainers[weapIdMax].transform.localPosition.x + width * parryDistance, posY);

                break;
            case 3:
                weaponContainers[1].transform.localPosition = new Vector2(-width + addvalue, posY);
                weaponContainers[2].transform.localPosition = new Vector2(0, posY);
                weaponContainers[3].transform.localPosition = new Vector2(width - addvalue, posY);

                weaponContainers[0].transform.localPosition = new Vector2(weaponContainers[weapIdMax].transform.localPosition.x + width * parryDistance, posY);
                break;
            case 4:
                weaponContainers[1].transform.localPosition = new Vector2(-width*1.5f+ addvalue*1.5f, posY);
                weaponContainers[2].transform.localPosition = new Vector2(-width /2f + addvalue / 2, posY);
                weaponContainers[3].transform.localPosition = new Vector2(width/2f - addvalue / 2, posY);
                weaponContainers[4].transform.localPosition = new Vector2(width*1.5f-addvalue * 1.5f, posY);

                weaponContainers[0].transform.localPosition = new Vector2(weaponContainers[weapIdMax].transform.localPosition.x + width * parryDistance, posY);
                break;
            case 5:
                weaponContainers[1].transform.localPosition = new Vector2(-width*2 + addvalue*2, posY);
                weaponContainers[2].transform.localPosition = new Vector2(-width + addvalue, posY);
                weaponContainers[3].transform.localPosition = new Vector2(0, posY);
                weaponContainers[4].transform.localPosition = new Vector2(width - addvalue, posY);
                weaponContainers[5].transform.localPosition = new Vector2(width * 2 - addvalue*2, posY);

                weaponContainers[0].transform.localPosition = new Vector2(weaponContainers[weapIdMax].transform.localPosition.x + width * parryDistance, posY);
                break;
            default:
                break;
        }



    }

    void LifesDisplay()
    {
        int lifes = Camera.main.GetComponent<GManager>().GetHeroLifes();
        for (int i = 0; i < lifesInHud.Count; i++)
        {
            if (i < lifes)
            {
                lifesInHud[i].enabled = true;
            }
            else
            {
                lifesInHud[i].enabled = false;
            }
        }
    }

    private void WeaponHighlight()
    {
        int choosenWeapon = Camera.main.GetComponent<GManager>().GetWeaponID();
        Color tempAlpha;
        Color tempAlpha2;
        for (int i = 1; i < weapFrameBG.Count; i++)
        {
            tempAlpha = weapFrameBG[i].color;
            tempAlpha2 = weapFrame[i].color;
            if (choosenWeapon == i)
            {
                tempAlpha.a = 1f;
                tempAlpha2.a = 1f;
                weapAmmoInnactive[i].enabled = false;
            }
            else
            {
                tempAlpha.a = 0.2f;
                tempAlpha2.a = 0.4f;
                weapAmmoInnactive[i].enabled = true;
            }
            weapFrameBG[i].color = tempAlpha;
            weapFrame[i].color = tempAlpha2;
        }
    }

    void AmmoDisplay()
    {
        int weapIdMax = Camera.main.GetComponent<GManager>().GetWeaponIDMax();
        List<float> weapAmmo = Camera.main.GetComponent<GManager>().GetWeaponsAmmo();
        List<float> weapAmmoMax = Camera.main.GetComponent<GManager>().GetWeaponsAmmoMax();
        for (int i = 0; i <= weapIdMax; i++)
        {
            //Se saca la escala según la municion actual y máxima
            float ammoScale = weapAmmo[i] / weapAmmoMax[i];
            //Se escala en X la barra
            weapAmmoInHud[i].transform.localScale = new Vector3(ammoScale, weapAmmoInHud[i].transform.localScale.y, weapAmmoInHud[i].transform.localScale.z);
        }
    }

    void ParryDisplay()
    {
        List<float> weapAmmo = Camera.main.GetComponent<GManager>().GetWeaponsAmmo();
        List<float> weapAmmoMax = Camera.main.GetComponent<GManager>().GetWeaponsAmmoMax();
        Color tempAlpha = weapFrameBG[0].color;
        Color tempAlpha2 = weapFrame[0].color;
        if (weapAmmo[0] >= weapAmmoMax[0])
        {
            tempAlpha.a = 1f;
            tempAlpha2.a = 1f;
            weapAmmoInnactive[0].enabled = false;
        }
        else
        {
            tempAlpha.a = 0.2f;
            tempAlpha2.a = 0.4f;
            weapAmmoInnactive[0].enabled = true;
        }
        weapFrameBG[0].color = tempAlpha;
        weapFrame[0].color = tempAlpha2;
    }
}
