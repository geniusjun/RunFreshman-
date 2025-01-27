using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy3 : BaseEnemy
{
    public float stopDistance = 5.0f; // 플레이어와 멈출 거리
    public float fireCooldown = 2.0f; // 발사 간격
    private float fireTimer;

    public float orbitRadius = 3.0f;    // 반원 궤적의 반지름
    public float bulletSpeed = 2.0f;   // 총알 속도
    private string bulletType = "EnemyBullet 0"; // 총알 타입 (풀 매니저에서 매칭되는 이름)

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
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(target.position, rigid.position);

            // 일정 거리 이내에 들어오면 특수 이동 활성화
            if (distanceToPlayer <= stopDistance)
            {
                isSpecialMove = true; // 특수 이동 시작
                rigid.velocity = Vector2.zero; // 멈추기
                return; // 기본 이동 로직 종료
            }

            // 기본 이동 로직 실행
            base.FixedUpdate();
        }
        else
        {
            // 특수 이동 로직 실행
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

        // 쿨타임에 따라 발사
        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0.0f; // 쿨타임 초기화

            if (Random.value < 0.6f)
            {
                StartCoroutine(FireBulletPattern());
            }
        }
        else
        {
            // 발사 대기 상태에서 행동
            rigid.velocity = Vector2.zero; // 멈춘 상태 유지
        }
    }

    IEnumerator FireBulletPattern()
    {
        // 발사 당시의 플레이어 위치 저장
        Vector2 targetPosition = GameManager.instance.player.transform.position;

        GameObject bullet = GameManager.instance.pool.Get(bulletType);
        bullet.transform.position = rigid.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // 각도 설정
        float angleStep = 10.0f; // 각도 간격
        float startAngle = 90.0f;
        float endAngle = 270.0f;

        Vector2 origin = rigid.position; // 탄환 시작 위치
        List<Vector2> path = new List<Vector2>();

        // 반원 궤적 계산
        for (float angle = startAngle; angle <= endAngle; angle += angleStep)
        {
            float radian = angle * Mathf.Deg2Rad;
            path.Add(origin + new Vector2(
                Mathf.Cos(radian) * orbitRadius,
                Mathf.Sin(radian) * orbitRadius
            ));
        }

        // 탄환 발사
        foreach (var point in path)
        {
            // 반원 궤적의 점에서 플레이어 위치로 향하는 방향 계산
            Vector2 direction = ((Vector2)point - (Vector2)bullet.transform.position).normalized;

            // 처음 저장된 플레이어 위치로 최종 보정
            Vector2 targetDirection = (targetPosition - (Vector2)bullet.transform.position).normalized;

            // 부드럽게 반원을 그리면서 플레이어 위치를 향하게 함
            bulletRb.velocity = Vector2.Lerp(direction, targetDirection, 0.3f).normalized * bulletSpeed;

            // 궤적 점 간 이동 대기
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(2.0f);
        bullet.SetActive(false);
        isSpecialMove = false;
    }
}
