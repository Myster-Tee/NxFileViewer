using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Utils.MVVM.Localization;

namespace Emignatik.NxFileViewer.Localization
{
    public class LocalizationManager : LocalizationManager<ILocalizationKeys>
    {
        public static LocalizationManager Instance { get; } = new();
    }
}