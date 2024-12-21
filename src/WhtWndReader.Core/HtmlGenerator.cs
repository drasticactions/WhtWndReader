// <copyright file="HtmlGenerator.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;
using System.Text;
using FishyFlip;
using FishyFlip.Lexicon;
using FishyFlip.Lexicon.App.Bsky.Actor;
using FishyFlip.Lexicon.Com.Whtwnd.Blog;
using FishyFlip.Models;
using FishyFlip.Tools;
using Markdig;
using Markdig.Extensions.AutoLinks;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using WhtWndReader.Exceptions;

namespace WhtWndReader;

/// <summary>
/// Html Generator.
/// </summary>
public static class HtmlGenerator
{
    /// <summary>
    /// Generates the empty html.
    /// </summary>
    /// <returns>Empty HTML template.</returns>
    public static string GenerateEmptyHtml() => new EmptyTemplate().RenderAsync().Result;

    /// <summary>
    /// Generates the entry html.
    /// </summary>
    /// <param name="atProtocol">ATProtocol.</param>
    /// <param name="entry">Entry.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>String.</returns>
    public static async Task<string> GenerateEntryHtmlAsync(this ATProtocol atProtocol, Entry entry, CancellationToken? cancellationToken = default)
    {
        var document = Markdown.Parse(entry.Content ?? string.Empty);
        foreach (LinkInline link in document.Descendants<LinkInline>())
        {
            link.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }

        foreach (AutolinkInline link in document.Descendants<AutolinkInline>())
        {
            link.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }
        return await new PostTemplate(new PostTemplateModel() { Content = document.ToHtml() }).RenderAsync();
    }

    /// <summary>
    /// Generates the entry html with cached images.
    /// </summary>
    /// <param name="atProtocol">ATProtocol.</param>
    /// <param name="entry">Entry.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns>String.</returns>
    public static async Task<string> GenerateEntryHtmlWithCachedImagesAsync(this ATProtocol atProtocol, Entry entry, CancellationToken? cancellationToken = default)
    {
        var document = Markdown.Parse(entry.Content ?? string.Empty);
        var images = document.Descendants<LinkInline>().Where(n => n.IsImage);
        foreach (var image in images)
        {
            var imageResult = await atProtocol.Client.GetByteArrayAsync(image.Url, cancellationToken ?? CancellationToken.None);
            var base64 = Convert.ToBase64String(imageResult);
            image.Url = $"data:image/png;base64,{base64}";
        }

        foreach (LinkInline link in document.Descendants<LinkInline>())
        {
            link.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }

        foreach (AutolinkInline link in document.Descendants<AutolinkInline>())
        {
            link.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }

        return await new PostTemplate(new PostTemplateModel() { Content = document.ToHtml() }).RenderAsync();
    }
}