namespace GameSystem
{
    /// <summary>
    /// ゲーム管理クラス
    /// </summary>
    public static class GameManager
    {
        // セーブ/ロード関連
        /// <summary>セーブデータをロードします。</summary>
        public static void LoadSaveData()
        {
        }

        /// <summary>セーブデータを保存します。</summary>
        public static void SaveData()
        {
        }

        // ゲーム進行関連
        /// <summary>ゲームを初期化して開始します。</summary>
        public static void StartGame()
        {
            // 最初のタイムラインを再生して、Playerを操作可能にする
        }

        /// <summary>ゲームを終了します。</summary>
        public static void EndGame()
        {
            // タイトルに戻る
        }

        /// <summary>ゲームを一時停止します。</summary>
        public static void PauseGame()
        {
            // ポーズ画面を表示して、Playerを操作不可能にする。タイムスケールを0にする
        }

        /// <summary>ゲームを再開します。</summary>
        public static void ResumeGame()
        {
            // ポーズ画面を非表示にして、Playerを操作可能にする。タイムスケールを1にする
        }

        /// <summary>ゲームオーバー処理を実行します。</summary>
        public static void GameOver()
        {
            // ゲームオーバー画面を表示して、Playerを操作不可能にする
            // ゲームオーバー画面でリトライボタンを押したら、ゲームをリスタートする
        }

        /// <summary>ゲームクリア処理を実行します。</summary>
        public static void GameClear()
        {
            // ゲームクリア画面を表示して、Playerを操作不可能にする
            // Clearした時の演出を再生する
        }
    }
}