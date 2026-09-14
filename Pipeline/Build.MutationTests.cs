using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Octokit;
using Serilog;
using static Fallout.Common.Tools.DotNet.DotNetTasks;
using ProductHeaderValue = Octokit.ProductHeaderValue;
using Project = Fallout.Solutions.Project;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	private static bool DisableMutationTests = false;

	/// <summary>
	///     Disjoint slices of the mutated source, so that the full run can be spread over parallel jobs.
	/// </summary>
	/// <remarks>
	///     A slice mutates its own patterns minus the patterns of every slice before it, and the last slice mutates
	///     everything the others do not. The whole source is therefore covered by construction: a file in a new folder
	///     falls into the last slice instead of silently dropping out of the score.
	///     The patterns start with <c>**/</c> because Stryker does not document what its globs are relative to, and end
	///     in <c>/*.cs</c> because the subject folders are flat - a nested folder would fall into the last slice.
	/// </remarks>
	private static readonly (string Name, string[] Patterns)[] MutationSlices =
	[
		("collections-enumerable", ["**/That/Collections/ThatEnumerable*.cs",]),
		("collections-other", ["**/That/Collections/*.cs",]),
		("numbers", ["**/That/Numbers/*.cs",]),
		("dates",
		[
			"**/That/DateOnlys/*.cs", "**/That/DateTimeOffsets/*.cs", "**/That/DateTimes/*.cs",
			"**/That/TimeOnlys/*.cs", "**/That/TimeSpans/*.cs",
		]),
		("rest", []),
	];

	[Parameter("The slice of the source to mutate - when unset, the whole project is mutated")]
	readonly string MutationSlice;

	AbsolutePath StrykerOutputDirectory => ArtifactsDirectory / "Stryker";
	AbsolutePath StrykerToolPath => TestResultsDirectory / "dotnet-stryker";

	Target MutationTestsCore => _ => _
		.DependsOn(Compile)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.OnlyWhenDynamic(() => Repository.Branch != "main" && Repository.Tags.Count == 0)
		.Executes(() =>
		{
			ExecuteMutationTest(Solution.aweXpect_Core,
				[..FrameworkUnitTestProjects, Solution.Tests.aweXpect_Core_Tests,]);
		});

	Target MutationTestsMain => _ => _
		.DependsOn(Compile)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.Executes(() =>
		{
			ExecuteMutationTest(Solution.aweXpect,
				[Solution.Tests.aweXpect_Tests, Solution.Tests.aweXpect_Internal_Tests,]);
		});

	Target MutationTestsComment => _ => _
		.After(MutationTestsMain)
		.After(MutationTestsCore)
		.DependsOn(MutationTestsDashboard)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.Executes(async () =>
		{
			if (!File.Exists(ArtifactsDirectory / "aweXpect" / "PR.txt"))
			{
				Log.Debug("Missing PR.txt file in artifacts");
			}

			string prNumber = File.ReadAllText(ArtifactsDirectory / "aweXpect" / "PR.txt");
			Log.Debug("Pull request number: {PullRequestId}", prNumber);
			List<string> mutationCommentBodies = [];
			foreach (AbsolutePath file in ArtifactsDirectory.GetFiles("MutationTest_*.md", 2))
			{
				string body = await File.ReadAllTextAsync(file);
				mutationCommentBodies.Add(body);
			}

			if (mutationCommentBodies.Count == 0)
			{
				Log.Warning("No files matching \"MutationTest_*.md\" found");
				return;
			}

			if (int.TryParse(prNumber, out int prId))
			{
				GitHubClient gitHubClient = new(new ProductHeaderValue("Fallout"));
				Credentials tokenAuth = new(GithubToken);
				gitHubClient.Credentials = tokenAuth;
				IReadOnlyList<IssueComment> comments =
					await gitHubClient.Issue.Comment.GetAllForIssue("Testably",
						"aweXpect", prId);
				IssueComment existingComment = null;
				Log.Information($"Found {comments.Count} comments");
				foreach (IssueComment comment in comments)
				{
					if (comment.Body.Contains("## :alien: Mutation Results"))
					{
						Log.Information($"Found comment: {comment.Body}");
						existingComment = comment;
					}
				}

				string body = "## :alien: Mutation Results"
				              + Environment.NewLine
				              + $"[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect%2Fpull/{prId}/merge)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect/pull/{prId}/merge)"
				              + Environment.NewLine
				              + string.Join(Environment.NewLine, mutationCommentBodies);
				if (existingComment == null)
				{
					Log.Information($"Create comment:\n{body}");
					await gitHubClient.Issue.Comment.Create("Testably", "aweXpect",
						prId, body);
				}
				else
				{
					Log.Information($"Update comment:\n{body}");
					await gitHubClient.Issue.Comment.Update("Testably", "aweXpect",
						existingComment.Id, body);
				}
			}
		});

	Target MutationTestsDashboard => _ => _
		.After(MutationTestsMain)
		.After(MutationTestsCore)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.Executes(async () =>
		{
			ArtifactsDirectory.CreateDirectory();
			await "MutationTestsCore".DownloadArtifactTo(ArtifactsDirectory / "aweXpect.Core", GithubToken);
			await DownloadMainMutationReport();

			Dictionary<Project, Project[]> projects;
			if (Repository.Branch != "main" && Repository.Tags.Count == 0)
			{
				projects = new Dictionary<Project, Project[]>
				{
					{
						Solution.aweXpect, [Solution.Tests.aweXpect_Tests, Solution.Tests.aweXpect_Internal_Tests,]
					},
					{
						Solution.aweXpect_Core, [..FrameworkUnitTestProjects, Solution.Tests.aweXpect_Core_Tests,]
					},
				};
			}
			else
			{
				projects = new Dictionary<Project, Project[]>
				{
					{
						Solution.aweXpect, [Solution.Tests.aweXpect_Tests, Solution.Tests.aweXpect_Internal_Tests,]
					},
				};
			}

			string apiKey = Environment.GetEnvironmentVariable("STRYKER_DASHBOARD_API_KEY");
			foreach (KeyValuePair<Project, Project[]> project in projects)
			{
				string branchName = File.ReadAllText(ArtifactsDirectory / project.Key.Name / "BranchName.txt");
				string reportComment =
					File.ReadAllText(ArtifactsDirectory / project.Key.Name / "Stryker" / "reports" /
					                 "mutation-report.json");
				using HttpClient client = new();
				client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
				// https://stryker-mutator.io/docs/General/dashboard/#send-a-report-via-curl
				HttpResponseMessage response = await client.PutAsync(
					$"https://dashboard.stryker-mutator.io/api/reports/github.com/Testably/aweXpect/{branchName}?module={project.Key.Name}",
					new StringContent(reportComment, new MediaTypeHeaderValue("application/json")));
				string responseContent = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					Log.Information("Uploaded the {Module} mutation report ({Size} bytes): {Response}",
						project.Key.Name, reportComment.Length, responseContent);
				}
				else
				{
					// Without this the job stays green while the dashboard keeps showing the previous score.
					Assert.Fail(
						$"Could not upload the {project.Key.Name} mutation report ({reportComment.Length} bytes), " +
						$"the dashboard answered {(int)response.StatusCode} {response.ReasonPhrase}: {responseContent}");
				}
			}
		});

	/// <summary>
	///     Collects the <c>aweXpect</c> mutation report into the single-report layout that the dashboard upload expects.
	/// </summary>
	/// <remarks>
	///     The full run is sliced over parallel jobs, so the slice reports have to be merged back together. Pull requests
	///     mutate only their own changes and stay unsliced, which is why a single artifact is still accepted.
	/// </remarks>
	private async Task DownloadMainMutationReport()
	{
		AbsolutePath projectDirectory = ArtifactsDirectory / "aweXpect";
		List<(string Name, AbsolutePath Report)> sliceReports = [];
		foreach ((string name, string[] _) in MutationSlices)
		{
			AbsolutePath sliceDirectory = projectDirectory / name;
			await $"MutationTestsMain-{name}".DownloadArtifactTo(sliceDirectory, GithubToken);
			AbsolutePath report = sliceDirectory / "Stryker" / "reports" / "mutation-report.json";
			if (File.Exists(report))
			{
				sliceReports.Add((name, report));
			}
		}

		if (sliceReports.Count == 0)
		{
			Log.Information("Found no mutation slices, so the run was not sliced");
			await "MutationTestsMain".DownloadArtifactTo(projectDirectory, GithubToken);
			return;
		}

		if (sliceReports.Count != MutationSlices.Length)
		{
			// Publishing now would drop the mutants of the missing slices and report a score for a subset of the source.
			Assert.Fail(
				$"Only {sliceReports.Count} of {MutationSlices.Length} mutation slices reported: " +
				$"{string.Join(", ", sliceReports.Select(slice => slice.Name))}");
		}
		else
		{
			File.Copy(projectDirectory / sliceReports[0].Name / "BranchName.txt",
				projectDirectory / "BranchName.txt", true);
			MergeMutationReports(sliceReports, projectDirectory);
		}
	}

	/// <summary>
	///     Merges the <paramref name="sliceReports" /> into a single mutation report, by combining their files.
	/// </summary>
	private static void MergeMutationReports(List<(string Name, AbsolutePath Report)> sliceReports,
		AbsolutePath projectDirectory)
	{
		JsonObject merged = null;
		JsonObject mergedFiles = new();
		int totalMutants = 0;
		foreach ((string name, AbsolutePath report) in sliceReports)
		{
			JsonObject slice = JsonNode.Parse(File.ReadAllText(report))!.AsObject();
			merged ??= slice;
			int sliceMutants = 0;
			foreach (KeyValuePair<string, JsonNode> file in slice["files"]!.AsObject())
			{
				int mutants = file.Value?["mutants"]?.AsArray().Count ?? 0;
				sliceMutants += mutants;
				if (!mergedFiles.TryGetPropertyValue(file.Key, out JsonNode existing))
				{
					mergedFiles[file.Key] = file.Value?.DeepClone();
					continue;
				}

				if (mutants > 0 && existing?["mutants"]?.AsArray().Count > 0)
				{
					// Both slices mutated the file, so its mutants would be counted twice in the score.
					Assert.Fail($"The mutation slices overlap in '{file.Key}'");
				}
				else if (mutants > 0)
				{
					mergedFiles[file.Key] = file.Value?.DeepClone();
				}
			}

			totalMutants += sliceMutants;
			Log.Information("The slice '{Slice}' contributed {Mutants} mutants", name, sliceMutants);
		}

		merged!["files"] = mergedFiles;
		AbsolutePath mergedReport = projectDirectory / "Stryker" / "reports" / "mutation-report.json";
		mergedReport.Parent.CreateDirectory();
		File.WriteAllText(mergedReport, merged.ToJsonString());
		// Compare this against the mutant count of an unsliced run to verify that the slices still cover every file.
		Log.Information("Merged {SliceCount} slices into {FileCount} files with {Mutants} mutants",
			sliceReports.Count, mergedFiles.Count, totalMutants);
	}

	private void ExecuteMutationTest(Project project, Project[] testProjects)
	{
		AbsolutePath toolPath = TestResultsDirectory / "dotnet-stryker";
		AbsolutePath configFile = toolPath / "Stryker.Config.json";
		AbsolutePath strykerOutputDirectory = ArtifactsDirectory / "Stryker";
		strykerOutputDirectory.CreateOrCleanDirectory();
		toolPath.CreateOrCleanDirectory();

		// Stryker resolves the project references before it builds the solution in `Debug`, but the
		// `Compile` target leaves `project.assets.json` restored for `Release`, where `aweXpect` and
		// `aweXpect.Core.Tests` reference the released packages instead of the projects. Without this
		// restore, Stryker compiles the mutants against both assemblies and fails with CS0433.
		DotNetRestore(_ => _
			.SetProjectFile(Solution)
			.SetConfigFile(RootDirectory / "nuget.config"));

		DotNetToolInstall(_ => _
			.SetPackageName("dotnet-stryker")
			.SetToolInstallationPath(toolPath));

		string branchName = BranchName;
		if (GitHubActions?.Ref.StartsWith("refs/tags/", StringComparison.OrdinalIgnoreCase) == true)
		{
			string version = GitHubActions.Ref.Substring("refs/tags/".Length);
			branchName = "release/" + version;
			Log.Information("Use release branch analysis for '{BranchName}'", branchName);
		}

		File.WriteAllText(ArtifactsDirectory / "BranchName.txt", branchName);

		string mutateSection = "";
		if (!string.IsNullOrEmpty(MutationSlice))
		{
			string[] patterns = GetMutatePatterns(MutationSlice);
			Log.Information("Mutate the slice '{MutationSlice}': {Patterns}", MutationSlice,
				string.Join(" ", patterns));
			mutateSection = $"\"mutate\": [\n\t\t\t{string.Join(",\n\t\t\t",
				patterns.Select(pattern => $"\"{pattern}\""))}\n\t\t],\n\t\t";
		}

		// TEMPORARY: mutate the whole project even on a branch, so that the mutation jobs can be exercised at
		// their real size without merging to `main` first. Revert this commit before merging.
		bool mutateOnlyTheChanges = false;

		string configText = $$"""
		                      {
		                      	"stryker-config": {
		                      		"project-info": {
		                      			"name": "github.com/Testably/aweXpect",
		                      			"module": "{{project.Name}}",
		                      			"version": "{{branchName}}"
		                      		},
		                      		"test-projects": [
		                      			{{string.Join(",\n\t\t\t", testProjects.Select(PathForJson))}}
		                      		],
		                      		"project": {{PathForJson(project)}},
		                      		"target-framework": "net8.0",
		                      		"since": {
		                      			"target": "main",
		                      			"enabled": {{mutateOnlyTheChanges.ToString().ToLowerInvariant()}},
		                      			"ignore-changes-in": [
		                      				"**/.github/**/*.*"
		                      			]
		                      		},
		                      		{{mutateSection}}"mutation-level": "Advanced"
		                      	}
		                      }
		                      """;
		File.WriteAllText(configFile, configText);
		Log.Debug($"Created '{configFile}':{Environment.NewLine}{configText}");

		// `--log-to-file` always logs at trace level, independent of the console verbosity, so it says why a test
		// session was retried without drowning the job log. The file lands below `-O` and rides along in the
		// uploaded artifacts, which - unlike a runner that is torn down - survive the job timeout.
		string arguments =
			$"-f \"{configFile}\" -O \"{strykerOutputDirectory}\" -r \"Markdown\" -r \"cleartext\" -r \"json\" --log-to-file";

		string executable = EnvironmentInfo.IsWin ? "dotnet-stryker.exe" : "dotnet-stryker";
		IProcess process = ProcessTasks.StartProcess(
				Path.Combine(toolPath, executable),
				arguments,
				Solution.Directory)
			.AssertWaitForExit();
		if (process.ExitCode != 0)
		{
			Assert.Fail(
				$"Stryker did not execute successfully for {project.Name}: (exit code {process.ExitCode}).");
		}

		File.WriteAllText(ArtifactsDirectory / $"MutationTest_{project.Name}.md",
			CreateMutationCommentBody(project.Name));

		if (GitHubActions?.IsPullRequest == true)
		{
			Log.Information($"Write pull request number to PR.txt: {GitHubActions?.PullRequestNumber}");
			File.WriteAllText(ArtifactsDirectory / "PR.txt", GitHubActions?.PullRequestNumber?.ToString());
		}
	}

	string CreateMutationCommentBody(string projectName)
	{
		string[] fileContent =
			File.ReadAllLines(ArtifactsDirectory / "Stryker" / "reports" / "mutation-report.md");
		StringBuilder sb = new();
		sb.AppendLine($"<!-- START {projectName} -->");
		sb.AppendLine($"### {projectName}");
		sb.AppendLine("<details>");
		sb.AppendLine("<summary>Details</summary>");
		sb.AppendLine();
		int count = 0;
		foreach (string line in fileContent.Skip(1))
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			if (line.StartsWith("#"))
			{
				if (++count == 1)
				{
					sb.AppendLine();
					sb.AppendLine("</details>");
					sb.AppendLine();
				}

				sb.AppendLine("##" + line);
				continue;
			}

			if (count == 0 &&
			    line.StartsWith("|") &&
			    line.Contains("| N\\/A"))
			{
				continue;
			}

			sb.AppendLine(line);
		}

		sb.AppendLine($"<!-- END {projectName} -->");
		string body = sb.ToString();
		return body;
	}

	static string PathForJson(Project project)
		=> $"\"{project.Path.ToString().Replace(@"\", @"\\")}\"";

	/// <summary>
	///     The Stryker <c>mutate</c> patterns for the <paramref name="sliceName" /> slice: its own patterns, minus the
	///     patterns of every slice declared before it.
	/// </summary>
	/// <remarks>
	///     The last slice declares no patterns of its own, so it is left with nothing but exclusions - which Stryker
	///     reads as "mutate every file that does not match one of them".
	/// </remarks>
	static string[] GetMutatePatterns(string sliceName)
	{
		int index = Array.FindIndex(MutationSlices, slice => slice.Name == sliceName);
		if (index < 0)
		{
			throw new ArgumentException(
				$"Unknown mutation slice '{sliceName}'. Use one of: {string.Join(", ", MutationSlices.Select(slice => slice.Name))}",
				nameof(sliceName));
		}

		return
		[
			..MutationSlices[index].Patterns,
			..MutationSlices.Take(index).SelectMany(slice => slice.Patterns)
				.Select(pattern => "!" + pattern),
		];
	}
}
