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

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // �ι�° ���ڴ� ���ù��� �� �ʿ����� �ʴ�
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
                                                                                                ? 0 : 1;   // �̰� ���̵𺰷� ���� sprite �ٸ��� �Ϸ��� �������ذ�

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++) // �̰� ���� ������ ItemData�� �������İ� Ǯ�Ŵ����� ���������� ������ �׶��� index�� prefabId�� ����!
        {
            if (data.projecttile[bulletCnt] == GameManager.instance.pool.prefabs[index])  // Scriptable Object�� �������� ���ؼ� projecttile�� �ε����� �ƴ� ���������� �����Ѱ���!
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
                speed = 0.7f * Character.WeaponRate;  // ���Ÿ� ���⿡�� speed = ����ӵ� �������� ����
                break;
        }

        // Hand Set
        Hand hand = player.hands[(int)data.itemType];  // enum���� int������ ��
        hand.spriter.sprite = data.hand[bulletCnt];
        hand.gameObject.SetActive(true);
         
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);  // Ư�� �Լ� ȣ���� ��� �ڽĿ��� ����ϴ� �Լ�
    }

    void Batch() // ��ǻ�� ���п�� batch���� �׳� ��ġ�� ����
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
                bullet.parent = transform; //Weapon n ���ٰ� ����ֱ�! �θ� �����ֱ�!
            }
                
            
            bullet.localPosition = Vector3.zero; //  ��װ� �����ɶ� PoolManager ��ġ���� ���� �� �Ŀ� �θ� ������Ʈ�� �Ҵ�Ǵϱ� ������ġ�� �ʱ�ȭ
            bullet.localRotation = Quaternion.identity; // ȸ����

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -100 �� ���������̴� ���� �����̶� ��.

            // Animator ����ȭ
            Animator bulletAnimator = bullet.GetComponent<Animator>();
            if (bulletAnimator != null && transform.childCount > 0)
            {
                // ���� Animator�� ����ȭ (ù ��° ���⸦ ��������)
                Animator referenceAnimator = transform.GetChild(0).GetComponent<Animator>();
                if (referenceAnimator != null)
                {
                    AnimatorStateInfo stateInfo = referenceAnimator.GetCurrentAnimatorStateInfo(0);
                    float normalizedTime = stateInfo.normalizedTime % 1; // �ִϸ��̼� ���� ���� (0~1 ����)
                    bulletAnimator.Play(stateInfo.fullPathHash, 0, normalizedTime);
                    bulletAnimator.speed = referenceAnimator.speed; // �ӵ� ����ȭ
                }
            }
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 baseDir = (targetPos - transform.position).normalized; // �⺻ ���� ����

        // ���� ���� ���
        float angleStep = 25f; // �߻�ü �� ���� (���� ����)
        float startAngle = -((count - 1) * angleStep) / 2; // ��ä�� ���� ����

        for (int i = 0; i < count; i++)
        {
            float angle = startAngle + (i * angleStep); // ���� �߻�ü�� ����
            Quaternion rotation = Quaternion.Euler(0, 0, angle); // ȸ���� ���

            // ������ ������ ���� ���
            Vector3 bulletDir = rotation * baseDir;

            // �Ѿ� ���� �� �ʱ�ȭ
            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, bulletDir); // �Ѿ� �������� ȸ��
            bullet.GetComponent<Bullet>().Init(damage, count/2, bulletDir);
        }

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }
}
