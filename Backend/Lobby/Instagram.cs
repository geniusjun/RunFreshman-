using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instagram : MonoBehaviour
{
    public string instagramURL = "https://www.instagram.com/run_gc2025?igsh=MWtmMzI1YTl4eWduZg=="; // 원하는 인스타그램 페이지 URL

    public void OpenPage()
    {
        Application.OpenURL(instagramURL); // URL 열기
    }
}
