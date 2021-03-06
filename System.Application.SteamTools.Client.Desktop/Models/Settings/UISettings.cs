using System;
using System.Application.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Application.Models.Settings
{
    public static class UISettings
    {
        /// <summary>
        /// ����
        /// </summary>
        public static SerializableProperty<short> Theme { get; }
            = new SerializableProperty<short>(GetKey(), Providers.Local) { AutoSave = true };

        /// <summary>
        /// ����
        /// </summary>
        public static SerializableProperty<string> Language { get; }
            = new SerializableProperty<string>(GetKey(), Providers.Local) { AutoSave = true };



        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(UISettings) + "." + propertyName;
        }
    }
}
