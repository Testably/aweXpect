using Fallout.Common;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.ReportGenerator;
using static Fallout.Common.Tools.ReportGenerator.ReportGeneratorTasks;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	Target CodeCoverage => _ => _
		.DependsOn(TestFrameworks)
		.DependsOn(UnitTests)
		.Executes(() =>
		{
			ReportGenerator(s => s
				.SetProcessToolPath(NuGetToolPathResolver.GetPackageExecutable("ReportGenerator", "ReportGenerator.dll",
					framework: "net8.0"))
				.SetTargetDirectory(TestResultsDirectory / "reports")
				// The VSTest projects write `coverage.cobertura.xml` into a directory per run, the projects of the
				// Microsoft.Testing.Platform a file per run into the results directory itself.
				.AddReports(TestResultsDirectory / "**/coverage.cobertura.xml", TestResultsDirectory / "*.cobertura.xml")
				.AddReportTypes(ReportTypes.OpenCover)
				.AddFileFilters("-*.g.cs")
				.SetAssemblyFilters("+aweXpect*"));
		});
}
