using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HikanyanLaboratory.URP
{
    public class BloomControl : VolumeProfileControlBase<Bloom>
    {
        [SerializeField] float _intensity = 1.0f;

        public override void OnInitialize()
        {
            
        }

        public override void OnValidateEvent(Bloom bloom)
        {
            // エディタで変更があった時の処理
            bloom.intensity.value = _intensity;
        }

        public override void UpdateEvent()
        {
            // ProfileからBloomコンポーネントを取得
            // if (_volume.profile.TryGet<Bloom>(out var bloom))
            // {
            //     // Bloomの強度をシリアライズフィールドの_intensityに基づいて更新
            //     bloom.intensity.value = _intensity;
            // }
        }
    }
}