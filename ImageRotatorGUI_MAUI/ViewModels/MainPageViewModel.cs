using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using ImageRotatorBackend.Models;
using ImageRotatorBackend.Services;
using ImageRotatorGUI_MAUI.Converters;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ImageRotatorGUI_MAUI.ViewModels
{
    #region CONSTRUCTORS
    public partial class MainPageViewModel : BaseViewModel
    {
        #region MEMBERS
        public ObservableCollection<SelectedImageInfo> SelectedImageFiles { get; } = new();
        public ObservableCollection<FileResult> LoadedImageFiles { get; } = new();
        public ObservableCollection<RotateTaskInfo> ConsoleMessagesList { get; } = new();

        [ObservableProperty]
        double rotationDegrees;

        private readonly IFolderPicker _folderPicker;
        #endregion

        #region CONSTRUCTORS
        public MainPageViewModel(IFolderPicker folderPicker)
        {
            Title = "Image Rotator";
            _folderPicker = folderPicker;
        }
        #endregion

        #region PRIVATE_METHODS_ASYNC
        [RelayCommand]
        async Task BrowseFiles()
        {
            if (IsBusy) return;

            var fileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                {DevicePlatform.WinUI, new[] {".jpg"} }
            });

            try
            {
                IsBusy = true;

                var results = await FilePicker.Default.PickMultipleAsync(new PickOptions { FileTypes=fileTypes});
                if (results != null)
                {
                    foreach (var result in results)
                    {
                        LoadedImageFiles.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error With File Browser!", $"An error occured while trying to access or use the file browser\n{ex.Message}", "OK");
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        async void LoadedFilesSelectionChange(FileResult file)
        {
            if (IsBusy) return;

            try
            {
                foreach(SelectedImageInfo selectionInfo in SelectedImageFiles)
                {
                    if(selectionInfo.FileInfo.FullPath == file.FullPath)
                    {
                        SelectedImageFiles.Remove(selectionInfo);
                        return;
                    }
                }
                
                SelectedImageFiles.Add(new SelectedImageInfo { FileInfo = file});
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error With File Browser!", $"An error occured while trying to access or use the file browser\n{ex.Message}", "OK");
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        async Task ImageTapped(Image image)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if(image.IsSet(Image.SourceProperty)) 
                {
                    image.RemoveBinding(Image.SourceProperty);
                }

                if (image.IsSet(Image.RotationProperty))
                {
                    image.RemoveBinding(Image.RotationProperty);

                    foreach(SelectedImageInfo info in SelectedImageFiles)
                    {
                        if(info.FileInfo.FullPath == ((FileImageSource)image.Source).File)
                        {
                            info.IsSelected = false;
                            //info.DegreesRotation = image.Rotation;
                            info.Image = image;
                            return;
                        }
                    }
                }
                else
                {
                    /*
                    image.SetBinding(Image.RotationProperty, new Binding 
                    { 
                        Path = "RotationDegrees",
                        Source = new RelativeSourceExtension { AncestorType = typeof(MainPageViewModel), Mode = RelativeBindingSourceMode.FindAncestorBindingContext }
                    });
                    */

                    image.BindingContext = this;
                    image.SetBinding(Image.RotationProperty, "RotationDegrees");

                    foreach (SelectedImageInfo info in SelectedImageFiles)
                    {
                        if (info.FileInfo.FullPath == ((FileImageSource)image.Source).File)
                        {
                            info.IsSelected = true;
                            //info.DegreesRotation = image.Rotation;
                            info.Image = image;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error With File Browser!", $"An error occured while trying to access or use the file browser\n{ex.Message}", "OK");
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        async Task SaveEdits()
        {
            if(IsBusy) return;

            try
            {
                IsBusy = true;

                string pickedFolder = await _folderPicker.PickFolder();

#pragma warning disable CS1998
                await Task.Run(() => Parallel.ForEach(SelectedImageFiles, async imgFile =>
                {
                    RotateTaskInfo info = new()
                    {
                        ImageName = imgFile.FileInfo.FileName,
                        ThreadNum = Environment.CurrentManagedThreadId,
                        ElapsedTime = new()
                    };

                    ConsoleMessagesList.Add(info);

                    Stopwatch watch = new();
                    watch.Start();
                    info.ElapsedTime = watch.Elapsed;
                    System.Drawing.Bitmap rotatedImage = ImageRotator.RotateImage(imgFile.FileInfo, imgFile.Image.Rotation);

#if WINDOWS
                using var stream = new FileStream(
                    new String(pickedFolder + "\\" + "rotated_" + imgFile.FileInfo.FileName),
                    FileMode.OpenOrCreate,
                    FileAccess.Write);
                await Task.Run(() => rotatedImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg));
#endif

                    watch.Stop();
                    info.ElapsedTime = watch.Elapsed;
                }));
#pragma warning restore CS1998
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error With File Browser!", $"An error occured while trying to access or use the file browser\n{ex.Message}", "OK");
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }
        /*
        [RelayCommand]
        async Task GetLineCountAsync()
        {
            if (IsBusy || string.IsNullOrEmpty(textFileLocation)) return;

            try
            {
                IsBusy = true;

                TotalCount = await LineCounterService.CountLinesAsync(TextFileLocation, new Progress<int>(count => ActiveLineCount = count));
                return;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error Reading Text!", $"An error occured while trying to count the text file's lines\n{ex.Message}", "OK");
                ActiveLineCount = 0;
                TotalCount = 0;
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task BrowseFiles(PickOptions options)
        {
            if (IsBusy) return;

            try 
            {
                var result = await FilePicker.Default.PickAsync(options);
                if(result != null)
                {
                    TextFileLocation = result.FullPath;
                }
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error With File Browser!", $"An error occured while trying to access or use the file browser\n{ex.Message}", "OK");
                return;
            }
            finally
            { 
                IsBusy = false; 
            }
        }
        */
#endregion
    }
#endregion
}
