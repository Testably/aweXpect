using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tools.Git;
using Serilog;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	/// <summary>
	///     The source directories of each package. A commit belongs to the release notes of a package when it changes
	///     one of them.
	/// </summary>
	static readonly (string Package, string[] Directories)[] PackageDirectories =
	[
		("aweXpect", [
			"Source/aweXpect", "Source/aweXpect.Analyzers", "Source/aweXpect.Analyzers.CodeFixers",
			"Source/aweXpect.Generators", "Source/aweXpect.SourceGenerators",
		]),
		("aweXpect.Core", ["Source/aweXpect.Core", "Source/aweXpect.Core.Generators",]),
		("aweXpect.Json", ["Source/aweXpect.Json",]),
		("aweXpect.Web", ["Source/aweXpect.Web",]),
		("aweXpect.Xml", ["Source/aweXpect.Xml",]),
		("aweXpect.Reflection", ["Source/aweXpect.Reflection",]),
		("aweXpect.Snapshots", ["Source/aweXpect.Snapshots",]),
		("aweXpect.Snapshots.DiffEngine", ["Source/aweXpect.Snapshots.DiffEngine",]),
		("aweXpect.Snapshots.PublicApi", ["Source/aweXpect.Snapshots.PublicApi",]),
	];

	static readonly string[] ReleaseNotesCategories =
		["⚠️ Breaking changes", "🚀 New features", "🐞 Bug fixes", "🧰 Other changes",];

	/// <summary>
	///     Writes the changes of each package since the last stable release into <c>Artifacts/ReleaseNotes.md</c>.
	/// </summary>
	/// <remarks>
	///     Pull requests are squash-merged with a conventional-commit title, so every commit on <c>main</c> is one pull
	///     request and its subject tells the kind of change. Only the first parents are followed, so that the history of
	///     a package that was imported with a merge commit counts as that one commit.
	/// </remarks>
	Target ReleaseNotes => _ => _
		.After(Pack)
		.Executes(() =>
		{
			string previousRelease = GitTasks
				.Git("describe --tags --abbrev=0 --match v* --exclude v*-* HEAD^", logOutput: false)
				.Single().Text;

			StringBuilder sb = new();
			foreach ((string package, string[] directories) in PackageDirectories)
			{
				ILookup<string, string> changes = GitTasks
					.Git($"log --first-parent {previousRelease}..HEAD --format=%s -- {string.Join(" ", directories):nq}",
						logOutput: false)
					.Select(output => output.Text)
					.ToLookup(GetReleaseNotesCategory);

				sb.AppendLine($"## {package}").AppendLine();
				if (changes.Count == 0)
				{
					sb.AppendLine($"No changes since {previousRelease}.").AppendLine();
					continue;
				}

				foreach (string category in ReleaseNotesCategories.Where(changes.Contains))
				{
					sb.AppendLine($"### {category}").AppendLine();
					foreach (string subject in changes[category])
					{
						sb.AppendLine($"- {subject}");
					}

					sb.AppendLine();
				}
			}

			ArtifactsDirectory.CreateDirectory();
			File.WriteAllText(ArtifactsDirectory / "ReleaseNotes.md", sb.ToString());
			Log.Information("Release notes since {PreviousRelease}:\n{ReleaseNotes}", previousRelease, sb);
		});

	static string GetReleaseNotesCategory(string subject)
	{
		Match match = Regex.Match(subject, @"\A(?<type>[a-z]+)(\([^)]*\))?(?<breaking>!)?:");
		if (match.Groups["breaking"].Success)
		{
			return ReleaseNotesCategories[0];
		}

		return match.Groups["type"].Value switch
		{
			"feat" => ReleaseNotesCategories[1],
			"fix" => ReleaseNotesCategories[2],
			_ => ReleaseNotesCategories[3],
		};
	}
}
