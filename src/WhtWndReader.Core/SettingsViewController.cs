// <copyright file="SettingsViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;
using DA.UI.TableView;
using DA.UI.TableView.Elements;

namespace WhtWndReader;

/// <summary>
/// Settings View Controller.
/// </summary>
public sealed class SettingsViewController : UITableViewController
{
    private ThirdPartyViewController thirdPartyViewController;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewController"/> class.
    /// </summary>
    public SettingsViewController()
    {
        this.thirdPartyViewController = new ThirdPartyViewController();
        this.Title = NSBundle.MainBundle.GetLocalizedString("Settings", "Settings");
        this.View!.BackgroundColor = UIColor.SystemBackground;
        this.TableView = new Root()
        {
            new Section("Version")
            {
                new StringElement(Assembly.GetExecutingAssembly().GetName().Version!.ToString()),
            },
            new Section()
            {
                ActionElement.Create(NSBundle.MainBundle.GetLocalizedString("Third Party Licenses", "Third Party Licenses"), () =>
                {
                    this.NavigationController!.PushViewController(this.thirdPartyViewController, true);
                }),
            },
        };
    }
}