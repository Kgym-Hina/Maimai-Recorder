using System;
using System.IO;
using System.Linq;
using System.Threading;
using RecordAppBlazor.Data;
using RecordAppBlazor.Pages;

namespace RecordAppBlazor.Function;

public static class RecordManager
{
    static RecordManager()
    {
        // 确保目录存在
        var p = PropertiesManager.Properties.TempRecordPath;
        var e = Directory.Exists(p);
        if (!e)
        {
            Directory.CreateDirectory(p);
        }
        
        
        
        // 删除上次的进程锁
        if (File.Exists(PropertiesManager.Properties.RecordLockPath))
        {
            File.Delete(PropertiesManager.Properties.RecordLockPath);
        }
        
        // 删除上次的文件
        var files = Directory.GetFiles(p);
        if (files.Length > 0)
        {
            files.ToList().ForEach(File.Delete);
        }
    }
    
    public static string StartRecording(string? code)
    {
        BufferManager.StopBuffer();
        
        code = code.ToMD5();
        Console.Out.WriteLine("start code: " + code);
        if (string.IsNullOrWhiteSpace(code))
        {
            return "不能为空";
        }

        if (RecordingService.GetAllRecordings().Any(r => r.Code == code))
        {
            return "此code已经被使用";
        }
        
        // 在指定时间后停止录制
        var timeNow = DateTime.Now;
        if (timeNow.Hour >= PropertiesManager.Properties.StopTime.Hour && timeNow.Minute >= PropertiesManager.Properties.StopTime.Minute)
        {
            return "今日的招募已经结束~";
        }

        // 写入进程锁
        File.WriteAllText(PropertiesManager.Properties.RecordLockPath,
            $"{code}::{DateTime.Now.Add(PropertiesManager.Properties.RecordTimeSpan)}");
        
        // 清空目录
        var path = Obs.client.GetRecordDirectory();
        Console.Out.WriteLine("path: " + path);
        var files = Directory.GetFiles(path);

        if (files.Length > 0)
        {
            files.ToList().ForEach(File.Delete);
        }
        
        // 开始录制
        Obs.client.SetCurrentProgramScene(PropertiesManager.Properties.RecordingSceneName);
        Obs.client.StartRecord();

        return "200";
    }
    
    public static string StopRecording(string? code)
    {
        Console.Out.WriteLine("stop code: " + code);
        

        code = code.ToMD5();

        // 停止录制
        Obs.client.StopRecord();
        
        // 等待OBS处理
        Thread.Sleep(2000);
        
        // 获取文件
        var path = Obs.client.GetRecordDirectory();
        var files = Directory.GetFiles(path);
        Console.Out.WriteLine("files count: " + files.Length);
        var p =PropertiesManager.Properties.TempRecordPath;
        
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

        // 恢复缓存
        BufferManager.StartBuffer();
        
        // 恢复场景
        Obs.client.SetCurrentProgramScene(PropertiesManager.Properties.NormalSceneName);

        return "200";
    }

}