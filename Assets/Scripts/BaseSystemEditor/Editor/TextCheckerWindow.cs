using UnityEngine;
using UnityEditor;
using FoundationUtility;
using System.Collections.Generic;
using TMPro;
using DataManagement;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Data.SqlTypes;
using UnityEngine.UIElements;

public class TextCheckerWindow : EditorWindow
{
    class TextCheck
    {
        public string Name;
        public GameObject ObjRef;
        public string Text;
        public TMPro.TextMeshProUGUI TMPro;
        public LocalizedText LzTxt;
    };

    List<TextCheck> _checkList = new List<TextCheck>();
    Dictionary<string, string> _revTextDic = new Dictionary<string, string>();
    Vector2 _scrollPosition = Vector2.zero;
    Vector2 _scrollPositionTextView = Vector2.zero;

    [MenuItem("VTNTools/TextCheckerWindow")]
    static void CreateWindow()
    {
        if (!MasterData.Instance.IsSetupComplete)
        {
            MasterData.Instance.Setup().Forget();
        }
        TextCheckerWindow window = (TextCheckerWindow)EditorWindow.GetWindow(typeof(TextCheckerWindow));
        window.Show();
        window.Init().Forget();
    }

    async UniTask Init()
    {
        await UniTask.WaitUntil(() => MasterData.Instance.IsSetupComplete);
        Refresh();
    }

    void Refresh()
    {
        if (MasterData.TextMaster != null)
        {
            _revTextDic.Clear();
            foreach (var k in MasterData.TextMaster.GetKeys())
            {
                _revTextDic.Add(MasterData.TextMaster[k], k);
            }
        }

        //TextMeshProがついているコンポーネントをシーンから全権検索する
        var list = GameObject.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        _checkList.Clear();
        foreach (var t in list)
        {
            _checkList.Add(new TextCheck()
            {
                Name = t.gameObject.name,
                ObjRef = t.gameObject,
                Text = t.text,
                TMPro = t,
                LzTxt = t.GetComponent<LocalizedText>()
            });
        }
    }

    void MasterDataRefresh()
    {
        MasterData.Instance.Setup(() =>
        {
            Refresh();
        }).Forget();

    }

    void AddLocalizeComponent()
    {
        foreach (var chk in _checkList)
        {
            if (chk.LzTxt == null)
            {
                Debug.Log($"{chk.Name}にスクリプトをアタッチ");
                chk.LzTxt = chk.ObjRef.AddComponent<LocalizedText>();
                chk.LzTxt.SetComponent(chk.TMPro);
            }

            var txt = chk.LzTxt?.GetKeyName() != "" && _revTextDic.Values.Contains(chk.LzTxt?.GetKeyName());
            if (txt)
            {
                continue;
            }

            var hasText = _revTextDic.ContainsKey(chk.Text);
            if (hasText)
            {
                Debug.Log($"{chk.Name}のローカライズテキストのキーを自動設定");
                chk.LzTxt.SetString(_revTextDic[chk.Text]);
                continue;
            }

            Debug.LogWarning($"{chk.Name}は未対応のまま");
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("TextMeshProがLocalize対応しているかなリスト");
        EditorGUILayout.Space(10);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.Space(10);
            if (GUILayout.Button("更新する", GUILayout.Width(200)))
            {
                Refresh();
            }
            
            for(var i=0; i<5;i++)
                EditorGUILayout.Space(10);

            if (GUILayout.Button("一括対応する", GUILayout.Width(200)))
            {
                AddLocalizeComponent();
            }

            for (var i = 0; i < 5; i++)
                EditorGUILayout.Space(10);

            if (GUILayout.Button("テキストマスタの中身を見る", GUILayout.Width(200)))
            {

            }

            for (var i = 0; i < 5; i++)
                EditorGUILayout.Space(10);

            if (GUILayout.Button("マスタを更新する", GUILayout.Width(200)))
            {
                MasterDataRefresh();
            }
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.Space(10);

        GUIStyle style = GUI.skin.label;
        GUIStyleState styleState = new GUIStyleState();
        styleState.textColor = Color.red;
        style.normal = styleState;

        //テキストリストの描画
        EditorGUILayout.LabelField("TextLists:");
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("名前");
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("オブジェクト");
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("対応済か？");
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("内容のテキスト");
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("マスタ設定？");
            EditorGUILayout.Space(2);
            GUILayout.FlexibleSpace();
        }

        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition, GUILayout.Height(400.0f)))
        {
            foreach (var chk in _checkList)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(chk.Name);
                    EditorGUILayout.Space(2);
                    EditorGUILayout.ObjectField(chk.ObjRef, typeof(GameObject));
                    EditorGUILayout.Space(2);
                    if (chk.LzTxt)
                    {
                        EditorGUILayout.LabelField("対応済");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Localize未対応", style);
                    }
                    EditorGUILayout.Space(2);
                    EditorGUILayout.LabelField(chk.Text);
                    EditorGUILayout.Space(2);
                    var txt = chk.LzTxt?.GetKeyName() != "" && _revTextDic.Values.Contains(chk.LzTxt?.GetKeyName());
                    var hasText = _revTextDic.ContainsKey(chk.Text);
                    if (txt)
                    {
                        EditorGUILayout.LabelField("対応済");
                    }
                    else if (hasText)
                    {
                        EditorGUILayout.LabelField("テキストはある");
                    }
                    else
                    {
                        EditorGUILayout.LabelField("未対応", style);
                    }
                    GUILayout.FlexibleSpace();
                }
            }
            _scrollPosition = scrollView.scrollPosition;
        }

        EditorGUILayout.Space(10);

        //テキストリストの描画
        EditorGUILayout.LabelField("TextMaster:");
        EditorGUILayout.Space(5);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("キー");
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("内容のテキスト");
            EditorGUILayout.Space(2);
            GUILayout.FlexibleSpace();
        }
        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPositionTextView, GUILayout.Height(400.0f)))
        {
            if (MasterData.TextMaster != null)
            {
                foreach (var k in MasterData.TextMaster.GetKeys())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(k);
                        EditorGUILayout.Space(2);
                        EditorGUILayout.LabelField(MasterData.TextMaster[k]);
                        EditorGUILayout.Space(2);
                        GUILayout.FlexibleSpace();
                    }
                }
            }
            _scrollPositionTextView = scrollView.scrollPosition;
        }
    }
}