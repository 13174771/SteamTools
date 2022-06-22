using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Application.UI.Resx;

namespace System.Application.Services.Implementation;

abstract partial class CertificateManagerImpl
{
    protected const string TAG = "CertificateManager";

    protected readonly IPlatformService platformService;
    protected readonly IReverseProxyService reverseProxyService;

    protected ICertificateManager Interface => (ICertificateManager)this;

    public CertificateManagerImpl(
        IPlatformService platformService,
        IReverseProxyService reverseProxyService)
    {
        this.platformService = platformService;
        this.reverseProxyService = reverseProxyService;
    }

    /// <inheritdoc cref="ICertificateManager.RootCertificate"/>
    public abstract X509Certificate2? RootCertificate { get; set; }

    /// <inheritdoc cref="ICertificateManager.PfxPassword"/>
    public virtual string? PfxPassword { get; set; }

    protected abstract X509Certificate2? LoadRootCertificate();

    protected abstract void SharedTrustRootCertificate();

    protected abstract bool SharedCreateRootCertificate();

    protected abstract void SharedRemoveTrustedRootCertificate();

    static readonly object lockGenerateCertificate = new();

    /// <inheritdoc cref="ICertificateManager.GetCerFilePathGeneratedWhenNoFileExists"/>
    public string? GetCerFilePathGeneratedWhenNoFileExists()
    {
        var filePath = Interface.CerFilePath;
        lock (lockGenerateCertificate)
        {
            if (!File.Exists(filePath))
            {
                if (!GenerateCertificateUnlock(filePath)) return null;
            }
            return filePath;
        }
    }

    /// <summary>
    /// (❌🔒)生成 Root 证书
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    bool GenerateCertificateUnlock(string filePath)
    {
        var result = SharedCreateRootCertificate();
        if (!result || RootCertificate == null)
        {
            Log.Error(TAG, AppResources.CreateCertificateFaild);
            Toast.Show(AppResources.CreateCertificateFaild);
            return false;
        }

        RootCertificate.SaveCerCertificateFile(filePath);

        return true;
    }

    /// <summary>
    /// (✔️🔒)生成 Root 证书
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    protected bool GenerateCertificate(string? filePath = null)
    {
        filePath ??= Interface.CerFilePath;
        lock (lockGenerateCertificate)
        {
            return GenerateCertificateUnlock(filePath);
        }
    }

    /// <inheritdoc cref="ICertificateManager.TrustRootCertificate"/>
    public void TrustRootCertificate()
    {
        try
        {
            SharedTrustRootCertificate();

            if (OperatingSystem2.IsMacOS())
            {
                TrustRootCertificateMacOS();
            }
            else if (OperatingSystem2.IsLinux() && !OperatingSystem2.IsAndroid())
            {
                TrustRootCertificateLinux();
            }
        }
#if DEBUG
        catch (Exception e)
        {
            e.LogAndShowT(TAG, msg: "TrustRootCertificate Error");
        }
#else
        catch { }
#endif
    }

    [SupportedOSPlatform("macOS")]
    void TrustRootCertificateMacOS()
    {
        var filePath = GetCerFilePathGeneratedWhenNoFileExists();
        if (filePath == null) return;
        platformService.RunShell($"security add-trusted-cert -d -r trustRoot -k /Users/{Environment.UserName}/Library/Keychains/login.keychain-db \\\"{filePath}\\\"", true);
    }

    [SupportedOSPlatform("Linux")]
    void TrustRootCertificateLinux()
    {
        if (platformService.IsAdministrator)
        {
            var filePath = GetCerFilePathGeneratedWhenNoFileExists();
            if (filePath == null) return;
            platformService.RunShell($"sudo cp -f \"{filePath}\" \"{Path.Combine(IOPath.AppDataDirectory, $@"{IReverseProxyService.CertificateName}.Certificate.pem")}\"", false);
        }
        else
        {
            Browser2.Open(UrlConstants.OfficialWebsite_LiunxSetupCer);
        }
    }

    /// <inheritdoc cref="ICertificateManager.SetupRootCertificate"/>
    public bool SetupRootCertificate()
    {
        if (!GenerateCertificate()) return false;
        TrustRootCertificate();
        return IsRootCertificateInstalled;
    }

    /// <inheritdoc cref="ICertificateManager.DeleteRootCertificate"/>
    public bool DeleteRootCertificate()
    {
        if (reverseProxyService.ProxyRunning)
            return false;
        if (RootCertificate == null)
            return true;
        try
        {
            if (OperatingSystem2.IsMacOS())
            {
                DeleteRootCertificateMacOS();
            }
            else
            {
                SharedRemoveTrustedRootCertificate();
            }
            if (!IsRootCertificateInstalled)
            {
                RootCertificate = null;
                var pfxFilePath = Interface.PfxFilePath;
                if (File.Exists(pfxFilePath)) File.Delete(pfxFilePath);
            }
        }
        catch (CryptographicException)
        {
            // 取消删除证书
            return false;
        }
        catch (Exception e)
        {
            e.LogAndShowT(TAG, msg: "DeleteRootCertificate Error");
            return false;
        }
        return true;
    }

    [SupportedOSPlatform("macOS")]
    async void DeleteRootCertificateMacOS()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        var collection = store.Certificates.Find(X509FindType.FindByIssuerName, IReverseProxyService.RootCertificateName, false);
        foreach (var item in collection)
        {
            if (item != null)
            {
                try
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Remove(item);
                }
                catch
                {
                    await platformService.RunShellAsync($"security delete-certificate -Z \\\"{item.GetCertHashString()}\\\"", true);
                }
            }
        }
    }

    /// <inheritdoc cref="ICertificateManager.IsRootCertificateInstalled"/>
    public bool IsRootCertificateInstalled
    {
        get
        {
            if (RootCertificate == null)
                if (GetCerFilePathGeneratedWhenNoFileExists() == null) return false;
            return IsCertificateInstalled(RootCertificate);
        }
    }

    /// <summary>
    /// 检查证书是否已安装并信任
    /// </summary>
    /// <param name="certificate2"></param>
    /// <returns></returns>
    bool IsCertificateInstalled(X509Certificate2? certificate2)
    {
        if (certificate2 == null)
            return false;
        if (certificate2.NotAfter <= DateTime.Now)
            return false;

        if (!OperatingSystem2.IsAndroid() && OperatingSystem2.IsLinux())
        {
            // Linux 目前没有实现检测
            return true;
        }
        else if (OperatingSystem2.IsAndroid() || OperatingSystem2.IsMacOS())
        {
            return platformService.IsCertificateInstalled(certificate2);
        }
        else
        {
            using var store = new X509Store(OperatingSystem2.IsMacOS() ? StoreName.My : StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            return store.Certificates.Contains(certificate2);
        }
    }
}