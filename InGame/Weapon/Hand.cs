using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0); // 오른손은 위치를 바꿔야함.
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0, -35); //왼손은 회전을 바꿔야함.
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135); 
     
    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1]; // 자기자신한테도 SpriteRenderer가 포함되어있으면 [0]번째는 나임!    
    }

    void LateUpdate()
    {
        if (GameManager.instance.IsPaused)
            return;
        bool isReverse = player.flipX;
        
        if (isLeft) // 근접무기
        {
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 4 : 6;
        }
        else // 원거리 무기
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 6 : 4;
        }
    }


}
