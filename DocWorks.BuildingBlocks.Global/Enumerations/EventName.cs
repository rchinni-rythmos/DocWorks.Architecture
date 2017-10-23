using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.Global.Enumerations
{
    public enum EventName
    {
        #region Core Events [101 to 200]
        CoreCreateProject =101,
        CoreGetProjects=102,
        SetProjectComplete=103,
        #endregion

        #region GDrive Events [201 to 300]
        GDriveCreateProject = 201,
        #endregion

        #region SourceSync Events [301 to 400]
        SourceSyncValidateRepository = 301,
        SourceSyncGetRepositoryBranches = 302
        #endregion
    }
}
