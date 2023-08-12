using Newtonsoft.Json;

namespace RecordAppBlazor.Data;

public class Properties
{
    public TimeSpan RecordTimeSpan = TimeSpan.FromMinutes(2.5); // 录制的时长
    public TimeOnly StopTime = new(21, 45, 00); // 停止增加录制的时间
    public string FileServerRoot = "http://192.168.100.97:9090/";
}

public static class PropertiesManager
{
    public static readonly Properties? Properties;

    static PropertiesManager()
    {
        var path = $"{Environment.GetEnvironmentVariable("HOME")}/recordingConfig.json";
        if (File.Exists(path))
        {
            var fileContent = File.ReadAllText(path);

            Properties = JsonConvert.DeserializeObject<Properties>(fileContent);
        }
        else
        {
            Properties = new Properties();
            
            File.WriteAllText(path, JsonConvert.SerializeObject(Properties));
        }
    }
}