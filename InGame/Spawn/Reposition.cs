using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();      
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
       

        switch (transform.tag)
        {
            case "Ground":
                //타일맵 생성 로직
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);
                if (Mathf.Abs(diffX - diffY) <= 0.01f) {  // 추가버전
                    transform.Translate(Vector3.up * dirY * 80);
                    transform.Translate(Vector3.right * dirX * 80);
                }
                else if(diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 80); // 2번 뛰어서 40임   
                } else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 80);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    transform.Translate(ran + dist * 2);
                }
                break;
        }
    }
}
