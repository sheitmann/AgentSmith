using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Resources.Shell;

namespace AgentSmith {
	[ZoneMarker]
	public class ZoneMarker : IRequire<IAgentSmithZone> {
		public ZoneMarker() {
		}
	}
}