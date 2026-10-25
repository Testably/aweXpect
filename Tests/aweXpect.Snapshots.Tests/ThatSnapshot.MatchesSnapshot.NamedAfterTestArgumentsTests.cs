using System.Globalization;
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
			[Arguments("Sgt. Pepper's Lonely Hearts Club Band (Remastered Super Deluxe Edition, Disc One)")]
			[Arguments("Sgt. Pepper's Lonely Hearts Club Band (Remastered Super Deluxe Edition, Disc Two)")]
			public async Task WhenArgumentsAreLong_ShouldShortenTheNameAndAppendAHashOfTheWholeName(string title)
			{
				string name = title.EndsWith("One)")
					? "Sgt. Pepper's Lonely Hearts Club Band (Remastered Super~e0c652ac"
					: "Sgt. Pepper's Lonely Hearts Club Band (Remastered Super~cd159f2a";

				await ShouldBeNamed(name, result => result,
					"a file name has a limited length, and the hash keeps two names apart that only differ after the first 55 characters");
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
			public async Task WhenNamed_ShouldUseTheName(string title)
			{
				await ShouldBeNamed("cover", result => result.Named("cover"),
					$"an explicit name replaces the name of the arguments, here of {title}");
			}

			[Test]
			public async Task WhenTestHasNoArguments_ShouldKeepTheSnapshotOfTheTest()
			{
				await ShouldBeNamed(null, result => result,
					"a test without parameters has only one case, so its snapshot keeps its name");
			}

			[Test]
			[Arguments(new[] { 1, 2, 3, })]
			public async Task WithArray_ShouldWriteItsItems(int[] tracks)
			{
				await ShouldBeNamed("[1,2,3]", result => result,
					$"the name of the type of the {tracks.Length} tracks would be the same in every case");
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
				Func<SnapshotResult<string, IThat<string?>>, SnapshotResult<string, IThat<string?>>> configure,
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
