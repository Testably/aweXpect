using Fallout.Common;
using Fallout.Solutions;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	Target ApiChecks => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			Project[] projects = BuildScope switch
			{
				BuildScope.CoreOnly => [Solution.Tests.aweXpect_Core_Api_Tests,],
				BuildScope.MainOnly => [Solution.Tests.aweXpect_Api_Tests,],
				_ => [Solution.Tests.aweXpect_Core_Api_Tests, Solution.Tests.aweXpect_Api_Tests,],
			};

			DotNetTest(s => s
				.SetConfiguration(Configuration == Configuration.Debug || BuildScope == BuildScope.CoreOnly
					? "Debug"
					: "Release")
				.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
				.EnableNoBuild()
				.SetResultsDirectory(TestResultsDirectory)
				.CombineWith(
					projects,
					(settings, project) => settings
						.SetProjectFile(project)
						.AddLoggers($"trx;LogFileName={project.Name}.trx")), completeOnFailure: true);
		});
}
