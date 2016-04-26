using System;
using System.Collections.Generic;

namespace AgentSmith.MemberMatch
{
    public class AccessLevelDescription
    {
        public readonly AccessLevels AccessLevel;
        public readonly string Name;
        public readonly string Description;
        public readonly bool ShowEffective;
        public readonly bool ShowNotEffective;

        public AccessLevelDescription(AccessLevels accessLevel, string name, string description, bool showEffective, bool showNotEffective)
        {
            AccessLevel = accessLevel;
            Name = name;
            Description = description;
            ShowEffective = showEffective;
            ShowNotEffective = showNotEffective;
        }

        public static Dictionary<AccessLevels, AccessLevelDescription> Descriptions
        {
            get
            {
                Dictionary<AccessLevels, AccessLevelDescription> descrs = new Dictionary<AccessLevels, AccessLevelDescription>();
                descrs.Add(AccessLevels.Any, new AccessLevelDescription(AccessLevels.Any, "Any", "Any possible visibility.", true, true));
                descrs.Add(AccessLevels.Internal, new AccessLevelDescription(AccessLevels.Internal, "Internal", "Visible within the same assembly.", true, true));
                descrs.Add(AccessLevels.Private, new AccessLevelDescription(AccessLevels.Private, "Private", "Visible only from the same class.", true, true));
                descrs.Add(AccessLevels.Protected, new AccessLevelDescription(AccessLevels.Protected, "Protected", "Visible to subclasses.", true, true));
                descrs.Add(AccessLevels.ProtectedInternal, new AccessLevelDescription(AccessLevels.ProtectedInternal, "Protected Internal", "Visible to subclasses as well as from everywhere within the same assembly.", true, true));
                descrs.Add(AccessLevels.ProtectedAndInternal, new AccessLevelDescription(AccessLevels.ProtectedAndInternal, "Protected and Internal", "Visible to subclasses within the same assembly. Ex: protected member of internal class.", true, false));
                descrs.Add(AccessLevels.Public, new AccessLevelDescription(AccessLevels.Public, "Public", "Visible from everywhere.", true, true));
                return descrs;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}