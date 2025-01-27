using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public GameObject Shadows;

    private void Update()
    {
        if (GameManager.instance.playerId == 3)
        {
            // 3번 캐릭터의 그림자 위치를 설정
            Shadows.transform.position = GameManager.instance.player.transform.position - new Vector3(0, 0.85f, 0);
        }
        else
        {
            // 다른 캐릭터들의 그림자 위치를 설정
            Shadows.transform.position = GameManager.instance.player.transform.position - new Vector3(0, 0.6f, 0);
        }
    }
}