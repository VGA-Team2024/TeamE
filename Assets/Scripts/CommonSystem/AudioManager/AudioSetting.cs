using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HikanyanLaboratory.Audio
{
    [CreateAssetMenu(fileName = "Cri Audio Setting", menuName = "Audio Setting")]
    public class AudioSetting : ScriptableObject
    {
        [SerializeField] private AssetReference _acfAsset; // ACFファイルのAddressables参照
        [SerializeField] private List<TextAsset> _cueSheets; // ACB, AWBファイルのリスト
    }
}