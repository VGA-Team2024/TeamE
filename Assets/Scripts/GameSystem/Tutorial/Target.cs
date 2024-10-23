using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour
{
    [SerializeField] float _destroyTime = 3f;
    Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _rb.useGravity = true;
        Destroy(gameObject, _destroyTime);
    }
}
