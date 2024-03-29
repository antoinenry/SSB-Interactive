using System;
using System.Collections.Generic;

[Serializable] 
public class ButtonCounter
{
    private List<ButtonCountFrame> frames;

    public void AddFrame(ButtonCountFrame frame)
    {
        if (frames == null) frames = new List<ButtonCountFrame>();
        int frameIndex = frames.FindIndex(f => f.time == frame.time);
        if (frameIndex != -1)
        {
            frame.AddPresses(frames[frameIndex].totalPresses);
            frames[frameIndex] = frame;
        }
        else
        {
            frames.Add(frame);
            frames.Sort(ButtonCountFrame.CompareByAge);
        }
    }

    public void AddFrame(float time, ButtonCountData[] data) => AddFrame(new(time, data));

    public void AddFrame(float time, string json) => AddFrame(time, ButtonCountData.Deserialize(json));

    public void RemoveAllFramesBefore(float time) => frames?.RemoveAll(f => f.time < time);

    public List<ButtonCountDelta> GetButtonCounts(float fromTime, float toTime)
    {
        List<ButtonCountDelta> getButtonCounts = new List<ButtonCountDelta>();
        if (frames == null) return getButtonCounts;
        List<ButtonCountFrame> getFrames = frames.FindAll(f => f.time >= fromTime && f.time <= toTime);
        foreach(ButtonCountFrame frame in getFrames)
        {
            if (frame.totalPresses == null) continue;
            foreach(string buttonID in frame.totalPresses.Keys)
            {
                int buttonIndex = getButtonCounts.FindIndex(b => b.buttonID == buttonID);
                if (buttonIndex == -1)
                {
                    getButtonCounts.Add(new(buttonID, frame.time, frame.totalPresses[buttonID]));
                }
                else
                {
                    ButtonCountDelta delta = getButtonCounts[buttonIndex];
                    delta.AddFrame(frame);
                    getButtonCounts[buttonIndex] = delta;
                }
            }
        }
        return getButtonCounts;
    }
}