using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;
using System.Linq;

public class PasswordChange : LoginBase
{
    [SerializeField]
    private Image imagePasswordOld;              // ���� ��й�ȣ �ʵ� ���� ����
    [SerializeField]
    private Image imagePasswordNew;              // �� ��й�ȣ �ʵ� ���� ����

    [SerializeField]
    private TMP_InputField inputFieldPasswordOld; // ���� ��й�ȣ �Է� �ʵ�
    [SerializeField]
    private TMP_InputField inputFieldPasswordNew; // �� ��й�ȣ �Է� �ʵ�

    [SerializeField]
    private Button btnUpdatePassword;            // "��й�ȣ ����" ��ư

    private void OnEnable()
    {
        // ��й�ȣ ���� �˾� ���� �ʱ�ȭ
        ResetUI(imagePasswordOld);
        ResetUI(imagePasswordNew);
        SetMessage("���� ��й�ȣ�� �� ��й�ȣ�� �Է��ϼ���.");
    }

    public void OnClickUpdatePassword()
    {
        // �Է� �ʵ� ���� �ʱ�ȭ
        ResetUI(imagePasswordOld);
        ResetUI(imagePasswordNew);

        string oldPassword = inputFieldPasswordOld.text.Trim();
        string newPassword = inputFieldPasswordNew.text.Trim();


        // ���� ��й�ȣ �ʵ尡 ����ִ��� Ȯ��
        if (IsFieldDataEmpty(imagePasswordOld, oldPassword, "���� ��й�ȣ")) return;

        // �� ��й�ȣ �ʵ尡 ����ִ��� Ȯ��
        if (IsFieldDataEmpty(imagePasswordNew, newPassword, "�� ��й�ȣ")) return;


        // "��й�ȣ ����" ��ư ��Ȱ��ȭ
        btnUpdatePassword.interactable = false;
        SetMessage("��й�ȣ ���� ���Դϴ�...");

        // �ڳ� ������ ��й�ȣ ���� ��û
        UpdatePassword(oldPassword, newPassword);
    }

    private void UpdatePassword(string oldPassword, string newPassword)
    {
        // ��й�ȣ ����
        Backend.BMember.UpdatePassword(oldPassword, newPassword, callback =>
        {
            // "��й�ȣ ����" ��ư Ȱ��ȭ
            btnUpdatePassword.interactable = true;

            // ��й�ȣ ���� ����
            if (callback.IsSuccess())
            {
                SetMessage("��й�ȣ�� ���������� ����Ǿ����ϴ�.");
                inputFieldPasswordOld.text = string.Empty;
                inputFieldPasswordNew.text = string.Empty;
            }
            // ��й�ȣ ���� ����
            else
            {
                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 400: // �߸��� ��û (��ȿ���� ���� ��й�ȣ ��)
                        message = "���� ��й�ȣ�� �߸��Ǿ��ų� �� ��й�ȣ�� ��ȿ���� �ʽ��ϴ�.";
                        break;
                    case 401: // ���� ���� (�α��� ���� ��ȿ���� ����)
                        message = "�α����� ����Ǿ����ϴ�. �ٽ� �α����ϼ���.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                // �߸��� �Է¿� ���� �ǵ�� ����
                GuideForIncorrectlyEnterData(imagePasswordOld, message);
            }
        });
    }
}