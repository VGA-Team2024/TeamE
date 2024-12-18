using System.Collections.Generic;
using UnityEngine;
using CommonSystem.Option;

public class OptionPresenter : MonoBehaviour
{
    [SerializeField] private CriControlPresenter _criControlPresenter;
    [SerializeField] private OptionView _optionView;
    private OptionModel _optionModel;

    private void Start()
    {
        _optionModel = new OptionModel();

        // 設定の初期化
        _optionModel.LoadSettings();

        // オプション画面の初期化
        InitializeOptionSettings();

        _optionView.Initialize(CloseOptionPanel);
    }

    /// <summary>
    /// オプション画面の設定を初期化します。
    /// </summary>
    private void InitializeOptionSettings()
    {
        // CriControlPresenterを介して音量の初期値を設定
        var initialVolumes = new Dictionary<SoundType, float>
        {
            { SoundType.MASTER, _optionModel.MasterVolume },
            { SoundType.BGM, _optionModel.BGMVolume },
            { SoundType.SE, _optionModel.SEVolume },
            { SoundType.VOICE, _optionModel.SEVolume }
        };

        _criControlPresenter.Initialize(initialVolumes);
    }

    /// <summary>
    /// 設定を保存します。
    /// </summary>
    public void SaveSettings()
    {
        // サウンド設定を保存
        var currentVolumes = _criControlPresenter.GetCurrentVolumes();
        _optionModel.MasterVolume = currentVolumes[SoundType.MASTER];
        _optionModel.BGMVolume = currentVolumes[SoundType.BGM];
        _optionModel.SEVolume = currentVolumes[SoundType.SE];
        _optionModel.SaveSettings();
    }

    /// <summary>
    /// 設定をリセットします。
    /// </summary>
    public void ResetSettings()
    {
        _optionModel.ResetToDefault();
        InitializeOptionSettings();
    }

    /// <summary>
    /// オプションパネルを開きます。
    /// </summary>
    public void OpenOptionPanel()
    {
        _optionView.ShowOptionPanel();
    }

    /// <summary>
    /// オプションパネルを閉じます。
    /// </summary>
    public void CloseOptionPanel()
    {
        _optionView.HideOptionPanel();
    }
}