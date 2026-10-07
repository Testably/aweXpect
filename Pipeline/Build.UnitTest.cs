using System;
using System.Collections.Generic;
using System.Linq;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Solutions;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	const string NetFramework = "net48";

	/// <summary>
	///     Selects the tests of the category <c>Slow</c> (<c>TestCategories.Slow</c> in <c>Tests/Shared</c>), which
	///     are explicit and therefore left out of a run without a filter.
	/// </summary>
	const string SlowTestsFilter = "/*/*/*/*[Category=Slow]";

	/// <summary>
	///     The projects with slow tests. A project that gets its first slow test must be added here, otherwise the
	///     pipeline never runs it, and a project without slow tests must not be listed, because the run of its slow
	///     tests fails when it finds none.
	/// </summary>
	Project[] ProjectsWithSlowTests =>
	[
		Solution.Tests.aweXpect_Core_Tests,
		Solution.Tests.aweXpect_Generators_Tests,
		Solution.Tests.aweXpect_Tests,
		Solution.Tests.aweXpect_Docs_Tests,
	];

	Target UnitTests => _ => _
		.DependsOn(DotNetFrameworkUnitTests)
		.DependsOn(DotNetUnitTests);

	Target DotNetFrameworkUnitTests => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.OnlyWhenDynamic(() => EnvironmentInfo.IsWin)
		.Executes(() =>
		{
			RunUnitTests(UnitTestProjects(BuildScope), framework => framework == NetFramework,
				BuildScope == BuildScope.CoreOnly ? Configuration.Debug : Configuration, TestResultsDirectory, true);
		});

	Target DotNetUnitTests => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.Executes(() =>
		{
			RunUnitTests(UnitTestProjects(BuildScope), framework => framework != NetFramework,
				BuildScope == BuildScope.CoreOnly ? Configuration.Debug : Configuration, TestResultsDirectory, true);
		});

	Project[] UnitTestProjects(BuildScope buildScope)
		=> buildScope switch
		{
			BuildScope.CoreOnly => [Solution.Tests.aweXpect_Core_Tests,],
			BuildScope.MainOnly =>
			[
				Solution.Tests.aweXpect_Analyzers_Tests,
				Solution.Tests.aweXpect_Generators_Tests,
				Solution.Tests.aweXpect_Tests,
				Solution.Tests.aweXpect_Internal_Tests,
				Solution.Tests.aweXpect_Docs_Tests,
			],
			_ =>
			[
				Solution.Tests.aweXpect_Core_Tests,
				Solution.Tests.aweXpect_Analyzers_Tests,
				Solution.Tests.aweXpect_Generators_Tests,
				Solution.Tests.aweXpect_Tests,
				Solution.Tests.aweXpect_Internal_Tests,
				Solution.Tests.aweXpect_Docs_Tests,
			],
		};

	Target DebugUnitTests => _ => _
		.Unlisted()
		.DependsOn(UnitTests)
		.OnlyWhenDynamic(() => BuildScope != BuildScope.Default)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration.Debug)
				.EnableNoLogo());

			// The projects that `UnitTests` did not run in Debug: under `CoreOnly` it runs its own projects in Debug.
			Project[] projects = UnitTestProjects(BuildScope == BuildScope.CoreOnly
				? BuildScope.MainOnly
				: BuildScope.Default);

			AbsolutePath resultsDirectory = TestResultsDirectory / Configuration.Debug;
			RunUnitTests(projects, framework => framework != NetFramework,
				Configuration.Debug, resultsDirectory, false);

			if (EnvironmentInfo.IsWin)
			{
				RunUnitTests(projects, framework => framework == NetFramework,
					Configuration.Debug, resultsDirectory, false);
			}
		});

	Target UnitTestsWithCoverage => _ => _
		.DependsOn(UnitTests)
		.DependsOn(DebugUnitTests);

	/// <summary>
	///     Runs the tests of the <paramref name="projects" /> for their matching target frameworks, and then their
	///     slow tests.
	/// </summary>
	/// <remarks>
	///     The test projects are executables of the Microsoft.Testing.Platform, which <c>dotnet test</c> only runs
	///     when the whole repository opts into that platform, so they are started with <c>dotnet run</c>.
	///     <para />
	///     With <paramref name="withReports" />, every run writes a TRX file and a Cobertura coverage file named
	///     after the project, the target framework and the step into the <paramref name="resultsDirectory" />.
	/// </remarks>
	void RunUnitTests(Project[] projects, Func<string, bool> includeFramework, Configuration configuration,
		AbsolutePath resultsDirectory, bool withReports)
	{
		(Project Project, string Framework)[] testRuns = projects
			.SelectMany(project => (project.GetTargetFrameworks() ?? [])
				.Where(includeFramework)
				.Select(framework => (project, framework)))
			.ToArray();

		Assert.NotEmpty(testRuns);

		// All runs are part of one invocation, so that a failed run neither skips the remaining runs nor the slow tests.
		DotNetRun(s => s
				.SetConfiguration(configuration)
				.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
				.EnableNoBuild()
				.CombineWith(
					testRuns
						.Select(testRun => (testRun.Project, testRun.Framework, IsSlow: false))
						.Concat(testRuns
							.Where(testRun => ProjectsWithSlowTests.Contains(testRun.Project))
							.Select(testRun => (testRun.Project, testRun.Framework, IsSlow: true))),
					(settings, testRun) => settings
						.SetProjectFile(testRun.Project)
						.SetFramework(testRun.Framework)
						.SetProcessAdditionalArguments(TestArguments(
							testRun.IsSlow
								? $"{testRun.Project.Name}_{testRun.Framework}_Slow"
								: $"{testRun.Project.Name}_{testRun.Framework}",
							testRun.IsSlow, resultsDirectory, withReports))),
			completeOnFailure: true);
	}

	/// <summary>
	///     The arguments for the test application behind <c>dotnet run</c>.
	/// </summary>
	static string[] TestArguments(string name, bool isSlow, AbsolutePath resultsDirectory, bool withReports)
	{
		List<string> arguments = ["--", "--disable-logo", $"--results-directory \"{resultsDirectory}\"",];
		if (isSlow)
		{
			// A project that is listed without having slow tests fails here instead of passing with nothing run.
			arguments.AddRange([$"--treenode-filter \"{SlowTestsFilter}\"", "--minimum-expected-tests 1",]);
		}

		if (withReports)
		{
			arguments.AddRange(
			[
				"--report-trx",
				$"--report-trx-filename {name}.trx",
				"--coverage",
				"--coverage-output-format cobertura",
				$"--coverage-output {name}.cobertura.xml",
			]);
		}

		return arguments.ToArray();
	}
}
