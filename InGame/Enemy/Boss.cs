using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int score;
    public int damage;
    public bool isLive;

    public Rigidbody2D rigid;
    public Animator anim;
    public WaitForFixedUpdate wait;
    public Rigidbody2D target; 
    public Collider2D coll;

    public GameObject hudScore;
    public GameObject bossTxt;
    public GameObject pool;


    private Vector3 playerPos;
    private Vector3 myPos;


    //���� ���� ����
    public string bulletType1 = "boss_bullet1"; // �Ѿ� Ÿ�� (Ǯ �Ŵ������� ��Ī�Ǵ� �̸�)
    public string bulletType2 = "boss_bullet2"; // �Ѿ� Ÿ�� (Ǯ �Ŵ������� ��Ī�Ǵ� �̸�)
    public string bulletType3 = "boss_bullet3"; // �Ѿ� Ÿ�� (Ǯ �Ŵ������� ��Ī�Ǵ� �̸�)
    public string bulletType4 = "boss_bullet4"; // �Ѿ� Ÿ�� (Ǯ �Ŵ������� ��Ī�Ǵ� �̸�)

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    //4�� ������ ���� ����

    // ������ ������
    // ������ ������
    public GameObject laserPrefab;

    // ������ ���� ��ġ (���ڰ� ���·� �� ������Ʈ�� ��ġ)
    public Transform[] laserPoints; // ���ڰ� ���·� ��ġ�� �� ������Ʈ �迭

    // ȸ�� �ӵ� �� ���� �ð�
    public float rotationSpeed = 50f;
    public float laserLifetime = 5f; // ������ ���� �ð�



    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        health = maxHealth;
        rigid.velocity = Vector3.zero;
        coll.enabled = false;
        playerPos = target.transform.position;
        Vector3 dir = target.transform.up;
        myPos = playerPos + dir * 5f;
        transform.position = myPos;
        anim.SetTrigger("isSpawn");
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive) // Ư�� �̵� ���� �ƴ� ���� �⺻ �̵�
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))    // �´� �ִϸ��̼� �۵��߿��� �����ۿ� ������������! �׷��� �˹����!
            return;


        Vector2 dirVec = (target.position - rigid.position).normalized;
        rigid.velocity = dirVec * speed;

    }

    public void Think()
    {
        // ���� ���� (����)
        patternIndex = Random.Range(0, 4);

        curPatternCount = 0;

        // ���õ� ���� ����
        switch (patternIndex)
        {
            case 0:
                PatternOne();
                break;
            case 1:
                PatternTwo();
                break;
            case 2:
                PatternThree();
                break;
            case 3:
                PatternFour();
                break;
        }
    }

    private void PatternOne() // ��ä�� �߻�!
    { 
        // �÷��̾� �������� ��� �׸��� �߻� (")" ����)
        Vector2 targetPosition = target.position;
        Vector2 directionToPlayer = (targetPosition - (Vector2)rigid.position).normalized;

        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float curveFactor = 30f; // ��� ����

        for (int i = -2; i <= 2; i++)
        {
            float angle = baseAngle + i * curveFactor;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            // �Ѿ� ���� �� ����
            GameObject bullet = GameManager.instance.pool.Get(bulletType1);
            bullet.transform.position = rigid.position;

            // �Ѿ��� ȸ���� �÷��̾ ���ϵ��� ����
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            Rigidbody2D bullet1R = bullet.GetComponent<Rigidbody2D>();
            bullet1R.velocity = direction.normalized * 4f; // �ӵ� ���� (5f ����)
        }
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("PatternOne", 0.7f);
        }
        else
        {
            Invoke("Think", 4);
        }
            
    }
    private void PatternTwo() //�� ���鼭 �߻�
    {

        float rotationSpeed = 5f; // ������ ȸ�� �ӵ�

        for (int i = 0; i < curPatternCount; i++)
        {
            float angle = (360f / curPatternCount) * i + (curPatternCount * rotationSpeed);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            // �Ѿ� ���� �� ����
            GameObject bullet = GameManager.instance.pool.Get(bulletType2);
            bullet.transform.position = transform.position;

            // �Ѿ��� ȸ���� �÷��̾ ���ϵ��� ����
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = direction.normalized * 4f;
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("PatternTwo", 0.3f);
        }
        else
        {
            Invoke("Think", 7);
        }
    }
    private void PatternThree() // �� õõ�� �׸���
    {
        // �÷��̾� ���� ���
        Vector2 playerPosition = GameManager.instance.player.transform.position;
        Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;

        // ȸ�� �߽� ������ �������� ������ ����
        float angle = Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex];
        Vector2 offset = new Vector2(
            directionToPlayer.x * Mathf.Cos(angle) - directionToPlayer.y * Mathf.Sin(angle),
            directionToPlayer.x * Mathf.Sin(angle) + directionToPlayer.y * Mathf.Cos(angle)
        );

        // �Ѿ� ���� �� ����
        GameObject bullet = GameManager.instance.pool.Get(bulletType3);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D bullet3R = bullet.GetComponent<Rigidbody2D>();
        bullet3R.AddForce(offset.normalized * 3, ForceMode2D.Impulse);

        // ���� ī����
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("PatternThree", 0.1f);
        }
        else
        {
            Invoke("Think", 5);
        }
    }
      private void PatternFour()
    {
        CreateLasers();

        if (curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("PatternFour", 1f);
        }
        else
        {
            Invoke("Think", 12);
        }
    }

    void CreateLasers()
    {
        if (laserPrefab == null || laserPoints.Length == 0)
        {
            Debug.LogError("������ ������ �Ǵ� ������ ����Ʈ�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // `Horizontal`�� `Vertical` �� �������θ� ����
        string[] laserDirections = { "Horizontal", "Vertical" };


        foreach (string direction in laserDirections)
        {
            Quaternion rotation = Quaternion.identity;

            // ���⿡ ���� ȸ�� ����
            switch (direction.ToUpper())
            {
                case "HORIZONTAL":
                    rotation = Quaternion.Euler(0, 0, 0); // ���� ���� (0��)
                    break;
                case "VERTICAL":
                    rotation = Quaternion.Euler(0, 0, 90); // ���� ���� (90��)
                    break;
                default:
                    Debug.LogWarning($"�� �� ���� ����: {direction}. �⺻ ȸ��(0��)���� �����մϴ�.");
                    break;
            }

            // ������ ����
            GameObject laser = Instantiate(laserPrefab, transform.position, rotation, transform);

            // �������� ���� �ִϸ��̼��� ���� �� ȸ���ϵ��� ó��
            StartCoroutine(StartLaserRotationAfterDelay(laser, 1f)); // 1�� �� ȸ�� ����
        }

        curPatternCount++;
    }
    IEnumerator StartLaserRotationAfterDelay(GameObject laser, float delay)
    {
        float elapsedTime = 0f;

        // ��� �ð� ���ȿ��� ���� ��ġ�� ��� ����
        while (elapsedTime < delay)
        {
            if (laser != null && transform != null)
            {
                laser.transform.position = transform.position; // ���� ��ġ�� ���������� ����
            }
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // Collider Ȱ��ȭ
        Collider2D laserCollider = laser.GetComponent<Collider2D>();
        if (laserCollider != null)
            laserCollider.enabled = true;

        // ������ ȸ�� ����
        StartCoroutine(RotateAndDestroyLaser(laser));
    }

    IEnumerator RotateAndDestroyLaser(GameObject laser)
    {
        float rotationSpeed = 50f; // ȸ�� �ӵ�
        float rotationDuration = 8f; // ȸ�� ���� �ð�
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            if (laser != null)
            {
                // ������ ��ġ�� ���� ��ġ�� ����ȭ
                laser.transform.position = transform.position;

                // �������� ���� �߽� �������� ȸ��
                laser.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // �ı� �ִϸ��̼� Ʈ����
        Animator laserAnimator = laser.GetComponent<Animator>();
        if (laserAnimator != null)
        {
            laserAnimator.SetTrigger("PatternEnd");
        }

        // �ı� �ִϸ��̼� ���ȿ��� ��ġ�� �����ϸ� 1�� ���
        float destroyDelay = 1f;
        elapsedTime = 0f;

        while (elapsedTime < destroyDelay)
        {
            if (laser != null && transform != null)
            {
                laser.transform.position = transform.position;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        if (laser != null)
        {
            Destroy(laser);
            Debug.Log("������ �ı� �Ϸ�");
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;
        if (collision.gameObject == GameManager.instance.enemyCleaner)
            return;
        if(GameManager.instance.IsPaused)
            return;

        health -= collision.GetComponent<Bullet>().damage;

        if (health > 0)
        { 

            anim.SetTrigger("Hit");

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else if(health <= 0)
        {
            isLive = false;
            coll.enabled = false;
            GameManager.instance.player.coll.enabled = false; 
            GameManager.instance.AddScore(score);
            GameManager.instance.kill++;

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

            hudScore.GetComponent<HUD>().updateHud();

            pool.SetActive(false);
            GameManager.instance.player.rigid.velocity = Vector3.zero;
            GameManager.instance.player.anim.SetFloat("Speed", 0);
            GameManager.instance.IsPaused = true;

            StartCoroutine(BossDeathSequence());

        }
    }

    IEnumerator BossDeathSequence()
    { 
        bossTxt.SetActive(true);
        // �÷��̾� ���� ��ġ ���
        Vector3 playerPosition = GameManager.instance.player.transform.position; // �÷��̾� ���� ��ġ
        Vector3 playerDirection = GameManager.instance.player.transform.up; // �÷��̾��� �� ����
        Vector3 targetPosition = playerPosition + playerDirection * 4f; // �÷��̾� �� 3 ���� �Ÿ�

        // ������ �÷��̾� ���� ��ġ�� õõ�� �̵�
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // ��ǥ ��ġ�� õõ�� �̵�
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 1.0f; // �̵� �ӵ� ���� (1.0f�� �ӵ�)

            yield return null; // ���� �����ӱ��� ���
        }
        // �̵� �Ϸ� �� Rigidbody ��Ȱ��ȭ
        rigid.simulated = false; // ���� ���� �ߴ�
        rigid.velocity = Vector3.zero; // �ܿ� �ӵ� ����
        bossTxt.SetActive(true);

        // ���� ��� �ִϸ��̼� ����
        anim.SetTrigger("Dead"); // ��� �ִϸ��̼� ����
        GameManager.instance.GameWin(); // ���� �¸� ó��
    }


    public void Dead()
    {
        gameObject.SetActive(false);
    }

}
