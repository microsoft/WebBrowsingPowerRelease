using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EdgeBrowsingPower.Services;

public static class CredentialsUpdater
{
    private const string TargetScenarioName = "OutlookLogin";

    public static void Update(string credentialsPath, string msaAccountId, string password)
    {
        string json = File.ReadAllText(credentialsPath);
        JsonNode root = JsonNode.Parse(json) ?? throw new JsonException("Credentials JSON is empty.");

        bool usernameUpdated = false;
        bool passwordUpdated = false;

        UpdateNode(root, msaAccountId, password, ref usernameUpdated, ref passwordUpdated);

        if (!usernameUpdated || !passwordUpdated)
        {
            throw new EdgeBrowsingPowerException("The credentials file does not contain username and password fields for OutlookLogin.");
        }

        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        File.WriteAllText(credentialsPath, root.ToJsonString(options));
    }

    private static void UpdateNode(
        JsonNode node,
        string msaAccountId,
        string password,
        ref bool usernameUpdated,
        ref bool passwordUpdated)
    {
        if (node is JsonObject jsonObject)
        {
            if (IsTargetScenario(jsonObject))
            {
                UpdateCredentialsInNode(jsonObject, msaAccountId, password, ref usernameUpdated, ref passwordUpdated);
                return;
            }

            foreach (KeyValuePair<string, JsonNode?> property in jsonObject)
            {
                if (property.Value is not null)
                {
                    UpdateNode(property.Value, msaAccountId, password, ref usernameUpdated, ref passwordUpdated);
                }
            }

            return;
        }

        if (node is JsonArray jsonArray)
        {
            foreach (JsonNode? child in jsonArray)
            {
                if (child is not null)
                {
                    UpdateNode(child, msaAccountId, password, ref usernameUpdated, ref passwordUpdated);
                }
            }
        }
    }

    private static bool IsTargetScenario(JsonObject jsonObject)
    {
        return jsonObject.TryGetPropertyValue("ScenarioName", out JsonNode? scenarioNameNode)
            && scenarioNameNode is JsonValue scenarioNameValue
            && scenarioNameValue.TryGetValue(out string? scenarioName)
            && string.Equals(scenarioName, TargetScenarioName, StringComparison.Ordinal);
    }

    private static void UpdateCredentialsInNode(
        JsonNode node,
        string msaAccountId,
        string password,
        ref bool usernameUpdated,
        ref bool passwordUpdated)
    {
        if (node is JsonObject jsonObject)
        {
            foreach (KeyValuePair<string, JsonNode?> property in jsonObject.ToList())
            {
                if (property.Key.Equals("username", StringComparison.OrdinalIgnoreCase))
                {
                    jsonObject[property.Key] = JsonValue.Create(msaAccountId);
                    usernameUpdated = true;
                    continue;
                }

                if (property.Key.Equals("password", StringComparison.OrdinalIgnoreCase))
                {
                    jsonObject[property.Key] = JsonValue.Create(password);
                    passwordUpdated = true;
                    continue;
                }

                if (property.Value is not null)
                {
                    UpdateCredentialsInNode(property.Value, msaAccountId, password, ref usernameUpdated, ref passwordUpdated);
                }
            }

            return;
        }

        if (node is JsonArray jsonArray)
        {
            foreach (JsonNode? child in jsonArray)
            {
                if (child is not null)
                {
                    UpdateCredentialsInNode(child, msaAccountId, password, ref usernameUpdated, ref passwordUpdated);
                }
            }
        }
    }
}