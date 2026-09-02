using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Solutions;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitVersion;
using Fallout.Common.Utilities;
using Fallout.Common.Utilities.Collections;
using Serilog;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	string BranchName;
	AssemblyVersion CoreVersion;
	AssemblyVersion MainVersion;
	string SemVer;

	Target CalculateNugetVersion => _ => _
		.Unlisted()
		.Executes(() =>
		{
			string preRelease = "-CI";
			if (GitHubActions == null)
			{
				preRelease = "-DEV";
			}
			else if (GitHubActions.Ref.StartsWith("refs/tags/", StringComparison.OrdinalIgnoreCase))
			{
				int preReleaseIndex = GitHubActions.Ref.IndexOf('-');
				preRelease = preReleaseIndex > 0 ? GitHubActions.Ref[preReleaseIndex..] : "";
			}

			CoreVersion = AssemblyVersion.FromGitVersion(GitVersionTasks.GitVersion(s => s
					.SetFramework("net8.0")
					.SetNoFetch(true)
					.SetNoCache(true)
					.DisableProcessOutputLogging()
					.SetUpdateAssemblyInfo(false)
					.AddProcessAdditionalArguments("/overrideconfig", "tag-prefix=core/v"))
				.Result, preRelease);

			GitVersion gitVersion = GitVersionTasks.GitVersion(s => s
					.SetFramework("net8.0")
					.SetNoFetch(true)
					.SetNoCache(true)
					.DisableProcessOutputLogging()
					.SetUpdateAssemblyInfo(false))
				.Result;

			MainVersion = AssemblyVersion.FromGitVersion(gitVersion, preRelease);
			SemVer = gitVersion.SemVer;
			BranchName = gitVersion.BranchName;

			if (GitHubActions?.IsPullRequest == true)
			{
				string buildNumber = GitHubActions.RunNumber.ToString();
				Console.WriteLine(
					$"Branch spec is a pull request. Adding build number {buildNumber}");

				SemVer = string.Join('.', gitVersion.SemVer.Split('.').Take(3).Union([buildNumber,]));
			}

			Console.WriteLine($"SemVer = {SemVer}");
		});

	Target Clean => _ => _
		.Unlisted()
		.Before(Restore)
		.Executes(() =>
		{
			ArtifactsDirectory.CreateOrCleanDirectory();
			Log.Information("Cleaned {path}...", ArtifactsDirectory);

			TestResultsDirectory.CreateOrCleanDirectory();
			Log.Information("Cleaned {path}...", TestResultsDirectory);
		});

	Target Restore => _ => _
		.Unlisted()
		.DependsOn(Clean)
		.Executes(() =>
		{
			DotNetRestore(s => s
				.SetProjectFile(Solution)
				.EnableNoCache()
				.SetConfigFile(RootDirectory / "nuget.config"));
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.DependsOn(CalculateNugetVersion)
		.Executes(() =>
		{
			ReportSummary(s => s
				.WhenNotNull(MainVersion, (summary, version) => summary
					.AddPair("Version", version.FileVersion))
				.WhenNotNull(CoreVersion, (summary, version) => summary
					.AddPair("Core", version.FileVersion)));

			if (BuildScope != BuildScope.CoreOnly)
			{
				ClearNugetPackages(Solution.aweXpect.Directory / "bin");
				UpdateReadme(MainVersion.FileVersion, false);

				DotNetBuild(s => s
					.SetProjectFile(Solution)
					.SetConfiguration(Configuration)
					.EnableNoLogo()
					.SetVersion(MainVersion.FileVersion + CoreVersion.PreRelease)
					.SetAssemblyVersion(MainVersion.FileVersion)
					.SetFileVersion(MainVersion.FileVersion)
					.SetInformationalVersion(MainVersion.InformationalVersion));
			}

			ClearNugetPackages(Solution.aweXpect_Core.Directory / "bin");
			UpdateReadme(CoreVersion.FileVersion, true);

			Dictionary<Project, Configuration> projects = new()
			{
				{
					Solution.aweXpect_Core, Configuration
				},
			};
			if (BuildScope == BuildScope.CoreOnly)
			{
				projects.Add(Solution.Tests.aweXpect_Core_Tests, Configuration.Debug);
				projects.Add(Solution.Tests.aweXpect_Core_Api_Tests, Configuration.Debug);
			}

			foreach (var (project, configuration) in projects)
			{
				DotNetBuild(s => s
					.SetProjectFile(project)
					.SetConfiguration(configuration)
					.EnableNoLogo()
					.SetProcessAdditionalArguments($"/p:SolutionDir={RootDirectory}")
					.SetVersion(CoreVersion.FileVersion + CoreVersion.PreRelease)
					.SetAssemblyVersion(CoreVersion.FileVersion)
					.SetFileVersion(CoreVersion.FileVersion)
					.SetInformationalVersion(CoreVersion.InformationalVersion));
			}
		});

	private void UpdateReadme(string fileVersion, bool forCore)
	{
		string version;
		if (GitHubActions?.Ref.StartsWith("refs/tags/", StringComparison.OrdinalIgnoreCase) == true)
		{
			version = GitHubActions.Ref.Substring("refs/tags/".Length);
		}
		else
		{
			version = string.Join('.', fileVersion.Split('.').Take(3));
			if (version.IndexOf('-') != -1)
			{
				version = "v" + version.Substring(0, version.IndexOf('-'));
			}
		}

		Log.Information("Update readme using '{Version}' as version", version);

		StringBuilder sb = new();
		string[] lines = File.ReadAllLines(Solution.Directory / "README.md");
		sb.AppendLine(lines.First());
		sb.AppendLine(lines.Skip(1).First());
		sb.AppendLine(
			$"[![Changelog](https://img.shields.io/badge/Changelog-{version}-blue)](https://github.com/Testably/aweXpect/releases/tag/{version})");
		foreach (string line in lines.Skip(2))
		{
			if (line.StartsWith("[![Build](https://github.com/Testably/aweXpect/actions/workflows/build.yml") ||
			    line.StartsWith("[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure"))
			{
				continue;
			}

			if (line.StartsWith("[![Coverage](https://sonarcloud.io/api/project_badges/measure"))
			{
				sb.AppendLine(line
					.Replace(")", $"&branch=release/{version})"));
				continue;
			}

			if (line.StartsWith("[![Mutation testing badge](https://img.shields.io/endpoint"))
			{
				sb.AppendLine(line
					.Replace("%2Fmain)", $"%2Frelease%2F{version.Replace("/", "%2F")})")
					.Replace("/main)", $"/release/{version})"));
				continue;
			}

			sb.AppendLine(line);
		}

		File.WriteAllText(ArtifactsDirectory / "README.md", sb.ToString());
	}

	private static void ClearNugetPackages(string binPath)
	{
		if (Directory.Exists(binPath))
		{
			foreach (string package in Directory.EnumerateFiles(binPath, "*nupkg", SearchOption.AllDirectories))
			{
				File.Delete(package);
			}
		}
	}

	public record AssemblyVersion(string FileVersion, string InformationalVersion, string PreRelease)
	{
		public static AssemblyVersion FromGitVersion(GitVersion gitVersion, string preRelease)
		{
			if (gitVersion is null)
			{
				return null;
			}

			return new AssemblyVersion(gitVersion.AssemblySemVer, gitVersion.InformationalVersion, preRelease);
		}
	}
}
