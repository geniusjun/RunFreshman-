using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd;

public class RegisterAccount : LoginBase
{
    [SerializeField]
    private Image imageID;                          // ID �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldID;            // ID �ʵ� �ؽ�Ʈ ���� ����
    [SerializeField]
    private Image imagePW;                          // PW �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldPW;            // PW �ʵ� �ؽ�Ʈ ���� ����
    [SerializeField]
    private Image imageConfirmPW;                   // Confirm PW �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldConfirmPW;     // Confirm PW �ʵ� �ؽ�Ʈ ���� ����
    [SerializeField]
    private Image imageEmail;                       // Email �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldEmail;         // Email �ʵ� �ؽ�Ʈ ���� ����

    [SerializeField]
    private Image imageNickname;                    // �г��� �ʵ� ���� ����
    [SerializeField]
    private TMP_InputField inputFieldNickname;      // �г��� �ʵ� �ؽ�Ʈ ���� ����

    [SerializeField]
    private Button btnRegisterAccount;              // ���� ���� ��ư (��ȣ�ۿ� ����/�Ұ���)

    private const string adminID = "ckdwnsdl";       // ������ ���� ID
    private const string adminPW = "cjswoshckd44!"; // ������ ���� PW

    /// <summary>
    /// "���� ����" ��ư�� ������ �� ȣ��
    /// </summary>
    public void OnClickRegisterAccount()
    {
        // �Է� �ʵ� �ʱ�ȭ
        ResetUI(imageID, imagePW, imageConfirmPW, imageEmail, imageNickname);

        // �ʵ� �� ����ִ��� üũ
        if (IsFieldDataEmpty(imageID, inputFieldID.text, "���̵�")) return;
        if (IsFieldDataEmpty(imagePW, inputFieldPW.text, "��й�ȣ")) return;
        if (IsFieldDataEmpty(imageConfirmPW, inputFieldConfirmPW.text, "��й�ȣ Ȯ��")) return;
        if (IsFieldDataEmpty(imageEmail, inputFieldEmail.text, "���� �ּ�")) return;
        if (IsFieldDataEmpty(imageNickname, inputFieldNickname.text, "�г���")) return;

        // ��й�ȣ�� Ȯ�� ��й�ȣ�� �ٸ� ��
        if (!inputFieldPW.text.Equals(inputFieldConfirmPW.text))
        {
            GuideForIncorrectlyEnterData(imageConfirmPW, "��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            return;
        }

        // �̸��� ���� �˻�
        if (!inputFieldEmail.text.Contains("@"))
        {
            GuideForIncorrectlyEnterData(imageEmail, "���� ������ �߸��Ǿ����ϴ�. (ex. address@xx.xx)");
            return;
        }

        // �г��� ���� �� ���� ���� �õ�
        ValidateNicknameAndProceed();
    }

    /// <summary>
    /// �г��� ���� �� ȸ������ ����
    /// </summary>
    private void ValidateNicknameAndProceed()
    {
        string nickname = inputFieldNickname.text;

        // �г��� ��Ģ �˻�
        if (!IsNicknameRuleValid(nickname))
        {
            GuideForIncorrectlyEnterData(imageNickname, "�г����� ����ְų� | 20�� �̻� �̰ų� | ��/�ڿ� ������ �ֽ��ϴ�.");
            return;
        }

        btnRegisterAccount.interactable = false;
        SetMessage("�г��� ���� ���Դϴ�...");

        try
        {
            Backend.BMember.CustomLogin(adminID, adminPW, loginCallback =>
            {
                try
                {
                    if (loginCallback.IsSuccess())
                    {
                        Debug.Log("������ ���� �α��� ����");

                        Backend.BMember.CheckNicknameDuplication(nickname, callback =>
                        {
                            try
                            {
                                if (callback.IsSuccess())
                                {
                                    Debug.Log("�г��� ��� ����");
                                    LogoutAdminAndProceedToRegistration();
                                }
                                else
                                {
                                    HandleNicknameError(callback);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Debug.Log($"CheckNicknameDuplication ���� �߻�: {ex.Message}");
                                RestoreStateWithError("�г��� Ȯ�� �� ������ �߻��߽��ϴ�.");
                                LogoutAdminAccount();
                            }
                        });
                    }
                    else
                    {
                        Debug.Log($"������ ���� �α��� ����: {loginCallback.GetMessage()}");
                        RestoreStateWithError("�г��� ������ ������ �� �����ϴ�.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.Log($"CustomLogin ���� �߻�: {ex.Message}");
                    RestoreStateWithError("������ ���� �α��� �� ������ �߻��߽��ϴ�.");
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.Log($"ValidateNicknameAndProceed ���� �߻�: {ex.Message}");
            RestoreStateWithError("�� �� ���� ������ �߻��߽��ϴ�.");
        }
    }

    /// <summary>
    /// �г��� ��Ģ Ȯ��
    /// </summary>
    private bool IsNicknameRuleValid(string nickname)
    {
        // ����ְų� �������θ� �̷����
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return false;
        }

        // ���̰� 20�� �ʰ�
        if (nickname.Length > 20)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// �г��� �ߺ� �Ǵ� ��Ģ ���� ó��
    /// </summary>
    private void HandleNicknameError(BackendReturnObject callback)
    {
        try
        {
            string message;
            switch (int.Parse(callback.GetStatusCode()))
            {
                case 400: // �г��� ��Ģ ����
                    if (callback.GetMessage().Contains("nickname is too long"))
                    {
                        message = "�г����� �ʹ� ��ϴ�. 20�� ���Ϸ� �ۼ����ּ���.";
                    }
                    else if (callback.GetMessage().Contains("beginning or end of the nickname must not be blank"))
                    {
                        message = "�г��� ��/�ڿ� ������ ���Ե� �� �����ϴ�.";
                    }
                    else
                    {
                        message = "�г��� ��Ģ ����";
                    }
                    break;

                case 409: // �г��� �ߺ�
                    message = "�̹� ��� ���� �г����Դϴ�.";
                    break;

                default: // ��Ÿ ����
                    message = "�г��� Ȯ�� �� ������ �߻��߽��ϴ�.";
                    break;
            }

            Debug.Log($"�г��� ���� ����: {callback.GetMessage()}");
            GuideForIncorrectlyEnterData(imageNickname, message);

            // ���� ����
            RestoreStateWithError(message);

            // ������ ���� �α׾ƿ�
            LogoutAdminAccount();
        }
        catch (System.Exception ex)
        {
            Debug.Log($"HandleNicknameError �� ���� �߻�: {ex.Message}");
            RestoreStateWithError("�г��� ���� �� �� �� ���� ������ �߻��߽��ϴ�.");
            LogoutAdminAccount();
        }
    }

    /// <summary>
    /// ������ �α׾ƿ� �� ȸ������ ����
    /// </summary>
    private void LogoutAdminAndProceedToRegistration()
    {
        Backend.BMember.Logout(logoutCallback =>
        {
            if (logoutCallback.IsSuccess())
            {
                Debug.Log("������ ���� �α׾ƿ� ����");
                AttemptRegisterAccount(); // ȸ������ ����
            }
            else
            {
                Debug.Log("������ ���� �α׾ƿ� ����");
                RestoreStateWithError("�г��� ���� �� ������ �߻��߽��ϴ�.");
            }
        });
    }

    /// <summary>
    /// ������ ���� �α׾ƿ�
    /// </summary>
    private void LogoutAdminAccount()
    {
        Backend.BMember.Logout(logoutCallback =>
        {
            if (logoutCallback.IsSuccess())
            {
                Debug.Log("������ ���� �α׾ƿ� ����");
            }
            else
            {
                Debug.Log("������ ���� �α׾ƿ� ����");
            }

            // ��ư ���� ����
            btnRegisterAccount.interactable = true;
        });
    }

    /// <summary>
    /// ���� ������ ���� �޽��� ����
    /// </summary>
    private void RestoreStateWithError(string errorMessage)
    {
        SetMessage(errorMessage);
        btnRegisterAccount.interactable = true;
    }

    /// <summary>
    /// ���� ���� �õ�
    /// </summary>
    private void AttemptRegisterAccount()
    {
        SetMessage("���� ���� ���Դϴ�...");
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

                                // Email ���̺� �̸��� ����
                                SaveEmailToTable(inputFieldID.text, inputFieldEmail.text);

                                SetMessage($"���� ���� ����! ȯ���մϴ�, {inputFieldNickname.text}��!");
                                Utils.LoadScene(SceneNames.Lobby);
                            }
                            else
                            {
                                SetMessage("�г��� ������Ʈ �� ������ �߻��߽��ϴ�.");
                            }
                        });
                    }
                    else
                    {
                        SetMessage("�̸��� ������Ʈ �� ������ �߻��߽��ϴ�.");
                    }
                });
            }
            else
            {
                // ȸ������ ���� �� ó��
                string message;
                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 409: // �̹� �����ϴ� ���̵�
                        message = "�̹� �����ϴ� ���̵��Դϴ�.";
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
    /// �̸��� ������ Email ���̺� ����
    /// </summary>
    private void SaveEmailToTable(string userId, string email)
    {
        Param param = new Param();
        param.Add("userId", userId); // ����� ���̵�
        param.Add("email", email);   // ����� �̸���

        Backend.GameData.Insert("Email", param, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("�̸��� ������ Email ���̺� ���������� ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogError($"�̸��� ���� ���� ����: {callback.GetMessage()}");
            }
        });
    }

}