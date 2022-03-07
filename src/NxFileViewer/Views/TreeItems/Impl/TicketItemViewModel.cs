using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;
using Emignatik.NxFileViewer.Utils;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Tools.Es;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class TicketItemViewModel : PartitionFileEntryItemViewModel
    {
        private readonly TicketItem _ticketItem;

        public TicketItemViewModel(TicketItem ticketItem, IServiceProvider serviceProvider) : base(ticketItem, serviceProvider)
        {
            _ticketItem = ticketItem;
            RightsId = _ticketItem.RightsId?.ToString() ?? "";
            AccessKey = _ticketItem.AccessKey?.ToString() ?? "";
            PropertyMask = _ticketItem.PropertyMask.ToFlagsString();
        }

        [PropertyView]
        public string RightsId { get; }

        [PropertyView]
        public string AccessKey { get; }

        [PropertyView]
        public string Issuer => _ticketItem.Issuer;

        [PropertyView]
        public string AccountId => _ticketItem.AccountId.ToStrId();

        [PropertyView]
        public string DeviceId => _ticketItem.DeviceId.ToStrId();

        [PropertyView]
        public byte CryptoType => _ticketItem.CryptoType;

        [PropertyView]
        public byte FormatVersion => _ticketItem.FormatVersion;

        [PropertyView]
        public string TicketId => _ticketItem.TicketId.ToStrId();

        [PropertyView]
        public TitleKeyType TitleKeyType => _ticketItem.TitleKeyType;

        [PropertyView]
        public LicenseType LicenseType => _ticketItem.LicenseType;

        [PropertyView]
        public ushort TicketVersion => _ticketItem.TicketVersion;

        [PropertyView]
        public string PropertyMask { get; }

    }
}