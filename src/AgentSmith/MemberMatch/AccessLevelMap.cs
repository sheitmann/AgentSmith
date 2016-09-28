using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;

namespace AgentSmith.MemberMatch
{
    public static class AccessLevelMap
    {
        private static readonly Dictionary<AccessRights, AccessLevels> _rightsMap =
            new Dictionary<AccessRights, AccessLevels>();

        static AccessLevelMap()
        {
            _rightsMap.Add(AccessRights.INTERNAL, AccessLevels.Internal);
            _rightsMap.Add(AccessRights.PRIVATE, AccessLevels.Private);
            _rightsMap.Add(AccessRights.PROTECTED, AccessLevels.Protected);
            _rightsMap.Add(AccessRights.PROTECTED_AND_INTERNAL, AccessLevels.ProtectedAndInternal);
            _rightsMap.Add(AccessRights.PROTECTED_OR_INTERNAL, AccessLevels.ProtectedInternal);
            _rightsMap.Add(AccessRights.PUBLIC, AccessLevels.Public);
            _rightsMap.Add(AccessRights.NONE, AccessLevels.Public);
        }

        public static Dictionary<AccessRights, AccessLevels> Map
        {
            get { return _rightsMap; }
        }
    }
}