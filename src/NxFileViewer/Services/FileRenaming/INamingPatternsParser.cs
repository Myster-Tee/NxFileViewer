using Emignatik.NxFileViewer.Services.FileRenaming.Models;

namespace Emignatik.NxFileViewer.Services.FileRenaming
{
    public interface INamingPatternsParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePattern"></param>
        /// <returns></returns>
        INamingPatterns Parse(string basePattern);
    }
}
