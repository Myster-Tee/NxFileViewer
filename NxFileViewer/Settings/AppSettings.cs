using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Emignatik.NxFileViewer.Settings
{
    public class AppSettings : IAppSettings
    {
        private const string SETTINGS_FILE_PATH = "Settings.json";


        public string LastOpenedFile { get; set; }

        public string KeysFilePath { get; set; } = "Keys.dat";

        public void Load()
        {
            if (!File.Exists(SETTINGS_FILE_PATH))
                return;

            using var stream = File.OpenRead(SETTINGS_FILE_PATH);
            var appSettings = JsonSerializer.DeserializeAsync<AppSettings>(stream).Result;

            foreach (var propertyInfo in typeof(AppSettings).GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                propertyInfo.SetValue(this, propertyInfo.GetValue(appSettings));
            }
        }

        public void Save()
        {
            //TODO: implémenter la sauvegarde
        }
    }
}
