﻿using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Emignatik.NxFileViewer.Settings
{
    public interface IAppSettings: INotifyPropertyChanged
    {
        string LastSaveDir { get; set; }

        string LastOpenedFile { get; set; }

        string ProdKeysFilePath { get; set; }

        string ConsoleKeysFilePath { get; set; }

        string TitleKeysFilePath { get; set; }

        LogLevel LogLevel { get; set; }

        string ProdKeysDownloadUrl { get; set; }

        string TitleKeysDownloadUrl { get; set; }

        bool AlwaysReloadKeysBeforeOpen { get; set; }
    }
}