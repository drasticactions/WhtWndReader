// <copyright file="TaskExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using BigTed;
using Microsoft.Extensions.Logging;

namespace WhtWndReader;

/// <summary>
/// Task Extensions.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Run Task with Progress Hud.
    /// </summary>
    /// <param name="task">Task to run.</param>
    /// <param name="message">Message to display.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>Task.</returns>
    public static Task WithProgressHud(this Task task, string? message = null, ILogger? logger = null)
    {
        BTProgressHUD.ShowContinuousProgress(message, MaskType.Black);
        return task.ContinueWith(t =>
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                BTProgressHUD.Dismiss();
                if (t.IsFaulted)
                {
                    var errorString = $"{t.Exception.InnerException?.Message ?? t.Exception.Message}";
                    logger?.LogError(t.Exception, errorString);
                    BTProgressHUD.ShowErrorWithStatus(errorString, MaskType.Black, 3000);
                }
            });
        });
    }

    /// <summary>
    /// Fire and Forget Safe Async.
    /// </summary>
    /// <param name="task">Task to Fire and Forget.</param>
    /// <param name="handler">Error Handler.</param>
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
    public static async void FireAndForgetSafeAsync(this Task task, ILogger? handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            handler?.LogError(ex, "An error occured: {0}", ex.Message);
        }
    }
}