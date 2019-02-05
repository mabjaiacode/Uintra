﻿using System;
using LanguageExt;
using Uintra.Core.Permissions.Models;

namespace Uintra.Core.Permissions
{
    public interface IPermissionsService
    {
        EntityPermissions Get(Guid entityId);
        EntityGroupPermissions GetForGroup(Guid ermissionTargetId, IntranetMemberGroup role);
        bool Has(Guid entityId, IntranetMemberGroup role, Enum[] permissions);
        Unit Save(EntityPermissions permissions);
        Unit Save(EntityGroupPermissions permissions);
        Unit Add(Guid entityId, IntranetMemberGroup role, Enum[] permissions);
        Unit Remove(Guid entityId, IntranetMemberGroup role, Enum[] permissions);
    }
}