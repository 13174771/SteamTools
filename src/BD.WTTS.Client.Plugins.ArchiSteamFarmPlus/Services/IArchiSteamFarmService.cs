//#if (WINDOWS || MACCATALYST || MACOS || LINUX) && !(IOS || ANDROID)

//// ReSharper disable once CheckNamespace

//namespace BD.WTTS.Services;

///// <summary>
///// ArchiSteamFarm 服务
///// </summary>
//public partial interface IArchiSteamFarmService : IArchiSteamFarmHelperService
//{
//    static new IArchiSteamFarmService Instance => Ioc.Get<IArchiSteamFarmService>();

//    event Action<string>? OnConsoleWirteLine;

//    TaskCompletionSource<string>? ReadLineTask { get; }

//    bool IsReadPasswordLine { get; }

//    DateTimeOffset? StartTime { get; }

//    Version CurrentVersion { get; }

//    /// <summary>
//    /// 启动 ArchiSteamFarm
//    /// </summary>
//    /// <param name="args"></param>
//    Task<bool> StartAsync(string[]? args = null);

//    Task StopAsync();

//    /// <summary>
//    /// 执行 ArchiSteamFarm 指令
//    /// </summary>
//    /// <param name="command"></param>
//    Task<string?> ExecuteCommandAsync(string command);

//    /// <summary>
//    /// 获取 IPC 地址
//    /// </summary>
//    /// <returns></returns>
//    string GetIPCUrl();

//    /// <summary>
//    /// 获取 Bot 只读集合
//    /// </summary>
//    /// <returns></returns>
//    IReadOnlyDictionary<string, Bot>? GetReadOnlyAllBots();

//    GlobalConfig? GetGlobalConfig();

//    async void CommandSubmit(string? command)
//    {
//        if (string.IsNullOrEmpty(command))
//            return;

//        if (ReadLineTask is null)
//        {
//            if (command[0] == '!')
//            {
//                command = command.Remove(0, 1);
//            }
//            await ExecuteCommandAsync(command);
//        }
//        else
//        {
//            ReadLineTask.TrySetResult(command);
//        }
//    }

//    int CurrentIPCPortValue { get; protected set; }

//    int IArchiSteamFarmHelperService.IPCPortValue
//    {
//        get
//        {
//            CurrentIPCPortValue = ASFSettings.IPCPortId.Value;
//            if (CurrentIPCPortValue == default) CurrentIPCPortValue = ASFSettings.DefaultIPCPortIdValue;
//            if (ASFSettings.IPCPortOccupiedRandom.Value)
//            {
//                if (SocketHelper.IsUsePort(CurrentIPCPortValue))
//                {
//                    CurrentIPCPortValue = SocketHelper.GetRandomUnusedPort(IPAddress.Loopback);
//                    return CurrentIPCPortValue;
//                }
//            }
//            return CurrentIPCPortValue;
//        }
//    }

//    /// <summary>
//    /// 使用弹窗密码框输入自定义密钥并设置与保存
//    /// </summary>
//    /// <returns></returns>
//    Task SetEncryptionKeyAsync();
//}

//#endif