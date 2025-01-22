using UnityEngine;

public class KnockbackTrigger : MonoBehaviour
{
    [SerializeField] private RagdollController _ragdollController;
    [SerializeField] private float _power = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Knockback"))
        {
            _ragdollController.Active();
            var otherPos = other.transform.position;
            _ragdollController.AddForce(new Vector3(transform.position.x - otherPos.x, 0f, transform.position.z - otherPos.z).normalized * _power, ForceMode.Impulse);
        }
    }
}