using System.Globalization;
using System.Threading;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Snapshots.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class NamedAfterTestArgumentsTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			[Arguments("Abbey Road", 1969)]
			[Arguments("Revolver", 1966)]
			public async Task ShouldNameTheSnapshotAfterTheArgumentsOfTheTestCase(string title, int year)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().NamedAfterTestArguments.Set(true);

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that "foo"
					              matches the snapshot "MyTests.MyTest.{title}_{year}",
					              but the snapshot did not exist

					              Actual:
					              foo
					              """);
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Name = $"{title}_{year}", },
				});
			}

			[Test]
			[Arguments("1.2.3.4.5.6.7.8.9.10.11.12.13.14.15.16.17.18.19.20")]
			public async Task WhenArgumentsAreLongOnceEscaped_ShouldShortenTheName(string version)
			{
				await ShouldBeNamed("1.2.3.4.5.6.7.8.9.10.11.12.13~9b01af3b", result => result,
					$"each '.' in {version} takes three characters in the file name, where the length is limited");
			}

			[Test]
			[Arguments("Sgt. Pepper's Lonely Hearts Club Band (Remastered Super Deluxe Edition, Disc One)")]
			[Arguments("Sgt. Pepper's Lonely Hearts Club Band (Remastered Super Deluxe Edition, Disc Two)")]
			public async Task WhenArgumentsAreLong_ShouldShortenTheNameAndAppendAHashOfTheWholeName(string title)
			{
				string name = title.EndsWith("One)")
					? "Sgt. Pepper's Lonely Hearts Club Band (Remastered Sup~e0c652ac"
					: "Sgt. Pepper's Lonely Hearts Club Band (Remastered Sup~cd159f2a";

				await ShouldBeNamed(name, result => result,
					"a file name has a limited length, and the hash keeps two names apart that only differ after the first 55 characters in the file name");
			}

			[Test]
			[Arguments("Abbey Road")]
			public async Task WhenDisabled_ShouldKeepTheSnapshotOfTheTest(string title)
			{
				await ShouldBeNamed(null, result => result,
					$"changing the snapshot of every parameterized test, e.g. of {title}, is breaking, so it is opt-in",
					false);
			}

			[Test]
			[Arguments("Abbey Road")]
			public async Task WhenNamed_ShouldAppendTheNameToTheArguments(string title)
			{
				await ShouldBeNamed($"{title}_cover", result => result.Named("cover"),
					"each case of a parameterized test can hold several snapshots");
			}

			[Test]
			[Arguments("Abbey Road")]
			public async Task WhenNamedAfter_ShouldReplaceTheArguments(string title)
			{
				await ShouldBeNamed("Let It Be", result => result.NamedAfter("Let It Be"),
					$".NamedAfter(…) names the case explicitly instead of the arguments, here of {title}");
			}

			[Test]
			[Arguments("Sgt. Pepper's Lonely Hearts Club Band (Remastered Super Deluxe Edition")]
			public async Task WhenNamedAndTheNameBecomesLong_ShouldShortenTheWholeName(string title)
			{
				await ShouldBeNamed("Sgt. Pepper's Lonely Hearts Club Band (Remastered Sup~8e83c61c",
					result => result.Named("Disc One"),
					$"the name appended to the arguments of {title} counts towards the limited length");
			}

			[Test]
			[Arguments("Abbey Road")]
			public async Task WhenNamedTwice_ShouldAppendOnlyTheLastName(string title)
			{
				await ShouldBeNamed($"{title}_back", result => result.Named("cover").Named("back"),
					"a later name replaces an earlier one");
			}

			[Test]
			public async Task WhenTestHasNoArguments_ShouldKeepTheSnapshotOfTheTest()
			{
				await ShouldBeNamed(null, result => result,
					"a test without parameters has only one case, so its snapshot keeps its name");
			}

			[Test]
			public async Task WhenTestHasOnlyACancellationToken_ShouldKeepTheSnapshotOfTheTest(
				CancellationToken cancellationToken)
			{
				await ShouldBeNamed(null, result => result,
					$"the injected token (canceled: {cancellationToken.IsCancellationRequested}) does not tell cases apart");
			}

			[Test]
			[Arguments(new[] { 1, 2, 3, })]
			public async Task WithArray_ShouldWriteItsItems(int[] tracks)
			{
				await ShouldBeNamed("[1, 2, 3]", result => result,
					$"the name of the type of the {tracks.Length} tracks would be the same in every case");
			}

			[Test]
			[Arguments("Abbey Road")]
			public async Task WithCancellationToken_ShouldSkipIt(string title, CancellationToken cancellationToken)
			{
				await ShouldBeNamed(title, result => result,
					$"the injected token (canceled: {cancellationToken.IsCancellationRequested}) does not tell cases apart");
			}

			[Test]
			[Arguments(null, "")]
			public async Task WithNullAndEmptyString_ShouldWriteThemVisibly(string? artist, string title)
			{
				await ShouldBeNamed("null_\"\"", result => result,
					$"an empty part ({artist}{title}) would make the name ambiguous");
			}

			[Test]
			[Arguments(0.30000000000000004, 1.5f, 1234.5)]
			public async Task WithNumbers_ShouldWriteThemRoundTripInTheInvariantCulture(double first, float second,
				decimal third)
			{
				CultureInfo previousCulture = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("de-DE");
				try
				{
					await ShouldBeNamed("0.30000000000000004_1.5_1234.5", result => result,
						$"the name must be the same in every culture and on every target framework, also for {first}, {second} and {third}");
				}
				finally
				{
					CultureInfo.CurrentCulture = previousCulture;
				}
			}

			private async Task ShouldBeNamed(string? name,
				Func<StringSnapshotResult<string, IThat<string?>>, StringSnapshotResult<string, IThat<string?>>> configure,
				string because, bool isEnabled = true)
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = name, }] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ =
					Customize.aweXpect.Snapshots().NamedAfterTestArguments.Set(isEnabled);

				async Task Act()
					=> await configure(That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).DoesNotThrow().Because(because);
			}
		}
	}
}
