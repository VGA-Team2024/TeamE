using UnityEngine;
using UnityEngine.Rendering;

namespace TeamE.URP
{
    [ExecuteAlways, RequireComponent(typeof(Volume))]
    public abstract class VolumeProfileControlBase<T> : MonoBehaviour where T : VolumeComponent
    {
        private Volume _volume;
        protected T TargetComponent;

        private void OnEnable()
        {
            InitializeVolume();
            UpdateEvent();
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

            if (_volume != null && _volume.profile != null)
            {
                // VolumeProfileから指定された型のコンポーネントを取得または追加
                TargetComponent = FindOrCreateVolumeComponent(_volume.profile);
            }
        }

        private static T FindOrCreateVolumeComponent(VolumeProfile profile)
        {
            // 指定されたコンポーネントが存在するか確認
            foreach (var component in profile.components)
            {
                if (component is T targetComponent)
                {
                    return targetComponent;
                }
            }

            // 存在しない場合は新規に追加
            T newComponent = profile.Add<T>(true);
            return newComponent;
        }

        private void ValidateComponents()
        {
            if (_volume == null || _volume.profile == null)
                return;

            TargetComponent = FindOrCreateVolumeComponent(_volume.profile);
            if (TargetComponent != null)
            {
                OnValidateEvent(TargetComponent);
            }
        }

        /// <summary>
        /// エディター上で変更があったときに呼ばれる
        /// </summary>
        /// <param name="itemComponent"></param>
        protected abstract void OnValidateEvent(T itemComponent);

        /// <summary>
        /// 再生中に呼ばれる
        /// </summary>
        protected abstract void UpdateEvent();
    }
}
