using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.GDocFactory.Services
{
    class GDriveClient : IGDriveClient
    {
        public string CreateFolder()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
