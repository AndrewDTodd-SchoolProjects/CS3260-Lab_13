using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ImageRotatorBackend.Models
{
    public partial class RotateTaskInfo: ObservableObject
    {
        [ObservableProperty]
        int threadNum = 0;
        [ObservableProperty]
        string imageName;
        [ObservableProperty]
        TimeSpan elapsedTime;
    }
}
