using DynamicData;
using AppResources = BD.WTTS.Client.Resources.Strings;

// ReSharper disable once CheckNamespace
namespace BD.WTTS.UI.ViewModels;

[MP2Obj]
public sealed partial class SettingsPageViewModel : TabItemViewModel
{
    public SettingsPageViewModel()
    {
        SelectLanguage = ResourceService.Languages.FirstOrDefault(x => x.Key == UISettings.Language.ActualValue);
        this.WhenValueChanged(x => x.SelectLanguage, false)
              .Subscribe(x => UISettings.Language.ActualValue = x.Key);

        UpdateChannels = Enum2.GetAll<UpdateChannelType>();

        OpenFolder_Click = ReactiveCommand.Create<string>(OpenFolder);

#if (WINDOWS || MACCATALYST || MACOS || LINUX) && !(IOS || ANDROID)
        if (IApplication.IsDesktop())
        {
            SelectFont = ResourceService.Fonts.FirstOrDefault(x => x.Value == UISettings.FontName.ActualValue);
            this.WhenValueChanged(x => x.SelectFont, false)
                  .Subscribe(x => UISettings.FontName.ActualValue = x.Value);

            SelectImage_Click = ReactiveCommand.CreateFromTask(async () =>
            {
                FilePickerFileType fileTypes = new ValueTuple<string, string[]>[]
                {
                        ("Image Files", FileEx.Images.ToArray()),
                    //("All Files", new[] { "*", }),
                };
                await FilePicker2.PickAsync(SetBackgroundImagePath, fileTypes);
            });

            ResetImage_Click = ReactiveCommand.Create(() => SetBackgroundImagePath(null));
        }
#endif
    }

    public void OpenFolder(string tag)
    {
        var path = tag switch
        {
            IOPath.DirName_AppData => IOPath.AppDataDirectory,
            IOPath.DirName_Cache => IOPath.CacheDirectory,
            IApplication.LogDirName => IApplication.LogDirPath,
            _ => IOPath.BaseDirectory,
        };
        var hasKey = clickTimeRecord.TryGetValue(path, out var dt);
        var now = DateTime.Now;
        if (hasKey && (now - dt).TotalSeconds <= clickInterval) return;
        IPlatformService.Instance.OpenFolder(path);
        if (!clickTimeRecord.TryAdd(path, now)) clickTimeRecord[path] = now;
    }

#if WINDOWS
    public void SelectSteamProgramLocation()
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Steam|steam.exe";
        openFileDialog.ShowDialog();
        SteamSettings.SteamProgramPath.ActualValue = openFileDialog.FileName;
    }

#endif

#if (WINDOWS || MACCATALYST || MACOS || LINUX) && !(IOS || ANDROID)
    public void SetBackgroundImagePath(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            UISettings.WindowBackgroundCustomImagePath.ActualValue = string.Empty;
            return;
        }
        if (File.Exists(imagePath))
        {
            if (IOPath.TryOpenRead(imagePath, out var stream, out var _))
            {
                var (isImage, _) = FileFormat.IsImage(stream);
                if (isImage)
                {
                    UISettings.WindowBackgroundCustomImagePath.ActualValue = imagePath;
                    return;
                }
            }
        }
        Toast.Show(AppResources.Settings_UI_CustomBackgroundImage_Error);
    }

    public async void EditSteamParameter()
    {
        var vm = new TextBoxWindowViewModel
        {
            InputType = TextBoxWindowViewModel.TextBoxInputType.TextBox,
            Value = SteamSettings.SteamStratParameter.ActualValue,
        };
        if (await IWindowManager.Instance.ShowTaskDialogAsync(vm, AppResources.Edit + " " + AppResources.Settings_Steam_SteamStratParameter, subHeader: "可添加自定义的参数来启动 Steam", isDialog: false, isCancelButton: true))
        {
            SteamSettings.SteamStratParameter.ActualValue = vm.Value;
        }
    }
#endif

    ///// <summary>
    ///// 开始计算指定文件夹大小
    ///// </summary>
    ///// <param name="isStartCacheSizeCalc"></param>
    ///// <param name="sizeFormat"></param>
    ///// <param name="action"></param>
    //[Obsolete("use StartSizeCalcByCacheSize or StartSizeCalcByLogSize")]
    //public static void StartCacheSizeCalc(ref bool isStartCacheSizeCalc, string sizeFormat, Action<string> action, CancellationToken cancellationToken = default)
    //{
    //    if (isStartCacheSizeCalc) return;
    //    isStartCacheSizeCalc = true;
    //    string? dirPath;
    //    if (sizeFormat == AppResources.Settings_General_CacheSize)
    //    {
    //        dirPath = IOPath.CacheDirectory;
    //    }
    //    else if (sizeFormat == AppResources.Settings_General_LogSize)
    //    {
    //        dirPath = IApplication.LogDirPath;
    //    }
    //    else
    //    {
    //        dirPath = null;
    //    }
    //    if (dirPath != null)
    //    {
    //        StartCacheSizeCalc2(dirPath, value => action(sizeFormat.Format(value)), cancellationToken);
    //    }
    //}

    static void StartCacheSizeCalc2(string dirPath, Action<string> action, CancellationToken cancellationToken = default)
    {
        action(AppResources.Settings_General_Calcing);
        try
        {
            Task.Run(async () =>
            {
                var length = IOPath.GetDirectorySize(dirPath);
                var lenString = IOPath.GetDisplayFileSizeString(length);
                await MainThread2.InvokeOnMainThreadAsync(() =>
                {
                    action(lenString);
                });
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {

        }
    }

    static readonly Dictionary<string, string> cacheSizeCalcCache = new();

    static void StartCacheSizeCalc2(string dirPath, Func<string> getResString, Action<string> action)
    {
        if (cacheSizeCalcCache.TryGetValue(dirPath, out var value))
        {
            action(Action(value));
        }
        else
        {
            StartCacheSizeCalc2(dirPath, value => action(Action(value)));
        }

        string Action(string value) => getResString().Format(value);
    }

    public static void StartSizeCalcByCacheSize(Action<string> action)
        => StartCacheSizeCalc2(
            IOPath.CacheDirectory,
            () => AppResources.Settings_General_CacheSize,
            action);

    public static void StartSizeCalcByLogSize(Action<string> action)
        => StartCacheSizeCalc2(
            IApplication.LogDirPath,
            () => AppResources.Settings_General_LogSize,
            action);
}
