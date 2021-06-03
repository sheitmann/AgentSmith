﻿using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace AgentSmith.Test {

	[ZoneDefinition]
	public class AgentSmithTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IAgentSmithZone> { }

	[ZoneMarker]
	public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<AgentSmithTestEnvironmentZone> { }

	//[SetUpFixture]
	//public class AgentSmithTestsAssembly : ExtensionTestEnvironmentAssembly<AgentSmithTestEnvironmentZone> { }
}