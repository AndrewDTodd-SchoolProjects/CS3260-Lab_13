using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ImageRotatorBackend.Models
{
    public partial class SelectedImageInfo : ObservableObject
    {
        [ObservableProperty]
        FileResult fileInfo;
        [ObservableProperty]
        bool isSelected = false;

        public Image Image { get; set; }
        //public double DegreesRotation { get; set; }
    }
}
