// <copyright file="AuthorsViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip.Models;
using Microsoft.Extensions.Logging;
using WhtWndReader.Models;
using WhtWndReader.Services;

namespace WhtWndReader;

/// <summary>
/// Authors view controller.
/// </summary>
public sealed class AuthorsViewController : UITableViewController, IUITableViewDataSource, IUITableViewDelegate
{
    private const string CellIdentifier = "AuthorCell";
    private readonly BlogService blogService;
    private readonly UIBarButtonItem addButton;
    private readonly ILogger logger;
    private readonly UIBarButtonItem settingsButton;
    private readonly UINavigationController settingsNavigationController;
    private List<Author> tableItems = new();
    private UIRefreshControl refreshControl;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorsViewController"/> class.
    /// </summary>
    /// <param name="blogService">Blog service.</param>
    /// <param name="logger">Logger.</param>
    public AuthorsViewController(BlogService blogService, ILogger logger)
    {
        this.blogService = blogService;
        this.blogService.OnAuthorInserted += this.BlogService_OnAuthorInserted;
        this.logger = logger;
        this.refreshControl = new UIRefreshControl();
        this.TableView.RefreshControl = this.refreshControl;
        this.refreshControl.ValueChanged += async (sender, e) =>
        {
            this.refreshControl.BeginRefreshing();
            try
            {
                await this.RefreshAuthorsAsync();
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error refreshing authors.");
            }
            finally
            {
                this.refreshControl.EndRefreshing();
            }
        };
        this.View!.BackgroundColor = UIColor.SystemBackground;
        this.addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
        this.addButton.AccessibilityHint = NSBundle.MainBundle.GetLocalizedString("Add Author", "Add Author");
        this.Title = NSBundle.MainBundle.GetLocalizedString("Authors", "Authors");
        this.NavigationItem.RightBarButtonItem = this.addButton;
        this.addButton.Clicked += this.AddButton_Clicked;

        this.TableView.RegisterClassForCellReuse(typeof(UITableViewCell), CellIdentifier);
        this.TableView.Delegate = this;
        this.TableView.DataSource = this;
        this.LoadAuthorsAsync().FireAndForgetSafeAsync(this.logger);
        this.settingsNavigationController = new UINavigationController(new SettingsViewController());
        this.ModalInPresentation = true;
        this.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
        this.settingsButton = new UIBarButtonItem(
            UIImage.GetSystemImage("gear"),
            UIBarButtonItemStyle.Plain,
            (sender, args) =>
            {
                this.PresentViewController(this.settingsNavigationController, true, null);
            });
        this.settingsButton.AccessibilityHint = NSBundle.MainBundle.GetLocalizedString("Settings", "Settings");
        this.NavigationItem.LeftBarButtonItem = this.settingsButton;
    }

    /// <summary>
    /// Fired when an author is selected.
    /// </summary>
    public event EventHandler<Author?>? OnAuthorSelected;

    /// <inheritdoc/>
    public override nint RowsInSection(UITableView tableview, nint section)
    {
        return this.tableItems.Count;
    }

    /// <inheritdoc/>
    public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
    {
        return true;
    }

    /// <inheritdoc/>
    public override string TitleForDeleteConfirmation(UITableView tableView, Foundation.NSIndexPath indexPath)
    {
        return NSBundle.MainBundle.GetLocalizedString("Delete", "Delete");
    }

    /// <inheritdoc/>
    public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
    {
        switch (editingStyle)
        {
            case UITableViewCellEditingStyle.Delete:
                this.blogService.DeleteAuthorAsync(this.tableItems[indexPath.Row]).FireAndForgetSafeAsync();
                this.tableItems.RemoveAt(indexPath.Row);
                tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                break;
            case UITableViewCellEditingStyle.None:
                this.logger.LogWarning("CommitEditingStyle:None called");
                break;
        }
    }

    /// <inheritdoc/>
    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell(CellIdentifier) ??
                   new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

        var config = UIListContentConfiguration.CellConfiguration;
        if (string.IsNullOrEmpty(this.tableItems[indexPath.Row]?.DisplayName))
        {
            config.Text = this.tableItems[indexPath.Row]?.Handle;
        }
        else
        {
            config.Text = this.tableItems[indexPath.Row]?.DisplayName;
            config.SecondaryText = this.tableItems[indexPath.Row]?.Handle;
        }

        if (this.tableItems[indexPath.Row]?.Avatar is not null)
        {
            config.Image = UIImage.LoadFromData(NSData.FromArray(this.tableItems[indexPath.Row].Avatar!));
            config.ImageProperties.MaximumSize = new CGSize(40, 40);
            config.ImageProperties.CornerRadius = 20;
        }

        cell.ContentConfiguration = config;

        return cell;
    }

    /// <inheritdoc/>
    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        var author = this.tableItems[indexPath.Row];
        this.OnAuthorSelected?.Invoke(this, author);
    }

    private void BlogService_OnAuthorInserted(object? sender, Author e)
    {
        this.LoadAuthorsAsync().FireAndForgetSafeAsync(this.logger);
    }

    private void AddButton_Clicked(object? sender, System.EventArgs e)
    {
        var addAuthor = NSBundle.MainBundle.GetLocalizedString("Add Author", "Add Author");
        var enterAuthorsHandleOrDID = NSBundle.MainBundle.GetLocalizedString("Enter Author's handle or DID", "Enter Author's handle or DID");
        var cancel = NSBundle.MainBundle.GetLocalizedString("Cancel", "Cancel");
        var add = NSBundle.MainBundle.GetLocalizedString("Add", "Add");
        var placeholder = NSBundle.MainBundle.GetLocalizedString("Handle or DID", "Handle or DID");
        var addingAuthor = NSBundle.MainBundle.GetLocalizedString("Adding Author", "Adding Author");
        var uiAlertController =
            UIAlertController.Create(addAuthor, enterAuthorsHandleOrDID, UIAlertControllerStyle.Alert);
        uiAlertController.AddTextField(textField => { textField.Placeholder = placeholder; });
        uiAlertController.AddAction(UIAlertAction.Create(cancel, UIAlertActionStyle.Cancel, null));
        uiAlertController.AddAction(UIAlertAction.Create(add, UIAlertActionStyle.Default, action =>
        {
            var textField = uiAlertController.TextFields![0];
            if (ATIdentifier.TryCreate(textField.Text ?? string.Empty, out var atIdentifier))
            {
                this.blogService.InsertAuthorAsync(atIdentifier!)
                    .WithProgressHud(addingAuthor).FireAndForgetSafeAsync();
            }
        }));

        this.PresentViewController(uiAlertController, true, null);
    }

    private async Task LoadAuthorsAsync()
    {
        try
        {
            this.tableItems = await this.blogService.GetAuthorsAsync();
            this.BeginInvokeOnMainThread(() => this.TableView.ReloadData());
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to refresh data");
        }
    }

    private async Task RefreshAuthorsAsync()
    {
        try
        {
            this.tableItems = await this.blogService.RefreshAuthorsAsync();
            this.BeginInvokeOnMainThread(() => this.TableView.ReloadData());
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to refresh data");
        }
    }
}