using System.Runtime.CompilerServices;
using aweXpect.Customization;

namespace aweXpect.Snapshots.Orphans.Tests.TestHelpers;

/// <summary>
///     Disables accepting snapshots, so that the tests do not depend on <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> in the
///     environment of the test run.
/// </summary>
internal static class IgnoreAcceptEnvironment
{
	[ModuleInitializer]
	internal static void Initialize()
		=> Customize.aweXpect.Global.Snapshots().AcceptChanges.Set(false);
}
