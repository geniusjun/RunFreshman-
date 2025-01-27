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


    //패턴 관련 설정
    public string bulletType1 = "boss_bullet1"; // 총알 타입 (풀 매니저에서 매칭되는 이름)
    public string bulletType2 = "boss_bullet2"; // 총알 타입 (풀 매니저에서 매칭되는 이름)
    public string bulletType3 = "boss_bullet3"; // 총알 타입 (풀 매니저에서 매칭되는 이름)
    public string bulletType4 = "boss_bullet4"; // 총알 타입 (풀 매니저에서 매칭되는 이름)

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    //4번 레이저 패턴 관련

    // 레이저 프리팹
    // 레이저 프리팹
    public GameObject laserPrefab;

    // 레이저 생성 위치 (십자가 형태로 빈 오브젝트를 배치)
    public Transform[] laserPoints; // 십자가 형태로 배치된 빈 오브젝트 배열

    // 회전 속도 및 지속 시간
    public float rotationSpeed = 50f;
    public float laserLifetime = 5f; // 레이저 존재 시간



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
        if (!GameManager.instance.isLive) // 특수 이동 중이 아닐 때만 기본 이동
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))    // 맞는 애니매이션 작동중에는 물리작용 적용하지마라! 그래야 넉백되지!
            return;


        Vector2 dirVec = (target.position - rigid.position).normalized;
        rigid.velocity = dirVec * speed;

    }

    public void Think()
    {
        // 패턴 선택 (랜덤)
        patternIndex = Random.Range(0, 4);

        curPatternCount = 0;

        // 선택된 패턴 실행
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

    private void PatternOne() // 부채꼴 발사!
    { 
        // 플레이어 방향으로 곡선을 그리며 발사 (")" 형태)
        Vector2 targetPosition = target.position;
        Vector2 directionToPlayer = (targetPosition - (Vector2)rigid.position).normalized;

        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float curveFactor = 30f; // 곡선의 정도

        for (int i = -2; i <= 2; i++)
        {
            float angle = baseAngle + i * curveFactor;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            // 총알 생성 및 설정
            GameObject bullet = GameManager.instance.pool.Get(bulletType1);
            bullet.transform.position = rigid.position;

            // 총알의 회전을 플레이어를 향하도록 설정
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            Rigidbody2D bullet1R = bullet.GetComponent<Rigidbody2D>();
            bullet1R.velocity = direction.normalized * 4f; // 속도 설정 (5f 예시)
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
    private void PatternTwo() //원 돌면서 발사
    {

        float rotationSpeed = 5f; // 나선형 회전 속도

        for (int i = 0; i < curPatternCount; i++)
        {
            float angle = (360f / curPatternCount) * i + (curPatternCount * rotationSpeed);
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            // 총알 생성 및 설정
            GameObject bullet = GameManager.instance.pool.Get(bulletType2);
            bullet.transform.position = transform.position;

            // 총알의 회전을 플레이어를 향하도록 설정
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
    private void PatternThree() // 원 천천히 그리기
    {
        // 플레이어 방향 계산
        Vector2 playerPosition = GameManager.instance.player.transform.position;
        Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;

        // 회전 중심 방향을 기준으로 각도를 조정
        float angle = Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex];
        Vector2 offset = new Vector2(
            directionToPlayer.x * Mathf.Cos(angle) - directionToPlayer.y * Mathf.Sin(angle),
            directionToPlayer.x * Mathf.Sin(angle) + directionToPlayer.y * Mathf.Cos(angle)
        );

        // 총알 생성 및 설정
        GameObject bullet = GameManager.instance.pool.Get(bulletType3);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D bullet3R = bullet.GetComponent<Rigidbody2D>();
        bullet3R.AddForce(offset.normalized * 3, ForceMode2D.Impulse);

        // 패턴 카운팅
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
            Debug.LogError("레이저 프리팹 또는 레이저 포인트가 설정되지 않았습니다!");
            return;
        }

        // `Horizontal`과 `Vertical` 두 방향으로만 생성
        string[] laserDirections = { "Horizontal", "Vertical" };


        foreach (string direction in laserDirections)
        {
            Quaternion rotation = Quaternion.identity;

            // 방향에 따른 회전 설정
            switch (direction.ToUpper())
            {
                case "HORIZONTAL":
                    rotation = Quaternion.Euler(0, 0, 0); // 수평 방향 (0°)
                    break;
                case "VERTICAL":
                    rotation = Quaternion.Euler(0, 0, 90); // 수직 방향 (90°)
                    break;
                default:
                    Debug.LogWarning($"알 수 없는 방향: {direction}. 기본 회전(0°)으로 설정합니다.");
                    break;
            }

            // 레이저 생성
            GameObject laser = Instantiate(laserPrefab, transform.position, rotation, transform);

            // 레이저에 생성 애니메이션이 끝난 후 회전하도록 처리
            StartCoroutine(StartLaserRotationAfterDelay(laser, 1f)); // 1초 후 회전 시작
        }

        curPatternCount++;
    }
    IEnumerator StartLaserRotationAfterDelay(GameObject laser, float delay)
    {
        float elapsedTime = 0f;

        // 대기 시간 동안에도 보스 위치를 계속 따라감
        while (elapsedTime < delay)
        {
            if (laser != null && transform != null)
            {
                laser.transform.position = transform.position; // 보스 위치를 지속적으로 따라감
            }
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // Collider 활성화
        Collider2D laserCollider = laser.GetComponent<Collider2D>();
        if (laserCollider != null)
            laserCollider.enabled = true;

        // 레이저 회전 시작
        StartCoroutine(RotateAndDestroyLaser(laser));
    }

    IEnumerator RotateAndDestroyLaser(GameObject laser)
    {
        float rotationSpeed = 50f; // 회전 속도
        float rotationDuration = 8f; // 회전 지속 시간
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            if (laser != null)
            {
                // 레이저 위치를 보스 위치로 동기화
                laser.transform.position = transform.position;

                // 레이저를 보스 중심 기준으로 회전
                laser.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 파괴 애니메이션 트리거
        Animator laserAnimator = laser.GetComponent<Animator>();
        if (laserAnimator != null)
        {
            laserAnimator.SetTrigger("PatternEnd");
        }

        // 파괴 애니메이션 동안에도 위치를 유지하며 1초 대기
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
            Debug.Log("레이저 파괴 완료");
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
        // 플레이어 앞쪽 위치 계산
        Vector3 playerPosition = GameManager.instance.player.transform.position; // 플레이어 현재 위치
        Vector3 playerDirection = GameManager.instance.player.transform.up; // 플레이어의 앞 방향
        Vector3 targetPosition = playerPosition + playerDirection * 4f; // 플레이어 앞 3 단위 거리

        // 보스를 플레이어 앞쪽 위치로 천천히 이동
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // 목표 위치로 천천히 이동
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * Time.deltaTime * 1.0f; // 이동 속도 조절 (1.0f는 속도)

            yield return null; // 다음 프레임까지 대기
        }
        // 이동 완료 후 Rigidbody 비활성화
        rigid.simulated = false; // 물리 연산 중단
        rigid.velocity = Vector3.zero; // 잔여 속도 제거
        bossTxt.SetActive(true);

        // 보스 사망 애니메이션 실행
        anim.SetTrigger("Dead"); // 사망 애니메이션 실행
        GameManager.instance.GameWin(); // 게임 승리 처리
    }


    public void Dead()
    {
        gameObject.SetActive(false);
    }

}
