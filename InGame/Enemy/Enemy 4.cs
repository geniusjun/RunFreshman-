using System.Collections;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class Enemy4 : BaseEnemy
{
    public float explosionRadius = 3f; // ���� �ݰ�
    public float explosionDamage = 20f; // ���� ������
    public float triggerDistance = 25f; // ���� ���� �Ÿ�
    public float explosionDelay = 2.2f; // ���� �ð� -> �ִϸ��̼� ��� �ð�



    protected override void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (isExploding)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        if (isReadyExplode)        // ���� �غ����̶�� ũ����ó�� õõ�� �ٰ�����
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

        if (distanceToPlayer <= triggerDistance && explosionCoroutine == null) // �÷��̾ ��ó�� ����
        {
            explosionCoroutine = StartCoroutine(TriggerExplosion());
        }
    }

    private IEnumerator TriggerExplosion()
    {
        isReadyExplode = true;

        // ���� �� �����غ� �ִϸ��̼� ���
        anim.SetTrigger("Ready");

        yield return null;
    }
    
    public void Explode()
    {
        if (!isLive) return; // �̹� �׾����� �������� ����

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
                    GameManager.instance.health -= explosionDamage * Time.deltaTime; // �ʴ� ������ ����
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
            yield return null; // ���� ������ ���
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
            anim.SetBool("Dead", true); // Dead()�Լ��� Animation�� ������!
            coll.enabled = false; //�������ϵ� �ǰ� ����!
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
            explosionCoroutine = null; // �ʱ�ȭ
        }
        spriter.sortingOrder = 1;
        isLive = false;
        isExploding = false;
        isReadyExplode = false;
        rigid.velocity = Vector3.zero;
        gameObject.SetActive(false); // ���� �� ��Ȱ��ȭ
    }
}
