using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instagram : MonoBehaviour
{
    public string instagramURL = "https://www.instagram.com/run_gc2025?igsh=MWtmMzI1YTl4eWduZg=="; // ���ϴ� �ν�Ÿ�׷� ������ URL

    public void OpenPage()
    {
        Application.OpenURL(instagramURL); // URL ����
    }
}
