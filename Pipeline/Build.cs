using Fallout.Common;
using Fallout.Common.CI.GitHubActions;
using Fallout.Common.Git;
using Fallout.Common.IO;
using Fallout.Solutions;

namespace Build;

[GitHubActions(
	"Build",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = false,
	ImportSecrets = [nameof(GithubToken),]
)]
partial class Build : FalloutBuild
{
	[Parameter("Github Token")] readonly string GithubToken;
	[GitRepository] readonly GitRepository Repository;

	[Solution(GenerateProjects = true)] readonly Solution Solution;

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	AbsolutePath ArtifactsDirectory => RootDirectory / "Artifacts";
	AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";
	GitHubActions GitHubActions => GitHubActions.Instance;

	public static int Main() => Execute<Build>(x => x.Pack, x => x.ApiChecks, x => x.DocsTests, x => x.Benchmarks, x => x.CodeAnalysis);
}
