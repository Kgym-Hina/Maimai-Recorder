using System;
using System.IO;
using System.Linq;
using System.Threading;
using RecordAppBlazor.Data;
using RecordAppBlazor.Pages;

namespace RecordAppBlazor.Function;

public static class BufferManager
{
    public static bool isBuffering = false;
    public static bool isInitialized = false;
    public static string StartBuffer()
    {
        if (File.Exists(PropertiesManager.Properties.RecordLockPath))
        {
            return "当前正在录制中, 缓存已关闭";
        }
        
        isBuffering = true;
        
        Obs.client.StartReplayBuffer();
        return "200";
    }
    
    public static string StopBuffer()
    {
        isBuffering = false;
        Obs.client.StopReplayBuffer();
        return "200";
    }

    public static string SaveBuffer()
    {
        if (!isBuffering)
        {
            return "无缓存";
        }
        
        // 清空目录
        var path = Obs.client.GetRecordDirectory();
        Console.Out.WriteLine("path: " + path);
        var filesToDelete = Directory.GetFiles(path);

        if (filesToDelete.Length > 0)
        {
            filesToDelete.ToList().ForEach(File.Delete);
        }
        
        Obs.client.SaveReplayBuffer();
        
        Thread.Sleep(1000);
        
        // 生成随机code
        var rawCode = Guid.NewGuid().ToString();
        var code = rawCode.ToMD5();
        
        // 复制文件
        var files = Directory.GetFiles(path);
        Console.Out.WriteLine("files count: " + files.Length);
        var p = PropertiesManager.Properties.TempRecordPath;

        // 上次文件没有清空的情况
        if (files.Length != 1)
        {
            return "内部错误, 请联系管理员";
        }
        
        File.Copy(files[0], $"{p}/{code}.mkv");

        var recording = new Recording()
        {
            Code = code,
            Path = $"{p}/{code}.mkv",
            Status = FileStatus.Uploading
        };
        
        // 添加到列表
        RecordingService.AddRecording(recording);

        // 上传
        Upload.UploadFile($"{p}/{code}.mkv", code);
        
        // 重置进程锁
        File.Delete(PropertiesManager.Properties.RecordLockPath);
        
        // 更新前台状态
        RecordingService.AllRecordings.First(a => a.Code == code).Status = FileStatus.Ready;
        
        return $"请使用 {rawCode} 来下载调取的录像";
    }
}