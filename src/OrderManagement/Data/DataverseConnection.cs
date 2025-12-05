using Microsoft.PowerPlatform.Dataverse.Client;

namespace OrderManagement.Data;

/// <summary>
/// Configuration settings for Dataverse connection.
/// </summary>
public class DataverseSettings
{
    /// <summary>
    /// The URL of the Dataverse environment (e.g., https://orgname.crm.dynamics.com).
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The Azure AD Application (Client) ID for authentication.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Azure AD Application Client Secret for authentication.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The Azure AD Tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
}

/// <summary>
/// Interface for Dataverse connection management.
/// </summary>
public interface IDataverseConnection
{
    /// <summary>
    /// Gets the Dataverse service client instance.
    /// </summary>
    ServiceClient Client { get; }

    /// <summary>
    /// Checks if the connection to Dataverse is active.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Establishes a connection to Dataverse.
    /// </summary>
    Task ConnectAsync();
}

/// <summary>
/// Manages the connection to Microsoft Dataverse.
/// </summary>
public class DataverseConnection : IDataverseConnection, IDisposable
{
    private readonly DataverseSettings _settings;
    private readonly ILogger<DataverseConnection> _logger;
    private ServiceClient? _client;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the DataverseConnection class.
    /// </summary>
    /// <param name="settings">The Dataverse configuration settings.</param>
    /// <param name="logger">The logger instance.</param>
    public DataverseConnection(DataverseSettings settings, ILogger<DataverseConnection> logger)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the Dataverse service client instance.
    /// </summary>
    public ServiceClient Client
    {
        get
        {
            if (_client == null || !_client.IsReady)
            {
                throw new InvalidOperationException("Dataverse connection is not established. Call ConnectAsync first.");
            }
            return _client;
        }
    }

    /// <summary>
    /// Checks if the connection to Dataverse is active.
    /// </summary>
    public bool IsConnected => _client?.IsReady ?? false;

    /// <summary>
    /// Establishes a connection to Dataverse using client credentials.
    /// </summary>
    public async Task ConnectAsync()
    {
        if (_client?.IsReady == true)
        {
            _logger.LogInformation("Dataverse connection is already established.");
            return;
        }

        try
        {
            _logger.LogInformation("Establishing connection to Dataverse at {Url}", _settings.Url);

            var connectionString = BuildConnectionString();

            // Create the service client asynchronously
            await Task.Run(() =>
            {
                _client = new ServiceClient(connectionString);
            });

            if (_client != null && _client.IsReady)
            {
                _logger.LogInformation("Successfully connected to Dataverse.");
            }
            else
            {
                var errorMessage = _client?.LastError ?? "Unknown error";
                _logger.LogError("Failed to connect to Dataverse: {Error}", errorMessage);
                throw new InvalidOperationException($"Failed to connect to Dataverse: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to Dataverse");
            throw;
        }
    }

    /// <summary>
    /// Builds the connection string for Dataverse.
    /// </summary>
    private string BuildConnectionString()
    {
        return $"AuthType=ClientSecret;Url={_settings.Url};ClientId={_settings.ClientId};ClientSecret={_settings.ClientSecret};TenantId={_settings.TenantId};RequireNewInstance=true";
    }

    /// <summary>
    /// Disposes the Dataverse connection.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _client?.Dispose();
            _client = null;
        }

        _disposed = true;
    }
}
