using System.Collections.Generic;
using System.IO;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Solutions;
using Fallout.Common.Utilities.Collections;
using static Serilog.Log;

// ReSharper disable UnusedMember.Local
// ReSharper disable AllUnderscoreLocalParameterName

namespace Build;

partial class Build
{
	Target Pack => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			AbsolutePath packagesDirectory = ArtifactsDirectory / "Packages";
			packagesDirectory.CreateOrCleanDirectory();

			Project[] projects = BuildScope switch
			{
				BuildScope.CoreOnly => [Solution.aweXpect_Core,],
				BuildScope.MainOnly => [Solution.aweXpect,],
				_ => [Solution.aweXpect_Core, Solution.aweXpect,],
			};

			List<string> packages = new();
			foreach (Project project in projects)
			{
				foreach (string package in
				         Directory.EnumerateFiles(project.Directory / "bin", "*.nupkg", SearchOption.AllDirectories))
				{
					Directory.CreateDirectory(packagesDirectory / project.Name);
					File.Move(package, packagesDirectory / project.Name / Path.GetFileName(package));
					Debug("Found nuget package: {PackagePath}", package);
					packages.Add(Path.GetFileName(package));
				}

				foreach (string symbolPackage in
				         Directory.EnumerateFiles(project.Directory / "bin", "*.snupkg", SearchOption.AllDirectories))
				{
					File.Move(symbolPackage, packagesDirectory / project.Name / Path.GetFileName(symbolPackage));
					Debug("Found symbol package: {PackagePath}", symbolPackage);
				}
			}

			ReportSummary(s => s
				.AddPair("Packages", string.Join(", ", packages)));
		});
}
