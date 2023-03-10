using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rv.BitTorrentActors.UnitTests.Utils;

public static class EmbeddedResources
{
    public static Stream GetResourceStream(Assembly assembly, string searchName)
    {
        string resourceName = GetResourceNameOrThrow(assembly, searchName);
        Stream result = GetManifestResourceStreamOrThrow(assembly, resourceName);
        return result;
    }

    public static string ReadResourceAsString(Assembly assembly, string searchName)
    {
        string resourceName = GetResourceNameOrThrow(assembly, searchName);
        using Stream stream = GetManifestResourceStreamOrThrow(assembly, resourceName);
        using StreamReader reader = new StreamReader(stream);
        
        string result = reader.ReadToEnd();
        return result;
    }

    private static Stream GetManifestResourceStreamOrThrow(Assembly assembly, string resourceName)
    {
        Stream? result = assembly.GetManifestResourceStream(resourceName);

        if (result is null)
            throw new InvalidOperationException(
                $"Could not get stream for '{resourceName}' in assembly '{assembly.FullName}'.");

        return result;
    }

    private static string GetResourceNameOrThrow(Assembly assembly, string searchName)
    {
        string? result = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(n => n.Contains(searchName));

        if (result is null)
            throw new InvalidOperationException(
                $"Resource '{searchName}' not found in assembly '{assembly.FullName}'.");

        return result;
    }
}