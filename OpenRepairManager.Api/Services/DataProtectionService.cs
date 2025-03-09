using Microsoft.AspNetCore.DataProtection;

namespace OpenRepairManager.Api.Services;

public static class DataProtectionService
{
    private const string APP_NAME = "2fe9577b-de99-4c88-bf58-3276f6ab7407";
    private const string SECRET_CONFIG_FILE_NAME = "appsettingsecrets.json";

    public static IDataProtector GetDataProtector()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDataProtection()
            .SetApplicationName(APP_NAME)
            .PersistKeysToFileSystem(new DirectoryInfo($"{CurrentDirectory}/secrets"));
        var services = serviceCollection.BuildServiceProvider();
        var dataProtectionProvider = services.GetService<IDataProtectionProvider>();
        return dataProtectionProvider.CreateProtector(APP_NAME);
    }
		
    public static string CurrentDirectory
    {
        get { return Directory.GetCurrentDirectory(); }
    }

    public static string ConfigFileFullPath
    {
        get { return Path.Combine(CurrentDirectory, SECRET_CONFIG_FILE_NAME); }
    }
}