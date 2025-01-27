using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed; 

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (GameManager.instance.IsPaused)
            return;
        if (!GameManager.instance.isLive)
            return;
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;

                if(timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if(id == 0)
            Batch();

        if (id == 1)
            this.speed = speed * 1.2f;

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // 두번째 인자는 리시버가 꼭 필요하진 않다
    }

    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        int bulletCnt = (GameManager.instance.playerId == 0 || GameManager.instance.playerId == 1)
                                                                                                ? 0 : 1;   // 이거 아이디별로 무기 sprite 다르게 하려고 구분해준거

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++) // 이거 무기 형식을 ItemData의 무기형식과 풀매니저의 무기형식이 같을때 그때의 index를 prefabId로 지정!
        {
            if (data.projecttile[bulletCnt] == GameManager.instance.pool.prefabs[index])  // Scriptable Object의 독립성을 위해서 projecttile을 인덱스가 아닌 프리펩으로 설정한거임!
            {
                prefabId = index; 
                break;
            }
        }

        switch (id) {
            case 0:
                Batch();
                speed = 150 * Character.WeaponSpeed;
                break;
            default:
                speed = 0.7f * Character.WeaponRate;  // 원거리 무기에선 speed = 연사속도 작을수록 빠름
                break;
        }

        // Hand Set
        Hand hand = player.hands[(int)data.itemType];  // enum값을 int형으로 씀
        hand.spriter.sprite = data.hand[bulletCnt];
        hand.gameObject.SetActive(true);
         
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);  // 특정 함수 호출을 모든 자식에게 방송하는 함수
    }

    void Batch() // 컴퓨터 과학용어 batch말고 그냥 배치임 ㅇㅇ
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;
                
            if(index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform; //Weapon n 에다가 집어넣기! 부모 정해주기!
            }
                
            
            bullet.localPosition = Vector3.zero; //  얘네가 생성될때 PoolManager 위치에서 생성 된 후에 부모 오브젝트가 할당되니까 로컬위치를 초기화
            bullet.localRotation = Quaternion.identity; // 회전도

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 은 근접공격이니 무한 관통이란 뜻.

            // Animator 동기화
            Animator bulletAnimator = bullet.GetComponent<Animator>();
            if (bulletAnimator != null && transform.childCount > 0)
            {
                // 기존 Animator와 동기화 (첫 번째 무기를 기준으로)
                Animator referenceAnimator = transform.GetChild(0).GetComponent<Animator>();
                if (referenceAnimator != null)
                {
                    AnimatorStateInfo stateInfo = referenceAnimator.GetCurrentAnimatorStateInfo(0);
                    float normalizedTime = stateInfo.normalizedTime % 1; // 애니메이션 진행 상태 (0~1 사이)
                    bulletAnimator.Play(stateInfo.fullPathHash, 0, normalizedTime);
                    bulletAnimator.speed = referenceAnimator.speed; // 속도 동기화
                }
            }
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 baseDir = (targetPos - transform.position).normalized; // 기본 방향 벡터

        // 각도 간격 계산
        float angleStep = 25f; // 발사체 간 각도 (조정 가능)
        float startAngle = -((count - 1) * angleStep) / 2; // 부채꼴 시작 각도

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + (i * angleStep); // 현재 발사체의 각도
            Quaternion rotation = Quaternion.Euler(0, 0, angle); // 회전값 계산

            // 각도를 적용한 방향 계산
            Vector3 bulletDir = rotation * baseDir;

            // 총알 생성 및 초기화
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, bulletDir); // 총알 방향으로 회전
            bullet.GetComponent<Bullet>().Init(damage, count/2, bulletDir);
        }

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
