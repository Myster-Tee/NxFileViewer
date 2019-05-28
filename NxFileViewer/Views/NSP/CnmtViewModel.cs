using System;
using Emignatik.NxFileViewer.NSP.Models;

namespace Emignatik.NxFileViewer.Views.NSP
{
    public class CnmtViewModel : ViewModelBase
    {
        private readonly CnmtInfo _cnmtInfo;
        private string _tabTitle;


        public CnmtViewModel(CnmtInfo cnmtInfo)
        {
            _cnmtInfo = cnmtInfo ?? throw new ArgumentNullException(nameof(cnmtInfo));

            ControlPartition = _cnmtInfo.ControlPartition != null ? new ControlPartitionViewModel(_cnmtInfo.ControlPartition) : null;
        }

        public string TabTitle
        {
            get => _tabTitle;
            set
            {
                _tabTitle = value;
                NotifyPropertyChanged();
            }
        }

        public string TitleId => _cnmtInfo.TitleId;

        public string AppType => _cnmtInfo.Type.ToString();

        public uint? TitleVersion => _cnmtInfo.TitleVersion;

        public ControlPartitionViewModel ControlPartition { get; }

    }
}
