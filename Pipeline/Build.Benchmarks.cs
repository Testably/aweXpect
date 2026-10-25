using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.Git;
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
	private const string BenchmarkBranch = "benchmarks";

	/// <remarks>
	///     It differs from the heading of the former comments, one per package, so that they are not updated.
	/// </remarks>
	const string BenchmarkCommentHeading = "## :rocket: Benchmarks";

	/// <summary>
	///     The number of runs on <c>main</c> whose median is the baseline of the time, as a single run on a shared
	///     runner is too noisy.
	/// </summary>
	const int BaselineRuns = 10;

	const double RelativeChangeThreshold = 0.15;

	/// <summary>
	///     Leaves room for the summaries within the limit of a GitHub comment.
	/// </summary>
	const int MaxRawResultsLength = 45000;

	const string StrongWorse = "🔴";
	const string StrongBetter = "🟢";
	const string WeakWorse = "🔸";
	const string WeakBetter = "🔹";

	static readonly string[] ComparedLibraries = ["FluentAssertions", "TUnit",];

	[Parameter("The package whose benchmarks run - when unset, the benchmarks of all packages run")]
	readonly string BenchmarkPackage;

	/// <summary>
	///     Files that every benchmark depends on, besides the sources of the packages it measures.
	/// </summary>
	static readonly string[] SharedBenchmarkInputs =
	[
		"Directory.Build.props", "Directory.Packages.props", "global.json",
		"Source/Directory.Build.props", "Source/Directory.Build.targets",
		"Pipeline/Build.Benchmarks.cs", "Pipeline/PageBenchmarkReportGenerator.cs",
	];

	/// <summary>
	///     The benchmark project of each package, with the packages whose source it measures.
	/// </summary>
	(string Package, Project Project, string[] MeasuredPackages)[] BenchmarkProjects =>
	[
		("aweXpect", Solution.Benchmarks.aweXpect_Benchmarks, ["aweXpect", "aweXpect.Core",]),
		("aweXpect.Json", Solution.Benchmarks.aweXpect_Json_Benchmarks, ["aweXpect.Json", "aweXpect", "aweXpect.Core",]),
		("aweXpect.Web", Solution.Benchmarks.aweXpect_Web_Benchmarks, ["aweXpect.Web", "aweXpect.Core",]),
		("aweXpect.Xml", Solution.Benchmarks.aweXpect_Xml_Benchmarks, ["aweXpect.Xml", "aweXpect", "aweXpect.Core",]),
		("aweXpect.Reflection", Solution.Benchmarks.aweXpect_Reflection_Benchmarks,
			["aweXpect.Reflection", "aweXpect.Core",]),
	];

	Target BenchmarkDotNet => _ => _
		.Executes(() =>
		{
			AbsolutePath benchmarkDirectory = ArtifactsDirectory / "Benchmarks";
			benchmarkDirectory.CreateOrCleanDirectory();

			foreach ((string package, Project project) in GetBenchmarksToRun())
			{
				DotNetBuild(s => s
					.SetProjectFile(project)
					.SetConfiguration(Configuration.Release)
					.EnableNoLogo());

				DotNet(
					$"{project.Name}.dll --exporters json --filter * --artifacts \"{benchmarkDirectory / package}\"",
					project.Directory / "bin" / "Release");
			}
		});

	Target BenchmarkResult => _ => _
		.After(BenchmarkDotNet)
		.Executes(async () =>
		{
			foreach ((string package, Project project, string[] _measured) in BenchmarkProjects)
			{
				AbsolutePath report = GetBenchmarkReport(package, project);
				if (report.FileExists())
				{
					Log.Information("Report of {Package}:\n {FileContent}", package,
						await File.ReadAllTextAsync(report));
				}
			}
		});

	Target BenchmarkComment => _ => _
		.Executes(async () =>
		{
			// The artifact is untrusted, because the pull request controls the code that produces it, so the pull
			// request to comment on comes from the event of the workflow run.
			int? prId = await BuildExtensions.ResolvePullRequestOfWorkflowRun(GithubToken);
			if (prId == null)
			{
				Log.Information("Skip writing a comment, as the workflow run belongs to no pull request.");
				return;
			}

			foreach ((string package, Project _project, string[] _measured) in BenchmarkProjects)
			{
				await $"Benchmarks-{package}".DownloadArtifactTo(ArtifactsDirectory, GithubToken);
			}

			GitHubClient gitHubClient = new(new ProductHeaderValue("Fallout"));
			Credentials tokenAuth = new(GithubToken);
			gitHubClient.Credentials = tokenAuth;
			IReadOnlyList<IssueComment> comments =
				await gitHubClient.Issue.Comment.GetAllForIssue("Testably", "aweXpect", prId.Value);
			Log.Information($"Found {comments.Count} comments");

			// Only the reports of known packages are read, as the artifact is untrusted.
			List<BenchmarkCommentSection> sections = new();
			foreach ((string package, Project project, string[] _measured) in BenchmarkProjects)
			{
				AbsolutePath report = GetBenchmarkReport(package, project);
				if (!report.FileExists())
				{
					Log.Information("Skip the benchmarks of {Package}, as the workflow run produced no report.",
						package);
					continue;
				}

				Dictionary<string, BenchmarkMeasurement> measurements = ReadBenchmarkMeasurements(
					report.Parent / $"{project.Name}.HappyCaseBenchmarks-report-full-compressed.json");
				BenchmarkFile baselineFile =
					await DownloadBenchmarkFile(GetBenchmarkDataDirectory(package), "limited-data.js");
				sections.Add(new BenchmarkCommentSection(package,
					CreateBenchmarkSummary(package, measurements, ReadBenchmarkBaseline(baselineFile?.Content)),
					CreateRawBenchmarkResults(report, measurements)));
			}

			if (sections.Count == 0)
			{
				Log.Information("Skip writing a comment, as the workflow run produced no report.");
				return;
			}

			string body = CreateBenchmarkCommentBody(sections).AsUntrustedCommentContent();
			IssueComment existingComment =
				comments.LastOrDefault(comment => comment.IsPipelineComment(BenchmarkCommentHeading));
			if (existingComment == null)
			{
				Log.Information($"Create comment:\n{body}");
				await gitHubClient.Issue.Comment.Create("Testably", "aweXpect", prId.Value, body);
			}
			else
			{
				Log.Information($"Update comment:\n{body}");
				await gitHubClient.Issue.Comment.Update("Testably", "aweXpect", existingComment.Id, body);
			}
		});

	/// <remarks>
	///     The report is the history of <c>main</c> that the benchmark page shows, so a run that was started manually on
	///     another branch must not append to it.
	/// </remarks>
	Target BenchmarkReport => _ => _
		.After(BenchmarkDotNet)
		.OnlyWhenDynamic(() => GitHubActions?.Ref == "refs/heads/main")
		.Executes(async () =>
		{
			PageBenchmarkReportGenerator.CommitInfo commitInfo = GetBenchmarkCommitInfo();
			foreach ((string package, Project _project, string[] _measured) in BenchmarkProjects)
			{
				AbsolutePath resultsDirectory = ArtifactsDirectory / "Benchmarks" / package / "results";
				if (!resultsDirectory.DirectoryExists())
				{
					continue;
				}

				string dataDirectory = GetBenchmarkDataDirectory(package);
				BenchmarkFile currentFile = await DownloadBenchmarkFile(dataDirectory, "data.js");
				BenchmarkFile limitedFile = await DownloadBenchmarkFile(dataDirectory, "limited-data.js");
				List<string> benchmarkReports = new();
				foreach (string file in Directory.GetFiles(resultsDirectory, "*full-compressed.json"))
				{
					benchmarkReports.Add(await File.ReadAllTextAsync(file));
				}

				var (updatedFileContent, limitedFileContent) = PageBenchmarkReportGenerator
					.Append(commitInfo, currentFile?.Content ?? EmptyBenchmarkData, benchmarkReports, 50);
				if (!string.IsNullOrWhiteSpace(updatedFileContent))
				{
					await UploadBenchmarkFile(package, dataDirectory, "data.js", commitInfo, currentFile,
						updatedFileContent);
					await UploadBenchmarkFile(package, dataDirectory, "limited-data.js", commitInfo, limitedFile,
						limitedFileContent);
				}
			}
		});

	Target Benchmarks => _ => _
		.DependsOn(BenchmarkDotNet)
		.DependsOn(BenchmarkResult)
		.DependsOn(BenchmarkReport);

	const string EmptyBenchmarkData = "window.BENCHMARK_DATA = {}";

	/// <summary>
	///     In a pull request, only the benchmarks whose measured packages, project or shared inputs changed run;
	///     everywhere else all of them.
	/// </summary>
	IEnumerable<(string Package, Project Project)> GetBenchmarksToRun()
	{
		(string Package, Project Project, string[] MeasuredPackages)[] benchmarkProjects = BenchmarkProjects;
		if (!string.IsNullOrEmpty(BenchmarkPackage))
		{
			benchmarkProjects = benchmarkProjects.Where(benchmark => benchmark.Package == BenchmarkPackage).ToArray();
			if (benchmarkProjects.Length == 0)
			{
				throw new InvalidOperationException($"There is no benchmark project for '{BenchmarkPackage}'.");
			}
		}

		if (GitHubActions?.IsPullRequest != true)
		{
			return benchmarkProjects.Select(benchmark => (benchmark.Package, benchmark.Project));
		}

		string[] changedFiles = GitTasks
			.Git($"diff --name-only origin/{GitHubActions.BaseRef}...HEAD", logOutput: false)
			.Select(output => output.Text)
			.ToArray();
		List<(string Package, Project Project)> benchmarks = new();
		foreach ((string package, Project project, string[] measuredPackages) in benchmarkProjects)
		{
			string[] inputs = measuredPackages
				.SelectMany(measured => PackageDirectories.Single(p => p.Package == measured).Directories)
				.Select(directory => directory + "/")
				.Append(RootDirectory.GetRelativePathTo(project.Directory).ToString().Replace('\\', '/') + "/")
				.Concat(SharedBenchmarkInputs)
				.ToArray();
			if (changedFiles.Any(file => inputs.Any(input => file == input || file.StartsWith(input, StringComparison.Ordinal))))
			{
				benchmarks.Add((package, project));
			}
			else
			{
				Log.Information("Skip the benchmarks of {Package}, as the pull request does not change them.", package);
			}
		}

		return benchmarks;
	}

	AbsolutePath GetBenchmarkReport(string package, Project project)
		=> ArtifactsDirectory / "Benchmarks" / package / "results" /
		   $"{project.Name}.HappyCaseBenchmarks-report-github.md";

	/// <summary>
	///     The directory of the benchmark data of the <paramref name="package" /> on the benchmark branch, which the
	///     docs site reads.
	/// </summary>
	static string GetBenchmarkDataDirectory(string package)
		=> package == "aweXpect" ? "Docs/pages/static/js" : $"Docs/extensions/{package}/static/js";

	static PageBenchmarkReportGenerator.CommitInfo GetBenchmarkCommitInfo()
	{
		Output[] lines = GitTasks.Git("log -1").ToArray();
		string commitId = null, author = null, date = null, message = null;
		foreach (string line in lines.Select(x => x.Text))
		{
			if (commitId == null && line.StartsWith("commit "))
			{
				commitId = line.Substring("commit ".Length, 40);
				continue;
			}

			if (author == null && line.StartsWith("Author: "))
			{
				author = line.Substring("Author: ".Length);
				int index = author.IndexOf(" <", StringComparison.Ordinal);
				if (index > 0)
				{
					author = author.Substring(0, index);
				}

				continue;
			}

			if (date == null && line.StartsWith("Date:   "))
			{
				date = line.Substring("Date:   ".Length);
				continue;
			}

			if (commitId != null && author != null && date != null && !string.IsNullOrWhiteSpace(line))
			{
				message = line.Trim();
				break;
			}
		}

		return new PageBenchmarkReportGenerator.CommitInfo(commitId, author, date, message);
	}

	async Task UploadBenchmarkFile(
		string package,
		string dataDirectory,
		string filename,
		PageBenchmarkReportGenerator.CommitInfo commitInfo,
		BenchmarkFile currentFile,
		string updatedFileContent)
	{
		using HttpClient client = new();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("aweXpect");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GithubToken);
		GithubUpdateFile content = new(
			$"Update {package} benchmark for {commitInfo.Sha.Substring(0, 8)}: {commitInfo.Message} by {commitInfo.Author}",
			Base64Encode(updatedFileContent),
			currentFile?.Sha,
			BenchmarkBranch);
		// A new file is created without a sha.
		string json = JsonSerializer.Serialize(content,
			new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, });
		HttpResponseMessage response;
		// The benchmark jobs of the packages update the branch in parallel, which GitHub rejects with a conflict
		// when another job moved the branch in the meantime.
		for (int attempt = 1;; attempt++)
		{
			response = await client.PutAsync(
				$"https://api.github.com/repos/Testably/aweXpect/contents/{dataDirectory}/{filename}",
				new StringContent(json, Encoding.UTF8, "application/json"));
			if (response.StatusCode != HttpStatusCode.Conflict || attempt == 5)
			{
				break;
			}

			await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(2, 10)));
		}

		if (response.IsSuccessStatusCode)
		{
			Log.Information("Successfully updated the benchmark data...");
		}
		else
		{
			string responseContent = await response.Content.ReadAsStringAsync();
			Log.Error($"Could not update the benchmark data: {responseContent}");
		}
	}

	async Task<BenchmarkFile> DownloadBenchmarkFile(string dataDirectory, string filename)
	{
		using HttpClient client = new();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("aweXpect");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GithubToken);
		HttpResponseMessage response = await client.GetAsync(
			$"https://api.github.com/repos/Testably/aweXpect/contents/{dataDirectory}/{filename}?ref={BenchmarkBranch}");
		string responseContent = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		using JsonDocument document = JsonDocument.Parse(responseContent);
		var contentStream = await client.GetStreamAsync(
			$"https://raw.githubusercontent.com/Testably/aweXpect/refs/heads/{BenchmarkBranch}/{dataDirectory}/{filename}");
		using StreamReader reader = new(contentStream, Encoding.UTF8);
		string content = await reader.ReadToEndAsync();
		string sha = document.RootElement.GetProperty("sha").GetString();
		return new BenchmarkFile(content, sha);
	}

	static string Base64Encode(string plainText)
	{
		byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
		return Convert.ToBase64String(plainTextBytes);
	}

	static string Base64Decode(string base64EncodedData)
	{
		byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
		return Encoding.UTF8.GetString(base64EncodedBytes);
	}

	static string CreateBenchmarkCommentBody(List<BenchmarkCommentSection> sections)
	{
		StringBuilder sb = new();
		sb.AppendLine(BenchmarkCommentHeading);
		sb.AppendLine(
			$"Compared to `main`: memory to the latest run, time to the median of the last {BaselineRuns} runs, as the runners differ in speed. " +
			$"A relative change marks when aweXpect gets more than {FormatPercent(RelativeChangeThreshold).TrimStart('+')} faster or slower relative to " +
			$"{string.Join(" and ", ComparedLibraries)}: {StrongWorse} / {StrongBetter} against both of them, " +
			$"{WeakWorse} / {WeakBetter} against only one.");
		sb.AppendLine();
		int rawResultsLength = 0;
		foreach (BenchmarkCommentSection section in sections)
		{
			sb.Append(section.Summary);
			// The summaries take precedence over the raw results when the comment would get too long.
			rawResultsLength += section.RawResults.Length;
			sb.AppendLine(rawResultsLength <= MaxRawResultsLength
				? section.RawResults
				: $"_The raw results do not fit into this comment, see the artifact `Benchmarks-{section.Package}` of the workflow run._");
			sb.AppendLine();
		}

		return sb.ToString();
	}

	static string CreateBenchmarkSummary(string package, Dictionary<string, BenchmarkMeasurement> measurements,
		BenchmarkBaseline baseline)
	{
		StringBuilder sb = new();
		sb.AppendLine($"### {package}");
		if (baseline == null)
		{
			sb.AppendLine("_There are no results of `main` to compare to._");
			return sb.ToString();
		}

		string[] benchmarks = measurements.Keys
			.Where(method => method.EndsWith("_aweXpect", StringComparison.Ordinal))
			.Select(method => method.Substring(0, method.Length - "_aweXpect".Length))
			.Order(StringComparer.Ordinal)
			.ToArray();
		// The extension packages are only compared with some of the libraries.
		string[] libraries = ComparedLibraries
			.Where(library => measurements.Keys.Any(method => method.EndsWith($"_{library}", StringComparison.Ordinal)))
			.ToArray();
		string[] newBenchmarks = benchmarks.Where(name => !baseline.Benchmarks.ContainsKey(name)).ToArray();
		string[] removedBenchmarks = baseline.Benchmarks.Keys.Except(benchmarks).Order(StringComparer.Ordinal).ToArray();
		List<string> memoryRows = new();
		List<string> relativeRows = new();
		List<string> allRows = new();
		foreach (string name in benchmarks.Except(newBenchmarks))
		{
			BenchmarkMeasurement current = measurements[$"{name}_aweXpect"];
			BaselineBenchmark main = baseline.Benchmarks[name];
			string[] relativeCells =
				CompareRelativePerformance(measurements, name, main, libraries, out bool relativeChanged);
			if (current.Allocated != main.Allocated)
			{
				memoryRows.Add(
					$"| `{name}` | {FormatBytes(main.Allocated)} | {FormatBytes(current.Allocated)} | {FormatMemoryChange(main.Allocated, current.Allocated)} |");
			}

			if (relativeChanged)
			{
				relativeRows.Add($"| `{name}` | {string.Join(" | ", relativeCells)} |");
			}

			allRows.Add(
				$"| `{name}` | {FormatTime(main.Mean)} → {FormatTime(current.Mean)} ({FormatPercent(current.Mean / main.Mean - 1)}) | " +
				$"{FormatBytes(main.Allocated)} → {FormatBytes(current.Allocated)} | {string.Join(" | ", relativeCells)} |");
		}

		sb.AppendLine($"Compared to `main` @ `{baseline.Sha.Substring(0, 8)}`");
		sb.AppendLine();
		if (memoryRows.Count == 0 && relativeRows.Count == 0)
		{
			sb.AppendLine($"✅ No changes in memory or relative performance ({allRows.Count} benchmarks)");
			sb.AppendLine();
		}

		if (memoryRows.Count > 0)
		{
			sb.AppendLine("**Memory**");
			sb.AppendLine();
			sb.AppendLine("| Benchmark | main | PR | Δ |");
			sb.AppendLine("|---|--:|--:|--:|");
			memoryRows.ForEach(row => sb.AppendLine(row));
			sb.AppendLine();
		}

		if (relativeRows.Count > 0)
		{
			sb.AppendLine("**Relative performance** (how many times as long the other library takes as aweXpect, main → PR)");
			sb.AppendLine();
			sb.AppendLine($"| Benchmark | {string.Join(" | ", libraries.Select(library => $"vs {library}"))} |");
			sb.AppendLine($"|---|{string.Concat(libraries.Select(_ => "--:|"))}");
			relativeRows.ForEach(row => sb.AppendLine(row));
			sb.AppendLine();
		}

		if (newBenchmarks.Length > 0)
		{
			sb.AppendLine($"New, without results on `main`: {string.Join(", ", newBenchmarks.Select(name => $"`{name}`"))}");
			sb.AppendLine();
		}

		if (removedBenchmarks.Length > 0)
		{
			sb.AppendLine($"Removed: {string.Join(", ", removedBenchmarks.Select(name => $"`{name}`"))}");
			sb.AppendLine();
		}

		sb.AppendLine("<details>");
		sb.AppendLine("<summary>All benchmarks compared to main</summary>");
		sb.AppendLine();
		sb.AppendLine(
			$"| Benchmark | Time | Memory | {string.Join(" | ", libraries.Select(library => $"vs {library}"))} |");
		sb.AppendLine($"|---|--:|--:|{string.Concat(libraries.Select(_ => "--:|"))}");
		allRows.ForEach(row => sb.AppendLine(row));
		sb.AppendLine();
		sb.AppendLine("</details>");
		sb.AppendLine();
		return sb.ToString();
	}

	/// <summary>
	///     Compares the time of aweXpect relative to each of the <paramref name="libraries" /> with <c>main</c>.
	/// </summary>
	/// <remarks>
	///     The ratio of two benchmarks of the same run does not depend on the speed of the runner. A change against
	///     several libraries in the same direction is most likely caused by aweXpect and gets a stronger marker.
	/// </remarks>
	static string[] CompareRelativePerformance(Dictionary<string, BenchmarkMeasurement> measurements, string name,
		BaselineBenchmark main, string[] libraries, out bool changed)
	{
		BenchmarkMeasurement aweXpect = measurements[$"{name}_aweXpect"];
		double?[] ratios = libraries
			.Select(library => measurements.TryGetValue($"{name}_{library}", out BenchmarkMeasurement other)
				? aweXpect.Mean / other.Mean
				: (double?)null)
			.ToArray();
		double?[] relativeChanges = libraries
			.Select((library, index) => main.Ratios.TryGetValue(library, out double mainRatio) && ratios[index] != null
				? ratios[index] / mainRatio - 1
				: null)
			.ToArray();
		bool[] exceeds = relativeChanges
			.Select(relativeChange => relativeChange != null && Math.Abs(relativeChange.Value) > RelativeChangeThreshold)
			.ToArray();
		bool allAgree = libraries.Length > 1 && exceeds.All(x => x) &&
		                relativeChanges.Select(relativeChange => Math.Sign(relativeChange!.Value)).Distinct().Count() == 1;
		changed = exceeds.Any(x => x);
		return libraries.Select((library, index) =>
		{
			if (ratios[index] == null)
			{
				return "";
			}

			string cell = main.Ratios.TryGetValue(library, out double mainRatio)
				? $"{FormatRatio(mainRatio)} → {FormatRatio(ratios[index].Value)}"
				: $"→ {FormatRatio(ratios[index].Value)}";
			if (exceeds[index])
			{
				double relativeChange = relativeChanges[index]!.Value;
				string marker = (relativeChange > 0, allAgree) switch
				{
					(true, true) => StrongWorse,
					(true, false) => WeakWorse,
					(false, true) => StrongBetter,
					(false, false) => WeakBetter,
				};
				cell += $" {marker} {FormatPercent(relativeChange)}";
			}

			return cell;
		}).ToArray();
	}

	/// <summary>
	///     Reads the results of <c>main</c> from the benchmark data that the docs site shows.
	/// </summary>
	/// <remarks>
	///     A benchmark that was removed from <c>main</c> is ignored, as its last result belongs to an older commit than
	///     the latest one.
	/// </remarks>
	static BenchmarkBaseline ReadBenchmarkBaseline(string content)
	{
		const string prefix = "window.BENCHMARK_DATA = ";
		if (content?.StartsWith(prefix, StringComparison.Ordinal) != true)
		{
			return null;
		}

		using JsonDocument document = JsonDocument.Parse(content.Substring(prefix.Length));
		List<(string Sha, string Name, BaselineBenchmark Benchmark)> benchmarks = new();
		foreach (JsonProperty benchmark in document.RootElement.EnumerateObject())
		{
			JsonElement[] commits = benchmark.Value.GetProperty("commits").EnumerateArray().ToArray();
			Dictionary<string, double[]> datasets = benchmark.Value.GetProperty("datasets").EnumerateArray().ToDictionary(
				dataset => dataset.GetProperty("label").GetString(),
				dataset => dataset.GetProperty("data").EnumerateArray().Select(value => value.GetDouble()).ToArray());
			if (commits.Length == 0 ||
			    !datasets.TryGetValue("aweXpect time", out double[] times) || times.Length == 0 ||
			    !datasets.TryGetValue("aweXpect memory", out double[] allocated) || allocated.Length == 0)
			{
				continue;
			}

			Dictionary<string, double> ratios = new();
			foreach (string library in ComparedLibraries)
			{
				if (datasets.TryGetValue($"{library} time", out double[] otherTimes) && otherTimes.Length > 0)
				{
					// The libraries of a run are appended together, so the datasets align at their end.
					ratios[library] = Median(times.Reverse().Zip(otherTimes.Reverse(), (a, o) => a / o)
						.Take(BaselineRuns));
				}
			}

			benchmarks.Add((commits[^1].GetProperty("sha").GetString(), benchmark.Name,
				new BaselineBenchmark(Median(times.TakeLast(BaselineRuns)), allocated[^1], ratios)));
		}

		if (benchmarks.Count == 0)
		{
			return null;
		}

		string latestSha = benchmarks.GroupBy(benchmark => benchmark.Sha).MaxBy(group => group.Count())!.Key;
		return new BenchmarkBaseline(latestSha, benchmarks
			.Where(benchmark => benchmark.Sha == latestSha)
			.ToDictionary(benchmark => benchmark.Name, benchmark => benchmark.Benchmark));
	}

	static double Median(IEnumerable<double> values)
	{
		double[] sorted = values.Order().ToArray();
		int middle = sorted.Length / 2;
		return sorted.Length % 2 == 0 ? (sorted[middle - 1] + sorted[middle]) / 2 : sorted[middle];
	}

	static string FormatPercent(double relativeChange)
		=> (relativeChange * 100).ToString("+0;-0;±0", CultureInfo.InvariantCulture) + "%";

	/// <summary>
	///     Formats the <paramref name="ratio" /> of aweXpect to another library as the factor of the other library, as
	///     the docs site does, which keeps its precision when aweXpect is much faster.
	/// </summary>
	static string FormatRatio(double ratio)
		=> (1 / ratio).ToString(ratio > 0.1 ? "0.00" : "0.0", CultureInfo.InvariantCulture) + "×";

	static string FormatBytes(double bytes)
		=> bytes.ToString("N0", CultureInfo.InvariantCulture) + " B";

	static string FormatMemoryChange(double main, double current)
	{
		double difference = current - main;
		string change = $"{(difference > 0 ? StrongWorse : StrongBetter)} {difference.ToString("+#,0;-#,0", CultureInfo.InvariantCulture)} B";
		return main == 0 ? change : $"{change} ({FormatPercent(current / main - 1)})";
	}

	static string FormatTime(double nanoseconds)
		=> nanoseconds switch
		{
			< 1_000 => nanoseconds.ToString("0.0", CultureInfo.InvariantCulture) + " ns",
			< 1_000_000 => (nanoseconds / 1_000).ToString("0.00", CultureInfo.InvariantCulture) + " μs",
			_ => (nanoseconds / 1_000_000).ToString("0.00", CultureInfo.InvariantCulture) + " ms",
		};

	static string CreateRawBenchmarkResults(AbsolutePath report, Dictionary<string, BenchmarkMeasurement> measurements)
	{
		string[] fileContent = File.ReadAllLines(report);
		int meanColumn = -1;
		int allocatedColumn = -1;
		StringBuilder sb = new();
		sb.AppendLine("<details>");
		sb.AppendLine("<summary>Raw results</summary>");
		sb.AppendLine();
		int count = 0;
		foreach (string line in fileContent)
		{
			if (line.StartsWith("```"))
			{
				count++;
				if (count == 1)
				{
					sb.AppendLine("<pre>");
				}
				else if (count == 2)
				{
					sb.AppendLine("</pre>");
					sb.AppendLine();
				}

				continue;
			}

			if (line.StartsWith('|') && line.EndsWith('|'))
			{
				List<string> cells = line
					.Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
					.ToList();
				string method = cells[0];
				string meanComparison, allocatedComparison;
				if (method == "Method")
				{
					meanColumn = cells.IndexOf("Mean");
					allocatedColumn = cells.IndexOf("Allocated");
					meanComparison = allocatedComparison = "vs aweXpect";
				}
				else if (method.StartsWith('-'))
				{
					meanComparison = allocatedComparison = "---:";
				}
				else
				{
					meanComparison = CompareToAweXpect(measurements, method, m => m.Mean);
					allocatedComparison = CompareToAweXpect(measurements, method, m => m.Allocated);
				}

				cells.Insert(allocatedColumn + 1, allocatedComparison);
				cells.Insert(meanColumn + 1, meanComparison);
				AppendTableRow(sb, cells, method.EndsWith("_aweXpect"));
				continue;
			}

			sb.AppendLine(line);
		}

		sb.AppendLine();
		sb.AppendLine("</details>");
		return sb.ToString();
	}

	static Dictionary<string, BenchmarkMeasurement> ReadBenchmarkMeasurements(string path)
	{
		using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
		return document.RootElement.GetProperty("Benchmarks").EnumerateArray().ToDictionary(
			benchmark => benchmark.GetProperty("Method").GetString(),
			benchmark => new BenchmarkMeasurement(
				benchmark.GetProperty("Statistics").GetProperty("Mean").GetDouble(),
				benchmark.GetProperty("Memory").GetProperty("BytesAllocatedPerOperation").GetDouble()));
	}

	/// <summary>
	///     Returns by how many percent the <paramref name="method" /> is above or below the aweXpect benchmark of the
	///     same group.
	/// </summary>
	static string CompareToAweXpect(Dictionary<string, BenchmarkMeasurement> measurements, string method,
		Func<BenchmarkMeasurement, double> selector)
	{
		string aweXpectMethod = method.Substring(0, method.LastIndexOf('_') + 1) + "aweXpect";
		if (method == aweXpectMethod ||
		    !measurements.TryGetValue(method, out BenchmarkMeasurement measurement) ||
		    !measurements.TryGetValue(aweXpectMethod, out BenchmarkMeasurement aweXpect))
		{
			return "";
		}

		return FormatPercent((selector(measurement) / selector(aweXpect)) - 1);
	}

	static void AppendTableRow(StringBuilder sb, List<string> cells, bool bold)
	{
		sb.Append('|');
		foreach (string cell in cells)
		{
			sb.Append(' ');
			sb.Append(bold && cell != "" ? $"**{cell}**" : cell);
			sb.Append(" |");
		}

		sb.AppendLine();
	}

	// ReSharper disable InconsistentNaming
	// ReSharper disable NotAccessedPositionalProperty.Local
	private record GithubUpdateFile(string message, string content, string sha, string branch);
	// ReSharper restore NotAccessedPositionalProperty.Local
	// ReSharper restore InconsistentNaming

	private record BenchmarkFile(string Content, string Sha);

	private record BenchmarkMeasurement(double Mean, double Allocated);

	private record BaselineBenchmark(double Mean, double Allocated, Dictionary<string, double> Ratios);

	private record BenchmarkBaseline(string Sha, Dictionary<string, BaselineBenchmark> Benchmarks);

	private record BenchmarkCommentSection(string Package, string Summary, string RawResults);
}
