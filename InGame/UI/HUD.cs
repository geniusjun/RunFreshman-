using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Cinemachine.DocumentationSortingAttribute;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Score, Time, Health, Coffee}
    public InfoType type;

    TextMeshProUGUI myText;
    Slider mySlider;

    [Header("Health Bar Images")]
    public Image fillImage; // 슬라이더의 Fill 부분 이미지
    public Sprite[] healthSprites; // 체력 단계별 이미지 배열 (6개)

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>(); 
    }

    void LateUpdate() // 연산되고 하려고
    {
        if (GameManager.instance.IsPaused)
            return;
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp/maxExp; 
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level); 
                break;
            case InfoType.Score:
                myText.text = string.Format("{0:F0}", GameManager.instance.totalScore);   
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min,sec);
                break;
            case InfoType.Health:
                UpdateHealthBar();

                break;

            case InfoType.Coffee:
                myText.text = string.Format("X {0}", GameManager.instance.coffeecnt);
                break;

        }
    }

    public void updateHud() // 연산되고 하려고
    {
        if (GameManager.instance.IsPaused)
            return;
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1)];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Score:
                myText.text = string.Format("{0:F0}", GameManager.instance.totalScore);
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                UpdateHealthBar();

                break;
            case InfoType.Coffee:
                myText.text = string.Format("X {0}", GameManager.instance.coffeecnt);
                break;
        }
    }

    void UpdateHealthBar()
    {
        float curHealth = GameManager.instance.health;
        float maxHealth = GameManager.instance.maxHealth;

        // 슬라이더 값 업데이트
        mySlider.value = (maxHealth > 0) ? curHealth / maxHealth : 0;

        // 체력 비율에 따라 이미지 업데이트
        if (healthSprites != null && healthSprites.Length > 0 && fillImage != null)
        {
            float healthPercentage = mySlider.value;

            // 체력 비율에 따른 이미지 인덱스 계산 (0~5)
            int index = Mathf.FloorToInt(healthPercentage * (healthSprites.Length - 1));
            index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

            // 이미지 적용
            fillImage.sprite = healthSprites[index];
        }
    }

}
