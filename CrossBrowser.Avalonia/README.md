# CrossBrowser.Avalonia

## Windows Development setup

To run/debug this app on your Windows device you need to have the following:
1. A device with Windows installed. You may find the system requirements [here](https://learn.microsoft.com/en-us/visualstudio/releases/2022/system-requirements).
2. Your preferred .Net IDE Installed. You can use [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/).
3. [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) Installed.
4. Microsoft Edge WebView2. You can download it [here](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)
5. Change build configuration to `Avalonia-Win-Debug` or `Avalonia-Win-Release`


## Installing the Windows app
| :exclamation:  This is not signed so Windows will prompt you when you try to run it.   |
|-----------------------------------------|

If you would like to proceed, you can download the latest `x64` package [here](https://github.com/jeromemanzano/WebBrowser/releases/latest/CrossBrowser.Avalonia.exe). This is a self-contained single file Windows application so you don't need to install anything else. After downloading `CrossBrowser.Avalonia.exe` file, click it to run. It will take a while on first attempt but subsequent run should be faster.

## MacOS Development setup

To run/debug this app on your MacOS device you need to have the following:
1. A device with MacOS installed. Refer to your IDE's system requirements.
2. Your preferred .Net IDE Installed. You can use [Visual Studio 2022 for Mac](https://visualstudio.microsoft.com/vs/mac/) or [JetBrains Rider](https://www.jetbrains.com/rider/).
3. [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) Installed.
4. MacOS workload installed. After installing .NET 6.0, open a terminal and run the following command 
```Bash
dotnet workload install macos 
```
5. Change build configuration to `Avalonia-Mac-Debug` or `Avalonia-Mac-Release`

## Note
This is a WIP. There are still issues and some missing features compared to [CrossBrowser.WPF](https://github.com/jeromemanzano/WebBrowser/tree/main/WebBrowser.WPF).