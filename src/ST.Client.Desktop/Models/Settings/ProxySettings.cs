using System;
using System.Application.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Application.Models.Settings
{
    public static class ProxySettings
    {
        /// <summary>
        /// ����GOG�������
        /// </summary>
        public static SerializableProperty<bool> IsProxyGOG { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// ����windowsϵͳ����ģʽ
        /// </summary>
        public static SerializableProperty<bool> EnableWindowsProxy { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// ���ô���ű�
        /// </summary>
        public static SerializableProperty<bool> IsEnableScript { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// ��������ʱ�Զ���������
        /// </summary>
        public static SerializableProperty<bool> ProgramStartupRunProxy { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// �Ƿ�ֻ���Steam������������ýű�
        /// </summary>
        public static SerializableProperty<bool> IsOnlyWorkSteamBrowser { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// �����������״̬
        /// </summary>
        public static SerializableProperty<IReadOnlyCollection<string>> SupportProxyServicesStatus { get; }
            = new SerializableProperty<IReadOnlyCollection<string>>(GetKey(), Providers.Local, new List<string>());


        /// <summary>
        /// �ű�����״̬
        /// </summary>
        public static SerializableProperty<IReadOnlyCollection<int>> ScriptsStatus { get; }
            = new SerializableProperty<IReadOnlyCollection<int>>(GetKey(), Providers.Local, new List<int>());


        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(ProxySettings) + "." + propertyName;
        }
    }
}
