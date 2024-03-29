﻿using System.Threading;
using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public interface IHttpDownloader
{
    Task DownloadFileAsync(string url, string destFilePath, CancellationToken cancellationToken);
}