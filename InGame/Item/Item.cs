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
        icon = GetComponentsInChildren<Image>()[1];// Image 컴포넌트를 갖고있는 본인포함 자식들중에 n번째

        icon.sprite = data.itemIcon[0]; // 그 이미지의 sprite를 ItemData의 itemIcon으로 바꾸겠다~ 이마리양

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0]; // Item 오브젝트의 계층구조 text 순서에 따라감.
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
    {  //각 아이템 버튼이 실행시키는데 각자 Item Data를 들고있음!

        if (level == data.damages.Length)
        {
            gameObject.SetActive(false);
        }
        // 이걸 눌렀을 때인데 해야할까? -> 혹시 모르니 제외해놓음

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range: // 원거리는 count가 유명한 관통력임 ㅇㅇ
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

                    nextDamage += (data.baseDamage/2) * data.damages[level-1]; // data.damages 보면 퍼센트 증가로 되게끔 0.4 이렇게 해놓음 
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
