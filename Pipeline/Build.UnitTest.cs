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
			RunUnitTests(UnitTestProjects, framework => framework == NetFramework);
		});

	Target DotNetUnitTests => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.Executes(() =>
		{
			RunUnitTests(UnitTestProjects, framework => framework != NetFramework);
		});

	Project[] UnitTestProjects =>
	[
		Solution.Tests.aweXpect_Core_Tests,
		Solution.Tests.aweXpect_Analyzers_Tests,
		Solution.Tests.aweXpect_Generators_Tests,
		Solution.Tests.aweXpect_Tests,
		Solution.Tests.aweXpect_Internal_Tests,
		Solution.Tests.aweXpect_Docs_Tests,
		Solution.Tests.aweXpect_Json_Tests,
		Solution.Tests.aweXpect_Web_Tests,
		Solution.Tests.aweXpect_Web_Internal_Tests,
		Solution.Tests.aweXpect_Web_Samples_Tests,
		Solution.Tests.aweXpect_Xml_Tests,
		Solution.Tests.aweXpect_Reflection_Tests,
		Solution.Tests.aweXpect_Reflection_Internal_Tests,
		Solution.Tests.aweXpect_Snapshots_Tests,
		Solution.Tests.aweXpect_Snapshots_DiffEngine_Tests,
		Solution.Tests.aweXpect_Snapshots_Orphans_Tests,
		Solution.Tests.aweXpect_Snapshots_PublicApi_Tests,
		Solution.Tests.aweXpect_Snapshots_SourceRoots_Tests,
	];

	/// <summary>
	///     Runs the tests of the <paramref name="projects" /> for their matching target frameworks, and then their
	///     slow tests.
	/// </summary>
	/// <remarks>
	///     The test projects are executables of the Microsoft.Testing.Platform, which <c>dotnet test</c> only runs
	///     when the whole repository opts into that platform, so they are started with <c>dotnet run</c>.
	///     <para />
	///     Every run writes a TRX file and, when <see cref="CollectCoverage" />, a Cobertura coverage file named after
	///     the project, the target framework and the step into the <see cref="TestResultsDirectory" />.
	/// </remarks>
	void RunUnitTests(Project[] projects, Func<string, bool> includeFramework)
	{
		(Project Project, string Framework)[] testRuns = projects
			.SelectMany(project => (project.GetTargetFrameworks() ?? [])
				.Where(includeFramework)
				.Select(framework => (project, framework)))
			.ToArray();

		Assert.NotEmpty(testRuns);

		// All runs are part of one invocation, so that a failed run neither skips the remaining runs nor the slow tests.
		DotNetRun(s => s
				.SetConfiguration(Configuration)
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
							testRun.IsSlow, CollectCoverage, TestResultsDirectory))),
			completeOnFailure: true);
	}

	/// <summary>
	///     The arguments for the test application behind <c>dotnet run</c>.
	/// </summary>
	static string[] TestArguments(string name, bool isSlow, bool collectCoverage, AbsolutePath resultsDirectory)
	{
		List<string> arguments = ["--", "--disable-logo", $"--results-directory \"{resultsDirectory}\"",];
		if (isSlow)
		{
			// A project that is listed without having slow tests fails here instead of passing with nothing run.
			arguments.AddRange([$"--treenode-filter \"{SlowTestsFilter}\"", "--minimum-expected-tests 1",]);
		}

		arguments.AddRange(["--report-trx", $"--report-trx-filename {name}.trx",]);
		if (collectCoverage)
		{
			arguments.AddRange(
			[
				"--coverage",
				"--coverage-output-format cobertura",
				$"--coverage-output {name}.cobertura.xml",
			]);
		}

		return arguments.ToArray();
	}
}
