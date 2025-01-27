using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{
    public float damage;
    private SpriteRenderer spriteRenderer;
    private Collider2D Collider;
    private Rigidbody2D rigid;
    private bool isSyncing = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
        if(gameObject.CompareTag("EnemyBullet 1"))
        {
            Collider.enabled = false;
        }
        else
        {
            Collider.enabled = true;
        }
    }

    public void Init(float damage)
    {
        this.damage = damage;
    }

    public void StopMove()
    {
        gameObject.transform.position += new Vector3(0f, 3f, 0f);
        rigid.velocity = Vector3.zero;
    }



    /// <summary>
    /// BoxCollider2D를 스프라이트 크기에 맞게 동기화하고 활성화
    /// </summary>
    public void StartSyncAndEnableCollider()
    {
        if (spriteRenderer == null || Collider == null) return;

        gameObject.transform.position -= new Vector3(0f, 3f, 0f);

        isSyncing = true; // 동기화 활성화
        Collider.enabled = true;
        StartCoroutine(SyncCollider());
    }

    /// <summary>
    /// BoxCollider2D 동기화를 중지하고 비활성화
    /// </summary>
    public void StopSyncAndDisableCollider()
    {
        isSyncing = false; // 동기화 비활성화
        gameObject.transform.position += new Vector3(0f, 6f, 0f);
        if (Collider != null)
        {
            Collider.enabled = false;
        }
    }

    /// <summary>
    /// 동기화 작업 수행 (코루틴으로 지속적으로 업데이트)
    /// </summary>
    private IEnumerator SyncCollider()
    { 

        while (isSyncing)
        { 

            if (spriteRenderer == null || Collider == null) yield break;

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

            // SpriteRenderer 크기를 BoxCollider2D에 동기화
            boxCollider.size = spriteRenderer.bounds.size;

            // SpriteRenderer의 Pivot에 따라 Offset 조정
            Vector3 localSpritePivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;
            boxCollider.offset = localSpritePivot - (Vector3)spriteRenderer.bounds.extents;
            
            yield return null; // 다음 프레임까지 대기
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!collision.gameObject.CompareTag("Player"))
            return;

        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            GameManager.instance.health -= damage;

            if (GameManager.instance.health <= 0)
            {
                player.HandleDeath();
            }
            else
            {
                player.TriggerHitEffect(); // 색깔 변화 호출
            }

            if (gameObject.tag == "Laser")
                return;

            gameObject.SetActive(false);
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;
        gameObject.SetActive(false);
    }
}