using System.Linq;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Solutions;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.Xunit;
using static Fallout.Common.Tools.Xunit.XunitTasks;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	Target UnitTests => _ => _
		.DependsOn(DotNetFrameworkUnitTests)
		.DependsOn(DotNetUnitTests);

	Target DotNetFrameworkUnitTests => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.OnlyWhenDynamic(() => EnvironmentInfo.IsWin)
		.Executes(() =>
		{
			string[] testAssemblies = UnitTestProjects(BuildScope)
				.SelectMany(project =>
					project.Directory.GlobFiles(
						$"bin/{(Configuration == Configuration.Debug || BuildScope == BuildScope.CoreOnly ? "Debug" : "Release")}/net48/*.Tests.dll"))
				.Select(p => p.ToString())
				.ToArray();

			Assert.NotEmpty(testAssemblies.ToList());

			Xunit2(s => s
				.SetFramework("net48")
				.AddTargetAssemblies(testAssemblies)
			);
		});

	Target DotNetUnitTests => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.Executes(() =>
		{
			string net48 = "net48";
			DotNetTest(s => s
					.SetConfiguration(BuildScope == BuildScope.CoreOnly ? Configuration.Debug : Configuration)
					.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
					.EnableNoBuild()
					.SetDataCollector("XPlat Code Coverage")
					.SetResultsDirectory(TestResultsDirectory)
					.CombineWith(
						UnitTestProjects(BuildScope),
						(settings, project) => settings
							.SetProjectFile(project)
							.CombineWith(
								project.GetTargetFrameworks()?.Except([net48,]),
								(frameworkSettings, framework) => frameworkSettings
									.SetFramework(framework)
									.AddLoggers($"trx;LogFileName={project.Name}_{framework}.trx")
							)
					), completeOnFailure: true
			);
		});

	Project[] UnitTestProjects(BuildScope buildScope)
		=> buildScope switch
		{
			BuildScope.CoreOnly => [Solution.Tests.aweXpect_Core_Tests,],
			BuildScope.MainOnly =>
			[
				Solution.Tests.aweXpect_Analyzers_Tests,
				Solution.Tests.aweXpect_Tests,
				Solution.Tests.aweXpect_Internal_Tests,
			],
			_ =>
			[
				Solution.Tests.aweXpect_Core_Tests,
				Solution.Tests.aweXpect_Analyzers_Tests,
				Solution.Tests.aweXpect_Tests,
				Solution.Tests.aweXpect_Internal_Tests,
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
			
			string net48 = "net48";
			DotNetTest(s => s
					.SetConfiguration(Configuration.Debug)
					.SetProcessEnvironmentVariable("DOTNET_CLI_UI_LANGUAGE", "en-US")
					.EnableNoBuild()
					.SetResultsDirectory(TestResultsDirectory / Configuration.Debug)
					.CombineWith(
						UnitTestProjects(BuildScope.Default),
						(settings, project) => settings
							.SetProjectFile(project)
							.CombineWith(
								project.GetTargetFrameworks()?.Except([net48,]),
								(frameworkSettings, framework) => frameworkSettings
									.SetFramework(framework)
							)
					), completeOnFailure: true
			);

			if (EnvironmentInfo.IsWin)
			{
				string[] testAssemblies = UnitTestProjects(BuildScope)
					.SelectMany(project =>
						project.Directory.GlobFiles(
							$"bin/Debug/net48/*.Tests.dll"))
					.Select(p => p.ToString())
					.ToArray();

				Assert.NotEmpty(testAssemblies.ToList());

				Xunit2(s => s
					.SetFramework("net48")
					.AddTargetAssemblies(testAssemblies)
				);
			}
		});
	
	Target UnitTestsWithCoverage => _ => _
		.DependsOn(UnitTests)
		.DependsOn(DebugUnitTests);

}
