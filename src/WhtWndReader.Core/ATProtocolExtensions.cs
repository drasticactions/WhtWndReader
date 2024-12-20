// <copyright file="ATProtocolExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using FishyFlip;
using FishyFlip.Lexicon.App.Bsky.Actor;
using FishyFlip.Lexicon.Com.Atproto.Identity;
using FishyFlip.Lexicon.Com.Atproto.Sync;
using FishyFlip.Lexicon.Com.Whtwnd.Blog;
using FishyFlip.Models;
using FishyFlip.Tools;
using WhtWndReader.Exceptions;
using WhtWndReader.Models;

namespace WhtWndReader;

/// <summary>
/// ATProtocol Extensions.
/// </summary>
public static class ATProtocolExtensions
{
    /// <summary>
    /// Asynchronously retrieves a collection of author entries for the specified identifier.
    /// </summary>
    /// <param name="atProtocol">The ATProtocol instance used to perform the operation.</param>
    /// <param name="identifier">The identifier of the author whose entries are to be retrieved.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of AuthorEntry objects.</returns>
    /// <exception cref="WhtWndReaderException">Thrown when the author entries cannot be retrieved.</exception>
    public static async Task<IEnumerable<AuthorEntry>> GetAuthorEntriesAsync(this ATProtocol atProtocol, ATIdentifier identifier, CancellationToken? cancellationToken = default)
    {
        var entries = new List<AuthorEntry>();
        var cursor = string.Empty;

        do
        {
            var (result, error) = await atProtocol.ListEntryAsync(identifier, cursor: cursor, cancellationToken: cancellationToken ?? CancellationToken.None);
            if (error != null)
            {
                throw new WhtWndReaderException("Failed to get author entries", error);
            }

            if (result?.Records is null)
            {
                throw new WhtWndReaderException("Failed to get author entries");
            }

            foreach (var record in result.Records)
            {
                var item = (Entry)record.Value!;
                var entry = new AuthorEntry()
                {
                    Id = record.Uri!.ToString(),
                    AuthorId = identifier!.ToString() ?? throw new WhtWndReaderException("Failed to get author entries, Author ID missing"),
                    Title = item.Title,
                    CreatedAt = item.CreatedAt,
                    Content = item.Content,
                    Visibility = item.Visibility,
                    Html = await atProtocol.GenerateEntryHtmlAsync(item) ?? string.Empty,
                };
                entries.Add(entry);
            }

            cursor = result.Cursor;
        }
        while (!string.IsNullOrEmpty(cursor));

        return entries;
    }

    /// <summary>
    /// Asynchronously retrieves the author details for the specified identifier.
    /// </summary>
    /// <param name="atProtocol">The ATProtocol instance used to perform the operation.</param>
    /// <param name="identifier">The identifier of the author whose details are to be retrieved.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an Author object.</returns>
    /// <exception cref="WhtWndReaderException">Thrown when the author details cannot be retrieved.</exception>
    public static async Task<Author> GetAuthorAsync(this ATProtocol atProtocol, ATIdentifier identifier, CancellationToken? cancellationToken = default)
    {
        var (profile, error) = await atProtocol.Actor.GetProfileAsync(identifier, cancellationToken: cancellationToken ?? CancellationToken.None);
        if (error is not null)
        {
            throw new WhtWndReaderException(error.Detail?.Message ?? "Failed to get author", error);
        }

        var author = new Author()
        {
            Id = profile!.Did!.ToString(),
            DisplayName = profile.DisplayName,
            Handle = profile.Handle!.Handle,
        };

        if (string.IsNullOrEmpty(profile.Avatar))
        {
            author.Avatar = Utilities.GetPlaceholderIcon();
            return author;
        }

        author.Avatar = await atProtocol.Client.GetByteArrayAsync(profile.Avatar, cancellationToken ?? CancellationToken.None);
        return author;
    }
}