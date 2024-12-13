using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestCamera : MonoBehaviour
{
    [SerializeField] PlayerDummy playerDummy;

    void FixedUpdate()
    {
        transform.rotation = playerDummy.transform.rotation;

        transform.position = playerDummy.transform.position;
    }
}
