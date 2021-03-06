using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emignatik.NxFileViewer.Commands;
using Emignatik.NxFileViewer.Localization.Keys;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac;
using LibHac.FsSystem.NcaUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class NcaItemViewModel : PartitionFileEntryItemViewModel
    {
        private readonly NcaItem _ncaItem;
        private readonly IMenuItemViewModel _menuItemSaveNcaRaw;
        private readonly IMenuItemViewModel _menuItemSavePlaintextNca;

        public NcaItemViewModel(NcaItem ncaItem, IServiceProvider serviceProvider)
            : base(ncaItem, serviceProvider)
        {
            _ncaItem = ncaItem;

            _ncaItem.PropertyChanged += OnNcaItemPropertyChanged;

            var savePartitionFileCommand = serviceProvider.GetRequiredService<ISavePartitionFileCommand>();
            savePartitionFileCommand.PartitionFileItem = ncaItem;
            _menuItemSaveNcaRaw = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_SaveNcaFileRaw), savePartitionFileCommand);

            var savePlaintextNcaFileCommand = serviceProvider.GetRequiredService<ISavePlaintextNcaFileCommand>();
            savePlaintextNcaFileCommand.NcaItem = ncaItem;
            _menuItemSavePlaintextNca = CreateLocalizedMenuItem(nameof(ILocalizationKeys.ContextMenu_SaveNcaFilePlaintext), savePlaintextNcaFileCommand);
        }

        private void OnNcaItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_ncaItem.HeaderSignatureValidity))
                NotifyPropertyChanged(nameof(HeaderSignatureValidity));
        }

        public override IEnumerable<IMenuItemViewModel> GetOtherContextMenuItems()
        {
            yield return _menuItemSaveNcaRaw;
            yield return _menuItemSavePlaintextNca;
        }

        [PropertyView]
        public Validity HeaderSignatureValidity => _ncaItem.HeaderSignatureValidity;

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

        [PropertyView]
        public bool IsNca0 => _ncaItem.IsNca0;
    }
}
