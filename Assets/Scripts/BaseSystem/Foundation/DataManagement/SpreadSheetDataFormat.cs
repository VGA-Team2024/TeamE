using System;

/// <summary>
/// スプレッドシートからダウンロードしてくるデータたち
/// </summary>
namespace DataManagement
{
    /// <summary>
    /// システム用
    /// </summary>
    [Serializable]
    public class SpreadSheetMasterVersion
    {
        public string SheetName;
        public int Version;
    }

    [Serializable]
    public class SpreadSheetDataObject
    {
        public int Version;
        //xxx[] Data;
    }

    [Serializable]
    public class MasterDataVersion
    {
        public long TimeStamp;
        public SpreadSheetMasterVersion[] Data;
    }

    namespace SpreadSheet
    {
        /// <summary>
        /// テキストデータ
        /// </summary>
        [Serializable]
        public class TextData
        {
            public string Key;
            public string Text;
        }

        [Serializable]
        public class TextMaster : SpreadSheetDataObject
        {
            public TextData[] Data;
        }

        /// <summary>
        /// サンプルの敵データ
        /// </summary>
        [Serializable]
        public class EnemyData
        {
            public int Id;
            public string Name;
            public string ResourceName;
            public int SkillId;
        }

        [Serializable]
        public class EnemyMaster : SpreadSheetDataObject
        {
            public EnemyData[] Data;
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

        [Serializable]
        public class SkillMaster : SpreadSheetDataObject
        {
            public SkillData[] Data;
        }
        //
    }
}