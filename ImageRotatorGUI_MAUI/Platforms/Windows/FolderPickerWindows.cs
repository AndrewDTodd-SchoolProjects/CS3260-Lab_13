using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WindowsFolderPicker = Windows.Storage.Pickers.FolderPicker;

using ImageRotatorGUI_MAUI;

namespace ImageRotatorBackend.Services
{
    public class FolderPicker : IFolderPicker
    {
        /*
        public async Task<string> PickFolder()
        {
            var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                {DevicePlatform.WinUI, new[] {"*"} }
            });

            var results = await FilePicker.Default.PickAsync(new PickOptions { FileTypes = fileTypes });

            return results.FullPath;
        }
        */

        public async Task<string> PickFolder()
        {
            var folderPicker = new WindowsFolderPicker();
            // Make it work for Windows 10
            folderPicker.FileTypeFilter.Add("*");
            // Get the current window's HWND by passing in the Window object
            var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var result = await folderPicker.PickSingleFolderAsync();

            return result.Path;
        }
    }
}
