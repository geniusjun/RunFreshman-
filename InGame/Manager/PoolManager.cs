using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs; // 프리펩 보관

    List<GameObject>[] pools; // 풀 담당을 하는 리스트 자료구조

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

        // ... 선택한 풀의 비활성화된 게임 오브젝트 접근
        foreach(GameObject item in pools[index])
        {
            // ... 발견하면 select 변수에 할당
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // ... 못 찾았으면?
        if (select == null)
        {
            //... 새롭게 생성하고 select 변수에 할당
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }



}
