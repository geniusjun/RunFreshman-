using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }   

    public void Show()
    {
        Next();
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }

    public void Hide()
    {
          StartCoroutine(HideAndResume());
    }

    private IEnumerator HideAndResume()
    {
        // ��ư Ŭ�� ���� ���
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        // ����� ȿ�� ��Ȱ��ȭ
        AudioManager.instance.EffectBgm(false);

        // ���� �ð� ���
        yield return new WaitForSecondsRealtime(0.01f);

        rect.localScale = Vector3.zero;

        GameManager.instance.Resume();
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 1. ��� ������ ��Ȱ��ȭ
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int playerId = GameManager.instance.playerId;
        List<Item> selectableItems = new List<Item>();

        // 2. playerId�� ���� ���͸�
        foreach (Item item in items)
        {
            if (item.level >= item.data.damages.Length) continue; // ���� ������ ����

            // playerId 0, 2: ���� ���⸸ ����
            if ((playerId == 0 || playerId == 2) && item.data.itemType == ItemData.ItemType.Melee)
            {
                selectableItems.Add(item);
            }
            // playerId 1, 3: ���Ÿ� ���⸸ ����
            else if ((playerId == 1 || playerId == 3) && item.data.itemType == ItemData.ItemType.Range)
            {
                selectableItems.Add(item);
            }
            // ��Ÿ ������ �߰� (Glove, Shoe, 4�� ü�� �����۸� �߰�)
            else if (item.data.itemType == ItemData.ItemType.Glove ||
                     item.data.itemType == ItemData.ItemType.Shoe ||
                     (item.data.itemType == ItemData.ItemType.Heal && item == items[4]))
            {
                selectableItems.Add(item);
            }
        }

        // 3. �������� 3���� ������ ����
        selectableItems = selectableItems.OrderBy(x => Random.value).ToList();
        int count = Mathf.Min(3, selectableItems.Count);

        for (int i = 0; i < count; i++)
        {
            selectableItems[i].gameObject.SetActive(true);
        }

        // 4. ������ �������� 5��, 6�� ü�� ���������� ��ü
        if (count < 3)
        {
            for (int i = count; i < 3; i++)
            {
                // 5��, 6�� ü�� �������� ���������� Ȱ��ȭ
                items[5 + (i - count)].gameObject.SetActive(true);
            }
        }

        // ������ ȭ�� ǥ��
        rect.localScale = Vector3.one;
    }
}