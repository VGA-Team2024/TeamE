using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// ゲーム内イベントを管理するクラス
    /// </summary>
    public static class GameEventManager
    {
        private static readonly Queue<string> eventQueue = new Queue<string>(); // イベントキュー

        /// <summary>
        /// イベントをキューに追加します。
        /// </summary>
        /// <param name="eventName">追加するイベント名</param>
        public static void AddEvent(string eventName)
        {
            eventQueue.Enqueue(eventName);
            Debug.Log($"Event Added: {eventName}");
        }

        /// <summary>
        /// 次のイベントを処理します。
        /// </summary>
        public static void ProcessNextEvent()
        {
            if (eventQueue.Count > 0)
            {
                string nextEvent = eventQueue.Dequeue();
                Debug.Log($"Processing Event: {nextEvent}");
                GameManager.HandleEvent(nextEvent);
            }
        }

        /// <summary>
        /// 全イベントをクリアします。
        /// </summary>
        public static void ClearEvents()
        {
            eventQueue.Clear();
            Debug.Log("All Events Cleared.");
        }
    }
}