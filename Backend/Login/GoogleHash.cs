using UnityEngine;
using BackEnd;
using TMPro;

public class GoogleHash : MonoBehaviour
{
    [SerializeField] private TMP_InputField googleHash;

    public void GetGoogleHash()
    {
        // GetGoogleHash 호출
        string googleHashKey = Backend.Utils.GetGoogleHash();

        // 반환된 해시키가 비어있는지 확인
        if (!string.IsNullOrEmpty(googleHashKey))
        {
            Debug.Log($"Google Hash Key: {googleHashKey}"); // 반환된 SHA1 출력
            if (googleHash != null)
            {
                googleHash.text = googleHashKey; // TMP_InputField에 값 설정
            }
        }
        else
        {
            Debug.LogError("Google Hash Key를 가져오지 못했습니다. Backend 초기화를 확인하세요.");
        }
    }
}