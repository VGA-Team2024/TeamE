using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    float _timer = 0f;

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= 3f) Destroy(gameObject);
    }
}
