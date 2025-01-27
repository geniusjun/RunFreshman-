using BackEnd;
using System;
using System.Collections;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;

    enum Achive { UnlockPotato, UnlockBean } // 업적 데이터
    Achive[] achives;
    WaitForSecondsRealtime wait;
    string tableName = "AchiveData";
    string[] selectList = new string[] { "UnlockPotato", "UnlockBean" };

    bool[] isAchiveChecked; // 각 업적의 상태를 로컬에서 캐싱

    void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        isAchiveChecked = new bool[achives.Length]; // false로 초기화
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
            if (!isAchiveChecked[i]) // 아직 확인되지 않은 업적만 처리
            {
                CheckAchive(achives[i], i);
            }
        }
    }

    void CheckAchive(Achive achive, int index)
    {

        // 이미 처리된 업적이라면 추가 서버 요청 방지
        if (isAchiveChecked[index]) return;

        bool isAchive = false;

        // 업적 조건 확인
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

        // 조건이 거짓이라면 즉시 종료
        if (!isAchive) return;

        // 서버 요청: 업적 상태 확인 및 업데이트
        Backend.PlayerData.GetMyData(tableName, callback =>
        {
            if (callback.IsSuccess() && callback.FlattenRows().Count > 0)
            {
                var data = callback.FlattenRows()[0];
                string achiveName = achive.ToString();

                if (int.Parse(data[achiveName].ToString()) == 0) // 서버에서 아직 해금되지 않은 상태
                {
                    UpdateAchiveDataToServer(achiveName, 1); // 서버로 업적 갱신
                    ShowNotice(achive); // 알림 표시
                }

                isAchiveChecked[index] = true; // 로컬 캐시 갱신
            }
            else
            {
                Debug.LogError($"업적 데이터 갱신 실패 : {callback.GetMessage()}");
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
                Debug.LogError("서버 연결 실패 :  " + callback.GetMessage());
                return;
            }

            if (callback.FlattenRows().Count <= 0)
            {
                Debug.Log("유저의 업적 데이터를 찾을 수 없습니다. 생성중....");
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
                Debug.Log("업적데이터 동기화 완료");
                // 로컬 캐싱
                isAchiveChecked[i] = isUnlock; // 이미 해금된 상태라면 확인 완료로 설정
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
                Debug.Log($"{achiveName} 업적 상태 업데이트 성공");
            }
            else
            {
                Debug.LogError($"서버 연결 실패 : {callback.GetMessage()}");
            }
        });
    }

    void InitializeAchiveDataOnServer()
    {
        Param param = new Param();

        foreach (Achive achive in achives)
        {
            param.Add(achive.ToString(), 0); // 모든 업적을 잠금 상태로 초기화
        }

        Backend.PlayerData.InsertData(tableName, param, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("유저의 새로운 업적 데이터 생성 완료.");
            }
            else
            {
                Debug.LogError("서버 연결 실패 : " + callback.GetMessage());
            }
        });
    }
}