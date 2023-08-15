using System;
using System.IO;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace RecordAppBlazor.Data;

public class Properties
{
    public TimeSpan RecordTimeSpan = TimeSpan.FromMinutes(2.5); // 录制的时长
    public TimeOnly StopTime = new(21, 45, 00); // 停止增加录制的时间
    public string FileServerRoot;
    public string OBSWebsocketAddress = "ws://localhost:4455";
    public string OBSWebsocketPassword = "";
    public string TempRecordPath = $"{Environment.GetEnvironmentVariable("HOME")}/Records/";
    public string RecordLockPath = $"{Environment.GetEnvironmentVariable("HOME")}/record.lock";
    public bool AboutMessageVisibility = true;
    public MarkupString CustomAboutMessage =
        new MarkupString("<p>硬件采购/配置: <a href=\"https://space.bilibili.com/323737932\">Raid10没有1@Bilibili</a> </p>\n    <p>感谢城市英雄店的大力支持</p>");
    public bool ShowExperimentalMessage = false;
    public string SystemTitle = "街机游戏录制系统";
    public string SystemSubtitle = "Arcade Game Recording System-Z";
    public bool PlayersCanStopRecording = true;
    public bool PlayersCanStartRecording = true;
    public string RecordingSceneName = "Recording";
    public string NormalSceneName = "Game";
}

public static class PropertiesManager
{
    public static Properties Properties;

    public static void Initialize()
    {
        var path = $"{Environment.GetEnvironmentVariable("HOME")}/recordingConfig.json";
        if (File.Exists(path))
        {
            var fileContent = File.ReadAllText(path);

            Properties = JsonConvert.DeserializeObject<Properties>(fileContent);
            Console.Out.WriteLine(JsonConvert.SerializeObject(Properties));
        }
        else
        {
            Properties = new Properties();
            
            File.WriteAllText(path, JsonConvert.SerializeObject(Properties));
        }
    }
}