using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Keeps track of button counts though http client.

public class ClientButtonTracker : MonoBehaviour
{
    #region Counter structures
    // Counter for a single button with one ID: this is the format used on the http server. Used for response deserialization.
    [Serializable]
    public struct SingleButtonCount
    {
        [JsonPropertyName("name")] public string ButtonID { get => buttonID; set => buttonID = value; }
        [JsonPropertyName("count")] public int InputCount { get => count; set => count = value; }

        private string buttonID;
        private int count;
    }

    // Counter for multiple buttons at one certain time. Used to store and process the response.
    public struct MultipleButtonTimedCount
    {
        public float time;
        public Dictionary<string, int> counts;

        public int this[string k]
        {
            get
            {
                if (counts == null || counts.ContainsKey(k) == false) return 0;
                else return counts[k];
            }
        }

        public MultipleButtonTimedCount(float time, SingleButtonCount[] data)
        {
            this.time = time;
            int dataCount = data != null ? data.Length : 0;
            counts = new Dictionary<string, int>(dataCount);
            for (int i = 0; i < dataCount; i++)
            {
                SingleButtonCount d = data[i];
                if (counts.ContainsKey(d.ButtonID)) counts[d.ButtonID] += d.InputCount;
                else counts.Add(d.ButtonID, d.InputCount);
            }
        }

        static public int CompareByAge(MultipleButtonTimedCount a, MultipleButtonTimedCount b) => b.time.CompareTo(a.time);

        public void AddPresses(Dictionary<string, int> presses)
        {
            if (presses == null) return;
            if (counts == null)
            {
                counts = new(presses);
                return;
            }
            foreach (string key in presses.Keys)
            {
                if (counts.ContainsKey(key)) counts[key] += presses[key];
                else counts.Add(key, presses[key]);
            }
        }
    }
    #endregion

    public HttpRequestLoop requestLoop;

    public UnityEvent<MultipleButtonTimedCount> onCountUpdate;

    public MultipleButtonTimedCount Current { get; private set; }

    private void Awake() => requestLoop?.Init();
    private void OnEnable() => AddRequestListeners();
    private void OnDisable() => RemoveRequestListeners();
    private void Update() => requestLoop?.Update();

    public string GetLog()
    {
        string logText = requestLoop != null ? requestLoop.GetLog() : "Error : button request is NULL";
        foreach (KeyValuePair<string,int> b in Current.counts)
            logText += "\n- " + b.Key + ": " + b.Value;
        return logText;
    }
    
    private void AddRequestListeners()
    {
        if (requestLoop == null) return;
        requestLoop.onRequestEnd.AddListener(OnRequestEnd);
    }

    private void RemoveRequestListeners()
    {
        if (requestLoop == null) return;
        requestLoop.onRequestEnd.RemoveListener(OnRequestEnd);
    }

    private void OnRequestEnd(HttpRequest buttonRequest)
    {
        SingleButtonCount[] data = buttonRequest.DeserializeResponse<SingleButtonCount[]>();
        Current = new MultipleButtonTimedCount(buttonRequest.EndTime, data);
        onCountUpdate.Invoke(Current);
    }
}