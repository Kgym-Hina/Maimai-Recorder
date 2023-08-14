using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace RecordAppBlazor.Function;

public class Timer : BackgroundService
{
    private DateTime stopTime;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var locked = File.Exists($"{Environment.GetEnvironmentVariable("HOME")}/record.lock");
            // 检测是否有进程锁
            if (locked)
            {
                // 执行任务: 五秒钟检测一次是否超时
                var lockFile = await File.ReadAllTextAsync($"{Environment.GetEnvironmentVariable("HOME")}/record.lock", stoppingToken);
                var lockFileContent = lockFile.Split("::");
                stopTime = DateTime.Parse(lockFileContent[1]);
            
                if (DateTime.Now > stopTime)
                {
                    // 停止录制
                    RecordManager.StopRecording(lockFileContent[0]);
                }
            }
            
            // 检测buffer
            try
            {
                if (!locked && !BufferManager.isInitialized)
                {
                    BufferManager.isInitialized = true;
                    BufferManager.StartBuffer();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            // 暂停一段时间，例如每隔5秒执行一次
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            await Console.Out.WriteLineAsync("Timer executed (5s)");
        }
    }
}