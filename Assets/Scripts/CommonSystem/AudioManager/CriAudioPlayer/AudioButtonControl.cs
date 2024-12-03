using UnityEngine;
using UnityEngine.UI;

public class AudioButtonControl : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _stopButton;
    [SerializeField] private Toggle _loopingToggle;

    private CRIAudioManager _criAudioManager;
    private SoundType _soundType;
    private bool _isLoop;
    private string _cueSheet;
    private string _cueName;

    private void Awake()
    {
        // CRIAudioManager のインスタンスを取得
        _criAudioManager = CRIAudioManager.Instance;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(SoundType soundType, string cueSheet, string cueName)
    {
        _soundType = soundType;
        _loopingToggle.isOn = false;
        _isLoop = false;

        // ボタンとトグルのイベント登録
        _loopingToggle.onValueChanged.AddListener(Loop);
        _startButton.onClick.AddListener(Play);
        _pauseButton.onClick.AddListener(Pause);
        _resumeButton.onClick.AddListener(Resume);
        _stopButton.onClick.AddListener(Stop);

        SetCueSheet(cueSheet, cueName);
    }

    public void SetCueSheet(string cueSheet, string cueName)
    {
        _cueSheet = cueSheet;
        _cueName = cueName;
    }

    /// <summary>
    /// 再生
    /// </summary>
    public void Play()
    {
        // サウンドを再生 (プレイヤーにループ設定を適用)
        var player = GetPlayer();
        if (player == null) return;
        player.SetLoop(_isLoop);
        player.Play(_cueSheet, _cueName);
    }

    /// <summary>
    /// 一時停止
    /// </summary>
    public void Pause()
    {
        var player = GetPlayer();
        player?.Pause();
    }

    /// <summary>
    /// 再開
    /// </summary>
    public void Resume()
    {
        var player = GetPlayer();
        player?.Resume();
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        var player = GetPlayer();
        player?.Stop();
    }

    /// <summary>
    /// ループ設定
    /// </summary>
    /// <param name="isLoop">ループの有効/無効</param>
    public void Loop(bool isLoop)
    {
        _isLoop = isLoop;
        Debug.Log($"Loop: {_isLoop}");
    }

    /// <summary>
    /// サウンドプレイヤーを取得
    /// </summary>
    /// <returns>対応するサウンドプレイヤー</returns>
    private CRIAudioManager.SoundPlayer GetPlayer()
    {
        switch (_soundType)
        {
            case SoundType.BGM:
                return CRIAudioManager.BGM;
            case SoundType.SE:
                return CRIAudioManager.SE;
            case SoundType.VOICE:
                return CRIAudioManager.VOICE;
            default:
                Debug.LogWarning("Invalid SoundType");
                return null;
        }
    }
}