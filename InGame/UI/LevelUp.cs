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
        // 버튼 클릭 사운드 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        // 배경음 효과 비활성화
        AudioManager.instance.EffectBgm(false);

        // 일정 시간 대기
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
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int playerId = GameManager.instance.playerId;
        List<Item> selectableItems = new List<Item>();

        // 2. playerId에 따른 필터링
        foreach (Item item in items)
        {
            if (item.level >= item.data.damages.Length) continue; // 만렙 아이템 제외

            // playerId 0, 2: 근접 무기만 선택
            if ((playerId == 0 || playerId == 2) && item.data.itemType == ItemData.ItemType.Melee)
            {
                selectableItems.Add(item);
            }
            // playerId 1, 3: 원거리 무기만 선택
            else if ((playerId == 1 || playerId == 3) && item.data.itemType == ItemData.ItemType.Range)
            {
                selectableItems.Add(item);
            }
            // 기타 아이템 추가 (Glove, Shoe, 4번 체력 아이템만 추가)
            else if (item.data.itemType == ItemData.ItemType.Glove ||
                     item.data.itemType == ItemData.ItemType.Shoe ||
                     (item.data.itemType == ItemData.ItemType.Heal && item == items[4]))
            {
                selectableItems.Add(item);
            }
        }

        // 3. 랜덤으로 3개의 아이템 선택
        selectableItems = selectableItems.OrderBy(x => Random.value).ToList();
        int count = Mathf.Min(3, selectableItems.Count);

        for (int i = 0; i < count; i++)
        {
            selectableItems[i].gameObject.SetActive(true);
        }

        // 4. 부족한 아이템을 5번, 6번 체력 아이템으로 대체
        if (count < 3)
        {
            for (int i = count; i < 3; i++)
            {
                // 5번, 6번 체력 아이템을 순차적으로 활성화
                items[5 + (i - count)].gameObject.SetActive(true);
            }
        }

        // 레벨업 화면 표시
        rect.localScale = Vector3.one;
    }
}