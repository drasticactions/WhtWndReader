// <copyright file="SceneDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace WhtWndReader;

/// <summary>
/// Scene delegate.
/// </summary>
[Register("SceneDelegate")]
public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    /// <summary>
    /// Gets or sets the window.
    /// </summary>
    [Export("window")]
    public UIWindow? Window { get; set; }

    /// <summary>
    /// Called when the scene is about to connect to a session.
    /// Use this method to optionally configure and attach the UIWindow `window` to the provided UIWindowScene `scene`.
    /// If using a storyboard, the `window` property will automatically be initialized and attached to the scene.
    /// This delegate does not imply the connecting scene or session are new (see UIApplicationDelegate `GetConfiguration` instead).
    /// </summary>
    /// <param name="scene">The scene object being connected.</param>
    /// <param name="session">The session object containing details about the scene's session.</param>
    /// <param name="connectionOptions">Additional options for configuring the scene connection.</param>
    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
    {
        // Implementation here
    }

    /// <summary>
    /// Called as the scene is being released by the system.
    /// This occurs shortly after the scene enters the background, or when its session is discarded.
    /// Release any resources associated with this scene that can be re-created the next time the scene connects.
    /// The scene may re-connect later, as its session was not necessarily discarded (see UIApplicationDelegate `DidDiscardSceneSessions` instead).
    /// </summary>
    /// <param name="scene">The scene object being disconnected.</param>
    [Export("sceneDidDisconnect:")]
    public void DidDisconnect(UIScene scene)
    {
        // Implementation here
    }

    /// <summary>
    /// Called when the scene has moved from an inactive state to an active state.
    /// Use this method to restart any tasks that were paused (or not yet started) when the scene was inactive.
    /// </summary>
    /// <param name="scene">The scene object that became active.</param>
    [Export("sceneDidBecomeActive:")]
    public void DidBecomeActive(UIScene scene)
    {
        // Implementation here
    }

    /// <summary>
    /// Called when the scene will move from an active state to an inactive state.
    /// This may occur due to temporary interruptions (e.g., an incoming phone call).
    /// </summary>
    /// <param name="scene">The scene object that will resign active state.</param>
    [Export("sceneWillResignActive:")]
    public void WillResignActive(UIScene scene)
    {
        // Implementation here
    }

    /// <summary>
    /// Called as the scene transitions from the background to the foreground.
    /// Use this method to undo the changes made on entering the background.
    /// </summary>
    /// <param name="scene">The scene object that will enter the foreground.</param>
    [Export("sceneWillEnterForeground:")]
    public void WillEnterForeground(UIScene scene)
    {
        // Implementation here
    }

    /// <summary>
    /// Called as the scene transitions from the foreground to the background.
    /// Use this method to save data, release shared resources, and store enough scene-specific state information
    /// to restore the scene back to its current state.
    /// </summary>
    /// <param name="scene">The scene object that entered the background.</param>
    [Export("sceneDidEnterBackground:")]
    public void DidEnterBackground(UIScene scene)
    {
        // Implementation here
    }
}