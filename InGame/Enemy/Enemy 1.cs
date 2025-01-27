using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy1 : BaseEnemy 
{

    public override void Init(SpawnData data)
    {
        base.Init(data);
        health = Mathf.Min(100, maxHealth);
    }

}
