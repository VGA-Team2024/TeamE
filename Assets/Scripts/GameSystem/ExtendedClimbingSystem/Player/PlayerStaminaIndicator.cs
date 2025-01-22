using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaIndicator : MonoBehaviour, IIndicator
{
    [SerializeField] private Image _indicatorImage;
    private Vector3 _defaultScale;
    private void Awake()
    {
        _defaultScale = _indicatorImage.transform.localScale;
    }

    public void SetRatio(float ratio)
    {
        _indicatorImage.transform.localScale = _defaultScale * ratio;
    }
}