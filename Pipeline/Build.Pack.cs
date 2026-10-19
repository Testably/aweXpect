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
	/// <summary>
	///     The projects whose packages are released.
	/// </summary>
	Project[] PackageProjects =>
	[
		Solution.aweXpect_Core,
		Solution.aweXpect,
		Solution.aweXpect_Json,
		Solution.aweXpect_Web,
		Solution.aweXpect_Xml,
		Solution.aweXpect_Reflection,
	];

	Target Pack => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			AbsolutePath packagesDirectory = ArtifactsDirectory / "Packages";
			packagesDirectory.CreateOrCleanDirectory();

			List<string> packages = new();
			foreach (Project project in PackageProjects)
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
