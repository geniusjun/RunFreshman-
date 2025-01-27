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
        int curScore = GameManager.instance.totalScore; // 현재 점수
        score[0].text = string.Format("점수 : {0:F0} 점", curScore); // 점수 표시

        titleWin.SetActive(true); // 승리 결과창 활성화

        rankRegister.UpdateMyBestRankData(curScore, (isNewHighScore) =>
        {
            if (isNewHighScore)
            {
                titleWin.transform.GetChild(0).gameObject.SetActive(true); // 새 최고 점수 축하 메시지
                titleWin.transform.GetChild(1).gameObject.SetActive(true); // 추가 UI 표시
            }
            else
            {
                titleWin.transform.GetChild(0).gameObject.SetActive(true); // 기존 축하 메시지
                titleWin.transform.GetChild(1).gameObject.SetActive(false); // 추가 UI 비활성화
            }
        });
    }

    public void Lose() // 최고점수 갱신도 같이쓰는중!
    {
        int curScore = GameManager.instance.totalScore;// 현재 점수
        score[1].text = string.Format("점수 : {0:F0} 점", curScore); //점수 표시


        titleOver.SetActive(true); //결과창 활성화

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
