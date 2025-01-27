using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy5 : BaseEnemy
{
    public float stopDistance = 4.0f; // �÷��̾�� ���� �Ÿ�
    public float fireCooldown = 4.0f; // �Ѿ� �߻� ��Ÿ��
    public float bulletSpeed = 2.0f; // �Ѿ� �ӵ�
    private string bulletType = "EnemyBullet 1"; // �Ѿ� Ÿ�� (Ǯ �Ŵ������� ��Ī�Ǵ� �̸�)

    private float fireTimer = 0.0f; // ��Ÿ�� Ÿ�̸�


    public override void Init(SpawnData data)
    {
        base.Init(data);
        health = Mathf.Min(100, maxHealth);
        fireTimer = 0.0f;
        anim.SetBool("IsAttack", false);
    }

    protected override void FixedUpdate()
    {
        if (GameManager.instance.IsPaused || !isLive)
            return;

        float distanceToPlayer = Vector2.Distance(target.position, rigid.position);

        if (distanceToPlayer <= stopDistance && !isSpecialMove)
        {
            isSpecialMove = true; // Ư�� �̵� ����
            rigid.velocity = Vector2.zero; // ����
            anim.SetTrigger("Stand");
        }

        if (isSpecialMove)
        {
            ManageFireCooldown(); // ��Ÿ�� ����
        }
        else
        {
            base.FixedUpdate(); // �⺻ �̵� ���� ����
        }
    }
    private void ManageFireCooldown()
    {
        fireTimer += Time.fixedDeltaTime;

        // ��Ÿ���� �Ϸ�Ǹ� �߻�
        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0.0f; // ��Ÿ�� �ʱ�ȭ
            if (Random.value < 0.6f)
            {
                StartCoroutine(FireThreeBullets());
            }
        }
    }

    private IEnumerator FireThreeBullets()
    {
        anim.SetBool("IsAttack", true);

        for (int i = 0; i < 4; i++) // �� �� �߻�
        {
            // �Ѿ� ��������
            GameObject bullet = GameManager.instance.pool.Get(bulletType);

            if (bullet == null)
            {
                yield break; // null�̸� �߻縦 �ߴ�
            }  


            if (bullet != null)
            {
                bullet.transform.position = rigid.position; // �߻� ��ġ
                bullet.transform.rotation = Quaternion.identity; // �ʱ� ȸ��

                // ���� ���� ����
                Vector2 randomDirection = Random.insideUnitCircle.normalized;

                // �Ѿ� �̵�
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.velocity = randomDirection * bulletSpeed;
            }

            yield return new WaitForSeconds(0.2f); // �߻� ����
        }
        anim.SetBool("IsAttack", false);
        isSpecialMove = false;
    }
}