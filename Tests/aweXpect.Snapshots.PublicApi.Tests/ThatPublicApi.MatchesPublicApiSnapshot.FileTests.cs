using System.IO;
using static aweXpect.Snapshots.PublicApi.Tests.TestHelpers.Compiler;

namespace aweXpect.Snapshots.PublicApi.Tests;

public sealed partial class ThatPublicApi
{
	public sealed partial class MatchesPublicApiSnapshot
	{
		public sealed class FileTests
		{
			[Fact]
			public async Task ShouldDescribeThePublicApiOfTheAssemblyInTheFile()
			{
				FileInfo subject = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
					namespace Library;
					public class Api
					{
					    public string Name { get; set; } = "";
					    public void Play(int volume = 5) { }
					}
					"""));

				await That(subject).MatchesPublicApiSnapshot();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				FileInfo? subject = null;

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "*",
					             but it was <null>
					             """).AsWildcard().IgnoringNewlineStyle();
			}
		}
	}
}
