using Fallout.Common;
using Fallout.Solutions;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	Target ApiChecks => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			Project project = Solution.Tests.aweXpect_Api_Tests;

			// Under `CoreOnly` only the Debug build of aweXpect references the changed aweXpect.Core.
			DotNetTest(s => s
				.SetConfiguration(Configuration == Configuration.Debug || BuildScope == BuildScope.CoreOnly
					? "Debug"
					: "Release")
				.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
				.EnableNoBuild()
				.SetResultsDirectory(TestResultsDirectory)
				.SetProjectFile(project)
				.AddLoggers($"trx;LogFileName={project.Name}.trx"));
		});
}
