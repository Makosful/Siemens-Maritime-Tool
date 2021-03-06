﻿using Schwartz.Siemens.Core.Entities.Rigs;

namespace Schwartz.Siemens.Core.ApplicationServices
{
    public interface IRigService : ICrudService<Rig>
    {
        Rig UpdateLocation(Rig imo);
    }
}