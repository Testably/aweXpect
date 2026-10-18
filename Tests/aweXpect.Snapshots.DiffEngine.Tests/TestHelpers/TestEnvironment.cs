using System.Runtime.CompilerServices;
using aweXpect.Customization;
using DiffEngine;

namespace aweXpect.Snapshots.DiffEngine.Tests.TestHelpers;

/// <summary>
///     Turns DiffEngine off, so that no diff tool is ever started by the tests, and disables accepting snapshots, so
///     that the tests do not depend on <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> in the environment of the test run.
/// </summary>
internal static class TestEnvironment
{
	[ModuleInitializer]
	internal static void Initialize()
	{
		DiffRunner.Disabled = true;
		Customize.aweXpect.Global.Snapshots().Acceptance.Set(SnapshotAcceptance.None);
	}
}
