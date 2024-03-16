using GG.Portal.Components.Models;
using Microsoft.JSInterop;

namespace GG.Portal.Components
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class JsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public JsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./JsInterop.js").AsTask());
        }

        public async Task ToggleFullscreen()
        {
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync("toggleFullscreen");
        }

        public async Task<bool> IsFullscreen()
        {
            var module = await moduleTask.Value;

            return await module.InvokeAsync<bool>("isFullscreen");
        }

        public async Task OpenFullscreen()
        {
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync("openFullscreen");
        }

        public async Task CloseFullscreen()
        {
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync("closeFullscreen");
        }

        public async ValueTask<string> Prompt(string message)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("showPrompt", message);
        }

        public async ValueTask<BoundingClientRect> GetBoundingClientRect(string elementId)
        {
            var module = await moduleTask.Value;

            var result = await module.InvokeAsync<BoundingClientRect>("getBoundingClientRect", elementId);

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
