using RecordAppBlazor.Data;

namespace RecordAppBlazor.Function;

public static class RecordManager
{
    static RecordManager()
    {
        // 确保目录存在
        var p = $"{Environment.GetEnvironmentVariable("HOME")}/Records/";
        var e = Directory.Exists(p);
        if (!e)
        {
            Directory.CreateDirectory(p);
        }
        
        // 删除上次的进程锁
        if (File.Exists($"{Environment.GetEnvironmentVariable("HOME")}/record.lock"))
        {
            File.Delete($"{Environment.GetEnvironmentVariable("HOME")}/record.lock");
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
        File.WriteAllText($"{Environment.GetEnvironmentVariable("HOME")}/record.lock",
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
        Obs.client.StartRecord();

        return "200";
    }
    
    public static string StopRecording(string? code)
    {
        Console.Out.WriteLine("stop code: " + code);

        // 停止录制
        Obs.client.StopRecord();
        
        Thread.Sleep(2000);

        // 重置进程锁
        File.Delete($"{Environment.GetEnvironmentVariable("HOME")}/record.lock");
        
        // 复制文件
        var path = Obs.client.GetRecordDirectory();
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

        BufferManager.StartBuffer();

        return "200";
    }

}