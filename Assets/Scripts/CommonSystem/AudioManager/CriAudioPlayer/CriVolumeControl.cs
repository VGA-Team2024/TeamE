using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CriVolumeControl : MonoBehaviour
{
    [SerializeField] private Text _volumeText;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private InputField _volumeInputField;
    [SerializeField] private SoundType _soundType;
    public SoundType SoundType => _soundType;
    private float _currentValue;

    /// <summary>
    /// 初期化メソッド
    /// </summary>
    public void Initialize(string label, float initialValue,
        UnityAction<float> onSliderChanged, UnityAction<string> onInputChanged)
    {
        // UI要素の初期化
        _volumeText.text = label;
        _volumeSlider.minValue = 0;
        _volumeSlider.maxValue = 100;
        _volumeSlider.value = initialValue * 100;
        _volumeSlider.onValueChanged.AddListener(onSliderChanged);
        _volumeSlider.onValueChanged.AddListener(OnSliderChanged);

        // 初期値を整数として設定
        _volumeInputField.text = Mathf.FloorToInt(initialValue * 100).ToString(CultureInfo.CurrentCulture);
        _volumeInputField.onEndEdit.AddListener(onInputChanged);
        _volumeInputField.onEndEdit.AddListener(OnInputChanged);

        // 内部状態の初期化
        _currentValue = initialValue;
    }

    /// <summary>
    /// スライダー変更時のローカル処理
    /// </summary>
    private void OnSliderChanged(float value)
    {
        _currentValue = value / 100;

        // 入力フィールドの値を整数として更新
        _volumeInputField.text = Mathf.FloorToInt(value).ToString(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// 入力フィールド変更時のローカル処理
    /// </summary>
    private void OnInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            _currentValue = result / 100;

            // スライダーの値を更新
            _volumeSlider.value = Mathf.FloorToInt(result);
        }
        else
        {
            Debug.LogWarning("入力が無効です。音量は数値である必要があります。");
        }
    }

    private void OnDestroy()
    {
        // イベントリスナーの解除
        _volumeSlider.onValueChanged.RemoveAllListeners();
        _volumeInputField.onEndEdit.RemoveAllListeners();
    }
}