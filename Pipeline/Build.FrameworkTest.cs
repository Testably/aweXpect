using System.Linq;
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
	Project[] FrameworkUnitTestProjects =>
	[
		Solution.Tests.Frameworks.aweXpect_Frameworks_Fallback_Tests,
		Solution.Tests.Frameworks.aweXpect_Frameworks_MsTest3_Tests,
		Solution.Tests.Frameworks.aweXpect_Frameworks_MsTest4_Tests,
		Solution.Tests.Frameworks.aweXpect_Frameworks_NUnit5_Tests,
		Solution.Tests.Frameworks.aweXpect_Frameworks_NUnit4_Tests,
		Solution.Tests.Frameworks.aweXpect_Frameworks_NUnit3_Tests,
		Solution.Tests.Frameworks.aweXpect_Frameworks_Xunit2_Tests,
	];

	Target TestFrameworks => _ => _
		.DependsOn(VsTestFrameworks)
		.DependsOn(TestingPlatformFrameworks)
		.DependsOn(XunitTestingPlatformFrameworks);

	Target VsTestFrameworks => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.Executes(() =>
		{
			var testCombinations =
				from project in FrameworkUnitTestProjects
				let frameworks = project.GetTargetFrameworks()
				let supportedFrameworks = EnvironmentInfo.IsWin ? frameworks : frameworks.Except(["net48",])
				from framework in supportedFrameworks
				select new
				{
					project,
					framework,
				};

			DotNetTest(s =>
			{
				s = s
					.SetConfiguration(Configuration)
					.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
					.EnableNoBuild()
					.SetResultsDirectory(TestResultsDirectory);
				if (CollectCoverage)
				{
					s = s
						.SetDataCollector("XPlat Code Coverage")
						.AddRunSetting(
							"DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.DoesNotReturnAttribute",
							"DoesNotReturnAttribute");
				}

				return s.CombineWith(
					testCombinations,
					(settings, v) => settings
						.SetProjectFile(v.project)
						.SetFramework(v.framework)
						.AddLoggers($"trx;LogFileName={v.project.Name}_{v.framework}.trx"));
			}, completeOnFailure: true);
		});

	Target TestingPlatformFrameworks => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.Executes(() =>
		{
			Project[] projects =
			[
				Solution.Tests.Frameworks.aweXpect_Frameworks_Tunit_Core_Tests,
				Solution.Tests.Frameworks.aweXpect_Frameworks_Tunit_Tests,
			];

			var testCombinations =
				from project in projects
				let frameworks = project.GetTargetFrameworks()
				from framework in frameworks
				select new
				{
					project,
					framework,
				};

			DotNetRun(s => s
				.SetConfiguration(Configuration)
				.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
				.EnableNoBuild()
				.CombineWith(
					testCombinations,
					(settings, v) => settings
						.SetProjectFile(v.project)
						.SetFramework(v.framework)
						.SetProcessAdditionalArguments(
							[
								"--",
								"--disable-logo",
								"--report-trx",
								$"--report-trx-filename {v.project.Name}_{v.framework}.trx",
								$"--results-directory {TestResultsDirectory}",
								..CollectCoverage
									? (string[])["--coverage", "--coverage-output-format \"cobertura\"",]
									: [],
							]
						)
				)
			);
		});

	Target XunitTestingPlatformFrameworks => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.Executes(() =>
		{
			Project[] projects =
			[
				Solution.Tests.Frameworks.aweXpect_Frameworks_Xunit3_Core_Tests,
				Solution.Tests.Frameworks.aweXpect_Frameworks_Xunit3_Tests,
			];

			var testCombinations =
				from project in projects
				let frameworks = project.GetTargetFrameworks()
				from framework in frameworks
				select new
				{
					project,
					framework,
				};

			DotNetRun(s => s
				.SetConfiguration(Configuration)
				.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
				.EnableNoBuild()
				.CombineWith(
					testCombinations,
					(settings, v) => settings
						.SetProjectFile(v.project)
						.SetFramework(v.framework)
						.SetProcessAdditionalArguments(
							"--",
							"--report-xunit-trx",
							$"--report-xunit-trx-filename {v.project.Name}_{v.framework}.trx",
							$"--results-directory {TestResultsDirectory}"
						)
				)
			);
		});
}
