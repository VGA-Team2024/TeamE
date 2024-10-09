using UnityEngine;
using UnityEngine.Rendering;

namespace HikanyanLaboratory.URP
{
    [ExecuteAlways, RequireComponent(typeof(Volume))]
    public abstract class VolumeProfileControlBase<T> : MonoBehaviour where T : VolumeComponent
    {
        private Volume _volume;

        private void OnEnable()
        {
            InitializeVolume();
        }

        private void OnValidate()
        {
            InitializeVolume();
            ValidateComponents();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ValidateComponents();
            }
#endif
            UpdateEvent();
        }

        private void InitializeVolume()
        {
            if (_volume == null)
                _volume = GetComponent<Volume>();
        }

        private void ValidateComponents()
        {
            if (_volume == null || _volume.profile == null)
                return;

            foreach (var item in _volume.profile.components)
            {
                if (item is T itemComponent)
                {
                    OnValidateEvent(itemComponent);
                }
            }
        }

        public abstract void OnInitialize();

        /// <summary>
        /// エディター上で変更があったときに呼ばれる
        /// </summary>
        /// <param name="itemComponent"></param>
        public abstract void OnValidateEvent(T itemComponent);

        /// <summary>
        /// 再生中に呼ばれる
        /// </summary>
        public abstract void UpdateEvent();
    }
}