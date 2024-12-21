// <copyright file="AuthorEntriesViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Masonry;
using Microsoft.Extensions.Logging;
using WhtWndReader.Models;
using WhtWndReader.Services;

namespace WhtWndReader;

/// <summary>
/// Author Entries View Controller.
/// </summary>
public sealed class AuthorEntriesViewController : UITableViewController, IUITableViewDataSource, IUITableViewDelegate
{
    private const string CellIdentifier = "AuthorEntryCell";
    private readonly BlogService blogService;
    private readonly ILogger logger;
    private Author? author;
    private List<AuthorEntry> tableItems = new();
    private UIBarButtonItem refreshButton;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorEntriesViewController"/> class.
    /// </summary>
    /// <param name="blogService">Blog service.</param>
    /// <param name="logger">Logger.</param>
    public AuthorEntriesViewController(BlogService blogService, ILogger logger)
    {
        this.logger = logger;
        this.blogService = blogService;
        this.logger = logger;
        this.TableView.Delegate = this;
        this.TableView.DataSource = this;
        this.View!.BackgroundColor = UIColor.SystemBackground;

        this.TableView.RegisterClassForCellReuse(typeof(UITableViewCell), CellIdentifier);

        this.refreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh);
        this.NavigationItem.RightBarButtonItem = this.refreshButton;
        this.refreshButton.Clicked += this.RefreshButton_Clicked;
        this.refreshButton.Enabled = false;
    }

    /// <summary>
    /// Fired when an author entry is selected.
    /// </summary>
    public event EventHandler<AuthorEntry?>? OnAuthorEntrySelected;

    /// <summary>
    /// Load author entries.
    /// </summary>
    /// <param name="author">The author entries to load.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task LoadAuthorEntriesAsync(Author author)
    {
        ArgumentNullException.ThrowIfNull(author);

        if (this.author == author)
        {
            return;
        }

        this.refreshButton.Enabled = true;

        this.Title = author.DisplayName;
        this.author = author;
        this.tableItems = await this.blogService.GetAuthorEntriesAsync(author);
        this.InvokeOnMainThread(this.TableView.ReloadData);
    }

    /// <inheritdoc/>
    public override nint RowsInSection(UITableView tableview, nint section)
    {
        return this.tableItems.Count;
    }

    /// <inheritdoc/>
    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        var item = this.tableItems[indexPath.Row];
        this.logger.LogInformation("Selected {0}", item.Title);
        this.OnAuthorEntrySelected?.Invoke(this, item);
    }

    /// <inheritdoc/>
    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);
        var item = this.tableItems[indexPath.Row];
        var config = UIListContentConfiguration.CellConfiguration;
        config.Text = item.Title;
        config.SecondaryText = item.CreatedAt?.ToString("yyyy-MM-dd") ?? string.Empty;
        cell.ContentConfiguration = config;
        return cell;
    }

    /// <summary>
    /// Reset the view.
    /// </summary>
    /// <param name="author">Author.</param>
    public void Reset(Author author)
    {
        if (this.author?.Id == author.Id)
        {
            this.author = null;
            this.refreshButton.Enabled = false;
            this.tableItems.Clear();
            this.Title = string.Empty;
            this.TableView.ReloadData();
        }
    }

    private void RefreshButton_Clicked(object? sender, EventArgs e)
    {
        if (this.author is null)
        {
            return;
        }

        Task.Run(
            async () =>
        {
            this.tableItems = await this.blogService.InsertEntriesAsync(this.author!);
            this.InvokeOnMainThread(() => this.TableView.ReloadData());
        }).WithProgressHud(NSBundle.MainBundle.GetLocalizedString("Refreshing Entries", "Refreshing Entries"), this.logger).FireAndForgetSafeAsync(this.logger);
    }
}