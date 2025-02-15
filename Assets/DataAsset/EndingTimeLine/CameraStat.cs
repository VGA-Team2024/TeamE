using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStat : MonoBehaviour
{
   CinemachineVirtualCamera _virtualCamera;
   public CinemachineBasicMultiChannelPerlin CinemachineBasicMultiChannelPerlin;

    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
}
