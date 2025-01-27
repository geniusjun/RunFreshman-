using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int score;
    public int damage;
    public Rigidbody2D target;
    public SpriteRenderer spriter;

    public bool isLive;
    public bool isSpecialMove = false;
    public bool isReadyExplode = false; //무당이 몬스터 폭발준비 애니매이션 재생중에 Hit애니매이션 재생안하게끔 플래그 변수
    public bool isExploding = false; // 폭발 진행 중인지 확인


    public Rigidbody2D rigid;
    public WaitForFixedUpdate wait;
    public Animator anim;
    public Collider2D coll;


    public Coroutine explosionCoroutine;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate(); // 하나의 FixedUpdate(한번의 물리작용) 을 기다림
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive && !isSpecialMove) // 특수 이동 중이 아닐 때만 기본 이동
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))    // 맞는 애니매이션 작동중에는 물리작용 적용하지마라! 그래야 넉백되지!
            return;


            Vector2 dirVec = (target.position - rigid.position).normalized;
            rigid.velocity = dirVec * speed;
        
    }
    
    protected virtual void LateUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;
        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x; 
    }

    protected virtual void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
        explosionCoroutine = null;
        rigid.velocity = Vector3.zero;
        isReadyExplode = false;
        isExploding = false;
        
    }

    public virtual void Init(SpawnData data)
    {
        // 플레이 시간 기반으로 체력 증가
        float scalingFactorHp = 0.001f; // 체력 증가 계수 (시간당 증가 비율)
        float timeBonusHp = (GameManager.instance.gameTime) * scalingFactorHp;

        // 플레이 시간 기반으로 속도 증가 
        float scalingFactorSpeed = 0.00002f; // 체력 증가 계수 (시간당 증가 비율)
        float timeBonusSpeed = (GameManager.instance.gameTime) * scalingFactorSpeed;

        speed = Mathf.Min(5.0f, data.speed + (data.speed * timeBonusSpeed));
        maxHealth = data.health + (data.health * timeBonusHp);
        score = data.score;
        damage = data.damage;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;

        StartCoroutine(KnockBack());

        if(health > 0 && !isReadyExplode)
        {
            anim.SetTrigger("Hit");

            if (collision.gameObject == GameManager.instance.enemyCleaner)
                return;

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else if(health <= 0)
        {
                isLive = false;
                coll.enabled = false;
                rigid.simulated = false;
                isReadyExplode = false;
                rigid.velocity = Vector3.zero;
                spriter.sortingOrder = 1;
                anim.SetBool("Dead", true); // Dead()함수는 Animation이 실행중!
                GameManager.instance.AddScore(score);
                GameManager.instance.kill++;
                GameManager.instance.GetExp();

                if(GameManager.instance.isLive)
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        }
    }


    protected virtual IEnumerator KnockBack()
    {
        yield return wait; // 물리 프레임 기다림

        // 넉백 방향 계산
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;

        // 넉백 상태 설정
        isSpecialMove = true;

        // 기존 속도를 유지하며 넉백 힘 추가
        rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

        // 넉백 지속 시간 동안 대기
        yield return new WaitForSeconds(0.1f);

        // 넉백 종료
        isSpecialMove = false;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void DeleteShadow()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}   
    