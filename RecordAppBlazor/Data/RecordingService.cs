using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using RecordAppBlazor.Function;

namespace RecordAppBlazor.Data;

public static class RecordingService
{
    public static List<Recording> AllRecordings = new();
    public static void AddRecording(Recording recording)
    {
        AllRecordings.Add(recording);
    }
    
    public static Recording[] GetAllRecordings()
    {
        return AllRecordings.ToArray();
    }
    
    public static Recording GetRecording(string code)
    {
        return AllRecordings.First(r => r.Code == code.ToMD5());
    }
    
    
}