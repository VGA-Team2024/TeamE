using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] GameObject _destroyEffect;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] float _gravity = 0.2f;
    [SerializeField] float _destroyTime = 3f;
    [SerializeField] float _moveSpeed = 3f;

    void Start()
    {
        Destroy(gameObject , _destroyTime);
        _rigidBody.velocity = transform.forward * _moveSpeed;
    }
    private void FixedUpdate()
    {
        _rigidBody.velocity -= new Vector3(0,_gravity,0) * Time.fixedDeltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Field")
        {
            var obj = Instantiate(_destroyEffect , transform.position , transform.rotation ,null);
            Destroy(obj , 3f);
            Destroy(gameObject);
        }
    }
}
