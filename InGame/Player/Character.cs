using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static float Speed
    {
        get
        {
            switch (GameManager.instance.playerId)
            {
                case 1:
                case 3:
                    return 1.2f;
                default:
                    return 1f;
            }
        }
    }

    public static float WeaponSpeed  // �������� ��� ���� �ӵ�
    {
        get { return GameManager.instance.playerId == 0 ? 1.2f : 1f; } // 1��° ĳ�� �������� ���� �ӵ� ����.
    }

    public static float WeaponRate // �ѽ�� ���� ���� Weapon.speed�� Timer�� �Ѿ�� ��⶧���� speed�� �۾ƾ� �� ������! �� Weapon.Init() speed ���ϴ� ���� ����, WeaponRate���� �۾ƾ� speed�� �۰� �ʹ� ������!
    {
        get { return GameManager.instance.playerId == 1 ? 0.8f : 1f; } // 2��° ĳ�� ����ӵ� ������~
    }

    public static float Damage
    {
        get { return GameManager.instance.playerId == 3 ? 1.2f : 1f; } //4��° ĳ�� ���Ÿ� ���ݷ� ���� ä�� ����.
    }
    public static int Count
    {
        get { return GameManager.instance.playerId == 2 ? 1 : 0; } // 3��° ĳ�� ����ī��Ʈ �Ѱ� �� ���� ä�� ����.
    }
}
