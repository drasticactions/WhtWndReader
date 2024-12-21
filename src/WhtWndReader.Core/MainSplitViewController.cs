// <copyright file="MainSplitViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using Microsoft.Extensions.Logging;
using WhtWndReader.Models;
using WhtWndReader.Services;

namespace WhtWndReader;

/// <summary>
/// Main Split View Controller.
/// </summary>
public sealed class MainSplitViewController : UISplitViewController
{
    private readonly ILogger logger;
    private readonly AuthorsViewController authorsViewController;
    private readonly AuthorEntriesViewController authorEntriesViewController;
    private readonly EntryViewController entryViewController;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainSplitViewController"/> class.
    /// </summary>
    public MainSplitViewController()
        : base(UISplitViewControllerStyle.TripleColumn)
    {
        var atProtocolBuilder = new ATProtocolBuilder();
        var atProtocol = atProtocolBuilder.Build();

        var loggerFactory = new LoggerFactory();
        this.logger = loggerFactory.CreateLogger<MainSplitViewController>();

        var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WhtWndReader", "WhtWndReader.db");
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
        var dbService = new DatabaseService(databasePath);
        dbService.InitializeAsync().FireAndForgetSafeAsync(this.logger);
        var blogService = new BlogService(atProtocol, dbService);

        this.authorsViewController = new AuthorsViewController(blogService, this.logger);
        this.authorsViewController.OnAuthorSelected += this.OnAuthorSelected;
        this.authorEntriesViewController = new AuthorEntriesViewController(blogService, this.logger);
        this.authorEntriesViewController.OnAuthorEntrySelected += this.OnAuthorEntrySelected;
        this.entryViewController = new EntryViewController(blogService, this.logger);

        this.SetViewController(this.authorsViewController, UISplitViewControllerColumn.Primary);
        this.SetViewController(this.authorEntriesViewController, UISplitViewControllerColumn.Supplementary);
        this.SetViewController(this.entryViewController, UISplitViewControllerColumn.Secondary);
        this.PreferredDisplayMode = UISplitViewControllerDisplayMode.TwoBesideSecondary;
    }

    /// <inheritdoc/>
    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
        this.ShowColumn(UISplitViewControllerColumn.Primary);
    }

    private void OnAuthorSelected(object? sender, Author? e)
    {
        if (e == null)
        {
            // Todo: Clear the author entries view controller.
            return;
        }

        this.ShowColumn(UISplitViewControllerColumn.Supplementary);
        this.authorEntriesViewController.LoadAuthorEntriesAsync(e).FireAndForgetSafeAsync(this.logger);
    }

    private void OnAuthorEntrySelected(object? sender, AuthorEntry? e)
    {
        if (e == null)
        {
            // Todo: Clear the author entries view controller.
            return;
        }

        this.entryViewController.LoadEntry(e);
        this.ShowColumn(UISplitViewControllerColumn.Secondary);
    }
}