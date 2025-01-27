using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner; // 직접 만든 스크립트도 컴포넌트 처럼 선언후 초기화가능!
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;
    public GameObject bossArea;
    public Transform myShadow;
    public Animator anim;
    public Collider2D coll;
    public Rigidbody2D rigid;

    SpriteRenderer spriter;
    Color originalColor;

    private Coroutine hitEffectCoroutine;  // 현재 실행 중인 코루틴 참조


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();  
        spriter = GetComponent<SpriteRenderer>();
        originalColor = spriter.color;
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();  
        coll = GetComponent<Collider2D>();
        hands = GetComponentsInChildren<Hand>(true); // 인자값 true를 넣으면 비활성화 된 오브젝트들도 초기화 된다!
    }

    private void Start()
    {
        if (GameManager.instance.playerId == 3)
        {
            // 3번 캐릭터의 그림자 위치를 아래로 설정
            myShadow.localPosition = new Vector3(0, -1.2f, 0);
        }
        else
        {
            // 다른 캐릭터의 그림자 위치를 기본 값으로 설정
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
        
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime; // normalized도 Inspector에서 설정 가능 럭키비키
        rigid.MovePosition(rigid.position + nextVec);
    }

    // 이제 Input.GetAxisRaw("Horizontal") 이거 이렇게 방향값 안받고 InputSystem 이용해서 간단하게 받음
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>(); // .Get<>(); 붙히는 이유는 형식이 다르기 때문에 <T>값을 가져오는 함수이다! Inspector에서 설정해놓은 Vector2값으로 설정

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

        GameManager.instance.isLive = false; // 게임 상태를 죽음으로 설정

        // 하위 객체 비활성화
        for (int index = 2; index < transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.SetActive(false);
        }

        anim.SetTrigger("Dead"); // 죽는 애니메이션 실행
        GameManager.instance.GameOver();
    }

    void OnCollisionStay2D(Collision2D collision) //몬스터 부비부비
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
        

        // 체력이 0 이하로 내려갔는지 확인
        if (GameManager.instance.health <= 0)
        {
            HandleDeath(); // 죽음 처리 호출
        }

        TriggerHitEffect(); // 색깔 변화 호출
    }

    public void TriggerHitEffect()
    {
        // 기존 코루틴이 실행 중이면 중지하고 색상 복구
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
            spriter.color = originalColor;
        }

        // 새로운 코루틴 실행
        hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        spriter.color = new Color(1f, 0f, 0f, 0.5f); // 피격 색상 변경
        yield return new WaitForSeconds(0.2f); // 지속 시간
        spriter.color = originalColor; // 원래 색상 복구
        hitEffectCoroutine = null; // 코루틴 참조 해제
    }
}
