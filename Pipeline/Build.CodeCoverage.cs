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
				// The generic format of Sonar lists the lines per file. A format that lists them per method leaves out the
				// lines of a lambda in a generic class, because ReportGenerator keeps them in a class without methods.
				.AddReportTypes(ReportTypes.SonarQube)
				.AddFileFilters("-*.g.cs")
				.SetAssemblyFilters("+aweXpect*"));
		});
}
