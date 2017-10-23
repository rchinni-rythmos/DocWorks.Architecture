using DocWorks.GDocFactory.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.GDocFactory.Services
{
    interface IGDriveClient
    {
        string CreateChildFolderOfRoot(string folderName);

        string CreateChildFolder(string folderName, string parentFolderId);

        string CreateDocumentInFolder(string documentName, string parentFolderId);

        string CreateDocumentInFolder(string documentName, string content, string parentFolderId);
    }
}
