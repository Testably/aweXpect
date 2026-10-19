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

			DotNetRun(s => s
				.SetConfiguration(Configuration)
				.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
				.EnableNoBuild()
				.SetProjectFile(project)
				.SetProcessAdditionalArguments(
					"--",
					"--disable-logo",
					$"--results-directory \"{TestResultsDirectory}\"",
					"--report-trx",
					$"--report-trx-filename {project.Name}.trx"));
		});
}
