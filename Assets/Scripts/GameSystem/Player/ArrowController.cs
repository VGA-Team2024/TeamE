using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] GameObject _destroyEffect;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _trail;
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
        if (collision.collider.CompareTag("Player")) return;
        
        var obj = Instantiate(_destroyEffect , transform.position , transform.rotation ,null);
        if (_trail) _trail.SetParent(obj.transform);
        Destroy(obj , 3f);
        Destroy(gameObject, 0.01f);
    }
}
