using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class FindID : LoginBase
{
    [SerializeField]
    private Image imageEmail;               // E-mail 필드 색상 변경
    [SerializeField]
    private TMP_InputField inputFieldEmail; // E-mail 필드 텍스트 정보 추출

    [SerializeField]
    private Button btnFindID;               // "아이디 찾기" 버튼 (상호작용 가능/불가능)
    [SerializeField]
    private Image btnFindIDImage;           // "아이디 찾기" 버튼의 이미지 (색상 조절)

    private Color originalColor;            // 버튼의 원래 색상

    void Start()
    {

        // 버튼의 원래 색상 저장
        if (btnFindIDImage != null)
        {
            originalColor = btnFindIDImage.color;
        }
    }

    public void OnClickFindID()
    {
        // 매개변수로 입력한 InputField UI의 색상과 message 내용 초기화
        ResetUI(imageEmail);

        // 필드값이 비어있는지 체크
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "메일 주소")) return;

        // 메일 형식 검사
        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorrectlyEnterData(imageEmail, "메일 형식이 잘못되었습니다.(ex. address@xx.xx)");
            return;
        }

        // "아이디 찾기" 버튼의 상호작용 비활성화 및 색상 변경
        btnFindID.interactable = false;
        if (btnFindIDImage != null)
        {
            SetButtonTransparency(0.5f); // 버튼 투명도 50%
        }
        SetMessage("메일 발송중입니다..");

        // 뒤끝 서버 아이디 찾기 시도
        FindCustomID();

        // 5초 후 버튼 다시 활성화 및 색상 복구
        Invoke(nameof(EnableFindIDButton), 5f);
    }

    private void FindCustomID()
    {
        Backend.BMember.FindCustomID(inputFieldEmail.text, callback =>
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
                    case 404: // 해당 이메일의 게이머가 없는 경우
                        message = "해당 이메일을 사용하는 사용자가 없습니다.";
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

    private void EnableFindIDButton()
    {
        btnFindID.interactable = true;
        if (btnFindIDImage != null)
        {
            SetButtonTransparency(1f); // 버튼 투명도 원래대로
        }
    }

    private void SetButtonTransparency(float alpha)
    {
        Color color = originalColor;
        color.a = alpha; // 투명도 설정
        btnFindIDImage.color = color;
    }
}