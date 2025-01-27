using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class FindPW : LoginBase
{
    [SerializeField]
    private Image imageID;                          // ID �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldID;            // ID �ʵ� �ؽ�Ʈ ���� ����
    [SerializeField]
    private Image imageEmail;                       // Email �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldEmail;         // Email �ʵ� �ؽ�Ʈ ���� ����

    [SerializeField]
    private Button btnFindPW;                       // "��й�ȣ ã��" ��ư (��ȣ�ۿ� ����/�Ұ���)
    [SerializeField]
    private Image btnFindPWImage;                   // "��й�ȣ ã��" ��ư�� �̹��� (���� ����)

    private Color originalColor;                    // ��ư�� ���� ����

    void Start()
    {
        // ��ư�� ���� ���� ����
        if (btnFindPWImage != null)
        {
            originalColor = btnFindPWImage.color;
        }
    }

    public void OnClickFindPW()
    {
        // �Ű������� �Է��� InputField UI�� ����� message ���� �ʱ�ȭ
        ResetUI(imageID, imageEmail);

        // �ʵ尪�� ����ִ��� üũ
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "���̵�")) return;
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "���� �ּ�")) return;

        // ���� ���� �˻�
        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorrectlyEnterData(imageEmail, "���� ������ �߸��Ǿ����ϴ�.(ex. address@xx.xx)");
            return;
        }

        // "��й�ȣ ã��" ��ư�� ��ȣ�ۿ� ��Ȱ��ȭ �� ���� ����
        btnFindPW.interactable = false;
        if (btnFindPWImage != null)
        {
            SetButtonTransparency(0.5f); // ��ư ���� 50%
        }
        SetMessage("���� �߼����Դϴ�..");

        // �ڳ� ���� ��й�ȣ ã�� �õ�
        FindCustomPW();

        // 5�� �� ��ư �ٽ� Ȱ��ȭ �� ���� ����
        Invoke(nameof(EnableFindPWButton), 5f);
    }

    public void FindCustomPW()
    {
        // ��й�ȣ�� �ʱ�ȭ�ϰ�, �ʱ�ȭ�� ��й�ȣ ������ �̸��Ϸ� �߼�
        Backend.BMember.ResetPassword(inputFieldID.text, inputFieldEmail.text, callback =>
        {
            // ���� �߼� ����
            if (callback.IsSuccess())
            {
                SetMessage($"{inputFieldEmail.text} �ּҷ� ������ �߼��Ͽ����ϴ�.");
            }
            // ���� �߼� ����
            else
            {
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 400: // �ش� �̸����� ���̸Ӱ� ���� ���
                        message = "�ش� �̸����� ����ڰ� �����ϴ�.";
                        break;
                    case 404: //�߸��� CustomId�� �Է��� ���
                        message = "�ش� ID�� ����ڸ� ã�� �� �����ϴ�.";
                        break;
                    case 429: // 24�ð� �̳��� 5ȸ �̻� ���̵�/��й�ȣ ã�⸦ �õ��� ���
                        message = "24�ð� �̳��� 5ȸ �̻� ���̵�/��й�ȣ ã�⸦ �õ��߽��ϴ�.";
                        break;
                    default:
                        // statusCode : 400 => ������Ʈ�� Ư�����ڰ� �߰��� ��� (�ȳ� ���� �̹߼� �� ���� �߻�)
                        message = callback.GetMessage();
                        break;
                }
                if (message.Contains("�̸���"))
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
            SetButtonTransparency(1f); // ��ư ���� �������
        }
    }

    private void SetButtonTransparency(float alpha)
    {
        Color color = originalColor;
        color.a = alpha; // ���� ����
        btnFindPWImage.color = color;
    }
}
