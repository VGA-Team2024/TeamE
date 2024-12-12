using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Time.deltaTime * 10.0f * Vector3.left);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Time.deltaTime * 10.0f * Vector3.right);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Time.deltaTime * 10.0f * Vector3.forward);
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Time.deltaTime * 10.0f * Vector3.back);
        }
    }
}
