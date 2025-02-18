using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyCameraTransition : MonoBehaviour
{
    [SerializeField] private CinemachineBrain _cinemachineBrain;
    [SerializeField] private CinemachineVirtualCamera _releasePointCamera;
    [SerializeField] private CinemachineVirtualCamera _killPointCamera;
    [SerializeField, InspectorVariantName("弱点を見る時間")] private float _viewTime = 0.25f;
    [SerializeField] private PauseController _pauseController;
    
    public async UniTaskVoid ViewFinishPoints()
    {
        _pauseController.StopPause = true;
        Time.timeScale = 0;
        _releasePointCamera.Priority = 999;
        await UniTask.WaitForSeconds(_cinemachineBrain.m_DefaultBlend.BlendTime + _viewTime, true, cancellationToken: destroyCancellationToken);
        _releasePointCamera.Priority = -1;
        _killPointCamera.Priority = 999;
        await UniTask.WaitForSeconds(_cinemachineBrain.m_DefaultBlend.BlendTime + _viewTime, true, cancellationToken: destroyCancellationToken);
        _killPointCamera.Priority = -1;
        Time.timeScale = 1;
        _cinemachineBrain.m_DefaultBlend.m_Time = 0;
        await UniTask.Yield(destroyCancellationToken);
        _cinemachineBrain.m_DefaultBlend.m_Time = 2;
        _pauseController.StopPause = false;
    } 
}