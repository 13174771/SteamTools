﻿using MetroTrilithon.Serialization;
using SteamTool.Core.Common;
using SteamTools.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SteamTools.Properties;

namespace SteamTools.Models.Settings
{
    public abstract class SettingsHost
    {
        private static readonly Dictionary<Type, SettingsHost> instances = new Dictionary<Type, SettingsHost>();
        private readonly Dictionary<string, object> cachedProperties = new Dictionary<string, object>();

        protected virtual string CategoryName => this.GetType().Name;

        protected SettingsHost()
        {
            instances[this.GetType()] = this;
        }

        /// <summary>
        /// 请参阅 <see cref="SerializableProperty{T}"/> 缓存在当前实例中获取
        ///  如果没有缓存，请根据<see cref="create" />创建它
        /// </summary>
        /// <returns></returns>
        protected SerializableProperty<T> Cache<T>(Func<string, SerializableProperty<T>> create, [CallerMemberName] string propertyName = "")
        {
            var key = this.CategoryName + "." + propertyName;

            if (this.cachedProperties.TryGetValue(key, out object obj) && obj is SerializableProperty<T> property1)
                return property1;

            var property = create(key);
            this.cachedProperties[key] = property;

            return property;
        }

        #region Load / Save

        public static void Load()
        {
            try
            {
                Providers.Local.Load();

                //每次成功读取完成时保存一份bak
                if (File.Exists(Providers.LocalFilePath))
                    File.Copy(Providers.LocalFilePath, Providers.LocalFilePath + ".bak", true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //MessageBox.Show("Local Settings Load Error :" + ex.ToString(), $"{ProductInfo.Title} {ProductInfo.VersionString} Error");
                if (WindowService.Current.ShowDialogWindow(Providers.LocalFilePath + "\n" + Resources.ConfigLoadError))
                {
                    if (File.Exists(Providers.LocalFilePath))
                    {
                        File.Copy(Providers.LocalFilePath, Providers.LocalFilePath + ".error", true);
                        if (File.Exists(Providers.LocalFilePath + ".bak"))
                            File.Copy(Providers.LocalFilePath + ".bak", Providers.LocalFilePath, true);
                        else
                            File.Delete(Providers.LocalFilePath);
                    }
                    Providers.Local.Load();
                }
                else
                {
                    File.Delete(Providers.LocalFilePath);
                    Providers.Local.Load();
                }
            }

            try
            {
                Providers.Roaming.Load();

                //每次成功读取完成时保存一份bak
                if (File.Exists(Providers.RoamingFilePath))
                    File.Copy(Providers.RoamingFilePath, Providers.RoamingFilePath + ".bak", true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //MessageBox.Show("Roaming Settings Load Error :" + ex.ToString(), $"{ProductInfo.Title} {ProductInfo.VersionString} Error");
                if (WindowService.Current.ShowDialogWindow(Providers.RoamingFilePath + "\n" + Resources.ConfigLoadError))
                {
                    if (File.Exists(Providers.RoamingFilePath))
                    {
                        File.Copy(Providers.RoamingFilePath, Providers.RoamingFilePath + ".error", true);
                        if (File.Exists(Providers.RoamingFilePath + ".bak"))
                            File.Copy(Providers.RoamingFilePath + ".bak", Providers.RoamingFilePath, true);
                        else
                            File.Delete(Providers.RoamingFilePath);
                    }
                    Providers.Roaming.Load();
                }
                else
                {
                    File.Delete(Providers.RoamingFilePath);
                    Providers.Roaming.Load();
                }
            }
        }

        public static void Save()
        {
            #region const message

            const string message = @"无法保存配置文件（{0}）。

错误详细信息：{1}";

            #endregion

            try
            {
                Providers.Local.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(message, Providers.LocalFilePath, ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            try
            {
                Providers.Roaming.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(message, Providers.RoamingFilePath, ex.Message), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// <typeparamref name ="T" />获取类型的配置对象的唯一实例。
        /// </summary>
        public static T Instance<T>() where T : SettingsHost, new()
        {
            return instances.TryGetValue(typeof(T), out SettingsHost host) ? (T)host : new T();
        }
    }
}
