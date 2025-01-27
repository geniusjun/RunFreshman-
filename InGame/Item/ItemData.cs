using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string[] itemName;

    [System.Serializable]
    public class TextElement // [TextArea]를 적용하기 위한 커스텀 클래스
    {
        [TextArea(2, 5)] // 최소 3줄, 최대 5줄
        public string text;
    }

    public TextElement[] itemDesc; // 배열로 사용

    public Sprite[] itemIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# Weapon")]
    public GameObject[] projecttile;
    public Sprite[] hand;

}
