// <copyright file="ThirdPartyViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Masonry;
using WebKit;

namespace WhtWndReader;

/// <summary>
/// Third Party View Controller.
/// </summary>
public sealed class ThirdPartyViewController : UIViewController
{
    private readonly ThirdPartyWebview webView;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThirdPartyViewController"/> class.
    /// </summary>
    public ThirdPartyViewController()
    {
        var config = new WKWebViewConfiguration();
        this.Title = NSBundle.MainBundle.GetLocalizedString("Third Party Licenses", "Third Party Licenses");
        this.webView = new ThirdPartyWebview(CGRect.Empty, config);
        this.View!.AddSubview(this.webView);

        this.webView.TranslatesAutoresizingMaskIntoConstraints = false;
        this.webView.MakeConstraints(x =>
        {
            x.Top.EqualTo(this.View.SafeAreaLayoutGuideTop());
            x.Bottom.EqualTo(this.View.SafeAreaLayoutGuideBottom());
            x.Leading.EqualTo(this.View.SafeAreaLayoutGuideLeading());
            x.Trailing.EqualTo(this.View.SafeAreaLayoutGuideTrailing());
        });

        this.webView.SetSource(new ThirdPartyTemplate().Render());
    }

    private class ThirdPartyWebview : WKWebView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdPartyWebview"/> class.
        /// </summary>
        /// <param name="frame">CGRect Frame.</param>
        /// <param name="configuration">Webview Configuration.</param>
        public ThirdPartyWebview(CGRect frame, WKWebViewConfiguration configuration)
            : base(frame, configuration)
        {
            this.NavigationDelegate = new CustomWebViewNavigationDelegate();
        }

        /// <summary>
        /// Set source on webview.
        /// </summary>
        /// <param name="html">Html.</param>
        public void SetSource(string html)
        {
            this.InvokeOnMainThread(() => this.LoadHtmlString(new NSString(html), null));
        }

        public class CustomWebViewNavigationDelegate : WKNavigationDelegate
        {
            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                var url = navigationAction.Request.Url;
                var navigationType = navigationAction.NavigationType;

                // Check if it's an external link
                if (navigationType == WKNavigationType.LinkActivated)
                {
                    // Check if the URL is not part of your domain/website
                    UIApplication.SharedApplication.OpenUrl(url, new UIApplicationOpenUrlOptions() { OpenInPlace = false }, null);
                    decisionHandler(WKNavigationActionPolicy.Cancel);
                    return;
                }

                // Allow internal navigation
                decisionHandler(WKNavigationActionPolicy.Allow);
            }
        }
    }
}