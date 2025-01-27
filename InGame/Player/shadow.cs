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
            // 3�� ĳ������ �׸��� ��ġ�� ����
            Shadows.transform.position = GameManager.instance.player.transform.position - new Vector3(0, 0.85f, 0);
        }
        else
        {
            // �ٸ� ĳ���͵��� �׸��� ��ġ�� ����
            Shadows.transform.position = GameManager.instance.player.transform.position - new Vector3(0, 0.6f, 0);
        }
    }
}