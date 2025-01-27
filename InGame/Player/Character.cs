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

    public static float WeaponSpeed  // 근접무기 삥삥 도는 속도
    {
        get { return GameManager.instance.playerId == 0 ? 1.2f : 1f; } // 1번째 캐릭 근접무기 도는 속도 빠름.
    }

    public static float WeaponRate // 총쏘는 기준 보면 Weapon.speed가 Timer를 넘어갈때 쏘기때문에 speed가 작아야 더 빨리쏨! 즉 Weapon.Init() speed 구하는 공식 보면, WeaponRate값이 작아야 speed도 작게 초반 설정됌!
    {
        get { return GameManager.instance.playerId == 1 ? 0.8f : 1f; } // 2번째 캐릭 연사속도 빠르고~
    }

    public static float Damage
    {
        get { return GameManager.instance.playerId == 3 ? 1.2f : 1f; } //4번째 캐릭 원거리 공격력 높은 채로 시작.
    }
    public static int Count
    {
        get { return GameManager.instance.playerId == 2 ? 1 : 0; } // 3번째 캐릭 무기카운트 한개 더 많은 채로 시작.
    }
}
