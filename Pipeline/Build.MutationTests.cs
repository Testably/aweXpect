using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
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

	private const long MaxMutationReportSize = 100 * 1024 * 1024;

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
	private static readonly (string Name, string[] Patterns)[] MainMutationSlices =
	[
		("collections-enumerable", ["**/That/Collections/ThatEnumerable*.cs",]),
		("collections-other", ["**/That/Collections/*.cs",]),
		("numbers", ["**/That/Numbers/*.cs",]),
		("dates",
		[
			"**/That/DateOnlys/*.cs", "**/That/DateTimeOffsets/*.cs", "**/That/DateTimes/*.cs",
			"**/That/TimeOnlys/*.cs", "**/That/TimeSpans/*.cs",
		]),
		("helpers", ["**/Helpers/*.cs",]),
		("infrastructure", ["**/Results/*.cs", "**/Equivalency/*.cs", "**/Options/*.cs", "**/Polyfills/*.cs",]),
		("rest", []),
	];

	/// <summary>
	///     Disjoint slices of the mutated <c>aweXpect.Core</c> source, following the same rules as
	///     <see cref="MainMutationSlices" />.
	/// </summary>
	/// <remarks>
	///     The <c>Core</c> folder is the only one with nested folders, so its nested files get a recursive slice of their
	///     own, because the last slice would otherwise silently pick up everything below it.
	/// </remarks>
	private static readonly (string Name, string[] Patterns)[] CoreMutationSlices =
	[
		("engine", ["**/Core/*.cs",]),
		("engine-nested", ["**/Core/**/*.cs",]),
		("options-strings", ["**/Options/StringEqualityOptions*.cs",]),
		("options", ["**/Options/*.cs",]),
		("formatting", ["**/Formatting/*.cs", "**/Equivalency/*.cs",]),
		("recording", ["**/Recording/*.cs", "**/Signaling/*.cs",]),
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
		.Executes(() =>
		{
			ExecuteMutationTest(Solution.aweXpect_Core,
				[..FrameworkUnitTestProjects, Solution.Tests.aweXpect_Core_Tests,], CoreMutationSlices);
		});

	Target MutationTestsMain => _ => _
		.DependsOn(Compile)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.Executes(() =>
		{
			ExecuteMutationTest(Solution.aweXpect,
				[Solution.Tests.aweXpect_Tests, Solution.Tests.aweXpect_Internal_Tests,], MainMutationSlices);
		});

	Target MutationTestsComment => _ => _
		.After(MutationTestsMain)
		.After(MutationTestsCore)
		.DependsOn(MutationTestsDashboard)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.Executes(async () =>
		{
			// The artifacts are untrusted, because the pull request controls the code that produces them, so the pull
			// request to comment on comes from the event of the workflow run.
			int? prId = await BuildExtensions.ResolvePullRequestOfWorkflowRun(GithubToken);
			if (prId == null)
			{
				Log.Information(
					"The workflow run belongs to no open pull request, so there is no mutation comment to write");
				return;
			}

			List<string> mutationCommentBodies = [];
			foreach (AbsolutePath file in ArtifactsDirectory.GetFiles("MutationTest_*.md", 2))
			{
				mutationCommentBodies.Add(await File.ReadAllTextAsync(file));
			}

			if (mutationCommentBodies.Count == 0)
			{
				Log.Warning("No files matching \"MutationTest_*.md\" found");
				return;
			}

			GitHubClient gitHubClient = new(new ProductHeaderValue("Fallout"));
			Credentials tokenAuth = new(GithubToken);
			gitHubClient.Credentials = tokenAuth;
			IReadOnlyList<IssueComment> comments =
				await gitHubClient.Issue.Comment.GetAllForIssue("Testably",
					"aweXpect", prId.Value);
			IssueComment existingComment = null;
			Log.Information($"Found {comments.Count} comments");
			foreach (IssueComment comment in comments)
			{
				if (comment.IsPipelineComment("## :alien: Mutation Results"))
				{
					Log.Information($"Found comment: {comment.Body}");
					existingComment = comment;
				}
			}

			string body = "## :alien: Mutation Results"
			              + Environment.NewLine
			              + $"[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect%2Fpull/{prId}/merge)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect/pull/{prId}/merge)"
			              + Environment.NewLine
			              + string.Join(Environment.NewLine, mutationCommentBodies).AsUntrustedCommentContent();
			if (existingComment == null)
			{
				Log.Information($"Create comment:\n{body}");
				await gitHubClient.Issue.Comment.Create("Testably", "aweXpect",
					prId.Value, body);
			}
			else
			{
				Log.Information($"Update comment:\n{body}");
				await gitHubClient.Issue.Comment.Update("Testably", "aweXpect",
					existingComment.Id, body);
			}
		});

	Target MutationTestsDashboard => _ => _
		.After(MutationTestsMain)
		.After(MutationTestsCore)
		.OnlyWhenDynamic(() => !DisableMutationTests)
		.OnlyWhenDynamic(() => BuildScope == BuildScope.Default)
		.Executes(async () =>
		{
			string version = await GetMutationDashboardVersion();
			if (version == null)
			{
				Log.Information("The run has no version on the mutation dashboard, so there is nothing to publish");
				return;
			}

			ArtifactsDirectory.CreateDirectory();
			List<Project> projects = [];
			List<Project> projectsWithoutReport = [];
			foreach ((Project project, string artifactName, (string Name, string[] Patterns)[] slices) in
			         new[]
			         {
				         (Solution.aweXpect, "MutationTestsMain", MainMutationSlices),
				         (Solution.aweXpect_Core, "MutationTestsCore", CoreMutationSlices),
			         })
			{
				bool? hasReport = await DownloadSlicedMutationReport(project.Name, artifactName, slices);
				if (hasReport is null)
				{
					// A run without an artifact for the project did not mutate it.
					continue;
				}

				projects.Add(project);
				if (hasReport == false)
				{
					projectsWithoutReport.Add(project);
				}
			}

			if (projectsWithoutReport.Count == projects.Count)
			{
				// The mutation tests only run in the default build scope, so a run without any report is not a
				// failure - it is one where they never executed, and there is nothing to publish.
				Log.Information("No project produced a mutation report, so there is nothing to publish");
				return;
			}

			if (projectsWithoutReport.Count > 0)
			{
				// One report but not the other means a run that was meant to publish and did not get that far.
				Assert.Fail(
					$"No mutation report for {string.Join(", ", projectsWithoutReport.Select(project => project.Name))}");
			}

			string apiKey = Environment.GetEnvironmentVariable("STRYKER_DASHBOARD_API_KEY");
			foreach (Project project in projects)
			{
				AbsolutePath reportFile = ArtifactsDirectory / project.Name / "Stryker" / "reports" /
				                          "mutation-report.json";
				if (new FileInfo(reportFile).Length > MaxMutationReportSize)
				{
					Assert.Fail($"The {project.Name} mutation report is larger than {MaxMutationReportSize} bytes");
				}

				string reportComment = File.ReadAllText(reportFile);
				if (JsonNode.Parse(reportComment) is not JsonObject)
				{
					Assert.Fail($"The {project.Name} mutation report is no JSON object");
				}

				using HttpClient client = new();
				client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
				// https://stryker-mutator.io/docs/General/dashboard/#send-a-report-via-curl
				HttpResponseMessage response = await client.PutAsync(
					$"https://dashboard.stryker-mutator.io/api/reports/github.com/Testably/aweXpect/{version}?module={project.Name}",
					new StringContent(reportComment, new MediaTypeHeaderValue("application/json")));
				string responseContent = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					Log.Information("Uploaded the {Module} mutation report ({Size} bytes): {Response}",
						project.Name, reportComment.Length, responseContent);
				}
				else
				{
					// Without this the job stays green while the dashboard keeps showing the previous score.
					Assert.Fail(
						$"Could not upload the {project.Name} mutation report ({reportComment.Length} bytes), " +
						$"the dashboard answered {(int)response.StatusCode} {response.ReasonPhrase}: {responseContent}");
				}
			}
		});

	/// <summary>
	///     Collects the <paramref name="projectName" /> mutation report into the single-report layout that the dashboard
	///     upload expects, by merging the <paramref name="slices" /> published as <paramref name="artifactName" />.
	/// </summary>
	/// <remarks>
	///     The full run is sliced over parallel jobs, so the slice reports have to be merged back together. Pull requests
	///     mutate only their own changes and stay unsliced, which is why a single artifact is still accepted.
	/// </remarks>
	/// <returns>
	///     Whether a report was collected, or <see langword="null" /> when the run did not mutate the project at all. An
	///     unsliced run whose mutation tests never executed uploads its artifact without one.
	/// </returns>
	private async Task<bool?> DownloadSlicedMutationReport(string projectName, string artifactName,
		(string Name, string[] Patterns)[] slices)
	{
		AbsolutePath projectDirectory = ArtifactsDirectory / projectName;
		List<(string Name, AbsolutePath Report)> sliceReports = [];
		foreach ((string name, string[] _) in slices)
		{
			AbsolutePath sliceDirectory = projectDirectory / name;
			await $"{artifactName}-{name}".DownloadArtifactTo(sliceDirectory, GithubToken);
			AbsolutePath report = sliceDirectory / "Stryker" / "reports" / "mutation-report.json";
			if (File.Exists(report))
			{
				sliceReports.Add((name, report));
			}
		}

		if (sliceReports.Count == 0)
		{
			Log.Information("Found no mutation slices for {Project}, so the run was not sliced", projectName);
			await artifactName.DownloadArtifactTo(projectDirectory, GithubToken);
			if (!Directory.Exists(projectDirectory))
			{
				return null;
			}

			return File.Exists(projectDirectory / "Stryker" / "reports" / "mutation-report.json");
		}

		if (sliceReports.Count != slices.Length)
		{
			// Publishing now would drop the mutants of the missing slices and report a score for a subset of the source.
			Assert.Fail(
				$"Only {sliceReports.Count} of {slices.Length} {projectName} mutation slices reported: " +
				$"{string.Join(", ", sliceReports.Select(slice => slice.Name))}");
		}
		else
		{
			MergeMutationReports(sliceReports, projectDirectory);
		}

		return true;
	}

	/// <summary>
	///     The version under which the dashboard stores the mutation reports of the analysed run, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	/// <remarks>
	///     The artifacts are untrusted, because a pull request controls the code that produces them, so the version
	///     comes from the event alone, and a run that a pull request triggered can only publish to the version of
	///     that pull request.
	/// </remarks>
	private async Task<string> GetMutationDashboardVersion()
	{
		string version = null;
		if (GitHubActions?.EventName != "workflow_run")
		{
			// The reports come from this very run, so its own ref identifies them.
			string gitRef = GitHubActions?.Ref ?? "";
			if (gitRef.StartsWith("refs/tags/", StringComparison.Ordinal))
			{
				version = "release/" + gitRef.Substring("refs/tags/".Length);
			}
			else if (gitRef.StartsWith("refs/heads/", StringComparison.Ordinal))
			{
				version = gitRef.Substring("refs/heads/".Length);
			}
		}
		else if (Environment.GetEnvironmentVariable("WorkflowRunEvent") == "pull_request")
		{
			int? prId = await BuildExtensions.ResolvePullRequestOfWorkflowRun(GithubToken);
			return prId == null ? null : $"pull/{prId}/merge";
		}
		else
		{
			version = Environment.GetEnvironmentVariable("WorkflowRunHeadBranch");
		}

		// The version becomes a part of the path that the report is sent to.
		return version != null &&
		       Regex.IsMatch(version, @"\A[A-Za-z0-9][A-Za-z0-9._-]*(/[A-Za-z0-9][A-Za-z0-9._-]*)*\z")
			? version
			: null;
	}

	/// <summary>
	///     Merges the <paramref name="sliceReports" /> into a single mutation report, by combining their files.
	/// </summary>
	/// <remarks>
	///     Every slice reports every file, and the ones outside it keep their mutants with the status <c>Ignored</c>,
	///     so only the mutants that a slice actually tested tell which slice a file belongs to.
	/// </remarks>
	private static void MergeMutationReports(List<(string Name, AbsolutePath Report)> sliceReports,
		AbsolutePath projectDirectory)
	{
		JsonObject merged = null;
		JsonObject mergedFiles = new();
		Dictionary<string, string> contributingSlice = new();
		int totalMutants = 0;
		foreach ((string name, AbsolutePath report) in sliceReports)
		{
			JsonObject slice = JsonNode.Parse(File.ReadAllText(report))!.AsObject();
			merged ??= slice;
			int sliceMutants = 0;
			foreach (KeyValuePair<string, JsonNode> file in slice["files"]!.AsObject())
			{
				int mutants = TestedMutants(file.Value);
				sliceMutants += mutants;
				if (!mergedFiles.TryGetPropertyValue(file.Key, out JsonNode existing))
				{
					mergedFiles[file.Key] = file.Value?.DeepClone();
					if (mutants > 0)
					{
						contributingSlice[file.Key] = name;
					}

					continue;
				}

				if (mutants > 0 && TestedMutants(existing) > 0)
				{
					// Both slices tested the file, so its mutants would be counted twice in the score.
					Assert.Fail(
						$"The mutation slices '{contributingSlice[file.Key]}' ({TestedMutants(existing)} mutants) " +
						$"and '{name}' ({mutants} mutants) overlap in '{file.Key}'");
				}
				else if (mutants > 0)
				{
					mergedFiles[file.Key] = file.Value?.DeepClone();
					contributingSlice[file.Key] = name;
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

	/// <summary>
	///     The mutants of the <paramref name="file" /> that its slice actually ran, which is what tells the slices
	///     apart: a mutant the mutate filter removed is reported as <c>Ignored</c> and one that did not build as
	///     <c>CompileError</c>, and every slice reports those for every file.
	/// </summary>
	private static int TestedMutants(JsonNode file)
		=> file?["mutants"]?.AsArray()
			.Count(mutant => mutant?["status"]?.GetValue<string>() is not ("Ignored" or "CompileError")) ?? 0;

	private void ExecuteMutationTest(Project project, Project[] testProjects,
		(string Name, string[] Patterns)[] slices)
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

		string mutateSection = "";
		if (!string.IsNullOrEmpty(MutationSlice))
		{
			string[] patterns = GetMutatePatterns(slices, MutationSlice);
			Log.Information("Mutate the slice '{MutationSlice}': {Patterns}", MutationSlice,
				string.Join(" ", patterns));
			mutateSection = $"\"mutate\": [\n\t\t\t{string.Join(",\n\t\t\t",
				patterns.Select(pattern => $"\"{pattern}\""))}\n\t\t],\n\t\t";
		}

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
		                      			"enabled": {{(BranchName != "main").ToString().ToLowerInvariant()}},
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

		// The unit tests are executables of the Microsoft.Testing.Platform, which the default runner (VSTest) cannot
		// run. The slow tests are explicit, so Stryker leaves them out like every run without a filter does.
		string arguments =
			$"-f \"{configFile}\" -O \"{strykerOutputDirectory}\" -r \"Markdown\" -r \"cleartext\" -r \"json\" --test-runner mtp";

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
	static string[] GetMutatePatterns((string Name, string[] Patterns)[] slices, string sliceName)
	{
		int index = Array.FindIndex(slices, slice => slice.Name == sliceName);
		if (index < 0)
		{
			throw new ArgumentException(
				$"Unknown mutation slice '{sliceName}'. Use one of: {string.Join(", ", slices.Select(slice => slice.Name))}",
				nameof(sliceName));
		}

		return
		[
			..slices[index].Patterns,
			..slices.Take(index).SelectMany(slice => slice.Patterns)
				.Select(pattern => "!" + pattern),
		];
	}
}
