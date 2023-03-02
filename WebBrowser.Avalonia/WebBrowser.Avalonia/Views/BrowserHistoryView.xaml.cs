using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using WebBrowser.Models;
using WebBrowser.ViewModels;

namespace WebBrowser.Avalonia.Views;

public partial class BrowserHistoryView : UserControl, IActivatableView
{
    public BrowserHistoryView()
    {
        InitializeComponent();

        this.WhenActivated(compositeDisposable =>
        {
            ((BrowserHistoryViewModel) DataContext!)
                .History
                .ToObservableChangeSet()
                .Sort(SortExpressionComparer<HistoryModel>.Descending(model => model.LocalTimeStamp))
                .GroupOn(model => model.Host)
                .Transform(group => new Grouping<string, HistoryModel>(group))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var collection)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(compositeDisposable);
            
            HistoryListBox.Items = collection;
        });
    }
}