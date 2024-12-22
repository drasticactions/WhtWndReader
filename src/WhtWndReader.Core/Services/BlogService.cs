// <copyright file="BlogService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Models;
using WhtWndReader.Exceptions;
using WhtWndReader.Models;

namespace WhtWndReader.Services;

/// <summary>
/// Blog Service.
/// </summary>
public class BlogService
{
    private readonly ATProtocol atProtocol;
    private readonly DatabaseService databaseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlogService"/> class.
    /// </summary>
    /// <param name="atProtocol">ATProtocol.</param>
    /// <param name="databaseService">Database Service.</param>
    public BlogService(ATProtocol atProtocol, DatabaseService databaseService)
    {
        this.atProtocol = atProtocol;
        this.databaseService = databaseService;
    }

    /// <summary>
    /// Fired when an author is inserted.
    /// </summary>
    public event EventHandler<Author>? OnAuthorInserted;

    /// <summary>
    /// Fired when an author is deleted.
    /// </summary>
    public event EventHandler<Author>? OnAuthorDeleted;

    /// <summary>
    /// Gets the ATProtocol.
    /// </summary>
    public ATProtocol ATProtocol => this.atProtocol;

    /// <summary>
    /// Get authors.
    /// </summary>
    /// <returns>Authors.</returns>
    public Task<List<Author>> GetAuthorsAsync()
        => this.databaseService.GetAuthorsAsync();

    /// <summary>
    /// Refresh author information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authors.</returns>
    public async Task<List<Author>> RefreshAuthorsAsync(CancellationToken? cancellationToken = default)
    {
        var authors = await this.GetAuthorsAsync();
        var authorUpdateTasks = authors.Select(n => this.ATProtocol.GetAuthorAsync(ATIdentifier.Create(n.Id)!));
        var updatedAuthors = await Task.WhenAll(authorUpdateTasks);
        await this.databaseService.UpdateAuthorsAsync(updatedAuthors);
        return await this.GetAuthorsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Get author entries.
    /// </summary>
    /// <param name="author">Author.</param>
    /// <returns>Entries.</returns>
    public Task<List<AuthorEntry>> GetAuthorEntriesAsync(Author author)
        => this.databaseService.GetAuthorEntriesAsync(author.Id);

    /// <summary>
    /// Insert an author.
    /// </summary>
    /// <param name="id">ID of author.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Author.</returns>
    public async Task<Author> InsertAuthorAsync(ATIdentifier id, CancellationToken? cancellationToken = default)
    {
        var token = cancellationToken ?? CancellationToken.None;

        var author = await this.ATProtocol.GetAuthorAsync(id!, token).ConfigureAwait(false);

        var result = await this.databaseService.InsertOrReplaceAuthorAsync(author).ConfigureAwait(false);

        if (result > 0)
        {
            await this.InsertEntriesAsync(author, token).ConfigureAwait(false);
        }

        this.OnAuthorInserted?.Invoke(this, author);

        return author;
    }

    /// <summary>
    /// Insert entries for an author.
    /// </summary>
    /// <param name="author">The author.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>List of author entries.</returns>
    public async Task<List<AuthorEntry>> InsertEntriesAsync(Author author, CancellationToken? cancellationToken = default)
    {
        if (!ATIdentifier.TryCreate(author.Id, out var id))
        {
            throw new WhtWndReaderException("Invalid identifier");
        }

        var token = cancellationToken ?? CancellationToken.None;

        var entries = await this.ATProtocol.GetAuthorEntriesAsync(id!, token);

        await this.databaseService.DeleteAllAuthorEntriesAsync(id!.ToString()!);
        var authorEntries = entries as AuthorEntry[] ?? entries.ToArray();
        await this.databaseService.InsertAllAuthorEntriesAsync(authorEntries!);
        return await this.databaseService.GetAuthorEntriesAsync(id!.ToString()!);
    }

    /// <summary>
    /// Delete an author.
    /// </summary>
    /// <param name="author">Author to delete.</param>
    /// <returns>Rows changed.</returns>
    public async Task<int> DeleteAuthorAsync(Author author)
    {
        var result = await this.databaseService.DeleteAuthorAsync(author.Id);
        this.OnAuthorDeleted?.Invoke(this, author);
        return result;
    }
}