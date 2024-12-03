using UnityEngine;


/// <summary>
/// ボリュームコントロールの設定クラス
/// </summary>
[System.Serializable]
public class VolumeControlSettings
{
    public string Label;       // UIに表示するラベル
   [SerializeField,Range(0,100)] public float InitialValue; // 初期音量（0.0～1.0）
    public SoundType SoundType; // 対応するサウンドタイプ
    public string CueSheet;   // キューシート名
    public string CueName;    // キュー名
}