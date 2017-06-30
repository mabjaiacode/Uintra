﻿using System.Collections.Generic;
using uIntra.Core.Activity;

namespace uIntra.Core.User.Permissions
{
    public interface IPermissionsService
    {
        bool IsRoleHasPermissions(IRole role, params string[] permissions);
        IEnumerable<string> GetRolePermission(IRole role);
        string GetPermissionFromTypeAndAction(IActivityType activityType, IntranetActivityActionEnum action);

        bool IsCurrentUserHasAccess(IActivityType activityType, IntranetActivityActionEnum action);
        bool IsUserHasAccess(IIntranetUser user, IActivityType activityType, IntranetActivityActionEnum action);
        bool IsUserWebmaster(IIntranetUser user);
    }
}