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

    private CRIAudioManager _criAudioManager;
    private float _currentValue;

    private void Awake()
    {
        // CRIAudioManager のインスタンスを取得
        _criAudioManager = CRIAudioManager.Instance;
    }

    public void Initialize(string label, float initialValue, SoundType soundType,
        UnityAction<float> onSliderChanged, UnityAction<string> onInputChanged)
    {
        _soundType = soundType;

        // UI要素の初期化
        _volumeText.text = label;
        _volumeSlider.minValue = 0;
        _volumeSlider.maxValue = 100;
        _volumeSlider.value = initialValue * 100;
        _volumeSlider.onValueChanged.AddListener(onSliderChanged);
        _volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        _volumeInputField.text = (initialValue * 100).ToString(CultureInfo.CurrentCulture);
        _volumeInputField.onEndEdit.AddListener(onInputChanged);
        _volumeInputField.onEndEdit.AddListener(OnInputChanged);

        // 内部状態の初期化
        _currentValue = initialValue;
    }

    private void OnSliderChanged(float value)
    {
        _currentValue = value / 100;

        // CRIAudioManager に音量を反映
        _criAudioManager?.SetVolume(_soundType, _currentValue);

        // 入力フィールドの更新
        _volumeInputField.text = value.ToString(CultureInfo.CurrentCulture);
    }

    private void OnInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            _currentValue = result / 100;

            // スライダーの値を更新
            _volumeSlider.value = result;

            // CRIAudioManager に音量を反映
            _criAudioManager?.SetVolume(_soundType, _currentValue);
        }
        else
        {
            Debug.LogWarning("入力が無効です。音量は数値である必要があります。");
        }
    }

    private void OnDestroy()
    {
        // イベントリスナーの解除
        _volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
        _volumeInputField.onEndEdit.RemoveListener(OnInputChanged);
    }
}
