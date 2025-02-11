using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestCamera : MonoBehaviour
{
    [SerializeField] PlayerDummy playerDummy;
    private void LateUpdate()
    {
        transform.position = playerDummy.transform.position + new Vector3(0, 2f, 0);
    }
}
