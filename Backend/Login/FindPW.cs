using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class FindPW : LoginBase
{
    [SerializeField]
    private Image imageID;                          // ID 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldID;            // ID 필드 텍스트 정보 추출
    [SerializeField]
    private Image imageEmail;                       // Email 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldEmail;         // Email 필드 텍스트 정보 추출

    [SerializeField]
    private Button btnFindPW;                       // "비밀번호 찾기" 버튼 (상호작용 가능/불가능)
    [SerializeField]
    private Image btnFindPWImage;                   // "비밀번호 찾기" 버튼의 이미지 (색상 조절)

    private Color originalColor;                    // 버튼의 원래 색상

    void Start()
    {
        // 버튼의 원래 색상 저장
        if (btnFindPWImage != null)
        {
            originalColor = btnFindPWImage.color;
        }
    }

    public void OnClickFindPW()
    {
        // 매개변수로 입력한 InputField UI의 색상과 message 내용 초기화
        ResetUI(imageID, imageEmail);

        // 필드값이 비어있는지 체크
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "메일 주소")) return;

        // 메일 형식 검사
        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorrectlyEnterData(imageEmail, "메일 형식이 잘못되었습니다.(ex. address@xx.xx)");
            return;
        }

        // "비밀번호 찾기" 버튼의 상호작용 비활성화 및 색상 변경
        btnFindPW.interactable = false;
        if (btnFindPWImage != null)
        {
            SetButtonTransparency(0.5f); // 버튼 투명도 50%
        }
        SetMessage("메일 발송중입니다..");

        // 뒤끝 서버 비밀번호 찾기 시도
        FindCustomPW();

        // 5초 후 버튼 다시 활성화 및 색상 복구
        Invoke(nameof(EnableFindPWButton), 5f);
    }

    public void FindCustomPW()
    {
        // 비밀번호를 초기화하고, 초기화된 비밀번호 정보를 이메일로 발송
        Backend.BMember.ResetPassword(inputFieldID.text, inputFieldEmail.text, callback =>
        {
            // 메일 발송 성공
            if (callback.IsSuccess())
            {
                SetMessage($"{inputFieldEmail.text} 주소로 메일을 발송하였습니다.");
            }
            // 메일 발송 실패
            else
            {
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 400: // 해당 이메일의 게이머가 없는 경우
                        message = "해당 이메일의 사용자가 없습니다.";
                        break;
                    case 404: //잘못된 CustomId를 입력한 경우
                        message = "해당 ID의 사용자를 찾을 수 없습니다.";
                        break;
                    case 429: // 24시간 이내에 5회 이상 아이디/비밀번호 찾기를 시도한 경우
                        message = "24시간 이내에 5회 이상 아이디/비밀번호 찾기를 시도했습니다.";
                        break;
                    default:
                        // statusCode : 400 => 프로젝트명 특수문자가 추가된 경우 (안내 메일 미발송 및 에러 발생)
                        message = callback.GetMessage();
                        break;
                }
                if (message.Contains("이메일"))
                {
                    GuideForIncorrectlyEnterData(imageEmail, message);
                }
                else
                {
                    SetMessage(message);
                }
            }
        });
    }

    private void EnableFindPWButton()
    {
        btnFindPW.interactable = true;
        if (btnFindPWImage != null)
        {
            SetButtonTransparency(1f); // 버튼 투명도 원래대로
        }
    }

    private void SetButtonTransparency(float alpha)
    {
        Color color = originalColor;
        color.a = alpha; // 투명도 설정
        btnFindPWImage.color = color;
    }
}
