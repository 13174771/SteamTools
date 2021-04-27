using System;
using System.Application.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Application.Models.Settings
{
    public static class SteamSettings
    {
        static SteamSettings()
        {
            IsRunSteamMinimized.ValueChanged += IsRunSteamMinimized_ValueChanged;
            IsRunSteamNoCheckUpdate.ValueChanged += IsRunSteamNoCheckUpdate_ValueChanged;
        }

        private static void IsRunSteamNoCheckUpdate_ValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            if (e.NewValue)
                SteamStratParameter.Value += " -noverifyfiles";
            else
                SteamStratParameter.Value = SteamStratParameter.Value.Replace("-silent", "").Trim();
        }

        private static void IsRunSteamMinimized_ValueChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            if (e.NewValue)
                SteamStratParameter.Value += " -silent";
            else
                SteamStratParameter.Value = SteamStratParameter.Value.Replace("-silent", "").Trim();
        }

        /// <summary>
        /// Steam��������
        /// </summary>
        public static SerializableProperty<string> SteamStratParameter { get; }
            = new SerializableProperty<string>(GetKey(), Providers.Local, string.Empty) { AutoSave = true };

        /// <summary>
        /// SteamƤ��
        /// </summary>
        public static SerializableProperty<string> SteamSkin { get; }
            = new SerializableProperty<string>(GetKey(), Providers.Local, string.Empty) { AutoSave = true };

        /// <summary>
        /// �Զ�����Steam
        /// </summary>
        public static SerializableProperty<bool> IsAutoRunSteam { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// Steam����ʱ��С��������
        /// </summary>
        public static SerializableProperty<bool> IsRunSteamMinimized { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// Steam����ʱ��������
        /// </summary>
        public static SerializableProperty<bool> IsRunSteamNoCheckUpdate { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        /// <summary>
        /// ��⵽Steam����ʱ������Ϣ֪ͨ
        /// </summary>
        public static SerializableProperty<bool> IsEnableSteamLaunchNotification { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(SteamSettings) + "." + propertyName;
        }
    }
}
