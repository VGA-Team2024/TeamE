using UnityEngine;

/// <summary>敵の弱点の処理を行うクラス</summary>
public class EnemyDownPointHandler : MonoBehaviour
{
    [SerializeField] private WeakPointSide _downPointSide;
    [SerializeField] private ParticleSystem _arrowHitEffect;
    [SerializeField] private EnemyController _enemyController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Arrow")) return;
        _arrowHitEffect?.Play();
        switch (_downPointSide)
        {
            case WeakPointSide.Right:
                _enemyController.GetDownRight();
                break;
                
            case WeakPointSide.Left:
                _enemyController.GetDownLeft();
                break;
        }
    }
    private enum WeakPointSide
    {
        Right, Left
    }
}
