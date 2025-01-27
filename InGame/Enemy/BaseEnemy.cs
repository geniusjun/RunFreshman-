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
    public bool isReadyExplode = false; //������ ���� �����غ� �ִϸ��̼� ����߿� Hit�ִϸ��̼� ������ϰԲ� �÷��� ����
    public bool isExploding = false; // ���� ���� ������ Ȯ��


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
        wait = new WaitForFixedUpdate(); // �ϳ��� FixedUpdate(�ѹ��� �����ۿ�) �� ��ٸ�
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive && !isSpecialMove) // Ư�� �̵� ���� �ƴ� ���� �⺻ �̵�
            return;
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))    // �´� �ִϸ��̼� �۵��߿��� �����ۿ� ������������! �׷��� �˹����!
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
        // �÷��� �ð� ������� ü�� ����
        float scalingFactorHp = 0.001f; // ü�� ���� ��� (�ð��� ���� ����)
        float timeBonusHp = (GameManager.instance.gameTime) * scalingFactorHp;

        // �÷��� �ð� ������� �ӵ� ���� 
        float scalingFactorSpeed = 0.00002f; // ü�� ���� ��� (�ð��� ���� ����)
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
                anim.SetBool("Dead", true); // Dead()�Լ��� Animation�� ������!
                GameManager.instance.AddScore(score);
                GameManager.instance.kill++;
                GameManager.instance.GetExp();

                if(GameManager.instance.isLive)
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        }
    }


    protected virtual IEnumerator KnockBack()
    {
        yield return wait; // ���� ������ ��ٸ�

        // �˹� ���� ���
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;

        // �˹� ���� ����
        isSpecialMove = true;

        // ���� �ӵ��� �����ϸ� �˹� �� �߰�
        rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

        // �˹� ���� �ð� ���� ���
        yield return new WaitForSeconds(0.1f);

        // �˹� ����
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
    