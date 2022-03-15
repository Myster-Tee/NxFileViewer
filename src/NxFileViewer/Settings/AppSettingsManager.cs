using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Emignatik.NxFileViewer.Localization;
using Emignatik.NxFileViewer.Utils;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettingsManager : IAppSettingsManager
    {
        private static readonly string _settingsFilePath;

        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;

        static AppSettingsManager()
        {
            _settingsFilePath = Path.Combine(PathHelper.CurrentAppDir, $"{AppDomain.CurrentDomain.FriendlyName}.settings.json");
        }

        public AppSettingsManager(ILoggerFactory loggerFactory, AppSettings appSettings)
        {
            _logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            RestoreDefaultSettings();
        }

        public IAppSettings Settings => _appSettings;

        public void RestoreDefaultSettings()
        {
            RecopyPropertyValues(typeof(IAppSettings), new AppSettings(), _appSettings);
        }

        public bool LoadSafe()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                    return false;

                var bytes = File.ReadAllBytes(_settingsFilePath);
                var settingsModel = JsonSerializer.Deserialize<AppSettings>(new ReadOnlySpan<byte>(bytes));
                if (settingsModel == null)
                    return false;

                RecopyPropertyValues(typeof(IAppSettings), settingsModel, _appSettings);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsLoadingError.SafeFormat(ex.Message));
                return false;
            }
        }


        private static void RecopyPropertyValues(IReflect type, object? source, object? dest)
        {
            if (source == null || dest == null)
                return;

            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                // Simple properties copy
                var propType = propertyInfo.PropertyType;
                if (IsSimpleType(propType))
                {
                    var getMethod = propertyInfo.GetGetMethod();
                    var setMethod = propertyInfo.GetSetMethod();
                    if (getMethod == null || setMethod == null) // Property should have both a getter and a setter to be copied
                        continue;

                    // Get source property value
                    var propValue = getMethod.Invoke(source, null);

                    if (propValue != null)
                        // Recopy property value to destination
                        setMethod.Invoke(dest, new[] { propValue });
                }
                else
                {
                    // Deep copy
                    var getMethod = propertyInfo.GetGetMethod();
                    if(getMethod == null)
                        continue;

                    var newSource = getMethod.Invoke(source, null);
                    var newDest = getMethod.Invoke(dest, null);

                    RecopyPropertyValues(propType, newSource, newDest);
                }
            }
        }

        /// <summary>
        /// Returns true if type is a value type (int, bool, etc.), a nullable value type (int?, bool?, etc.) or is string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsSimpleType(Type type)
        {
            if (type.IsValueType || type == typeof(string))
                return true;

            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType is { IsValueType: true };
        }

        public void SaveSafe()
        {
            try
            {
                using var stream = File.Create(_settingsFilePath);

                JsonSerializer.Serialize(new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }), _appSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LocalizationManager.Instance.Current.Keys.SettingsSavingError.SafeFormat(ex.Message));
            }
        }
    }
}