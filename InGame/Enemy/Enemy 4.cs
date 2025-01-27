using System.Collections;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class Enemy4 : BaseEnemy
{
    public float explosionRadius = 3f; // 폭발 반경
    public float explosionDamage = 20f; // 폭발 데미지
    public float triggerDistance = 25f; // 폭발 시작 거리
    public float explosionDelay = 2.2f; // 폭발 시간 -> 애니매이션 재생 시간



    protected override void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (isExploding)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        if (isReadyExplode)        // 폭발 준비중이라면 크리퍼처럼 천천히 다가오기
        {
            Vector2 dirVec = (target.position - rigid.position).normalized;
            rigid.velocity = dirVec * speed / 2;
        }
        else
        {
            rigid.velocity = Vector3.zero;
        }

        base.FixedUpdate();
    }

    public override void Init(SpawnData data)
    {
        base.Init(data);
        health = Mathf.Min(100, maxHealth);
        isExploding = false;
        isReadyExplode = false;
    }



    private void Update()
    {
        if (!isLive || isExploding || isReadyExplode)
            return;

        float distanceToPlayer = Vector2.Distance(target.position, transform.position);

        if (distanceToPlayer <= triggerDistance && explosionCoroutine == null) // 플레이어가 근처로 접근
        {
            explosionCoroutine = StartCoroutine(TriggerExplosion());
        }
    }

    private IEnumerator TriggerExplosion()
    {
        isReadyExplode = true;

        // 폭발 전 폭발준비 애니매이션 재생
        anim.SetTrigger("Ready");

        yield return null;
    }
    
    public void Explode()
    {
        if (!isLive) return; // 이미 죽었으면 폭발하지 않음

        anim.SetBool("Dead", false);

        isReadyExplode = false;
        isExploding = true;

        anim.SetTrigger("Bomb");


        StartCoroutine(ApplyExplosionDamage());

    }
    private IEnumerator ApplyExplosionDamage()
    {
        float elapsedTime = 0f;

        while (elapsedTime < explosionDelay)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    GameManager.instance.health -= explosionDamage * Time.deltaTime; // 초당 데미지 적용
                }
                else if (hitCollider.CompareTag("Enemy"))
                {
                    BaseEnemy enemy = hitCollider.GetComponent<BaseEnemy>();
                    if (enemy != null)
                    {
                        enemy.health -= explosionDamage * Time.deltaTime;
                    }
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임 대기
        }

        isLive = false; 
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;

        StartCoroutine(KnockBack());

        if (health > 0)
        {
            if (collision.gameObject == GameManager.instance.enemyCleaner)
                return;

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else if (health <= 0)
        {
            rigid.simulated = false;
            rigid.velocity = Vector3.zero; 
            anim.SetBool("Dead", true); // Dead()함수는 Animation이 실행중!
            coll.enabled = false; //폭발중일덴 피격 무시!
            GameManager.instance.AddScore(score);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        }
    }



    public void Dead4()
    {
        if (explosionCoroutine != null)
        {
            StopCoroutine(explosionCoroutine);
            explosionCoroutine = null; // 초기화
        }
        spriter.sortingOrder = 1;
        isLive = false;
        isExploding = false;
        isReadyExplode = false;
        rigid.velocity = Vector3.zero;
        gameObject.SetActive(false); // 폭발 후 비활성화
    }
}
