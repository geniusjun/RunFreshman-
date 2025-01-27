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
		// 닉네임이 없으면 gamer_id를 출력하고, 닉네임이 있으면 닉네임 출력
		textNickname.text = UserInfo.Data.nickname == null ?
							UserInfo.Data.gamerId : UserInfo.Data.nickname;
	}

	public void UpdateScore()
	{
		score.text = $"{BackendGameData.Instance.UserGameData.score}점";
	}
}

