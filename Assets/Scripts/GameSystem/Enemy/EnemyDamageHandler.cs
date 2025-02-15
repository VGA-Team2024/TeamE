using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyDamageHandler : MonoBehaviour
{
    [SerializeField, InspectorVariantName("ダメージを受ける弱点箇所")] private List<EnemyDamagePointHandler> _damagePointHandlers;
    [SerializeField, InspectorVariantName("トドメを刺す弱点箇所")] private EnemyDamagePointHandler _killPoint;
    [SerializeField, InspectorVariantName("解放する弱点箇所")] private EnemyDamagePointHandler _releasePoint;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private EnemyCameraTransition _cameraTransition;
    [SerializeField] private CanvasGroup _enemyHealthCanvasGroup;
    [SerializeField] private Image _enemyHealthImage;
    
    private void Awake()
    {
        _enemyHealthCanvasGroup.alpha = 0f;
        foreach (var damagePoint in _damagePointHandlers)
        {
            damagePoint.OnDamage += OnDamage;
        }

        _killPoint.OnDamage += () =>
        {
            TimelinePlayer.TimelineType = TimelineType.KillEnding;
            SceneManager.LoadScene("Timeline");
            _releasePoint.Locked = true;
        };
        _releasePoint.OnDamage += () =>
        {
            TimelinePlayer.TimelineType = TimelineType.ReleaseEnding;
            SceneManager.LoadScene("Timeline");
            _killPoint.Locked = true;
        };
    }

    private void OnDamage()
    {
        _enemyHealthCanvasGroup.alpha = 1f;
        _enemyHealthImage.fillAmount = (float)_damagePointHandlers.Count(damagePoint => !damagePoint.Locked) /
                                       _damagePointHandlers.Count;
        //  全ての弱点が破壊されたら
        if (_damagePointHandlers.TrueForAll(damagePoint => damagePoint.Locked))
        {
            _enemyController.SetDying();
            _killPoint.Locked = false;
            _releasePoint.Locked = false;
            _cameraTransition.ViewFinishPoints().Forget();
        }
    }
}