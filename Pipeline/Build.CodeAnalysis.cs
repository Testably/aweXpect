using Fallout.Common;
using Fallout.Common.Tools.SonarScanner;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	[Parameter("The key to push to sonarcloud")] [Secret] readonly string SonarToken;

	Target CodeAnalysisBegin => _ => _
		.Unlisted()
		.Before(Compile)
		.Before(CodeCoverage)
		.Executes(() =>
		{
			Configuration = Configuration.Debug;

			SonarScannerTasks.SonarScannerBegin(s => s
				.SetOrganization("testably")
				.SetProjectKey("Testably_aweXpect")
				.AddVSTestReports(TestResultsDirectory / "*.trx")
				.AddOpenCoverPaths(TestResultsDirectory / "reports" / "OpenCover.xml")
				.SetPullRequestOrBranchName(GitHubActions, BranchName)
				.SetVersion(SemVer)
				.SetToken(SonarToken));
		});

	Target CodeAnalysisEnd => _ => _
		.Unlisted()
		.DependsOn(Compile)
		.DependsOn(CodeCoverage)
		.OnlyWhenDynamic(() => IsServerBuild && (BuildScope == BuildScope.Default || GitHubActions.IsPullRequest))
		.Executes(() =>
		{
			SonarScannerTasks.SonarScannerEnd(s => s
				.SetToken(SonarToken));
		});

	Target CodeAnalysis => _ => _
		.DependsOn(CodeAnalysisBegin)
		.DependsOn(CodeAnalysisEnd);
}
