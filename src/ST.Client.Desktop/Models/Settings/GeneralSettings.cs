using System;
using System.Application.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Application.Models.Settings
{
    public static class GeneralSettings
    {
        static GeneralSettings()
        {
            WindowsStartupAutoRun.ValueChanged += WindowsStartupAutoRun_ValueChanged;
            IsEnableLogRecord.ValueChanged += IsEnableLogRecord_ValueChanged;
            //CreateDesktopShortcut.ValueChanged += CreateDesktopShortcut_ValueChanged;
        }

        private static void IsEnableLogRecord_ValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {

        }

        private static void WindowsStartupAutoRun_ValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {

        }

        /// <summary>
        /// �����Ƿ񿪻�������
        /// </summary>
        public static SerializableProperty<bool> WindowsStartupAutoRun { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        ///// <summary>
        ///// ���������ݷ�ʽ
        ///// </summary>
        //public static SerializableProperty<bool> CreateDesktopShortcut { get; }
        //    = new SerializableProperty<bool>(GetKey(), Providers.Roaming, false) { AutoSave = true };

        /// <summary>
        /// ��������ʱ��С��
        /// </summary>
        public static SerializableProperty<bool> IsStartupAppMinimized { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// �Ƿ���ʾ��ʼҳ
        /// </summary>
        public static SerializableProperty<bool> IsShowStartPage { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, true) { AutoSave = true };

        /// <summary>
        /// ������Ϸ�б��ػ���
        /// </summary>
        public static SerializableProperty<bool> IsSteamAppListLocalCache { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, true) { AutoSave = true };

        /// <summary>
        /// �Զ�������
        /// </summary>
        public static SerializableProperty<bool> IsAutoCheckUpdate { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, true) { AutoSave = true };

        /// <summary>
        /// ���ô�����־��¼
        /// </summary>
        public static SerializableProperty<bool> IsEnableLogRecord { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// �û����õ��ı��Ķ����ṩ�̣�����ƽֵ̨��ͬ��ֵ��ʽΪ ö���ַ��� �� ����·��
        /// </summary>
        public static SerializableProperty<Dictionary<Platform, string>?> TextReaderProvider { get; }
                = new SerializableProperty<Dictionary<Platform, string>?>(GetKey(), Providers.Local, null) { AutoSave = true };

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(GeneralSettings) + "." + propertyName;
        }
    }
}
