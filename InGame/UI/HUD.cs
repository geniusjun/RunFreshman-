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
    public Image fillImage; // �����̴��� Fill �κ� �̹���
    public Sprite[] healthSprites; // ü�� �ܰ躰 �̹��� �迭 (6��)

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>(); 
    }

    void LateUpdate() // ����ǰ� �Ϸ���
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

    public void updateHud() // ����ǰ� �Ϸ���
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

        // �����̴� �� ������Ʈ
        mySlider.value = (maxHealth > 0) ? curHealth / maxHealth : 0;

        // ü�� ������ ���� �̹��� ������Ʈ
        if (healthSprites != null && healthSprites.Length > 0 && fillImage != null)
        {
            float healthPercentage = mySlider.value;

            // ü�� ������ ���� �̹��� �ε��� ��� (0~5)
            int index = Mathf.FloorToInt(healthPercentage * (healthSprites.Length - 1));
            index = Mathf.Clamp(index, 0, healthSprites.Length - 1);

            // �̹��� ����
            fillImage.sprite = healthSprites[index];
        }
    }

}
