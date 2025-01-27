using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class FindID : LoginBase
{
    [SerializeField]
    private Image imageEmail;               // E-mail �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldEmail; // E-mail �ʵ� �ؽ�Ʈ ���� ����

    [SerializeField]
    private Button btnFindID;               // "���̵� ã��" ��ư (��ȣ�ۿ� ����/�Ұ���)
    [SerializeField]
    private Image btnFindIDImage;           // "���̵� ã��" ��ư�� �̹��� (���� ����)

    private Color originalColor;            // ��ư�� ���� ����

    void Start()
    {

        // ��ư�� ���� ���� ����
        if (btnFindIDImage != null)
        {
            originalColor = btnFindIDImage.color;
        }
    }

    public void OnClickFindID()
    {
        // �Ű������� �Է��� InputField UI�� ����� message ���� �ʱ�ȭ
        ResetUI(imageEmail);

        // �ʵ尪�� ����ִ��� üũ
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "���� �ּ�")) return;

        // ���� ���� �˻�
        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorrectlyEnterData(imageEmail, "���� ������ �߸��Ǿ����ϴ�.(ex. address@xx.xx)");
            return;
        }

        // "���̵� ã��" ��ư�� ��ȣ�ۿ� ��Ȱ��ȭ �� ���� ����
        btnFindID.interactable = false;
        if (btnFindIDImage != null)
        {
            SetButtonTransparency(0.5f); // ��ư ���� 50%
        }
        SetMessage("���� �߼����Դϴ�..");

        // �ڳ� ���� ���̵� ã�� �õ�
        FindCustomID();

        // 5�� �� ��ư �ٽ� Ȱ��ȭ �� ���� ����
        Invoke(nameof(EnableFindIDButton), 5f);
    }

    private void FindCustomID()
    {
        Backend.BMember.FindCustomID(inputFieldEmail.text, callback =>
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
                    case 404: // �ش� �̸����� ���̸Ӱ� ���� ���
                        message = "�ش� �̸����� ����ϴ� ����ڰ� �����ϴ�.";
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

    private void EnableFindIDButton()
    {
        btnFindID.interactable = true;
        if (btnFindIDImage != null)
        {
            SetButtonTransparency(1f); // ��ư ���� �������
        }
    }

    private void SetButtonTransparency(float alpha)
    {
        Color color = originalColor;
        color.a = alpha; // ���� ����
        btnFindIDImage.color = color;
    }
}