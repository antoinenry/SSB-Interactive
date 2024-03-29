using System;
using System.Collections.Generic;

[Serializable] 
public class ButtonCounter
{
    private struct Capture
    {
        public float time;
        public Dictionary<string, int> totalPresses;

        public Capture(float time, ButtonCountData[] data)
        {
            this.time = time;
            int dataCount = data != null ? data.Length : 0;
            totalPresses = new Dictionary<string, int>(dataCount);
            for (int i = 0; i < dataCount; i++)
            {
                ButtonCountData d = data[i];
                if (totalPresses.ContainsKey(d.ButtonID)) totalPresses[d.ButtonID] += d.InputCount;
                else totalPresses.Add(d.ButtonID, d.InputCount);
            }
        }

        static public int CompareByAge(Capture a, Capture b) => b.time.CompareTo(a.time);

        public void AddPresses(Dictionary<string, int> presses)
        {
            if (presses == null) return;
            if (totalPresses == null)
            {
                totalPresses = new(presses);
                return;
            }
            foreach (string key in presses.Keys)
            {
                if (totalPresses.ContainsKey(key)) totalPresses[key] += presses[key];
                else totalPresses.Add(key, presses[key]);
            }
        }
    }

    private List<Capture> captures;

    private void Add(Capture c)
    {
        if (captures == null) captures = new List<Capture>();
        int frameIndex = captures.FindIndex(f => f.time == c.time);
        if (frameIndex != -1)
        {
            c.AddPresses(captures[frameIndex].totalPresses);
            captures[frameIndex] = c;
        }
        else
        {
            captures.Add(c);
            captures.Sort(Capture.CompareByAge);
        }
    }

    public void Add(float time, ButtonCountData[] data) => Add(new(time, data));

    public void Add(float time, string json) => Add(time, ButtonCountData.Deserialize(json));

    public void ClearCapturesBefore(float time) => captures?.RemoveAll(f => f.time < time);

    public List<ButtonTimeSpawnData> GetButtonCounts(float fromTime, float toTime)
    {
        List<ButtonTimeSpawnData> getButtonCounts = new List<ButtonTimeSpawnData>();
        if (captures == null) return getButtonCounts;
        List<Capture> timedCaptures = captures.FindAll(f => f.time >= fromTime && f.time <= toTime);
        foreach(Capture c in timedCaptures)
        {
            if (c.totalPresses == null) continue;
            foreach(string buttonID in c.totalPresses.Keys)
            {
                int buttonIndex = getButtonCounts.FindIndex(b => b.buttonID == buttonID);
                if (buttonIndex == -1)
                {
                    getButtonCounts.Add(new(buttonID, c.time, c.totalPresses[buttonID]));
                }
                else
                {
                    ButtonTimeSpawnData delta = getButtonCounts[buttonIndex];
                    delta.AddCountAtTime(c.totalPresses[buttonID], c.time);
                    getButtonCounts[buttonIndex] = delta;
                }
            }
        }
        return getButtonCounts;
    }
}