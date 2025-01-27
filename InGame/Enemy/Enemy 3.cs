using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy3 : BaseEnemy
{
    public float stopDistance = 5.0f; // �÷��̾�� ���� �Ÿ�
    public float fireCooldown = 2.0f; // �߻� ����
    private float fireTimer;

    public float orbitRadius = 3.0f;    // �ݿ� ������ ������
    public float bulletSpeed = 2.0f;   // �Ѿ� �ӵ�
    private string bulletType = "EnemyBullet 0"; // �Ѿ� Ÿ�� (Ǯ �Ŵ������� ��Ī�Ǵ� �̸�)

    public override void Init(SpawnData data)
    {
        base.Init(data);
        health = Mathf.Min(80, maxHealth);
        fireTimer = 0.0f;
    }

    protected override void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!isSpecialMove)
        {
            // �÷��̾���� �Ÿ� ���
            float distanceToPlayer = Vector2.Distance(target.position, rigid.position);

            // ���� �Ÿ� �̳��� ������ Ư�� �̵� Ȱ��ȭ
            if (distanceToPlayer <= stopDistance)
            {
                isSpecialMove = true; // Ư�� �̵� ����
                rigid.velocity = Vector2.zero; // ���߱�
                return; // �⺻ �̵� ���� ����
            }

            // �⺻ �̵� ���� ����
            base.FixedUpdate();
        }
        else
        {
            // Ư�� �̵� ���� ����
            ManageFireCooldown();
        }
    }
    private void ManageFireCooldown()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!isLive)
            return;

        fireTimer += Time.fixedDeltaTime;

        // ��Ÿ�ӿ� ���� �߻�
        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0.0f; // ��Ÿ�� �ʱ�ȭ

            if (Random.value < 0.6f)
            {
                StartCoroutine(FireBulletPattern());
            }
        }
        else
        {
            // �߻� ��� ���¿��� �ൿ
            rigid.velocity = Vector2.zero; // ���� ���� ����
        }
    }

    IEnumerator FireBulletPattern()
    {
        // �߻� ����� �÷��̾� ��ġ ����
        Vector2 targetPosition = GameManager.instance.player.transform.position;

        GameObject bullet = GameManager.instance.pool.Get(bulletType);
        bullet.transform.position = rigid.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // ���� ����
        float angleStep = 10.0f; // ���� ����
        float startAngle = 90.0f;
        float endAngle = 270.0f;

        Vector2 origin = rigid.position; // źȯ ���� ��ġ
        List<Vector2> path = new List<Vector2>();

        // �ݿ� ���� ���
        for (float angle = startAngle; angle <= endAngle; angle += angleStep)
        {
            float radian = angle * Mathf.Deg2Rad;
            path.Add(origin + new Vector2(
                Mathf.Cos(radian) * orbitRadius,
                Mathf.Sin(radian) * orbitRadius
            ));
        }

        // źȯ �߻�
        foreach (var point in path)
        {
            // �ݿ� ������ ������ �÷��̾� ��ġ�� ���ϴ� ���� ���
            Vector2 direction = ((Vector2)point - (Vector2)bullet.transform.position).normalized;

            // ó�� ����� �÷��̾� ��ġ�� ���� ����
            Vector2 targetDirection = (targetPosition - (Vector2)bullet.transform.position).normalized;

            // �ε巴�� �ݿ��� �׸��鼭 �÷��̾� ��ġ�� ���ϰ� ��
            bulletRb.velocity = Vector2.Lerp(direction, targetDirection, 0.3f).normalized * bulletSpeed;

            // ���� �� �� �̵� ���
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(2.0f);
        bullet.SetActive(false);
        isSpecialMove = false;
    }
}
