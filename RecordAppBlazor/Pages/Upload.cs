using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.RS;
using Qiniu.RS.Model;
using Qiniu.Util;
using RecordAppBlazor.Data;

namespace RecordAppBlazor.Pages;

public static class Upload
{
    private static Mac Mac;
    private static Auth Auth;
    
    public static void Initialize()
    {
        Mac = new Mac(PropertiesManager.Properties.QiniuAccessKey, PropertiesManager.Properties.QiniuSecretKey);
        Auth = new Auth(Mac);
        
        Qiniu.Common.Config.AutoZone(
            PropertiesManager.Properties.QiniuAccessKey,
            PropertiesManager.Properties.QiniuBucket,
            PropertiesManager.Properties.UseHttps);
        
        Console.Out.WriteLine("Qiniu initialized");
    }
    
    public static void UploadFile(string localFilePath, string saveCode)
    {
        var bucket = PropertiesManager.Properties.QiniuBucket;
        
        // 上传策略，参见 
        // https://developer.qiniu.com/kodo/manual/put-policy
        var putPolicy = new PutPolicy()
        {
            Scope = $"{bucket}",
            DeleteAfterDays = 1,
        };
        // 上传策略有效期(对应于生成的凭证的有效期)          
        putPolicy.SetExpires(3600);
        
        var putPolicyString = putPolicy.ToJsonString();
        var token = Auth.CreateUploadToken(Mac, putPolicyString);
        var uploadManager = new UploadManager();
        var result = uploadManager.UploadFile(localFilePath, saveCode, token);
        
        // 重命名加入后缀
        var bucketManager = new BucketManager(Mac);
        var renameOp = bucketManager.Rename(bucket, $"{saveCode}", $"{saveCode}.mkv");
        
        Console.WriteLine(result);
    }

    // public static string GetFileInfo(string code)
    // {
    //     var mac = new Mac(
    //         PropertiesManager.Properties.QiniuAccessKey,
    //         PropertiesManager.Properties.QiniuSecretKey);
    //     var bucketManager = new BucketManager(mac);
    //     var result = bucketManager.Stat(PropertiesManager.Properties.QiniuBucket, code);
    //     Console.WriteLine(result);
    //     
    //     return result.Result.
    // }
}