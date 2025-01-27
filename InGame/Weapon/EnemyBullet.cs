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
    /// BoxCollider2D�� ��������Ʈ ũ�⿡ �°� ����ȭ�ϰ� Ȱ��ȭ
    /// </summary>
    public void StartSyncAndEnableCollider()
    {
        if (spriteRenderer == null || Collider == null) return;

        gameObject.transform.position -= new Vector3(0f, 3f, 0f);

        isSyncing = true; // ����ȭ Ȱ��ȭ
        Collider.enabled = true;
        StartCoroutine(SyncCollider());
    }

    /// <summary>
    /// BoxCollider2D ����ȭ�� �����ϰ� ��Ȱ��ȭ
    /// </summary>
    public void StopSyncAndDisableCollider()
    {
        isSyncing = false; // ����ȭ ��Ȱ��ȭ
        gameObject.transform.position += new Vector3(0f, 6f, 0f);
        if (Collider != null)
        {
            Collider.enabled = false;
        }
    }

    /// <summary>
    /// ����ȭ �۾� ���� (�ڷ�ƾ���� ���������� ������Ʈ)
    /// </summary>
    private IEnumerator SyncCollider()
    { 

        while (isSyncing)
        { 

            if (spriteRenderer == null || Collider == null) yield break;

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

            // SpriteRenderer ũ�⸦ BoxCollider2D�� ����ȭ
            boxCollider.size = spriteRenderer.bounds.size;

            // SpriteRenderer�� Pivot�� ���� Offset ����
            Vector3 localSpritePivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;
            boxCollider.offset = localSpritePivot - (Vector3)spriteRenderer.bounds.extents;
            
            yield return null; // ���� �����ӱ��� ���
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
                player.TriggerHitEffect(); // ���� ��ȭ ȣ��
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