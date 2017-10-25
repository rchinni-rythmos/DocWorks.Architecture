using DocWorks.GDocFactory.Configuration;
using DocWorks.GDocFactory.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocWorks.GDocFactory.Services
{
    public class GDriveClient : IGDriveClient
    {
        private const string GDriveFolderMimeType = "application/vnd.google-apps.folder";
        private const string GDriveDocumentMimeType = "application/vnd.google-apps.document";
        // private readonly IHostingEnvironment _hostingEnvironment;
        private readonly GDriveFactoryAppSettings _gdriveFactorySettings = null;

        public GDriveClient(GDriveFactoryAppSettings gdriveFactorySettings)
        {
            this._gdriveFactorySettings = gdriveFactorySettings;
        }

        public string CreateChildFolderOfRoot(string directoryTitle)
        {
            // TODO - think if GDriveFile Model is needed.
            // Can we just use the "File" class within GDrive API?
            var objGDriveFile = new GDriveFile
            {
                Parent = this._gdriveFactorySettings.RootFolderId,
                MimeType = GDriveFolderMimeType,
                Title = directoryTitle,
                Description = string.Empty,
            };

            return this.CreateFolder(objGDriveFile);
        }

        public string CreateChildFolder(string folderName, string parentFolderId)
        {
            var objGDriveFile = new GDriveFile
            {
                Parent = parentFolderId,
                MimeType = GDriveFolderMimeType,
                Title = folderName,
                Description = string.Empty,
            };

            return this.CreateFolder(objGDriveFile);
        }

        public string CreateDocumentInFolder(string documentName, string parentFolderId)
        {
            throw new NotImplementedException();
        }

        public string CreateDocumentInFolder(string documentName, string content, string parentFolderId)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content ?? ""));
            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File
            {
                Name = documentName,
                Description = string.Empty,
                MimeType = GDriveDocumentMimeType,
                Parents = new List<string>() { parentFolderId }
            };
            FilesResource.CreateMediaUpload request = null;
            using (DriveService driveService = this.AuthenticateServiceAccount())
            {
                request = driveService.Files.Create(body, stream, "text/html");
                request.Fields = "*";
                request.Upload();
            }
            return request.ResponseBody.Id;
        }

        private string CreateFolder(GDriveFile objGDriveFile)
        {
            string newFileId = null;

            // Create metaData for a new Directory
            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File
            {
                Name = objGDriveFile.Title,
                Description = objGDriveFile.Description,
                MimeType = objGDriveFile.MimeType,
                Parents = new List<string>() { objGDriveFile.Parent },
            };

            using (DriveService service = this.AuthenticateServiceAccount())
            {
                Google.Apis.Drive.v3.Data.File newDirectory = null;
                FilesResource.CreateRequest request = service.Files.Create(body);
                request.Fields = "*";
                newDirectory = request.Execute();
                newFileId = newDirectory.Id;
            }

            return newFileId;
        }

        private DriveService AuthenticateServiceAccount()
        {
            string serviceAccountEmail = this._gdriveFactorySettings.ServiceAccountEmail;
            string serviceAccountCredentialFilePath = this._gdriveFactorySettings.ServiceAccountCredentialFilePath; // "\\GoogleDriverServer-ba41e5383e7b.json";
            DriveService ds = null;
            string[] scopes = new string[] { DriveService.Scope.Drive }; // Full access
                                                                         // string webPath = _hostingEnvironment.ContentRootPath;
            string webPath = Directory.GetCurrentDirectory();

            // TODO: Pavan: Think of placing this json file in a better location
            if (Path.GetExtension(serviceAccountCredentialFilePath).ToLower() == ".json")
            {
                GoogleCredential credential;
                using (var stream = new FileStream(webPath + string.Empty + serviceAccountCredentialFilePath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                         .CreateScoped(scopes);
                }

                ds = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                });
            }
            else
            {
                throw new Exception("Unsupported Service accounts credentials.");
            }

            return ds;
        }

    }
}
