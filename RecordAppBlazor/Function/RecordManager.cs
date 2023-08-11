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
    }
    
    public static string StartRecording(string? code)
    {
        code = code.ToMD5();
        Console.Out.WriteLine("start code: " + code);
        if (string.IsNullOrWhiteSpace(code))
        {
            return "不能为空";
        }

        // 写入进程锁
        File.WriteAllText($"{Environment.GetEnvironmentVariable("HOME")}/record.lock",
            $"{code}::{DateTime.Now.Add(Properties.RecordTimeSpan)}");
        
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
            File.Copy(files[0], $"{p}/{code}.mp4");
        }
        else
        {
            return "内部错误, 请联系管理员";
        }

        return "200";
    }
}