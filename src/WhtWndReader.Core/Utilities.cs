using System.Reflection;

namespace WhtWndReader;

/// <summary>
/// Helper Utilities.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Get the Default Placeholder Icon.
    /// </summary>
    /// <returns>Image Byte Array.</returns>
    /// <exception cref="Exception">Thrown if can't get the image.</exception>
    public static byte[] GetPlaceholderIcon()
    {
        var resource = GetResourceFileContent("Assets.logo.png");
        if (resource is null)
        {
            throw new Exception("Failed to get placeholder icon.");
        }

        using MemoryStream ms = new MemoryStream();
        resource.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Get Resource File Content via FileName.
    /// </summary>
    /// <param name="fileName">Filename.</param>
    /// <returns>Stream.</returns>
    private static Stream? GetResourceFileContent(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "WhtWndReader.iOS." + fileName;

        return assembly.GetManifestResourceStream(resourceName);
    }
}