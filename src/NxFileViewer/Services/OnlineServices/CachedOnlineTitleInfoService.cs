using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emignatik.NxFileViewer.Services.OnlineServices;

public class CachedOnlineTitleInfoService : ICachedOnlineTitleInfoService
{
    private readonly IOnlineTitleInfoService _onlineTitleInfoService;

    private readonly Dictionary<string, IOnlineTitleInfo> _memoryCache = new();

    public CachedOnlineTitleInfoService(IOnlineTitleInfoService onlineTitleInfoService)
    {
        _onlineTitleInfoService = onlineTitleInfoService ?? throw new ArgumentNullException(nameof(onlineTitleInfoService));
    }

    public bool IsEnabled { get; set; } = true;

    public async Task<IOnlineTitleInfo?> GetTitleInfoAsync(string titleId)
    {
        if (!IsEnabled)
            return await _onlineTitleInfoService.GetTitleInfoAsync(titleId);

        if (_memoryCache.TryGetValue(titleId, out var cachedTitleInfo))
            return cachedTitleInfo;

        var newTitleInfo = await _onlineTitleInfoService.GetTitleInfoAsync(titleId);
        if (newTitleInfo != null)
            _memoryCache.Add(titleId, newTitleInfo);

        return newTitleInfo;
    }

}