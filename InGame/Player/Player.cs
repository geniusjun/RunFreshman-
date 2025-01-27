using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner; // ���� ���� ��ũ��Ʈ�� ������Ʈ ó�� ������ �ʱ�ȭ����!
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;
    public GameObject bossArea;
    public Transform myShadow;
    public Animator anim;
    public Collider2D coll;
    public Rigidbody2D rigid;

    SpriteRenderer spriter;
    Color originalColor;

    private Coroutine hitEffectCoroutine;  // ���� ���� ���� �ڷ�ƾ ����


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();  
        spriter = GetComponent<SpriteRenderer>();
        originalColor = spriter.color;
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();  
        coll = GetComponent<Collider2D>();
        hands = GetComponentsInChildren<Hand>(true); // ���ڰ� true�� ������ ��Ȱ��ȭ �� ������Ʈ�鵵 �ʱ�ȭ �ȴ�!
    }

    private void Start()
    {
        if (GameManager.instance.playerId == 3)
        {
            // 3�� ĳ������ �׸��� ��ġ�� �Ʒ��� ����
            myShadow.localPosition = new Vector3(0, -1.2f, 0);
        }
        else
        {
            // �ٸ� ĳ������ �׸��� ��ġ�� �⺻ ������ ����
            myShadow.localPosition = new Vector3(0, -0.7f, 0);
        }
    }

    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];    
    }

    void Update()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;

    }

    void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;
        
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime; // normalized�� Inspector���� ���� ���� ��Ű��Ű
        rigid.MovePosition(rigid.position + nextVec);
    }

    // ���� Input.GetAxisRaw("Horizontal") �̰� �̷��� ���Ⱚ �ȹް� InputSystem �̿��ؼ� �����ϰ� ����
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>(); // .Get<>(); ������ ������ ������ �ٸ��� ������ <T>���� �������� �Լ��̴�! Inspector���� �����س��� Vector2������ ����

    }
    void LateUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; 
        }
        
    }

    public void HandleDeath()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;

        GameManager.instance.isLive = false; // ���� ���¸� �������� ����

        // ���� ��ü ��Ȱ��ȭ
        for (int index = 2; index < transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.SetActive(false);
        }

        anim.SetTrigger("Dead"); // �״� �ִϸ��̼� ����
        GameManager.instance.GameOver();
    }

    void OnCollisionStay2D(Collision2D collision) //���� �κ�κ�
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;


        if (collision.gameObject.CompareTag("Enemy"))
        {
            BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                GameManager.instance.health -= Time.deltaTime * enemy.damage;
            }
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            GameManager.instance.health -= Time.deltaTime * collision.gameObject.GetComponent<Boss>().damage;
        }
        

        // ü���� 0 ���Ϸ� ���������� Ȯ��
        if (GameManager.instance.health <= 0)
        {
            HandleDeath(); // ���� ó�� ȣ��
        }

        TriggerHitEffect(); // ���� ��ȭ ȣ��
    }

    public void TriggerHitEffect()
    {
        // ���� �ڷ�ƾ�� ���� ���̸� �����ϰ� ���� ����
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
            spriter.color = originalColor;
        }

        // ���ο� �ڷ�ƾ ����
        hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        spriter.color = new Color(1f, 0f, 0f, 0.5f); // �ǰ� ���� ����
        yield return new WaitForSeconds(0.2f); // ���� �ð�
        spriter.color = originalColor; // ���� ���� ����
        hitEffectCoroutine = null; // �ڷ�ƾ ���� ����
    }
}
