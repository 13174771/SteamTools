using System;
using System.Application.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Application.Models.Settings
{
    public static class AuthSettings
    {
        static AuthSettings()
        {
            Authenticators.ValueChanged += Authenticators_ValueChanged;
        }

        private static void Authenticators_ValueChanged(object sender, ValueChangedEventArgs<string> e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (AuthSettings.IsCurrentDirectorySaveAuthData.Value)
                {
                    File.WriteAllText(Path.Combine(AppContext.BaseDirectory, Constants.AUTHDATA_FILE), e.NewValue);
                }
            }
        }

        /// <summary>
        /// ��������(ѹ���洢��
        /// </summary>
        public static SerializableProperty<string> Authenticators { get; }
            = new SerializableProperty<string>(GetKey(), Providers.Local) { AutoSave = true };

        /// <summary>
        /// �Ƿ��������ݴ洢�ڳ���·����
        /// </summary>
        public static SerializableProperty<bool> IsCurrentDirectorySaveAuthData { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Local, false) { AutoSave = true };

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(AuthSettings) + "." + propertyName;
        }
    }
}
