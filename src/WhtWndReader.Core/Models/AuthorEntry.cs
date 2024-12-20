// <copyright file="AuthorEntry.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using SQLite;

namespace WhtWndReader.Models;

/// <summary>
/// WhiteWind Author Entry.
/// </summary>
public class AuthorEntry
{

    /// <summary>
    /// Gets the ID.
    /// The ID author entry is the ATUri for the given entry, and should be unique.
    /// </summary>
    [PrimaryKey]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the author ID.
    /// The author ID is the ATProtocol DID value for the author of the entry.
    /// </summary>
    [Indexed]
    public string AuthorId { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the entry.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the created at date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the content of the entry.
    /// The content is the markdown content of the entry.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the HTML content of the entry.
    /// </summary>
    public string? Html { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the entry.
    /// </summary>
    public string? Visibility { get; set; }
}