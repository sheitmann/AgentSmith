using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Resources.Shell;

namespace AgentSmith {
	[ZoneMarker]
	public class ZoneMarker : IRequire<PsiFeaturesImplZone> {
		public ZoneMarker() {
		}
	}
}