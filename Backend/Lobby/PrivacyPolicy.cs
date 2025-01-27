using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicy : MonoBehaviour
{
    // 버튼 클릭 시 호출될 함수
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://storage.thebackend.io/82176621376af01d43df45461a3804d956707468a993e26b2e822fa33e4211f0/privacy.html");
    }
}