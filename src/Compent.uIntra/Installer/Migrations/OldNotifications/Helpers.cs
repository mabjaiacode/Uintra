﻿using System;
using uIntra.Core.Extensions;

namespace Compent.uIntra.Installer.Migrations
{
    internal static class Helpers
    {
        private static int _guidLength = Guid.Empty.ToString().Length;

        internal static Guid ParseIdFromQueryString(this string url, string afterSubstring)
        {
            var id = url.SubstringAfter(afterSubstring).Take(_guidLength);
            return Guid.Parse(id);
        }
    }
}