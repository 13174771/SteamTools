﻿using System.Collections.Generic;
using XEDeviceIdiom = Xamarin.Essentials.DeviceIdiom;

namespace System
{
    /// <summary>
    /// 设备种类，例如手机，平板，电视，手表
    /// </summary>
    [Flags]
    public enum DeviceIdiom
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 1,

        /// <summary>
        /// 手机
        /// </summary>
        Phone = 4,

        /// <summary>
        /// 平板
        /// </summary>
        Tablet = 8,

        /// <summary>
        /// 桌面端
        /// </summary>
        Desktop = 16,

        /// <summary>
        /// 电视
        /// </summary>
        TV = 32,

        /// <summary>
        /// 手表
        /// </summary>
        Watch = 64,
    }

    /// <summary>
    /// Enum 扩展 <see cref="DeviceIdiom"/>
    /// </summary>
    public static partial class IdiomEnumExtensions
    {
        /// <summary>
        /// 值是否在定义的范围中，排除 <see cref="DeviceIdiom.Unknown"/>
        /// </summary>
        /// <param name="deviceIdiom"></param>
        /// <returns></returns>
        public static bool IsDefined(this DeviceIdiom deviceIdiom)
        {
            return deviceIdiom != DeviceIdiom.Unknown && Enum.IsDefined(typeof(DeviceIdiom), deviceIdiom);
        }

        static readonly IReadOnlyDictionary<XEDeviceIdiom, DeviceIdiom> mapping = new Dictionary<XEDeviceIdiom, DeviceIdiom>
        {
            { XEDeviceIdiom.Phone, DeviceIdiom.Phone },
            { XEDeviceIdiom.Tablet, DeviceIdiom.Tablet },
            { XEDeviceIdiom.Desktop, DeviceIdiom.Desktop },
            { XEDeviceIdiom.TV, DeviceIdiom.TV },
            { XEDeviceIdiom.Watch, DeviceIdiom.Watch },
        };

        /// <summary>
        /// 将 <see cref="Xamarin.Essentials.DeviceIdiom"/> 转换为 <see cref="DeviceIdiom"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DeviceIdiom Convert(this XEDeviceIdiom value) => mapping.ContainsKey(value) ? mapping[value] : DeviceIdiom.Unknown;
    }
}