using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

namespace Build;

[GitHubActions(
	"Build",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = false,
	ImportSecrets = [nameof(GithubToken),]
)]
partial class Build : NukeBuild
{
	/// <summary>
	///     Set this flag temporarily when you introduce breaking changes in the core library.
	///     This will change the build pipeline to only build and publish the aweXpect.Core or aweXpect package.
	///     <para />
	///     Afterward, you can update the package reference in `Directory.Packages.props` and reset this flag.
	/// </summary>
	readonly BuildScope BuildScope = BuildScope.CoreOnly;

	[Parameter("Github Token")] readonly string GithubToken;
	[GitRepository] readonly GitRepository Repository;

	[Solution(GenerateProjects = true)] readonly Solution Solution;

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	AbsolutePath ArtifactsDirectory => RootDirectory / "Artifacts";
	AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";
	GitHubActions GitHubActions => GitHubActions.Instance;

	public static int Main() => Execute<Build>(x => x.Pack, x => x.ApiChecks, x => x.Benchmarks, x => x.CodeAnalysis);
}
