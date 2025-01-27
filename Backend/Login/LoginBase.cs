using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginBase : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMessage;

    /// <summary>
    /// �޽��� ����, InputFiled ���� �ʱ�ȭ
    /// </summary>
    protected void ResetUI(params Image[] images)
    {
        textMessage.text = string.Empty;

        for(int i = 0; i < images.Length; ++i)
        {
            images[i].color = Color.white;
        }
    }

    /// <summary>
    /// �Ű� ������ �ִ� ������ ���
    /// </summary>
    protected void SetMessage(string msg)
    {
        textMessage.text = msg;
    }
    /// <summary>
    /// �Է� ������ �ִ� InputField�� ���� ����
    /// ������ ���� �޽��� ���
    /// </summary>
    protected void GuideForIncorrectlyEnterData(Image image, string msg)
    {
        textMessage.text = msg;
        image.color = new Color(1f,0f,0f,0.3f);
    }
    /// <summary>
    /// �ʵ� ���� ����ִ��� Ȯ��(image:�ʵ�, field:����, result:��µ� ����)
    /// </summary>
    protected bool IsFieldDataEmpty(Image image, string field, string result)
    {
        if (field.Trim().Equals(""))
        {
            GuideForIncorrectlyEnterData(image, $"\"{result}\" �ʵ带 ä���ּ���.");

            return true;
        }
        return false;
    }
   

}
