using FishyFlip.Models;
using Masonry;
using Microsoft.Extensions.Logging;
using WebKit;
using WhtWndReader.Models;
using WhtWndReader.Services;

namespace WhtWndReader;

public sealed class EntryViewController : UIViewController
{
    private EntryWebview webView;
    private readonly BlogService blogService;
    private readonly ILogger logger;
    private AuthorEntry? entry;
    private readonly UIBarButtonItem shareButton;

    public EntryViewController(BlogService blogService, ILogger logger)
    {
        this.blogService = blogService;
        this.logger = logger;
        this.View!.BackgroundColor = UIColor.SystemBackground;
        this.webView = new EntryWebview(CGRect.Empty, new WKWebViewConfiguration());
        this.View.AddSubview(this.webView);

        this.shareButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
        this.NavigationItem.RightBarButtonItem = this.shareButton;
        this.shareButton.Clicked += this.ShareButton_Clicked;

        this.webView.TranslatesAutoresizingMaskIntoConstraints = false;
        this.webView.MakeConstraints(x =>
        {
            x.Top.EqualTo(this.View.SafeAreaLayoutGuideTop());
            x.Bottom.EqualTo(this.View.SafeAreaLayoutGuideBottom());
            x.Leading.EqualTo(this.View.SafeAreaLayoutGuideLeading());
            x.Trailing.EqualTo(this.View.SafeAreaLayoutGuideTrailing());
        });

        this.webView.SetSource(HtmlGenerator.GenerateEmptyHtml());
    }

    /// <summary>
    /// Load entry into webview.
    /// </summary>
    /// <param name="entry">Entry.</param>
    public void LoadEntry(AuthorEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (this.entry == entry)
        {
            return;
        }

        this.entry = entry;
        this.Title = entry.Title;
        var content = this.entry.Html ?? HtmlGenerator.GenerateEmptyHtml();
        this.webView.SetSource(content);
    }

    private void ShareButton_Clicked(object? sender, EventArgs e)
    {
        if (this.entry == null)
        {
            return;
        }

        if (!ATUri.TryCreate(this.entry.Id, out var uri))
        {
            return;
        }

        var whtwndUrl = $"https://whtwnd.com/{uri!.Identity}/{uri.Rkey}";
        var activityItems = new NSObject[] { new NSString(this.entry.Title), new NSUrl(whtwndUrl) };
        var activityController = new UIActivityViewController(activityItems, null);
        if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
        {
            activityController.PopoverPresentationController!.SourceView = this.View!;
            activityController.PopoverPresentationController!.SourceItem = this.NavigationItem.RightBarButtonItem;
        }

        this.PresentViewController(activityController, true, null);
    }

    private class EntryWebview : WKWebView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntryWebview"/> class.
        /// </summary>
        /// <param name="frame">CGRect Frame.</param>
        /// <param name="configuration">Webview Configuration.</param>
        public EntryWebview(CGRect frame, WKWebViewConfiguration configuration)
            : base(frame, configuration)
        {
        }

        /// <summary>
        /// Set source on webview.
        /// </summary>
        /// <param name="html">Html.</param>
        public void SetSource(string html)
        {
            this.InvokeOnMainThread(() => this.LoadHtmlString(new NSString(html), null));
        }
    }
}