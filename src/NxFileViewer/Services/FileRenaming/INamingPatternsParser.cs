using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;

namespace Emignatik.NxFileViewer.Services.FileRenaming
{
    public interface INamingPatternsParser
    {
        /// <summary>
        /// Parses all naming patters
        /// </summary>
        /// <param name="applicationPattern"></param>
        /// <returns></returns>
        public NamingPatterns Parse(string applicationPattern);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<ApplicationPatternPart> ParseApplicationPatterns(string pattern);
    }
}
