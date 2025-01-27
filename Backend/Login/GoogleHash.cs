using UnityEngine;
using BackEnd;
using TMPro;

public class GoogleHash : MonoBehaviour
{
    [SerializeField] private TMP_InputField googleHash;

    public void GetGoogleHash()
    {
        // GetGoogleHash ȣ��
        string googleHashKey = Backend.Utils.GetGoogleHash();

        // ��ȯ�� �ؽ�Ű�� ����ִ��� Ȯ��
        if (!string.IsNullOrEmpty(googleHashKey))
        {
            Debug.Log($"Google Hash Key: {googleHashKey}"); // ��ȯ�� SHA1 ���
            if (googleHash != null)
            {
                googleHash.text = googleHashKey; // TMP_InputField�� �� ����
            }
        }
        else
        {
            Debug.LogError("Google Hash Key�� �������� ���߽��ϴ�. Backend �ʱ�ȭ�� Ȯ���ϼ���.");
        }
    }
}