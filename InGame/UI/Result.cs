using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public GameObject titleWin;
    public GameObject titleOver;
    public Text[] score;

    public RankRegister rankRegister;

    public void Win()
    {
        int curScore = GameManager.instance.totalScore; // ���� ����
        score[0].text = string.Format("���� : {0:F0} ��", curScore); // ���� ǥ��

        titleWin.SetActive(true); // �¸� ���â Ȱ��ȭ

        rankRegister.UpdateMyBestRankData(curScore, (isNewHighScore) =>
        {
            if (isNewHighScore)
            {
                titleWin.transform.GetChild(0).gameObject.SetActive(true); // �� �ְ� ���� ���� �޽���
                titleWin.transform.GetChild(1).gameObject.SetActive(true); // �߰� UI ǥ��
            }
            else
            {
                titleWin.transform.GetChild(0).gameObject.SetActive(true); // ���� ���� �޽���
                titleWin.transform.GetChild(1).gameObject.SetActive(false); // �߰� UI ��Ȱ��ȭ
            }
        });
    }

    public void Lose() // �ְ����� ���ŵ� ���̾�����!
    {
        int curScore = GameManager.instance.totalScore;// ���� ����
        score[1].text = string.Format("���� : {0:F0} ��", curScore); //���� ǥ��


        titleOver.SetActive(true); //���â Ȱ��ȭ

        rankRegister.UpdateMyBestRankData(curScore, (isNewHighScore) =>
        {
            if (isNewHighScore)
            {
                titleOver.transform.GetChild(0).gameObject.SetActive(true);
                titleOver.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                titleOver.transform.GetChild(0).gameObject.SetActive(true);
                titleOver.transform.GetChild(1).gameObject.SetActive(false);
            }
        });
    }
}
