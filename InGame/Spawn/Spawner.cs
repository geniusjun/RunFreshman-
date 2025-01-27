using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    public float logBase = 14f; // �α��� base�� Ŭ���� é�Ͱ� õõ�� ����. �׷� �̰� �α��� ���̴ϱ� �۾ƾ� ���� Ŀ���ڳ�?
    public float linearMultiplier = 0.02f; // ���� ������ ����ġ ū ���� �ʹ� ���� ���� �ӵ��� ������ ����� ���� ���̵��� ���� ��, ���� ���� ���� ���� �ӵ��� ��ȭ�Ͽ� �÷��̾ ������ �ð��� �� �� ����

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

        // Mathf.Log(GameManager.instance.gameTime + 1, base) base ���� Ŭ���� ���� ���� �ӵ��� ��������, �������� ������ ����.
        // �α׿� ���� ����
        float logPart = Mathf.Log(GameManager.instance.gameTime + 1, logBase); // �α� ��� ����
        float linearPart = linearMultiplier * GameManager.instance.gameTime; // ���� ��� ����
        chapter = Mathf.FloorToInt(logPart + linearPart);
        // spawnData �迭 ���̸� �ʰ����� �ʵ��� ����
        chapter = Mathf.Clamp(chapter, 0, spawnData.Length - 1);

        // �÷��� �ð� ������� ����Ÿ�� ���ҽ�ų ��
        float scalingFactorRe = 0.002f;
        float timeBonusRe = GameManager.instance.gameTime * scalingFactorRe;

        //���� ���� ���� -> Moster spawnTime ���� �ʿ�! TODOOO
        if (timer > Mathf.Max(0.2f, spawnData[chapter].spawnTime - (spawnData[chapter].spawnTime * timeBonusRe))) //�ּ� �����ֱ� 0.2f�� ����!
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        //����ġ ������� �� ���� ���̵� ����
        int monsterID = GetMonsterID();

        // Ǯ�� �ý��ۿ��� ������Ʈ ��������
        GameObject enemyObj = GameManager.instance.pool.Get(spawnData[monsterID].spriteType.ToString());
        BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();

        // �� ������ �ʱ�ȭ
        enemy.Init(spawnData[monsterID]);

        // �ʱ�ȭ �� ��ġ ��ġ
        enemyObj.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
    }


    // ����ġ ��� ���� ����
    private int GetMonsterID()
    {
        List<int> weights = new List<int>();
        for (int i = 0; i <= chapter; i++)
        {
            weights.Add((chapter - i + 1) * 2); // ����ġ: �ֱ� �����ϼ��� ���� ��
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
                return i; // ���õ� ���� ID ��ȯ

            randomWeight -= weights[i];
        }

        return chapter; // �⺻������ ���� é�� ���� ��ȯ
    }
}


[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public string spriteType; // ���� Ÿ�Ը� ����?
    public float speed;
    public int health;
    public int maxHealth;
    public int score;
    public int damage;
}
