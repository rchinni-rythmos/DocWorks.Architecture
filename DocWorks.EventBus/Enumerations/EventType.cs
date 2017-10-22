﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DocWorks.BuildingBlocks.EventBus.Enumerations
{
    public enum EventType
    {
        Request = 1,
        ResponseSuccess = 2,
        ResponseFailure = 3
    }
}
