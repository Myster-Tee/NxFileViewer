using LibHac;

namespace Emignatik.NxFileViewer.Services
{
    public interface IKeySetProviderService
    {
        Keyset GetKeySet(bool forceReload = false);
    }
}