using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Application.Serialization
{
	public interface ISerializationProvider
	{
		bool IsLoaded { get; }

		void Save();

		void Load();

		/// <summary>
		/// ��provider�����¼��ص���ʱ����
		/// </summary>
		event EventHandler Reloaded;

		void SetValue<T>(string key, T value);

		bool TryGetValue<T>(string key, out T value);

		bool RemoveValue(string key);
	}
}
