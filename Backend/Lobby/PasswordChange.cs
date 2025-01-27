using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using System.Linq;

public class PasswordChange : LoginBase
{
    [SerializeField]
    private Image imagePasswordOld;              // 현재 비밀번호 필드 색상 변경
    [SerializeField]
    private Image imagePasswordNew;              // 새 비밀번호 필드 색상 변경

    [SerializeField]
    private TMP_InputField inputFieldPasswordOld; // 현재 비밀번호 입력 필드
    [SerializeField]
    private TMP_InputField inputFieldPasswordNew; // 새 비밀번호 입력 필드

    [SerializeField]
    private Button btnUpdatePassword;            // "비밀번호 변경" 버튼

    private void OnEnable()
    {
        // 비밀번호 변경 팝업 상태 초기화
        ResetUI(imagePasswordOld);
        ResetUI(imagePasswordNew);
        SetMessage("현재 비밀번호와 새 비밀번호를 입력하세요.");
    }

    public void OnClickUpdatePassword()
    {
        // 입력 필드 상태 초기화
        ResetUI(imagePasswordOld);
        ResetUI(imagePasswordNew);

        string oldPassword = inputFieldPasswordOld.text.Trim();
        string newPassword = inputFieldPasswordNew.text.Trim();


        // 현재 비밀번호 필드가 비어있는지 확인
        if (IsFieldDataEmpty(imagePasswordOld, oldPassword, "현재 비밀번호")) return;

        // 새 비밀번호 필드가 비어있는지 확인
        if (IsFieldDataEmpty(imagePasswordNew, newPassword, "새 비밀번호")) return;


        // "비밀번호 변경" 버튼 비활성화
        btnUpdatePassword.interactable = false;
        SetMessage("비밀번호 변경 중입니다...");

        // 뒤끝 서버에 비밀번호 변경 요청
        UpdatePassword(oldPassword, newPassword);
    }

    private void UpdatePassword(string oldPassword, string newPassword)
    {
        // 비밀번호 변경
        Backend.BMember.UpdatePassword(oldPassword, newPassword, callback =>
        {
            // "비밀번호 변경" 버튼 활성화
            btnUpdatePassword.interactable = true;

            // 비밀번호 변경 성공
            if (callback.IsSuccess())
            {
                SetMessage("비밀번호가 성공적으로 변경되었습니다.");
                inputFieldPasswordOld.text = string.Empty;
                inputFieldPasswordNew.text = string.Empty;
            }
            // 비밀번호 변경 실패
            else
            {
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 400: // 잘못된 요청 (유효하지 않은 비밀번호 등)
                        message = "현재 비밀번호가 잘못되었거나 새 비밀번호가 유효하지 않습니다.";
                        break;
                    case 401: // 인증 실패 (로그인 상태 유효하지 않음)
                        message = "로그인이 만료되었습니다. 다시 로그인하세요.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                // 잘못된 입력에 대한 피드백 제공
                GuideForIncorrectlyEnterData(imagePasswordOld, message);
            }
        });
    }
}