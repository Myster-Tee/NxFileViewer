using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Ncm;
using LibHac.Util;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class CnmtContentEntryItemViewModel : ItemViewModel
    {
        private readonly CnmtContentEntryItem _cnmtContentEntryItem;

        public CnmtContentEntryItemViewModel(CnmtContentEntryItem cnmtContentEntryItem, IServiceProvider serviceProvider) : base(cnmtContentEntryItem, serviceProvider)
        {
            _cnmtContentEntryItem = cnmtContentEntryItem ?? throw new ArgumentNullException(nameof(cnmtContentEntryItem));
        }

        [PropertyView]
        public string NcaId => _cnmtContentEntryItem.NcaId.ToStrId();

        [PropertyView]
        public string NcaHash => _cnmtContentEntryItem.NcaHash.ToHexString();      
        
        [PropertyView]
        public ContentType NcaContentType => _cnmtContentEntryItem.NcaContentType;

        [PropertyView]
        public string NcaSize => _cnmtContentEntryItem.NcaSize.ToFileSize();

    }
}
