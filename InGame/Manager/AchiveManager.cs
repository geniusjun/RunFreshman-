using BackEnd;
using System;
using System.Collections;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;

    enum Achive { UnlockPotato, UnlockBean } // ���� ������
    Achive[] achives;
    WaitForSecondsRealtime wait;
    string tableName = "AchiveData";
    string[] selectList = new string[] { "UnlockPotato", "UnlockBean" };

    bool[] isAchiveChecked; // �� ������ ���¸� ���ÿ��� ĳ��

    void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        isAchiveChecked = new bool[achives.Length]; // false�� �ʱ�ȭ
        wait = new WaitForSecondsRealtime(5);
    }

    void Start()
    {
        StartCoroutine(LoadAchiveDataFromServer());
    }

    void LateUpdate()
    {
        for (int i = 0; i < achives.Length; i++)
        {
            if (!isAchiveChecked[i]) // ���� Ȯ�ε��� ���� ������ ó��
            {
                CheckAchive(achives[i], i);
            }
        }
    }

    void CheckAchive(Achive achive, int index)
    {

        // �̹� ó���� �����̶�� �߰� ���� ��û ����
        if (isAchiveChecked[index]) return;

        bool isAchive = false;

        // ���� ���� Ȯ��
        switch (achive)
        {
            case Achive.UnlockPotato:
                if (GameManager.instance.isLive)
                    isAchive = GameManager.instance.kill >= 500;
                break;
            case Achive.UnlockBean:
                isAchive = GameManager.instance.gameTime >= 180f;
                break;
        }

        // ������ �����̶�� ��� ����
        if (!isAchive) return;

        // ���� ��û: ���� ���� Ȯ�� �� ������Ʈ
        Backend.PlayerData.GetMyData(tableName, callback =>
        {
            if (callback.IsSuccess() && callback.FlattenRows().Count > 0)
            {
                var data = callback.FlattenRows()[0];
                string achiveName = achive.ToString();

                if (int.Parse(data[achiveName].ToString()) == 0) // �������� ���� �رݵ��� ���� ����
                {
                    UpdateAchiveDataToServer(achiveName, 1); // ������ ���� ����
                    ShowNotice(achive); // �˸� ǥ��
                }

                isAchiveChecked[index] = true; // ���� ĳ�� ����
            }
            else
            {
                Debug.LogError($"���� ������ ���� ���� : {callback.GetMessage()}");
            }
        });
    }

    void ShowNotice(Achive achive)
    {
        for (int index = 0; index < uiNotice.transform.childCount; index++)
        {
            bool isActive = index == (int)achive;
            uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
        }

        StartCoroutine(NoticeRoutine());
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        yield return wait;
        uiNotice.SetActive(false);
    }

    IEnumerator LoadAchiveDataFromServer()
    {
        Backend.PlayerData.GetMyData(tableName, selectList, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("���� ���� ���� :  " + callback.GetMessage());
                return;
            }

            if (callback.FlattenRows().Count <= 0)
            {
                Debug.Log("������ ���� �����͸� ã�� �� �����ϴ�. ������....");
                InitializeAchiveDataOnServer();
                return;
            }

            var data = callback.FlattenRows()[0];
            for (int i = 0; i < achives.Length; i++)
            {
                string achiveName = achives[i].ToString();
                bool isUnlock = int.Parse(data[achiveName].ToString()) == 1;

                lockCharacter[i].SetActive(!isUnlock);
                unlockCharacter[i].SetActive(isUnlock);
                Debug.Log("���������� ����ȭ �Ϸ�");
                // ���� ĳ��
                isAchiveChecked[i] = isUnlock; // �̹� �رݵ� ���¶�� Ȯ�� �Ϸ�� ����
            }
        });

        yield return null;
    }

    void UpdateAchiveDataToServer(string achiveName, int value)
    {
        Param param = new Param();
        param.Add(achiveName, value);

        Backend.PlayerData.UpdateMyLatestData(tableName, param, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log($"{achiveName} ���� ���� ������Ʈ ����");
            }
            else
            {
                Debug.LogError($"���� ���� ���� : {callback.GetMessage()}");
            }
        });
    }

    void InitializeAchiveDataOnServer()
    {
        Param param = new Param();

        foreach (Achive achive in achives)
        {
            param.Add(achive.ToString(), 0); // ��� ������ ��� ���·� �ʱ�ȭ
        }

        Backend.PlayerData.InsertData(tableName, param, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("������ ���ο� ���� ������ ���� �Ϸ�.");
            }
            else
            {
                Debug.LogError("���� ���� ���� : " + callback.GetMessage());
            }
        });
    }
}