using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy2 : BaseEnemy    // �ϴ� ���� �ʿ� ��� �⺻�� �غ�
{

    public override void Init(SpawnData data)
    {
        base.Init(data);
        health = Mathf.Min(100, maxHealth);
    }

    protected override void FixedUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        // �⺻ �̵� ����: �˹����� �ƴϰ�, �뽬 ���� �ƴ� ���� ���� 
        if (!isSpecialMove && isLive)
        {
            base.FixedUpdate();
        }

        // �ڽ� Ŭ�������� �߰��� ������ ���� ���� , �˹����� �ƴҶ��� ����

        if (!isSpecialMove && isLive)
        {
            Vector2 dirVec = target.position - rigid.position;
            float distanceToTarget = dirVec.magnitude;

            if (distanceToTarget >= 4f && Random.Range(0f, 1f) < 0.05f)
            {
                StartCoroutine(SmoothDash(dirVec));
            }
        }
    }

    IEnumerator SmoothDash(Vector2 dir)
    {
        isSpecialMove = true;

        float dashForce = 5.0f; // ���� �� ũ��
        float dashDuration = 0.5f; // ���� ���� �ð�

        // ���� �������� �� �߰�
        rigid.AddForce(dir.normalized * dashForce, ForceMode2D.Impulse);

        // ���� ���� �ð� ���� ���
        yield return new WaitForSeconds(dashDuration);

        // �뽬 ����
        isSpecialMove = false;
        rigid.velocity = Vector2.zero; // �뽬 �ӵ� �ʱ�ȭ
    }

}
