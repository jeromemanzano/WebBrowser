# WebBrowser [![build, test and publish](https://github.com/jeromemanzano/WebBrowser/actions/workflows/build-test-publish.yml/badge.svg?branch=main)](https://github.com/jeromemanzano/WebBrowser/actions/workflows/build-test-publish.yml)

This is a demo web browser that runs on Windows

## Development setup
To run/debug this app on your you need to have the following:
1. A device with Windows installed. You may find the system requirements [here](https://learn.microsoft.com/en-us/visualstudio/releases/2022/system-requirements).
2. Your preferred .Net IDE Installed. You can use [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/).
3. [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) Installed.

## Installing the app
| :exclamation:  This is not signed so Windows will prompt you when you try to run it.   |
|-----------------------------------------|

If you would like to proceed, you can download the latest `x86` package [here](https://github.com/jeromemanzano/WebBrowser/releases/download/19/WebBrowser.WPF.exe). This is a self-contained single file Windows application so you don't need to install anything else. After downloading, just click on the `.exe` file. It will take a while on first attempt but subsequent run should be faster

## Nuget Packages
- [ReactiveUI](https://www.nuget.org/packages/ReactiveUI)
- [CefSharp.Wpf](https://www.nuget.org/packages/CefSharp.Wpf)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)
- [Akavache](https://www.nuget.org/packages/akavache)
- [NUnit](https://www.nuget.org/packages/NUnit)
- [Moq](https://www.nuget.org/packages/Moq)
