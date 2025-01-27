using BackEnd;
using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public void OnDeleteAccountAndLogout()
    {
        // 계정 탈퇴
        Backend.BMember.WithdrawAccount(callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("계정 탈퇴 성공");

                // 로그아웃 처리
                Backend.BMember.Logout();
                Debug.Log("로그아웃 성공");

                // 메인 화면 또는 로그인 화면으로 이동
                LoadLoginScene();
            }
            else
            {
                Debug.LogError("계정 탈퇴 실패: " + callback.GetMessage());
            }
        });
    }

    public void LogOut()
    {
        // 로그아웃 처리
        Backend.BMember.Logout();
        Debug.Log("로그아웃 성공");

        // 메인 화면 또는 로그인 화면으로 이동
        LoadLoginScene();
    }

    private void LoadLoginScene()
    {
        Utils.LoadScene(SceneNames.Login);
    }
}
