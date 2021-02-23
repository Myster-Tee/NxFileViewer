using System;
using System.ComponentModel;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class NcaItemViewModel : PartitionFileEntryItemViewModel
    {
        private readonly NcaItem _ncaItem;

        public NcaItemViewModel(NcaItem ncaItem, IServiceProvider serviceProvider)
            : base(ncaItem, serviceProvider)
        {
            _ncaItem = ncaItem;

            _ncaItem.PropertyChanged += OnNcaItemPropertyChanged;
        }

        private void OnNcaItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_ncaItem.HeaderSignatureValidity))
                NotifyPropertyChanged(nameof(HeaderSignatureValidity));

            if(e.PropertyName == nameof(_ncaItem.HashValidity))
                NotifyPropertyChanged(nameof(HashValidity));
        }

        [PropertyView]
        public Validity HeaderSignatureValidity => _ncaItem.HeaderSignatureValidity;

        [PropertyView] 
        public Validity HashValidity => _ncaItem.HashValidity;

        [PropertyView]
        public NcaContentType ContentType => _ncaItem.ContentType;

        [PropertyView]
        public TitleVersion SdkVersion => _ncaItem.SdkVersion;

        [PropertyView]
        public DistributionType DistributionType => _ncaItem.DistributionType;

        [PropertyView]
        public byte KeyGeneration => _ncaItem.KeyGeneration;

        [PropertyView]
        public int ContentIndex => _ncaItem.ContentIndex;

        [PropertyView]
        public NcaVersion FormatVersion => _ncaItem.FormatVersion;

        [PropertyView]
        public bool IsEncrypted => _ncaItem.IsEncrypted;
    }
}
