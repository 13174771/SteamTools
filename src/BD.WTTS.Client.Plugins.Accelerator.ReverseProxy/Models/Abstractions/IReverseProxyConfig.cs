#if (WINDOWS || MACCATALYST || MACOS || LINUX) && !(IOS || ANDROID)
// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.Configuration/FastGithubConfig.cs

// ReSharper disable once CheckNamespace
namespace BD.WTTS.Models.Abstractions;

public partial interface IReverseProxyConfig
{
#if !DISABLE_ASPNET_CORE && (WINDOWS || MACCATALYST || MACOS || LINUX) && !(IOS || ANDROID)
    internal YarpReverseProxyServiceImpl Service { get; }

    IDnsAnalysisService DnsAnalysis => Service.DnsAnalysis;
#endif

    int HttpProxyPort { get; set; }

    IReadOnlyCollection<AccelerateProjectDTO>? ProxyDomains { get; set; }

    IReadOnlyCollection<ScriptDTO>? ProxyScripts { get; set; }

    /// <summary>
    /// 是否匹配指定的域名
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    bool IsMatch(string domain) => TryGetDomainConfig(domain, out _);

    /// <summary>
    /// 尝试获取域名配置
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetDomainConfig(string domain, [MaybeNullWhen(false)] out IDomainConfig value);

    /// <summary>
    /// 尝试获取脚本配置
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetScriptConfig(string domain, [MaybeNullWhen(false)] out IEnumerable<IScriptConfig> value);

    /// <summary>
    /// 获取所有域名表达式
    /// </summary>
    /// <returns></returns>
    DomainPattern[] GetDomainPatterns();
}
#endif