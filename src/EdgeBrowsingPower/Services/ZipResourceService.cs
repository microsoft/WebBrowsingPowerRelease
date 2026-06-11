using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace EdgeBrowsingPower.Services;

public sealed class ZipResourceService
{
    private const string ArchiveResourceName = "EdgeBrowsingPower.Resources.EdgeBrowsingPowerV2.0.11.zip";
    private const string CredentialsFileName = "parameters.abl.credentials.json";

    public void Create(string destinationFolder, string msaAccountId, string password)
    {
        if (string.IsNullOrWhiteSpace(destinationFolder))
        {
            throw new EdgeBrowsingPowerException("Choose a destination folder before creating files.");
        }

        ExtractBundledArchive(destinationFolder);

        string credentialsPath = FindCredentialsFile(destinationFolder);
        UpdateCredentials(credentialsPath, msaAccountId, password);
    }

    private static void ExtractBundledArchive(string destinationFolder)
    {
        Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ArchiveResourceName)
            ?? throw new EdgeBrowsingPowerException("The bundled deployment archive could not be found.");

        using (resourceStream)
        {
            try
            {
                Directory.CreateDirectory(destinationFolder);
                ZipFile.ExtractToDirectory(resourceStream, destinationFolder, overwriteFiles: true);
            }
            catch (InvalidDataException ex)
            {
                throw new EdgeBrowsingPowerException("The bundled deployment archive could not be opened.", ex);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new EdgeBrowsingPowerException("The bundled deployment archive could not be extracted to the selected folder.", ex);
            }
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