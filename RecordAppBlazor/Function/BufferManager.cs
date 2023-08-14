using System;
using System.IO;
using System.Linq;
using System.Threading;
using RecordAppBlazor.Data;

namespace RecordAppBlazor.Function;

public static class BufferManager
{
    public static bool isBuffering = false;
    public static bool isInitialized = false;
    public static string StartBuffer()
    {
        if (File.Exists($"{Environment.GetEnvironmentVariable("HOME")}/record.lock"))
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
        var p = $"{Environment.GetEnvironmentVariable("HOME")}/Records";

        if (files.Length == 1)
        {
            File.Copy(files[0], $"{p}/{code}.mkv");
        }
        else
        {
            return "内部错误, 请联系管理员";
        }
        Console.Out.WriteLine("copy file success");
        RecordingService.AddRecording(new Recording()
        {
            Code = code,
            Path =  $"{p}/{code}.mkv"
        });
        
        return $"请使用 {rawCode} 来下载调取的录像";
    }
}