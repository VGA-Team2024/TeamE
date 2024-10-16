using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HikanyanLaboratory.URP
{
    public class BloomControl : VolumeProfileControlBase<Bloom>
    {
        [SerializeField] float _intensity = 1.0f;


        protected override void OnValidateEvent(Bloom bloom)
        {
            // エディタで変更があった時の処理
            bloom.intensity.value = _intensity;
        }

        protected override void UpdateEvent()
        {
            if (TargetComponent != null)
            {
                TargetComponent.intensity.value = _intensity;
            }
        }
    }
}