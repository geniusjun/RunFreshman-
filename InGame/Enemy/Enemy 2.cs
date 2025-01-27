using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy2 : BaseEnemy    // 일단 수정 필요 상속 기본만 해봄
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
        // 기본 이동 로직: 넉백중이 아니고, 대쉬 중이 아닐 때만 실행 
        if (!isSpecialMove && isLive)
        {
            base.FixedUpdate();
        }

        // 자식 클래스에서 추가로 정의한 돌진 로직 , 넉백중이 아닐때만 실행

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

        float dashForce = 5.0f; // 돌진 힘 크기
        float dashDuration = 0.5f; // 돌진 지속 시간

        // 돌진 방향으로 힘 추가
        rigid.AddForce(dir.normalized * dashForce, ForceMode2D.Impulse);

        // 돌진 지속 시간 동안 대기
        yield return new WaitForSeconds(dashDuration);

        // 대쉬 종료
        isSpecialMove = false;
        rigid.velocity = Vector2.zero; // 대쉬 속도 초기화
    }

}
