using System.Runtime.CompilerServices;
using aweXpect.Customization;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     Disables accepting snapshots for all tests, so that they do not depend on <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> in the
///     environment of the test run; <see cref="EnvironmentScope" /> enables the environment again for a single test.
/// </summary>
internal static class IgnoreAcceptEnvironment
{
	[ModuleInitializer]
	internal static void Initialize()
		=> Customize.aweXpect.Global.Snapshots().AcceptChanges.Set(false);
}
