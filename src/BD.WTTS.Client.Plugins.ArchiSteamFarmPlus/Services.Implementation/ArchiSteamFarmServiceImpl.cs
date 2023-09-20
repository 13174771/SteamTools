//#if (WINDOWS || MACCATALYST || MACOS || LINUX) && !(IOS || ANDROID)
//using ASFStrings = ArchiSteamFarm.Localization.Strings;
//using AppResources = BD.WTTS.Client.Resources.Strings;
//using ASFNLogManager = ArchiSteamFarm.LogManager;

//// ReSharper disable once CheckNamespace
//namespace BD.WTTS.Services.Implementation;

//public partial class ArchiSteamFarmServiceImpl : ReactiveObject, IArchiSteamFarmService, IIoc
//{
//    const string TAG = "ArchiSteamFarmS";

//    public ArchiSteamFarmServiceImpl()
//    {
//        ArchiSteamFarmLibrary.Init(this, IOPath.AppDataDirectory, IApplication.LogDirPathASF);
//    }

//    public event Action<string>? OnConsoleWirteLine;

//    public TaskCompletionSource<string>? ReadLineTask { get; set; }

//    private readonly AsyncLock @lock = new AsyncLock();

//    bool _IsReadPasswordLine;

//    public bool IsReadPasswordLine
//    {
//        get => _IsReadPasswordLine;
//        set => this.RaiseAndSetIfChanged(ref _IsReadPasswordLine, value);
//    }

//    public DateTimeOffset? StartTime { get; set; }

//    public Version CurrentVersion => SharedInfo.Version;

//    private bool isFirstStart = true;

//    T IIoc.GetRequiredService<T>() where T : class => Ioc.Get<T>();

//    T? IIoc.GetService<T>() where T : class => Ioc.Get_Nullable<T>();

//    static void InitCoreLoggers()
//    {
//        IApplication.LogDirPathASF = ASFPathHelper.GetLogDirectory(IApplication.LogDirPathASF);
//        if (ASFNLogManager.Configuration != null) return;
//        LoggingConfiguration config = new();
//        FileTarget fileTarget = new("File")
//        {
//            ArchiveFileName = ASFPathHelper.GetNLogArchiveFileName(IApplication.LogDirPathASF),
//            ArchiveNumbering = ArchiveNumberingMode.Rolling,
//            ArchiveOldFileOnStartup = true,
//            CleanupFileName = false,
//            ConcurrentWrites = false,
//            DeleteOldFileOnStartup = true,
//            FileName = ASFPathHelper.GetNLogFileName(IApplication.LogDirPathASF),
//            Layout = ASFPathHelper.NLogGeneralLayout,
//            ArchiveAboveSize = 10485760,
//            MaxArchiveFiles = 10,
//            MaxArchiveDays = 7,
//        };
//        //IApplication.InitializeTarget(config, fileTarget, NLogLevel.Debug);
//        var historyTarget = new HistoryTarget("History")
//        {
//            Layout = ASFPathHelper.NLogGeneralLayout,
//            MaxCount = 20,
//        };
//        //IApplication.InitializeTarget(config, historyTarget, NLogLevel.Debug);
//        ASFNLogManager.Configuration = config;
//    }

//    public async Task<bool> StartAsync(string[]? args = null)
//    {
//        try
//        {
//            StartTime = DateTimeOffset.Now;

//            if (isFirstStart)
//            {
//                InitCoreLoggers();

//                InitHistoryLogger();

//                ArchiSteamFarm.NLog.Logging.GetUserInputFunc = async (bool isPassword) =>
//                {
//                    using (await @lock.LockAsync())
//                    {
//                        ReadLineTask = new(TaskCreationOptions.AttachedToParent);
//                        IsReadPasswordLine = isPassword;
//#if NET6_0_OR_GREATER
//                        var result = await ReadLineTask.Task.WaitAsync(TimeSpan.FromSeconds(60));
//#else
//                            var result = await ReadLineTask.Task;
//#endif
//                        if (IsReadPasswordLine)
//                            IsReadPasswordLine = false;
//                        ReadLineTask = null;
//                        return result;
//                    }
//                };

//                await ReadEncryptionKeyAsync();

//                await Program.Init(args).ConfigureAwait(false);
//                isFirstStart = false;
//            }
//            else
//            {
//                if (!await Program.InitASF().ConfigureAwait(false))
//                {
//                    await StopAsync().ConfigureAwait(false);
//                    return false;
//                }
//            }
//            return true;
//        }
//        catch (Exception e)
//        {
//            e.LogAndShowT(TAG, msg: "ASF Start Fail.");
//            await StopAsync().ConfigureAwait(false);
//            return false;
//        }
//    }

//    public async Task StopAsync()
//    {
//        StartTime = null;
//        ReadLineTask?.TrySetResult("");
//        await Program.InitShutdownSequence();
//    }

//    async Task IArchiSteamFarmHelperService.Restart()
//    {
//        Toast.Show(AppResources.ASF_Restarting, ToastLength.Short);

//        var s = ASFService.Current;
//        if (s.IsASFRuning)
//        {
//            await s.StopASFCoreAsync(false);
//        }
//        await s.InitASFCoreAsync(false);

//        Toast.Show(AppResources.ASF_Restarted, ToastLength.Short);
//    }

//    LogLevel IArchiSteamFarmHelperService.MinimumLevel => IApplication.LoggerMinLevel;

//    private void InitHistoryLogger()
//    {
//        ArchiSteamFarm.NLog.Logging.InitHistoryLogger();

//        HistoryTarget? historyTarget = ArchiSteamFarm.LogManager.Configuration.AllTargets.OfType<HistoryTarget>().FirstOrDefault();

//        if (historyTarget != null)
//            historyTarget.NewHistoryEntry += (object? sender, HistoryTarget.NewHistoryEntryArgs newHistoryEntryArgs) =>
//            {
//                OnConsoleWirteLine?.Invoke(newHistoryEntryArgs.Message);
//            };
//    }

//    public async Task<string?> ExecuteCommandAsync(string command)
//    {
//        Bot? targetBot = Bot.Bots?.OrderBy(bot => bot.Key, Bot.BotsComparer).Select(bot => bot.Value).FirstOrDefault();

//        ASF.ArchiLogger.LogGenericInfo(command);

//        if (targetBot == null)
//        {
//            ASF.ArchiLogger.LogGenericWarning(ASFStrings.ErrorNoBotsDefined);
//            return null;
//        }

//        ASF.ArchiLogger.LogGenericInfo(ASFStrings.Executing);

//        ulong steamOwnerID = ASF.GlobalConfig?.SteamOwnerID ?? GlobalConfig.DefaultSteamOwnerID;

//        var access = targetBot.GetAccess(steamOwnerID);
//        string? response = await targetBot.Commands.Response(access, command, steamOwnerID);

//        if (!string.IsNullOrEmpty(response))
//            ASF.ArchiLogger.LogGenericInfo(response);
//        return response;
//    }

//    public IReadOnlyDictionary<string, Bot>? GetReadOnlyAllBots()
//    {
//        var bots = Bot.Bots;
//        //if (bots is not null)
//        //    foreach (var bot in bots.Values)
//        //    {
//        //        bot.AvatarUrl = GetAvatarUrl(bot);
//        //    }
//        return bots;
//    }

//    public async void SaveBot(Bot bot)
//    {
//        //var bot = Bot.GetBot(botName);
//        string filePath = Bot.GetFilePath(bot.BotName, Bot.EFileType.Config);
//        bool result = await BotConfig.Write(filePath, bot.BotConfig).ConfigureAwait(false);
//    }

//    public GlobalConfig? GetGlobalConfig()
//    {
//        return ASF.GlobalConfig;
//    }

//    public async void SaveGlobalConfig(GlobalConfig config)
//    {
//        string filePath = ASF.GetFilePath(ASF.EFileType.Config);
//        bool result = await GlobalConfig.Write(filePath, config).ConfigureAwait(false);
//        if (result)
//        {
//            Toast.Show("SaveGlobalConfig  " + result);
//        }
//    }

//    string IPCRootUrl
//    {
//        get
//        {
//            string? value = null;
//            var a = ArchiSteamFarm.IPC.ArchiKestrel.ServerAddresses;
//            var loopback = IPAddress.Loopback.ToString();
//            if (a != null)
//            {
//                value = a.FirstOrDefault(x => x.Contains(loopback) && x.StartsWith(String2.Prefix_HTTP))
//                    ?? a.FirstOrDefault(x => x.Contains(loopback))
//                    ?? a.FirstOrDefault();
//            }
//            if (string.IsNullOrWhiteSpace(value))
//            {
//                value = $"http://{loopback}:{CurrentIPCPortValue}";
//            }
//            return value;
//        }
//    }

//    public string GetIPCUrl()
//    {
//        var defaultUrl = IPCRootUrl;
//        string absoluteConfigDirectory = Path.Combine(ASFPathHelper.AppDataDirectory, SharedInfo.ConfigDirectory);
//        string customConfigPath = Path.Combine(absoluteConfigDirectory, SharedInfo.IPCConfigFile);
//        if (File.Exists(customConfigPath))
//        {
//            var configRoot = new ConfigurationBuilder().SetBasePath(absoluteConfigDirectory).AddJsonFile(SharedInfo.IPCConfigFile, false, true).Build();
//            var urlSection = configRoot.GetSection("Url").Value;
//            try
//            {
//                var url = new Uri(urlSection!);
//                if (IPAddress.Any.ToString() == url.Host)
//                {
//                    return defaultUrl;
//                }
//                else
//                {
//                    return url.AbsolutePath;
//                }
//            }
//            catch
//            {
//                return defaultUrl;
//            }
//        }
//        else
//        {
//            return defaultUrl;
//        }
//    }

//    public int CurrentIPCPortValue { get; set; }

//    //public static IEnumerable<HttpMessageHandler> GetAllHandlers()
//    //{
//    //    // 动态更改运行中的代理设置，遍历 ASF 中的 HttpClientHandler 设置新的 Proxy
//    //    var asf_handler = ASF.WebBrowser?.HttpClientHandler;
//    //    if (asf_handler != null) yield return asf_handler;
//    //    var bots = Bot.BotsReadOnly?.Values;
//    //    if (bots != null)
//    //    {
//    //        foreach (var bot in bots)
//    //        {
//    //            var bot_handler = bot.ArchiWebHandler.WebBrowser.HttpClientHandler;
//    //            if (bot_handler != null) yield return bot_handler;
//    //        }
//    //    }
//    //}

//    string EncryptionKey
//    {
//        set => ArchiCryptoHelper.SetEncryptionKey(value);
//    }

//    const string ASF_CRYPTKEY = "ASF_CRYPTKEY";
//    const string ASF_CRYPTKEY_DEF_VALUE = nameof(ArchiSteamFarm);

//    public async Task SetEncryptionKeyAsync()
//    {
//        var result = await TextBoxWindowViewModel.ShowDialogAsync(new TextBoxWindowViewModel
//        {
//            Title = AppResources.ASF_SetCryptKey,
//            Placeholder = ASF_CRYPTKEY,
//            InputType = TextBoxWindowViewModel.TextBoxInputType.Password,
//        });
//        var isUseDefaultCryptKey = string.IsNullOrEmpty(result) || result == ASF_CRYPTKEY_DEF_VALUE;
//        if (!isUseDefaultCryptKey)
//        {
//            await ISecureStorage.Instance.SetAsync(ASF_CRYPTKEY, result);
//        }
//        else
//        {
//            await ISecureStorage.Instance.RemoveAsync(ASF_CRYPTKEY);
//            result = ASF_CRYPTKEY_DEF_VALUE;
//        }
//        ArchiCryptoHelper.SetEncryptionKey(result!);
//    }

//    /// <summary>
//    /// 尝试读取已保存的自定义密钥并应用
//    /// </summary>
//    /// <returns></returns>
//    static async Task ReadEncryptionKeyAsync()
//    {
//        if (!ArchiCryptoHelper.HasDefaultCryptKey)
//        {
//            // 当前运行中已设置了自定义密钥，则跳过
//            return;
//        }
//        var result = await ISecureStorage.Instance.GetAsync(ASF_CRYPTKEY);
//        if (!string.IsNullOrEmpty(result))
//        {
//            ArchiCryptoHelper.SetEncryptionKey(result);
//        }
//    }
//}

//#endif