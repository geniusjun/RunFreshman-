using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    public float logBase = 14f; // 로그의 base가 클수록 챕터가 천천히 증가. 그럼 이게 로그의 밑이니까 작아야 값이 커지겠네?
    public float linearMultiplier = 0.02f; // 선형 증가의 가중치 큰 값은 초반 레벨 증가 속도를 빠르게 만들어 게임 난이도를 높일 수, 작은 값은 레벨 증가 속도를 완화하여 플레이어가 적응할 시간을 줄 수 있음

    int chapter;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();                                                                                                                                    
    }
    void Update()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;
        if (GameManager.instance.IsBossing)
            return;

        timer += Time.deltaTime;

        // Mathf.Log(GameManager.instance.gameTime + 1, base) base 값이 클수록 레벨 증가 속도가 느려지고, 작을수록 빠르게 증가.
        // 로그와 선형 결합
        float logPart = Mathf.Log(GameManager.instance.gameTime + 1, logBase); // 로그 기반 증가
        float linearPart = linearMultiplier * GameManager.instance.gameTime; // 선형 기반 증가
        chapter = Mathf.FloorToInt(logPart + linearPart);
        // spawnData 배열 길이를 초과하지 않도록 제한
        chapter = Mathf.Clamp(chapter, 0, spawnData.Length - 1);

        // 플레이 시간 기반으로 스폰타임 감소시킬 값
        float scalingFactorRe = 0.002f;
        float timeBonusRe = GameManager.instance.gameTime * scalingFactorRe;

        //몬스터 생성 조건 -> Moster spawnTime 조정 필요! TODOOO
        if (timer > Mathf.Max(0.2f, spawnData[chapter].spawnTime - (spawnData[chapter].spawnTime * timeBonusRe))) //최소 스폰주기 0.2f로 설정!
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        //가중치 기반으로 적 몬스터 아이디 선택
        int monsterID = GetMonsterID();

        // 풀링 시스템에서 오브젝트 가져오기
        GameObject enemyObj = GameManager.instance.pool.Get(spawnData[monsterID].spriteType.ToString());
        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();

        // 적 데이터 초기화
        enemy.Init(spawnData[monsterID]);

        // 초기화 후 위치 배치
        enemyObj.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
    }


    // 가중치 기반 몬스터 선택
    private int GetMonsterID()
    {
        List<int> weights = new List<int>();
        for (int i = 0; i <= chapter; i++)
        {
            weights.Add((chapter - i + 1) * 2); // 가중치: 최근 몬스터일수록 높은 값
        }

        int totalWeight = 0;
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        int randomWeight = Random.Range(0, totalWeight);

        for (int i = 0; i <= chapter; i++)
        {
            if (randomWeight < weights[i])
                return i; // 선택된 몬스터 ID 반환

            randomWeight -= weights[i];
        }

        return chapter; // 기본적으로 현재 챕터 몬스터 반환
    }
}


[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public string spriteType; // 몬스터 타입만 관리?
    public float speed;
    public int health;
    public int maxHealth;
    public int score;
    public int damage;
}
