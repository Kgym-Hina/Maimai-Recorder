# Maimai-Recorder
用于内录机台自助录播的系统.
目前处于能用的状态，后期将会进行优化.

本项目使用 [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/legalcode) 协议开源

# Roadmap

- [] 支持配置文件
- [] 支持多机台的录制管理
- [] 将项目结构标准化
- [] 支持自选录制时长
- [] 支持自动上传至在线服务, 返回链接

# Usage
```
dotnet run
```

请在OBS设置内将输出设置设为`~/Records/`以外的文件夹，并且请保证这个文件夹下没有任何其他文件（空文件夹）
此系统可以配合另一个项目 `待上传` 一同使用. 在Windows下可能需要对读取环境变量的部分进行修改，可在不修改的情况下直接在Linux/Mac上使用

# Modify

请在启动前修改`properties.cs`文件内的参数以适合所使用的店面
