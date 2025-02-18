using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHItGoTitle : MonoBehaviour
{
   [SerializeField] SceneTransition _sceneTransition;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            _sceneTransition.LoadTitle();
        }
    }
}
