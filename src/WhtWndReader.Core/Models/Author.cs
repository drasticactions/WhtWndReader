// <copyright file="Author.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using SQLite;

namespace WhtWndReader.Models;

/// <summary>
/// WhiteWind Author.
/// </summary>
public class Author
{
    /// <summary>
    /// Gets or sets the ID.
    /// The ID is an ATProtocol DID value. There should only be one author per DID.
    /// </summary>
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the handle.
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    /// Gets or sets the avatar.
    /// </summary>
    public byte[]? Avatar { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the author is a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }
}