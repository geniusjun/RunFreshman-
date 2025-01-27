using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy5 : BaseEnemy
{
    public float stopDistance = 4.0f; // 플레이어와 멈출 거리
    public float fireCooldown = 4.0f; // 총알 발사 쿨타임
    public float bulletSpeed = 2.0f; // 총알 속도
    private string bulletType = "EnemyBullet 1"; // 총알 타입 (풀 매니저에서 매칭되는 이름)

    private float fireTimer = 0.0f; // 쿨타임 타이머


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
            isSpecialMove = true; // 특수 이동 시작
            rigid.velocity = Vector2.zero; // 멈춤
            anim.SetTrigger("Stand");
        }

        if (isSpecialMove)
        {
            ManageFireCooldown(); // 쿨타임 관리
        }
        else
        {
            base.FixedUpdate(); // 기본 이동 로직 실행
        }
    }
    private void ManageFireCooldown()
    {
        fireTimer += Time.fixedDeltaTime;

        // 쿨타임이 완료되면 발사
        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0.0f; // 쿨타임 초기화
            if (Random.value < 0.6f)
            {
                StartCoroutine(FireThreeBullets());
            }
        }
    }

    private IEnumerator FireThreeBullets()
    {
        anim.SetBool("IsAttack", true);

        for (int i = 0; i < 4; i++) // 세 발 발사
        {
            // 총알 가져오기
            GameObject bullet = GameManager.instance.pool.Get(bulletType);

            if (bullet == null)
            {
                yield break; // null이면 발사를 중단
            }  


            if (bullet != null)
            {
                bullet.transform.position = rigid.position; // 발사 위치
                bullet.transform.rotation = Quaternion.identity; // 초기 회전

                // 랜덤 방향 설정
                Vector2 randomDirection = Random.insideUnitCircle.normalized;

                // 총알 이동
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.velocity = randomDirection * bulletSpeed;
            }

            yield return new WaitForSeconds(0.2f); // 발사 간격
        }
        anim.SetBool("IsAttack", false);
        isSpecialMove = false;
    }
}