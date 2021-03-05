using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.FsSystem.NcaUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class SectionItemViewModel : ItemViewModel
    {
        private readonly SectionItem _sectionItem;
        private readonly MenuItem _menuItemSaveSection;

        public SectionItemViewModel(SectionItem sectionItem, IServiceProvider serviceProvider)
            : base(sectionItem, serviceProvider)
        {
            _sectionItem = sectionItem;
            _sectionItem.PropertyChanged += OnSectionItemPropertyChanged;

            var saveSectionContentCommand = serviceProvider.GetRequiredService<ISaveSectionContentCommand>();
            saveSectionContentCommand.SectionItem = sectionItem;
            _menuItemSaveSection = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_SaveSectionItem), saveSectionContentCommand);
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
        public bool IsPatchSection => _sectionItem.IsPatchSection;

        [PropertyView]
        public NcaHashType HashType => _sectionItem.HashType;

        [PropertyView]
        public short Version => _sectionItem.Version;

        public override IEnumerable<MenuItem> GetOtherContextMenuItems()
        {
            yield return _menuItemSaveSection;
        }
    }
}