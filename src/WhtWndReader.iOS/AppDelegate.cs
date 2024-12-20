// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using Microsoft.Extensions.Logging;
using WhtWndReader.Services;

namespace WhtWndReader;

/// <summary>
/// Represents the application delegate, which is responsible for handling application-level events.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    /// <summary>
    /// Gets or sets the main window of the application.
    /// </summary>
    public override UIWindow? Window
    {
        get;
        set;
    }

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
        this.Window.MakeKeyAndVisible();
        return true;
    }
}
