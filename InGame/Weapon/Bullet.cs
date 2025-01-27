using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; //����

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0) // ������ -100(����)���� ū �Ϳ� ���ؼ��� �ӵ� ���� -> �ٰŸ������ per�� -100 �־����.
        {
            rigid.velocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -100)
            return;

        per--;

        if (per < 0)
        {

            rigid.velocity = Vector3.zero; // ����� �� �Ǽ� ������� �ӵ� 0���� �ʱ�ȭ�ϰ� SetActive(false) �ؾ��� Ǯ�� ���� ���ݴ�?
            gameObject.SetActive(false);
        }

      

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Melee);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        gameObject.SetActive(false);
    }
}
