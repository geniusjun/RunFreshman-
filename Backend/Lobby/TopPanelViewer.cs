using UnityEngine;
using TMPro;

public class TopPanelViewer : MonoBehaviour
{
	[SerializeField]
	private	TextMeshProUGUI	textNickname;
	[SerializeField]
	private TextMeshProUGUI score;

    private void Awake()
    {
		BackendGameData.Instance.onGameDataLoadEvent.AddListener(UpdateScore);
    }

    public void UpdateNickname()
	{
		// �г����� ������ gamer_id�� ����ϰ�, �г����� ������ �г��� ���
		textNickname.text = UserInfo.Data.nickname == null ?
							UserInfo.Data.gamerId : UserInfo.Data.nickname;
	}

	public void UpdateScore()
	{
		score.text = $"{BackendGameData.Instance.UserGameData.score}��";
	}
}

