using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace aweXpect.Api.Tests;

public sealed class ApiAcceptance
{
	/// <summary>
	///     Execute this test to record the current public API surface of the projects as shipped.
	/// </summary>
	/// <remarks>
	///     The build enforces the API surface listed in <c>Source/&lt;project&gt;/PublicAPI/&lt;framework&gt;/</c>
	///     with <c>Microsoft.CodeAnalysis.PublicApiAnalyzers</c>. This test empties those files, builds the projects
	///     with RS0016 downgraded to a warning and writes every reported symbol to <c>PublicAPI.Shipped.txt</c>, so
	///     <c>PublicAPI.Unshipped.txt</c> is left empty. The changes become part of the pull request and are reviewed
	///     accordingly.
	/// </remarks>
	[Test]
	[Explicit]
	public async Task AcceptApiChanges()
	{
		string[] projects = ["aweXpect", "aweXpect.Core",];
		string[] frameworks = Helper.GetTargetFrameworks().ToArray();

		foreach (string project in projects)
		{
			foreach (string framework in frameworks)
			{
				Helper.ResetPublicApi(project, framework);
			}
		}

		string buildOutput = await Helper.BuildWithUndeclaredPublicApiAsWarnings(projects[0]);
		Dictionary<(string Project, string Framework), SortedSet<string>> publicApi =
			Helper.ParseUndeclaredPublicApi(buildOutput);

		foreach (string project in projects)
		{
			foreach (string framework in frameworks)
			{
				await Expect.That(publicApi).ContainsKey((project, framework))
					.Because($"the build output should report the public API of {project} for {framework}");
				Helper.SetShippedPublicApi(project, framework, publicApi[(project, framework)]);
			}
		}
	}
}
