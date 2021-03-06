using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Emignatik.NxFileViewer.Utils.MVVM.Localization
{
    public abstract class LocalizationKeysBase : ILocalizationKeysBase
    {
        private PropertyInfo[]? _keyProperties;


        public abstract bool IsFallback { get; }

        public abstract string DisplayName { get; }

        public abstract string CultureName { get; }

        public abstract string LanguageAuto { get; }

        public bool TryGetValue(string key, out string value, StringComparison stringComparison = StringComparison.Ordinal)
        {
            value = key;

            var propertyInfo = FindProperty(key, stringComparison);
            if (propertyInfo == null)
            {
                Trace.TraceError($"«{nameof(LocalizationKeysBase)}» error: failed to get localized value from key «{key}», object of type «{this.GetType().Name}» is missing the corresponding property.");
                return false;
            }

            try
            {
                var propertyValue = (string?)propertyInfo.GetValue(this);
                if (propertyValue != null)
                {
                    value = propertyValue;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"«{nameof(LocalizationKeysBase)}» error: failed to get localized value from property «{propertyInfo.Name}» of object type «{this.GetType().Name}» (key was «{key}»): {ex.Message}");
                throw;
            }

            return false;
        }

        public string? this[string key]
        {
            get
            {
                if (TryGetValue(key, out var value, StringComparison.OrdinalIgnoreCase))
                    return value;
                return null;
            }
        }

        private PropertyInfo? FindProperty(string key, StringComparison stringComparison)
        {
            return GetKeyProperties().FirstOrDefault(propertyInfo => propertyInfo.PropertyType == typeof(string) && string.Equals(propertyInfo.Name, key, stringComparison));
        }

        private PropertyInfo[] GetKeyProperties()
        {
            if (_keyProperties != null)
                return _keyProperties;
            _keyProperties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            return _keyProperties;
        }
    }
}