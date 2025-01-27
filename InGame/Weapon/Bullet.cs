using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; //관통

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0) // 관통이 -100(무한)보다 큰 것에 대해서는 속도 적용 -> 근거리무기는 per로 -100 넣어놓음.
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

            rigid.velocity = Vector3.zero; // 관통력 다 되서 사라질때 속도 0으로 초기화하고 SetActive(false) 해야함 풀에 넣을 꺼잖니?
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
