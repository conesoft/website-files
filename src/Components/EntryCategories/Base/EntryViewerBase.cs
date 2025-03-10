using Conesoft.Files;
using Conesoft.Website.Files.Services;
using Microsoft.AspNetCore.Components;

namespace Conesoft.Website.Files.Components.EntryCategories.Base;

public abstract class EntryViewerBase : ComponentBase, IDisposable
{
    [Inject] protected FileHostingPaths HostingPaths { get; private set; } = default!;
    [Parameter] public Entry Entry { get; set; } = default!;
    [Parameter] public string Path { get; set; } = default!;
    [Parameter] public Category Category { get; set; } = default!;

    protected abstract Task OnLiveChange();

    CancellationTokenSource? cancellationTokenSource = null;
    void IDisposable.Dispose()
    {
        cancellationTokenSource?.Cancel();
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        OnLiveChange().Wait();

        if (HostingPaths.FileAt(Path) is Conesoft.Files.File file)
        {
            Task.Run(async () =>
            {
                cancellationTokenSource = await file.Live(async () =>
                {
                    await OnLiveChange();
                    await InvokeAsync(StateHasChanged);
                });
            });
        }
        if (HostingPaths.DirectoriesAt(Path) is Conesoft.Files.Directory[] directories && directories.Length > 0)
        {
            Task.Run(async () =>
            {
                cancellationTokenSource = await directories.Live(async () =>
                {
                    await OnLiveChange();
                    await InvokeAsync(StateHasChanged);
                });
            });
        }
    }
}