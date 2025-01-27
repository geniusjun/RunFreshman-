using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class RegisterAccount : LoginBase
{
    [SerializeField]
    private Image imageID;                          // ID 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldID;            // ID 필드 텍스트 정보 추출
    [SerializeField]
    private Image imagePW;                          // PW 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldPW;            // PW 필드 텍스트 정보 추출
    [SerializeField]
    private Image imageConfirmPW;                   // Confirm PW 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldConfirmPW;     // Confirm PW 필드 텍스트 정보 추출
    [SerializeField]
    private Image imageEmail;                       // Email 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldEmail;         // Email 필드 텍스트 정보 추출

    [SerializeField]
    private Image imageNickname;                    // 닉네임 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldNickname;      // 닉네임 필드 텍스트 정보 추출

    [SerializeField]
    private Button btnRegisterAccount;              // 계정 생성 버튼 (상호작용 가능/불가능)

    private const string adminID = "ckdwnsdl";       // 관리자 계정 ID
    private const string adminPW = "cjswoshckd44!"; // 관리자 계정 PW

    /// <summary>
    /// "계정 생성" 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnClickRegisterAccount()
    {
        // 입력 필드 초기화
        ResetUI(imageID, imagePW, imageConfirmPW, imageEmail, imageNickname);

        // 필드 값 비어있는지 체크
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "비밀번호")) return;
        if (IsFieldDataEmpty(imageConfirmPW, inputFieldConfirmPW.text, "비밀번호 확인")) return;
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "메일 주소")) return;
        if (IsFieldDataEmpty(imageNickname, inputFieldNickname.text, "닉네임")) return;

        // 비밀번호와 확인 비밀번호가 다를 때
        if (!inputFieldPW.text.Equals(inputFieldConfirmPW.text))
        {
            GuideForIncorrectlyEnterData(imageConfirmPW, "비밀번호가 일치하지 않습니다.");
            return;
        }

        // 이메일 형식 검사
        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorrectlyEnterData(imageEmail, "메일 형식이 잘못되었습니다. (ex. address@xx.xx)");
            return;
        }

        // 닉네임 검증 및 계정 생성 시도
        ValidateNicknameAndProceed();
    }

    /// <summary>
    /// 닉네임 검증 및 회원가입 로직
    /// </summary>
    private void ValidateNicknameAndProceed()
    {
        string nickname = inputFieldNickname.text;

        // 닉네임 규칙 검사
        if (!IsNicknameRuleValid(nickname))
        {
            GuideForIncorrectlyEnterData(imageNickname, "닉네임이 비어있거나 | 20자 이상 이거나 | 앞/뒤에 공백이 있습니다.");
            return;
        }

        btnRegisterAccount.interactable = false;
        SetMessage("닉네임 검증 중입니다...");

        try
        {
            Backend.BMember.CustomLogin(adminID, adminPW, loginCallback =>
            {
                try
                {
                    if (loginCallback.IsSuccess())
                    {
                        Debug.Log("관리자 계정 로그인 성공");

                        Backend.BMember.CheckNicknameDuplication(nickname, callback =>
                        {
                            try
                            {
                                if (callback.IsSuccess())
                                {
                                    Debug.Log("닉네임 사용 가능");
                                    LogoutAdminAndProceedToRegistration();
                                }
                                else
                                {
                                    HandleNicknameError(callback);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.Log($"CheckNicknameDuplication 예외 발생: {ex.Message}");
                                RestoreStateWithError("닉네임 확인 중 문제가 발생했습니다.");
                                LogoutAdminAccount();
                            }
                        });
                    }
                    else
                    {
                        Debug.Log($"관리자 계정 로그인 실패: {loginCallback.GetMessage()}");
                        RestoreStateWithError("닉네임 검증을 진행할 수 없습니다.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log($"CustomLogin 예외 발생: {ex.Message}");
                    RestoreStateWithError("관리자 계정 로그인 중 문제가 발생했습니다.");
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.Log($"ValidateNicknameAndProceed 예외 발생: {ex.Message}");
            RestoreStateWithError("알 수 없는 문제가 발생했습니다.");
        }
    }

    /// <summary>
    /// 닉네임 규칙 확인
    /// </summary>
    private bool IsNicknameRuleValid(string nickname)
    {
        // 비어있거나 공백으로만 이루어짐
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return false;
        }

        // 길이가 20자 초과
        if (nickname.Length > 20)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 닉네임 중복 또는 규칙 위반 처리
    /// </summary>
    private void HandleNicknameError(BackendReturnObject callback)
    {
        try
        {
            string message;
            switch (int.Parse(callback.GetStatusCode()))
            {
                case 400: // 닉네임 규칙 위반
                    if (callback.GetMessage().Contains("nickname is too long"))
                    {
                        message = "닉네임이 너무 깁니다. 20자 이하로 작성해주세요.";
                    }
                    else if (callback.GetMessage().Contains("beginning or end of the nickname must not be blank"))
                    {
                        message = "닉네임 앞/뒤에 공백이 포함될 수 없습니다.";
                    }
                    else
                    {
                        message = "닉네임 규칙 위반";
                    }
                    break;

                case 409: // 닉네임 중복
                    message = "이미 사용 중인 닉네임입니다.";
                    break;

                default: // 기타 에러
                    message = "닉네임 확인 중 문제가 발생했습니다.";
                    break;
            }

            Debug.Log($"닉네임 검증 실패: {callback.GetMessage()}");
            GuideForIncorrectlyEnterData(imageNickname, message);

            // 상태 복원
            RestoreStateWithError(message);

            // 관리자 계정 로그아웃
            LogoutAdminAccount();
        }
        catch (System.Exception ex)
        {
            Debug.Log($"HandleNicknameError 중 예외 발생: {ex.Message}");
            RestoreStateWithError("닉네임 검증 중 알 수 없는 문제가 발생했습니다.");
            LogoutAdminAccount();
        }
    }

    /// <summary>
    /// 관리자 로그아웃 후 회원가입 진행
    /// </summary>
    private void LogoutAdminAndProceedToRegistration()
    {
        Backend.BMember.Logout(logoutCallback =>
        {
            if (logoutCallback.IsSuccess())
            {
                Debug.Log("관리자 계정 로그아웃 성공");
                AttemptRegisterAccount(); // 회원가입 실행
            }
            else
            {
                Debug.Log("관리자 계정 로그아웃 실패");
                RestoreStateWithError("닉네임 검증 후 문제가 발생했습니다.");
            }
        });
    }

    /// <summary>
    /// 관리자 계정 로그아웃
    /// </summary>
    private void LogoutAdminAccount()
    {
        Backend.BMember.Logout(logoutCallback =>
        {
            if (logoutCallback.IsSuccess())
            {
                Debug.Log("관리자 계정 로그아웃 성공");
            }
            else
            {
                Debug.Log("관리자 계정 로그아웃 실패");
            }

            // 버튼 상태 복원
            btnRegisterAccount.interactable = true;
        });
    }

    /// <summary>
    /// 상태 복원과 에러 메시지 설정
    /// </summary>
    private void RestoreStateWithError(string errorMessage)
    {
        SetMessage(errorMessage);
        btnRegisterAccount.interactable = true;
    }

    /// <summary>
    /// 계정 생성 시도
    /// </summary>
    private void AttemptRegisterAccount()
    {
        SetMessage("계정 생성 중입니다...");
        Backend.BMember.CustomSignUp(inputFieldID.text, inputFieldPW.text, callback =>
        {
            btnRegisterAccount.interactable = true;

            if (callback.IsSuccess())
            {
                Backend.BMember.UpdateCustomEmail(inputFieldEmail.text, emailCallback =>
                {
                    if (emailCallback.IsSuccess())
                    {
                        Backend.BMember.UpdateNickname(inputFieldNickname.text, nicknameCallback =>
                        {
                            if (nicknameCallback.IsSuccess())
                            {

                                // Email 테이블에 이메일 저장
                                SaveEmailToTable(inputFieldID.text, inputFieldEmail.text);

                                SetMessage($"계정 생성 성공! 환영합니다, {inputFieldNickname.text}님!");
                                Utils.LoadScene(SceneNames.Lobby);
                            }
                            else
                            {
                                SetMessage("닉네임 업데이트 중 문제가 발생했습니다.");
                            }
                        });
                    }
                    else
                    {
                        SetMessage("이메일 업데이트 중 문제가 발생했습니다.");
                    }
                });
            }
            else
            {
                // 회원가입 실패 시 처리
                string message;
                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 409: // 이미 존재하는 아이디
                        message = "이미 존재하는 아이디입니다.";
                        GuideForIncorrectlyEnterData(imageID, message);
                        break;
                    default:
                        message = callback.GetMessage();
                        SetMessage(message);
                        break;
                }
            }
        });
    }
    /// <summary>
    /// 이메일 정보를 Email 테이블에 저장
    /// </summary>
    private void SaveEmailToTable(string userId, string email)
    {
        Param param = new Param();
        param.Add("userId", userId); // 사용자 아이디
        param.Add("email", email);   // 사용자 이메일

        Backend.GameData.Insert("Email", param, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("이메일 정보가 Email 테이블에 성공적으로 저장되었습니다.");
            }
            else
            {
                Debug.LogError($"이메일 정보 저장 실패: {callback.GetMessage()}");
            }
        });
    }

}