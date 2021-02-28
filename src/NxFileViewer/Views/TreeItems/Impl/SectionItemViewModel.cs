using System;
using System.ComponentModel;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.FsSystem.NcaUtils;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class SectionItemViewModel : ItemViewModel
    {
        private readonly SectionItem _sectionItem;

        public SectionItemViewModel(SectionItem sectionItem, IServiceProvider serviceProvider)
            : base(sectionItem, serviceProvider)
        {
            _sectionItem = sectionItem;
            _sectionItem.PropertyChanged += OnSectionItemPropertyChanged;
        }

        private void OnSectionItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_sectionItem.HashValidity))
                NotifyPropertyChanged(nameof(HashValidity));
        }

        [PropertyView]
        public Validity HashValidity => _sectionItem.HashValidity;

        [PropertyView] 
        public int SectionIndex => _sectionItem.SectionIndex;

        [PropertyView] 
        public NcaEncryptionType EncryptionType => _sectionItem.EncryptionType;

        [PropertyView]
        public NcaFormatType FormatType => _sectionItem.FormatType;

        [PropertyView]
        public NcaHashType HashType => _sectionItem.HashType;

        [PropertyView]
        public short Version => _sectionItem.Version;
    }
}