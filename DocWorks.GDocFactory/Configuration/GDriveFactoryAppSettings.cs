using System.Diagnostics.CodeAnalysis;

namespace DocWorks.GDocFactory.Configuration
{
    [SuppressMessage("Microsoft.CodeQuality.Analyzers", "CA1052:Type 'GDriveFactoryAppSettings' is a static holder type but is neither static nor NotInheritable", Justification = "Class used by configuration section for appsettings")]
    public class GDriveFactoryAppSettings
    {
        public string ServiceAccountEmail { get; set; }

        public string ServiceAccountCredentialFilePath { get; set; }

        public string RootFolderId { get; set; }
    }
}