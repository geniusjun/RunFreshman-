using BackEnd;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public void OnDeleteAccountAndLogout()
    {
        // ���� Ż��
        Backend.BMember.WithdrawAccount(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("���� Ż�� ����");

                // �α׾ƿ� ó��
                Backend.BMember.Logout();
                Debug.Log("�α׾ƿ� ����");

                // ���� ȭ�� �Ǵ� �α��� ȭ������ �̵�
                LoadLoginScene();
            }
            else
            {
                Debug.LogError("���� Ż�� ����: " + callback.GetMessage());
            }
        });
    }

    public void LogOut()
    {
        // �α׾ƿ� ó��
        Backend.BMember.Logout();
        Debug.Log("�α׾ƿ� ����");

        // ���� ȭ�� �Ǵ� �α��� ȭ������ �̵�
        LoadLoginScene();
    }

    private void LoadLoginScene()
    {
        Utils.LoadScene(SceneNames.Login);
    }
}
