using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoneContainer : MonoBehaviour
{
    [SerializeField] private Transform[] _bones;

    public Transform[] Bones
    {
        get => _bones;
        set => _bones = value;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(BoneContainer))]
public class BoneContainerEditor : Editor
{
    private BoneContainer _self;

    private void Awake()
    {
        _self = target as BoneContainer;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("全ての子オブジェクトを取得"))
        {
            _self.Bones = _self.transform.GetComponentsInChildren<Transform>();
        }
    }
}
#endif
