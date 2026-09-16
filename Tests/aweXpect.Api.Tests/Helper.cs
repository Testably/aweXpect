using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace aweXpect.Api.Tests;

internal static partial class Helper
{
	private const string Header = "#nullable enable";

	public static IEnumerable<string> GetTargetFrameworks()
	{
		string props = CombinedPaths("Source", "Directory.Build.props");
		XDocument project = XDocument.Load(props);
		XElement? targetFrameworks =
			project.XPathSelectElement("/Project/PropertyGroup/TargetFrameworks");
		foreach (string targetFramework in targetFrameworks!.Value.Split(';'))
		{
			yield return targetFramework;
		}
	}

	public static void ResetPublicApi(string project, string framework)
	{
		WritePublicApi(project, framework, "PublicAPI.Shipped.txt", []);
		WritePublicApi(project, framework, "PublicAPI.Unshipped.txt", []);
	}

	public static void SetShippedPublicApi(string project, string framework, IEnumerable<string> entries)
		=> WritePublicApi(project, framework, "PublicAPI.Shipped.txt", entries);

	/// <summary>
	///     Builds the <paramref name="project" /> and the projects it references with RS0016 and RS0017 downgraded to
	///     warnings, so that the compilation runs to the end and reports every undeclared public symbol.
	/// </summary>
	public static async Task<string> BuildWithUndeclaredPublicApiAsWarnings(string project)
	{
		ProcessStartInfo startInfo = new("dotnet")
		{
			WorkingDirectory = CombinedPaths(),
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
		};
		startInfo.ArgumentList.Add("build");
		startInfo.ArgumentList.Add(CombinedPaths("Source", project, $"{project}.csproj"));
		startInfo.ArgumentList.Add("--configuration");
		startInfo.ArgumentList.Add("Debug");
		startInfo.ArgumentList.Add("--verbosity");
		startInfo.ArgumentList.Add("quiet");
		startInfo.ArgumentList.Add("--nologo");
		// A reusable MSBuild worker node or compiler server started by this build would inherit the redirected
		// output pipes and outlive the build, so reading the output would never complete.
		startInfo.ArgumentList.Add("--disable-build-servers");
		// `%3B` is the MSBuild escape for `;`, which would otherwise separate properties.
		startInfo.ArgumentList.Add("-p:WarningsNotAsErrors=CS1591%3BRS0016%3BRS0017");
		// Only the compilation is needed; packing would require the analyzer projects to be built first.
		startInfo.ArgumentList.Add("-p:GeneratePackageOnBuild=false");
		startInfo.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en-US";

		using Process process = Process.Start(startInfo)!;
		Task<string> output = process.StandardOutput.ReadToEndAsync();
		Task<string> error = process.StandardError.ReadToEndAsync();
		await process.WaitForExitAsync();
		if (process.ExitCode != 0)
		{
			throw new InvalidOperationException(
				$"'dotnet build' failed with exit code {process.ExitCode}:{Environment.NewLine}{await output}{await error}");
		}

		return await output;
	}

	public static Dictionary<(string Project, string Framework), SortedSet<string>> ParseUndeclaredPublicApi(
		string buildOutput)
	{
		Dictionary<(string Project, string Framework), SortedSet<string>> result = new();
		foreach (Match match in UndeclaredPublicApiRegex().Matches(buildOutput))
		{
			(string, string) key = (match.Groups["project"].Value, match.Groups["framework"].Value);
			if (!result.TryGetValue(key, out SortedSet<string>? entries))
			{
				entries = new SortedSet<string>(StringComparer.Ordinal);
				result[key] = entries;
			}

			entries.Add(match.Groups["entry"].Value);
		}

		return result;
	}

	/// <summary>
	///     Matches a line such as
	///     <c>…: warning RS0016: Symbol 'static aweXpect.ThatString.IsEmpty(…) -> …' is not part of the declared public API (…) [C:\…\Source\aweXpect\aweXpect.csproj::TargetFramework=net8.0]</c>.
	/// </summary>
	[GeneratedRegex(
		@"warning RS0016: Symbol '(?<entry>.*)' is not part of the declared public API .*[\\/]Source[\\/](?<project>[^\\/]+)[\\/]\k<project>\.csproj::TargetFramework=(?<framework>[^\]]+)\]",
		RegexOptions.Multiline)]
	private static partial Regex UndeclaredPublicApiRegex();

	private static void WritePublicApi(string project, string framework, string fileName, IEnumerable<string> entries)
	{
		string path = CombinedPaths("Source", project, "PublicAPI", framework, fileName);
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllLines(path, entries.Prepend(Header));
	}

	private static string CombinedPaths(params string[] paths) =>
		Path.GetFullPath(Path.Combine(paths.Prepend(GetSolutionDirectory()).ToArray()));

	private static string GetSolutionDirectory([CallerFilePath] string path = "") =>
		Path.Combine(Path.GetDirectoryName(path)!, "..", "..");
}
