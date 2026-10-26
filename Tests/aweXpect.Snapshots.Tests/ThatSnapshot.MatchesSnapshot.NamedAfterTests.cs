using System.Collections.Generic;
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
		public sealed class NamedAfterTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task ShouldNameTheSnapshotAfterTheValuesOfTheArguments()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string title = "Abbey Road";
				int year = 1969;

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).NamedAfter(title, year);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that "foo"
					             matches the snapshot "MyTests.MyTest.Abbey Road_1969",
					             but the snapshot did not exist

					             Actual:
					             foo
					             """);
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Name = "Abbey Road_1969", },
				}).InAnyOrder();
			}

			[Test]
			public async Task WhenAccepting_ShouldAcceptTheNamedSnapshot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string title = "Let It Be";

				async Task Act()
					=> await That("id=42").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("42").Mask("<id>").NamedAfter(title).Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<FailException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with { Name = "Let It Be", }] = "id=<id>",
				});
			}

			[Test]
			public async Task WhenArgumentIsCollectionThatContainsItself_ShouldNotOverflowTheStack()
			{
				List<object> tracks = ["Come Together",];
				tracks.Add(tracks);

				await ShouldBeNamed("[\"Come Together\", [ *recursive* ]]",
					result => result.NamedAfter(tracks),
					"a collection that contains itself would otherwise end the test run");
			}

			[Test]
			[Arguments(1.0 / 3, "0.3333333333333333")]
			[Arguments(123456789012345.6, "123456789012345.6")]
			[Arguments(1234567890123456.7, "1234567890123456.8")]
			[Arguments(904097094155893.2, "904097094155893.2")]
			[Arguments(5.2037E-05, "5.2037E-05")]
			[Arguments(-8.511904705078307E-47, "-8.511904705078307E-47")]
			[Arguments(1e16, "10000000000000000")]
			[Arguments(1.23e17, "1.23E+17")]
			[Arguments(0.0001, "0.0001")]
			[Arguments(-1e-5, "-1E-05")]
			[Arguments(5e-324, "5E-324")]
			[Arguments(double.MaxValue, "1.7976931348623157E+308")]
			[Arguments(-0.0, "-0")]
			[Arguments(double.NegativeInfinity, "-Infinity")]
			[Arguments(5.9604644775390625E-08, "5.960464477539063E-08")]
			[Arguments(2.98023223876953125E-08, "2.9802322387695312E-08")]
			[Arguments(4.1045368012983762E-289, "4.1045368012983762E-289")]
			public async Task WhenArgumentIsDouble_ShouldWriteTheShortestTextThatRoundTrips(double value,
				string expected)
			{
				await ShouldBeNamed(expected,
					result => result.NamedAfter(value),
					".NET Framework writes other digits with \"R\" than .NET, but the name must be the same on every target framework");
			}

			[Test]
			[Arguments(1.0000001f, "1.0000001")]
			[Arguments(float.MaxValue, "3.4028235E+38")]
			[Arguments(16777216f, "16777216")]
			[Arguments(1e9f, "1E+09")]
			[Arguments(1e-45f, "1E-45")]
			[Arguments(0.1f, "0.1")]
			[Arguments(1.0758664E-24f, "1.0758664E-24")]
			[Arguments(-3.2145237E-37f, "-3.2145237E-37")]
			public async Task WhenArgumentIsFloat_ShouldWriteTheShortestTextThatRoundTrips(float value,
				string expected)
			{
				await ShouldBeNamed(expected,
					result => result.NamedAfter(value),
					".NET Framework writes other digits with \"R\" than .NET, but the name must be the same on every target framework");
			}

			[Test]
			public async Task WhenArgumentIsFormattable_ShouldUseTheInvariantCulture()
			{
				CultureInfo previousCulture = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("de-DE");
				try
				{
					decimal price = 12.99m;
					DateTime released = new(1969, 9, 26);

					await ShouldBeNamed("12.99_1969-09-26T00:00:00.0000000",
						result => result.NamedAfter(price, released),
						"the snapshot must have the same name on every machine, with a date in ISO 8601");
				}
				finally
				{
					CultureInfo.CurrentCulture = previousCulture;
				}
			}

			[Test]
			public async Task WhenArgumentIsNull_ShouldWriteNull()
			{
				string? title = null;

				await ShouldBeNamed("null",
					result => result.NamedAfter(title),
					"a missing value has no text of its own");
			}

			[Test]
			public async Task WhenArgumentIsRecord_ShouldWriteItsMembers()
			{
				Album album = new("Abbey Road", 1969);

				await ShouldBeNamed("ThatSnapshot.MatchesSnapshot.NamedAfterTests.Albu~b61ea1a9",
					result => result.NamedAfter(album),
					"a record is written like in a snapshot, with the name of its type and each member");
			}

			[Test]
			public async Task WhenArgumentIsSet_ShouldSortTheItems()
			{
				HashSet<string> titles = ["Let It Be", "Abbey Road",];

				await ShouldBeNamed("[\"Abbey Road\", \"Let It Be\"]",
					result => result.NamedAfter(titles),
					"the order of a set can differ from run to run");
			}

			[Test]
			public async Task WhenArgumentIsStringArray_ShouldWriteItsItems()
			{
				string[] titles = ["Abbey Road", "Let It Be",];

				await ShouldBeNamed("[\"Abbey Road\", \"Let It Be\"]",
					result => result.NamedAfter(titles),
					"a single array is one argument, like an array argument of the running test, and is not spread into the further arguments");
			}

			[Test]
			public async Task WhenArgumentIsTuple_ShouldUseTheInvariantCulture()
			{
				CultureInfo previousCulture = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("de-DE");
				try
				{
					(double, string) track = (4.5, "Something");

					await ShouldBeNamed("(4.5, \"Something\")",
						result => result.NamedAfter(track),
						"the snapshot must have the same name on every machine");
				}
				finally
				{
					CultureInfo.CurrentCulture = previousCulture;
				}
			}

			[Test]
			public async Task WhenArgumentThrowsInToString_ShouldNameTheSnapshotAfterTheException()
			{
				Track track = new();

				await ShouldBeNamed("[ToString of ThatSnapshot.MatchesSnapshot.NamedAfte~28799a7b",
					result => result.NamedAfter(track),
					"the exception is written like in a snapshot instead of failing the test");
			}

			[Test]
			public async Task WhenArgumentsAreWrittenLikeTestArguments_ShouldUseTheSameName()
			{
				double[] tracks = [1, 0.1 + 0.2,];

				await ShouldBeNamed("0.30000000000000004_[1, 0.30000000000000004]_\"\"",
					result => result.NamedAfter(0.1 + 0.2, tracks, ""),
					"the arguments of .NamedAfter(…) and of the running test are written alike");
			}

			[Test]
			public async Task WhenFifthArgumentIsNull_ShouldWriteNull()
			{
				string artist = "The Beatles";
				string title = "Abbey Road";
				int year = 1969;
				int tracks = 17;

				await ShouldBeNamed("The Beatles_Abbey Road_1969_17_null",
					result => result.NamedAfter(artist, title, year, tracks, null),
					"a single null is passed as the array of the further arguments");
			}

			[Test]
			public async Task WhenFifthArgumentIsStringArray_ShouldWriteItsItemsOneByOne()
			{
				string artist = "The Beatles";
				string title = "Abbey Road";
				int year = 1969;
				int tracks = 17;
				string[] labels = ["Apple", "EMI",];

				await ShouldBeNamed("The Beatles_Abbey Road_1969_17_Apple_EMI",
					result => result.NamedAfter(artist, title, year, tracks, labels),
					"an array of a reference type as the fifth and last argument is passed as the further arguments themselves");
			}

			[Test]
			public async Task WhenNamedAfterFiveArguments_ShouldUseAllOfThem()
			{
				string artist = "The Beatles";
				string title = "Abbey Road";
				int year = 1969;
				int tracks = 17;
				string label = "Apple";

				await ShouldBeNamed("The Beatles_Abbey Road_1969_17_Apple",
					result => result.NamedAfter(artist, title, year, tracks, label),
					"each argument adds its value");
			}

			[Test]
			public async Task WhenNamedAfterFourArguments_ShouldUseAllOfThem()
			{
				string artist = "The Beatles";
				string title = "Abbey Road";
				int year = 1969;
				int tracks = 17;

				await ShouldBeNamed("The Beatles_Abbey Road_1969_17",
					result => result.NamedAfter(artist, title, year, tracks),
					"each argument adds its value");
			}

			[Test]
			public async Task WhenNamedAfterOneArgument_ShouldUseIt()
			{
				string title = "Abbey Road";

				await ShouldBeNamed("Abbey Road",
					result => result.NamedAfter(title),
					"the value of the argument names the snapshot");
			}

			[Test]
			public async Task WhenNamedAfterSixArguments_ShouldUseAllOfThem()
			{
				string artist = "The Beatles";
				string title = "Abbey Road";
				int year = 1969;
				int tracks = 17;
				string label = "Apple";
				bool isRemastered = true;

				await ShouldBeNamed("The Beatles_Abbey Road_1969_17_Apple_True",
					result => result.NamedAfter(artist, title, year, tracks, label, isRemastered),
					"each argument adds its value");
			}

			[Test]
			public async Task WhenNamedAfterThreeArguments_ShouldUseAllOfThem()
			{
				string title = "Let It Be";
				int year = 1970;
				bool isLive = false;

				await ShouldBeNamed("Let It Be_1970_False",
					result => result.NamedAfter(title, year, isLive),
					"each argument adds its value");
			}

			[Test]
			public async Task WhenNamedBefore_ShouldReplaceTheName()
			{
				string title = "Abbey Road";

				await ShouldBeNamed("Abbey Road",
					result => result.Named("login").NamedAfter(title),
					"the last name wins, like for .Named(…)");
			}

			[Test]
			public async Task WhenParameterIsRenamed_ShouldKeepTheName()
			{
				Album album = new("Abbey Road", 1969);

				await ShouldBeNamed("Abbey Road_1969",
					result => result.NamedAfter(album.Title, album.Year),
					"only the values are written, so that renaming a parameter or a member keeps the snapshot");
			}

			[Test]
			public async Task WhenSnapshotIsUsedForAnotherValueWithOtherArguments_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = "Abbey Road", }] = "foo";
				store.Snapshots[Id with { Name = "Let It Be", }] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act(string title, string subject)
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).NamedAfter(title);

				await That(() => Act("Abbey Road", "foo")).DoesNotThrow();
				await That(() => Act("Let It Be", "bar")).DoesNotThrow()
					.Because("the cases of a parameterized test are kept apart by their arguments");
			}

			private async Task ShouldBeNamed(string name,
				Func<StringSnapshotResult<string, IThat<string?>>, StringSnapshotResult<string, IThat<string?>>> nameSnapshot,
				string because)
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = name, }] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await nameSnapshot(That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).DoesNotThrow().Because(because);
			}

			private sealed record Album(string Title, int Year);

			private sealed class Track
			{
				public override string ToString() => throw new InvalidOperationException("No title");
			}
		}
	}
}
