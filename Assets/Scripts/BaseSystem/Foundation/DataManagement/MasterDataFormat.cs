using Cysharp.Threading.Tasks;
using DataManagement.SpreadSheet;
using SerializableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ゲーム内で使用するデータたち
/// </summary>
namespace DataManagement
{
    /// <summary>
    /// システム用
    /// </summary>
    public interface IMasterData
    {
        string MasterName { get; }
    }

    public abstract class MasterDataBase<K, V> : IMasterData
    {
        public class DataDic : SerializableDictionary<K, V> { }

        protected DataDic _dic = new DataDic();
        
        public abstract string MasterName { get; }

        public V this[K id] => _dic.ContainsKey(id) ? _dic[id] : default;

        public abstract UniTask Marshal();

        /// <summary>
        /// ファイルからの読み込み
        /// </summary>
        /// <param name="masterName">マスタ名(省略可)</param>
        /// <returns></returns>
        protected async UniTask<SerializableDictionary<K, V>> LoadFromFile(string masterName = "default")
        {
            if(masterName == "default")
            {
                masterName = MasterName;
            }
            return await LocalData.LoadAsync<SerializableDictionary<K, V>>(MasterData.GetFileName(masterName));
        }

        /// <summary>
        /// データのシンプルな整形
        /// </summary>
        protected void pretty<T>(T[] data, Func<T, (K, V)> mapper)
        {
            foreach (var d in data)
            {
                var kv = mapper.Invoke(d);
                if (_dic.ContainsKey(kv.Item1))
                {
                    Debug.Log($"duplicate key:{kv.Item1}");
                    continue;
                }
                _dic.Add(kv.Item1, kv.Item2);
            }
        }

#if UNITY_EDITOR
        public K[] GetKeys()
        {
            return _dic.Keys.ToArray();
        }
#endif
    }



    /// <summary>
    /// テキストマスタ
    /// </summary>
    [Serializable]
    public class TextMaster : MasterDataBase<string, string>
    {
        public override string MasterName => "TextMaster";

        //public string this[string key] => _dic.ContainsKey(id) ? _dic[id] : default;

        public override async UniTask Marshal()
        {
            //テキストマスタを設定する
            //日本語を使う
            //TODO: 言語設定を見る

            // マスタ読み込み処理
            var text = await MasterData.LoadMasterData<SpreadSheet.TextMaster>("JP_Text");

            // 整形処理
            pretty(text.Data, (SpreadSheet.TextData data) => { return (data.Key, data.Text); });
        }
    }

    /// <summary>
    /// 敵マスタ
    /// </summary>
    [Serializable]
    public class EnemyMaster : MasterDataBase<int, EnemyMaster.EnemyData>
    {
        public override string MasterName => "EnemyMaster";

        /// <summary>
        /// スキルのデータ
        /// </summary>
        [Serializable]
        public class EnemyData
        {
            public int Id;
            public string Name;
            public string ResourceName;
            public SkillData Skill;

            public EnemyData(SpreadSheet.EnemyData data)
            {
                Id = data.Id;
                Name = data.Name;
                ResourceName = data.ResourceName;
            }
        }

        /// <summary>
        /// スキルのデータ
        /// </summary>
        [Serializable]
        public class SkillData
        {
            public int Id;
            public string Text;
        }

        public override async UniTask Marshal()
        {
            SpreadSheet.EnemyMaster enemy = default;
            SpreadSheet.SkillMaster skill = default;
            List<UniTask> masterDataDownloads = new List<UniTask>()
            {
                MasterData.LoadMasterData("Enemy", (SpreadSheet.EnemyMaster data) => { enemy = data; }),
                MasterData.LoadMasterData("Skill", (SpreadSheet.SkillMaster data) => { skill = data; })
            };
            await masterDataDownloads;

            // 整形処理
            pretty(enemy.Data, (SpreadSheet.EnemyData data) => { return (data.Id, new EnemyData(data)); });
        }
    }
}