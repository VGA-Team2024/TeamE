using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// レビューダイアログ
/// </summary>
public class ReviewWindow : MonoBehaviour
{
    [SerializeField] InputField _textField;
    [SerializeField] Button _sendButton;
    [SerializeField] GameObject _grayout;
    [SerializeField] TextMeshProUGUI _sendTxt;

    [SerializeField] ReviewStar[] _starts;
    GameEventRecorder _refEvent;
    Action _sendCallback;
    PlayerInput _playerInput;
    bool storedInput;
    int _currentStar = 0;


    void Setup()
    {
        //ちょっと重いけど
        _playerInput = FindObjectOfType<PlayerInput>();
        if (_playerInput)
        {
            storedInput = _playerInput.enabled;
            _playerInput.enabled = false;
        }

        //_starts = GetComponentsInChildren<ReviewStar>();
        _sendButton.enabled = false;
        foreach (var s in _starts)
        {
            int index = s.Index;
            s.Setup();
            var evt = s.gameObject.AddComponent<EventTrigger>();

            /*
            var hover = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            hover.callback.AddListener((d) => SetStar(index));
            evt.triggers.Add(hover);
            */

            Button btn = s.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                SetStar(index);
            });
        }
    }

    void SetStar(int index)
    {
        foreach (var s in _starts)
        {
            s.SetStar(s.Index <= index);
        }
        _currentStar = index;

        UpdateSendButton();
    }

    void UpdateSendButton()
    {
        if (_currentStar > 0 && !_sendButton.enabled)
        {
            _sendButton.enabled = true;
            _sendButton.onClick.AddListener(SendReview);
            _grayout.SetActive(false);
        }
    }

    async void SendReview()
    {
        int starNum = _currentStar;
        string reviewtext = _textField.text;
        _sendTxt.text = "送信中";
        await _refEvent.SendReview(starNum, reviewtext);
        _sendTxt.text = "送信完了！";
        _sendCallback?.Invoke();

        if (_playerInput)
        {
            _playerInput.enabled = storedInput;
        }

        Destroy(this.gameObject);
    }


    //static

    static public ReviewWindow Build(GameEventRecorder refEvent, Action callback)
    {
        var obj = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Review/Review.prefab").WaitForCompletion();
        var newObj = Utility.Instantiate(obj);
        Addressables.Release(obj);
        ReviewWindow review = newObj.GetComponent<ReviewWindow>();
        review._refEvent = refEvent;
        review._sendCallback = callback;
        review.Setup();

        return review;
    }
}