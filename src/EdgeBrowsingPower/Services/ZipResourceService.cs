using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace EdgeBrowsingPower.Services;

public sealed class ZipResourceService
{
    private const string ArchiveFileName = "EdgeBrowsingPowerV2.0.15.zip";
    private const string ArchivePartSearchPattern = ArchiveFileName + ".*";
    private const string CredentialsFileName = "parameters.abl.credentials.json";
    private const string DocumentationFileName = "Browsing_BatteryLife_v2.0.15_and_YouTubeToS_v1.0.2.pdf";

    public void Create(string destinationFolder, string msaAccountId, string password)
    {
        if (string.IsNullOrWhiteSpace(destinationFolder))
        {
            throw new EdgeBrowsingPowerException("Choose a destination folder before creating files.");
        }

        ExtractBundledArchive(destinationFolder);
        CopyBundledDocumentation(destinationFolder);

        string credentialsPath = FindCredentialsFile(destinationFolder);
        UpdateCredentials(credentialsPath, msaAccountId, password);
    }

    private static void ExtractBundledArchive(string destinationFolder)
    {
        string? temporaryArchivePath = null;

        try
        {
            using Stream archiveStream = OpenBundledArchive(out temporaryArchivePath)
                ?? throw new EdgeBrowsingPowerException("The bundled deployment archive could not be found.");

            Directory.CreateDirectory(destinationFolder);
            ZipFile.ExtractToDirectory(archiveStream, destinationFolder, overwriteFiles: true);
        }
        catch (InvalidDataException ex)
        {
            throw new EdgeBrowsingPowerException("The bundled deployment archive could not be opened.", ex);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new EdgeBrowsingPowerException("The bundled deployment archive could not be extracted to the selected folder.", ex);
        }
        finally
        {
            if (temporaryArchivePath is not null)
            {
                TryDeleteTemporaryArchive(temporaryArchivePath);
            }
        }
    }

    private static void TryDeleteTemporaryArchive(string temporaryArchivePath)
    {
        try
        {
            File.Delete(temporaryArchivePath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }
    }

    private static Stream? OpenBundledArchive(out string? temporaryArchivePath)
    {
        temporaryArchivePath = null;

        string archivePath = Path.Combine(AppContext.BaseDirectory, ArchiveFileName);
        if (File.Exists(archivePath))
        {
            return File.OpenRead(archivePath);
        }

        string[] archivePartPaths = Directory
            .GetFiles(AppContext.BaseDirectory, ArchivePartSearchPattern)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (archivePartPaths.Length == 0)
        {
            return null;
        }

        temporaryArchivePath = Path.Combine(
            Path.GetTempPath(),
            $"{Path.GetFileNameWithoutExtension(ArchiveFileName)}-{Guid.NewGuid():N}.zip");

        using (FileStream temporaryArchiveStream = File.Create(temporaryArchivePath))
        {
            foreach (string archivePartPath in archivePartPaths)
            {
                using FileStream archivePartStream = File.OpenRead(archivePartPath);
                archivePartStream.CopyTo(temporaryArchiveStream);
            }
        }

        return File.OpenRead(temporaryArchivePath);
    }

    private static void CopyBundledDocumentation(string destinationFolder)
    {
        string sourcePath = Path.Combine(AppContext.BaseDirectory, DocumentationFileName);
        string destinationPath = Path.Combine(destinationFolder, DocumentationFileName);

        try
        {
            if (!File.Exists(sourcePath))
            {
                throw new EdgeBrowsingPowerException("The bundled documentation file could not be found.");
            }

            File.Copy(sourcePath, destinationPath, overwrite: true);
        }
        catch (EdgeBrowsingPowerException)
        {
            throw;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new EdgeBrowsingPowerException("The bundled documentation file could not be copied to the selected folder.", ex);
        }
    }

    private static string FindCredentialsFile(string destinationFolder)
    {
        try
        {
            string? credentialsPath = Directory
                .EnumerateFiles(destinationFolder, CredentialsFileName, SearchOption.AllDirectories)
                .FirstOrDefault();

            return credentialsPath
                ?? throw new EdgeBrowsingPowerException("The extracted files do not include parameters.abl.credentials.json.");
        }
        catch (EdgeBrowsingPowerException)
        {
            throw;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new EdgeBrowsingPowerException("The extracted credentials file could not be located.", ex);
        }
    }

    private static void UpdateCredentials(string credentialsPath, string msaAccountId, string password)
    {
        try
        {
            CredentialsUpdater.Update(credentialsPath, msaAccountId, password);
        }
        catch (EdgeBrowsingPowerException)
        {
            throw;
        }
        catch (JsonException ex)
        {
            throw new EdgeBrowsingPowerException("The credentials file could not be parsed as valid JSON.", ex);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new EdgeBrowsingPowerException("The credentials file could not be updated.", ex);
        }
    }
}