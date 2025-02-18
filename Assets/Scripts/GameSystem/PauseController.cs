using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DepthOfField = UnityEngine.Rendering.Universal.DepthOfField;

public class PauseController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _pausePanel;
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private Button _firstSelectedButton;
    private DepthOfField _depthOfField;
    public bool IsPaused { get; set; }
    public bool StopPause { get; set; }
    private void Awake()
    {
        _pausePanel.interactable = false;
        _pausePanel.blocksRaycasts = false;
        _pausePanel.alpha = 0;
        if (_globalVolume.profile.TryGet(out DepthOfField dof))
        {
            _depthOfField = dof;
            _depthOfField.active = false;
        }
    }

    private void Start()
    {
        PlayerInputProvider.Instance.PauseAction.started += OnPause;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
        PlayerInputProvider.Instance.PauseAction.started -= OnPause;
    }

    void OnPause(InputAction.CallbackContext context)
    {
        SwitchPause(!IsPaused);
    }

    public void SwitchPause(bool pause)
    {
        if (StopPause) return;
        _pausePanel.interactable = pause;
        _pausePanel.blocksRaycasts = pause;
        _depthOfField.active = pause;
        IsPaused = pause;
        if (pause)
        {
            _pausePanel.alpha = 1;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            EventSystem.current.SetSelectedGameObject(_firstSelectedButton.gameObject);
        }
        else
        {
            _pausePanel.alpha = 0;
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}