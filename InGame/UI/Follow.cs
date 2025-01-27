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
            rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position); // World ���ִ� ĳ���� ��ġ�� UI(Screen)�� �ִ� ü�¹��� ��ġ�� �ٸ��� ��ȯ!
        }
    }

}
