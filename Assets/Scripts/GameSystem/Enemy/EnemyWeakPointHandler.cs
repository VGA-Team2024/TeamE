using UnityEngine;

/// <summary>敵の弱点の処理を行うクラス</summary>
public class EnemyWeakPointHandler : MonoBehaviour
{
    private enum WeakPointType
    {
        WeakPoint1, WeakPoint2, WeakPoint3, WeakPoint4
    }

    private enum WeakPointSide
    {
        Right, Left
    }
    
    [SerializeField] private WeakPointType weakPointType;
    
    [SerializeField] private WeakPointSide weakPointSide;

    [SerializeField] private EnemyController enemyController;

    private void OnTriggerEnter(Collider bow)
    {
        if (bow.CompareTag("Bow") && weakPointType == WeakPointType.WeakPoint2)
        {
            switch (weakPointSide)
            {
                case WeakPointSide.Right:
                    enemyController.GetDownRight();
                    break;
                
                case WeakPointSide.Left:
                    enemyController.GetDownLeft();
                    break;
            }
        }
    }
}
