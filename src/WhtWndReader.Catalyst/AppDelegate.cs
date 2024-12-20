// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace WhtWndReader;

/// <summary>
/// Application Delegate.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    /// <summary>
    /// Gets or sets the main window of the application.
    /// </summary>
    public override UIWindow? Window { get; set; }

    /// <summary>
    /// Called after the application has launched and its UI is ready to be displayed.
    /// </summary>
    /// <param name="application">The application instance.</param>
    /// <param name="launchOptions">A dictionary containing launch options.</param>
    /// <returns>True if the application launched successfully; otherwise, false.</returns>
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        this.Window = new UIWindow(UIScreen.MainScreen.Bounds);
        var vc = new MainSplitViewController();
        this.Window.RootViewController = vc;
        this.Window.WindowScene!.Titlebar!.TitleVisibility = UITitlebarTitleVisibility.Hidden;
        this.Window.MakeKeyAndVisible();
        return true;
    }
}