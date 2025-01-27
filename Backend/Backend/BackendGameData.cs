using UnityEngine;
using BackEnd;
using UnityEngine.Events;

public class BackendGameData
{
	[System.Serializable]
	public class GameDataLoadEvent : UnityEvent { }
	public GameDataLoadEvent onGameDataLoadEvent = new GameDataLoadEvent();

	private	static	BackendGameData	instance = null;
	public	static	BackendGameData	Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = new BackendGameData();
			}

			return instance;
		}
	}

	private	UserGameData userGameData = new UserGameData();
	public	UserGameData UserGameData => userGameData;

	private	string gameDataRowInDate = string.Empty;

	/// <summary>
	/// �ڳ� �ܼ� ���̺� ���ο� ���� ���� �߰�
	/// </summary>
	public void GameDataInsert()
	{
		// ���� ������ �ʱⰪ���� ����
		userGameData.Reset();
		
		// ���̺� �߰��� �����ͷ� ����
		Param param = new Param()
		{
			{ "score",		userGameData.score },
		};

		// ù ��° �Ű������� �ڳ� �ܼ��� "���� ���� ����" �ǿ� ������ ���̺� �̸�
		Backend.GameData.Insert("USER_DATA", param, callback =>
		{
			// ���� ���� �߰��� �������� ��
			if ( callback.IsSuccess() )
			{
				// ���� ������ ������
				gameDataRowInDate = callback.GetInDate();

				Debug.Log($"���� ���� ������ ���Կ� �����߽��ϴ�. : {callback}");

				onGameDataLoadEvent?.Invoke();
			}
			// �������� ��
			else
			{
				Debug.LogError($"���� ���� ������ ���Կ� �����߽��ϴ�. : {callback}");
			}
		});
	}

	/// <summary>
	/// �ڳ� �ܼ� ���̺��� ���� ������ �ҷ��� �� ȣ��
	/// </summary>
	public void GameDataLoad()
	{
		Backend.GameData.GetMyData("USER_DATA", new Where(), callback =>
		{
			// ���� ���� �ҷ����⿡ �������� ��
			if ( callback.IsSuccess() )
			{
				Debug.Log($"���� ���� ������ �ҷ����⿡ �����߽��ϴ�. : {callback}");

				// JSON ������ �Ľ� ����
				try
				{
					LitJson.JsonData gameDataJson = callback.FlattenRows();

					// �޾ƿ� �������� ������ 0�̸� �����Ͱ� ���� ��
					if ( gameDataJson.Count <= 0 )
					{
						Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");

						// ���� ������ ������ ���� ���� ����
						GameDataInsert();
					}
					else
					{
						// �ҷ��� ���� ������ ������
						gameDataRowInDate		= gameDataJson[0]["inDate"].ToString();
						// �ҷ��� ���� ������ userData ������ ����
						userGameData.score		= int.Parse(gameDataJson[0]["score"].ToString());

						onGameDataLoadEvent?.Invoke();
					}
				}
				// JSON ������ �Ľ� ����
				catch ( System.Exception e )
				{
					// ���� ������ �ʱⰪ���� ����
					userGameData.Reset();
					// try-catch ���� ���
					Debug.LogError(e);
				}
			}
			// �������� ��
			else
			{
				Debug.LogError($"���� ���� ������ �ҷ����⿡ �����߽��ϴ�. : {callback}");
			}
		});
	}

	/// <summary>
	/// �ڳ� �ܼ� ���̺� �ִ� ���� ������ ����
	/// </summary>
	public void GameDataUpdate(UnityAction action = null)
	{
		if (userGameData == null)
		{
			Debug.LogError("�������� �ٿ�ްų� ���� ������ �����Ͱ� �������� �ʽ��ϴ�." +
							"InsertȤ�� Load�� ���� �����͸� �������ּ���.");
			return;
		}

		Param param = new Param()
		{
			{ "score", userGameData.score }
		};
		
		//���� ������ ������(gameDataRowInDate)�� ������ ���� �޽��� ���
		if (string.IsNullOrEmpty(gameDataRowInDate))
		{
			Debug.LogError("������ inDate ������ ���� ���� ���� ������ ������ �����߽��ϴ�");
		}
		//���� ������ �������� ������ ���̺� ����Ǿ� �ִ� �� �� inDate �÷��� ����
		//�����ϴ� ������ owner_inDate�� ��ġ�ϴ� row�� �˻��Ͽ� �����ϴ� UpdateV2() ȣ��
		else
		{
			Debug.Log($"{gameDataRowInDate}�� ���� ���� ������ ������ ��û�մϴ�.");

			Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param, callback =>
			{
				if(callback.IsSuccess())
				{
					Debug.Log($"���� ���� ������ ������ �����߽��ϴ�. : {callback}");

					action?.Invoke();
				}
				else
				{
					Debug.LogError($"���� ���� ������ ������ �����߽��ϴ�.: {callback}");
				}
			});
		}
	}
}

