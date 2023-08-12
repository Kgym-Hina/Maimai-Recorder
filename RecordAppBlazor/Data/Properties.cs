namespace RecordAppBlazor.Data;

public static class Properties
{
    public static TimeSpan RecordTimeSpan = TimeSpan.FromMinutes(0.1); // 录制的时长
    public static TimeOnly StopTime = new(21, 30, 00); // 停止增加录制的时间
}