using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();   
    }

    void FixedUpdate()
    {

        if (GameManager.instance.playerId == 3)
        {
            rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position) - new Vector3(0, 50, 0);
        }
        else
        {
            rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position); // World 상에있는 캐릭터 위치랑 UI(Screen)에 있는 체력바의 위치가 다르니 변환!
        }
    }

}
