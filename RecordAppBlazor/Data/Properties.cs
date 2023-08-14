using Newtonsoft.Json;

namespace RecordAppBlazor.Data;

public class Properties
{
    public TimeSpan RecordTimeSpan = TimeSpan.FromMinutes(2.5); // 录制的时长
    public TimeOnly StopTime = new(21, 45, 00); // 停止增加录制的时间
    public string FileServerRoot;
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