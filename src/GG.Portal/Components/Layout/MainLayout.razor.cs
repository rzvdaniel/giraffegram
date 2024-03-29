﻿using GG.Portal.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace GG.Portal.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        private bool MenuCollapsed { get; set; }

        private NavMenu NavigationMenu { get; set; }

        [Inject] JsInterop JsInterop { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        //[Inject] MainLayoutViewModel MainLayoutViewModel { get; set; }

        private void ToggleCollapsed()
        {
            MenuCollapsed = !MenuCollapsed;
            //NavigationMenu.SetCollapsed(MenuCollapsed);
        }

        private async Task ToggleFullScreen()
        {
            await JsInterop.ToggleFullscreen();
        }

        private async Task Logout()
        {
            //await MainLayoutViewModel.Logout();

            NavigationManager.NavigateTo($"/");
        }
    }
}
