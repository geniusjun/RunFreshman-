using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs; // ������ ����

    List<GameObject>[] pools; // Ǯ ����� �ϴ� ����Ʈ �ڷᱸ��

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
        
        for(int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(string type)
    {
        int index = Array.FindIndex(prefabs, prefab => prefab.name == type);

        if (index == -1)
            throw new System.Exception($"Prefab {type} not found in PoolManager!");

        return Get(index);
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // ... ������ Ǯ�� ��Ȱ��ȭ�� ���� ������Ʈ ����
        foreach(GameObject item in pools[index])
        {
            // ... �߰��ϸ� select ������ �Ҵ�
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // ... �� ã������?
        if (select == null)
        {
            //... ���Ӱ� �����ϰ� select ������ �Ҵ�
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }



}
