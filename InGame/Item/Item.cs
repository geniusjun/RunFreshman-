using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level= 0;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;


    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];// Image ������Ʈ�� �����ִ� �������� �ڽĵ��߿� n��°

        icon.sprite = data.itemIcon[0]; // �� �̹����� sprite�� ItemData�� itemIcon���� �ٲٰڴ�~ �̸�����

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0]; // Item ������Ʈ�� �������� text ������ ����.
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName[0];

    }

    void OnEnable()
    {

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (GameManager.instance.playerId == 0 || GameManager.instance.playerId == 1)
                {
                    textLevel.text = "LV." + level;
                    icon.sprite = data.itemIcon[0];
                    textName.text = data.itemName[0];
                    textDesc.text = data.itemDesc[0].text;
                }
                else
                {
                    textLevel.text = "LV." + level;
                    icon.sprite = data.itemIcon[1];
                    textName.text = data.itemName[1];
                    textDesc.text = data.itemDesc[1].text;
                }
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textLevel.text = "LV." + (level + 1);
                textDesc.text = data.itemDesc[0].text;
                break;
            default:
                textLevel.text = "LV." + (level + 1);
                textDesc.text = data.itemDesc[0].text;
                break;
        }

        
    }


    public void OnClick()
    {  //�� ������ ��ư�� �����Ű�µ� ���� Item Data�� �������!

        if (level == data.damages.Length)
        {
            gameObject.SetActive(false);
        }
        // �̰� ������ ���ε� �ؾ��ұ�? -> Ȥ�� �𸣴� �����س���

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range: // ���Ÿ��� count�� ������ ������� ����
                if(level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += (data.baseDamage/2) * data.damages[level-1]; // data.damages ���� �ۼ�Ʈ ������ �ǰԲ� 0.4 �̷��� �س��� 
                    nextCount += data.counts[level-1];

                    weapon.LevelUp(nextDamage, nextCount);

                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }

                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.coffeecnt++;
                break;
                
        }
        



    }



}
