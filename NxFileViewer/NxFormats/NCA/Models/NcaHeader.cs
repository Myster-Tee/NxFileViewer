using System.Linq;
using Emignatik.NxFileViewer.NxFormats.NCA.Structs;

namespace Emignatik.NxFileViewer.NxFormats.NCA.Models
{
    /// <summary>
    /// NCA header data model
    /// (aggregates raw structures and shortcuts some interesting information)
    /// </summary>
    public class NcaHeader
    {
        public NcaHeaderStruct RawStruct { get; set; }

        public NcaContentType ContentType => RawStruct.ContentType;

        /// <summary>
        /// Always an array of 4 sections,
        /// Warning: section can be null when not defined
        /// </summary>
        public NcaSectionHeader[] Sections { get; set; }

        /// <summary>
        /// Gets the array of non null sections among the 4 sections of <see cref="Sections"/>
        /// </summary>
        public NcaSectionHeader[] DefinedSections
        {
            get
            {
                var sections = Sections ?? new NcaSectionHeader[0];
                return sections.Where(section => section != null).ToArray();
            }
        }
        
        /// <summary>
        /// Returns the section by index.
        /// Returns null if the section is not defined
        /// </summary>
        /// <param name="sectionIndex"></param>
        /// <returns></returns>
        public NcaSectionHeader GetSectionHeaderByIndex(NcaSectionIndex sectionIndex)
        {
            return Sections.FirstOrDefault(section => section != null && section.SectionIndex == sectionIndex);
        }

    }
}
