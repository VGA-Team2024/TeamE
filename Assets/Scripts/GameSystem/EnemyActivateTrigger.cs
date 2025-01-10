using UnityEngine;

public class EnemyActivateTrigger : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemyController.Activate();
            CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_boss");
            Destroy(gameObject);
        }
    }
}
