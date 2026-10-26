using Fallout.Common;

// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	/// <summary>
	///     The docs do not depend on the platform, so they run separately from the <see cref="UnitTests" />, which run
	///     on every platform.
	/// </summary>
	Target DocsTests => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			RunUnitTests([Solution.Tests.aweXpect_Docs_Tests,],
				framework => framework != NetFramework || EnvironmentInfo.IsWin);
		});
}
