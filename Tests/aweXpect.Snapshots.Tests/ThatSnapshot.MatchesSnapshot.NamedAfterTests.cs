using System.Collections.Generic;
using System.Globalization;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class NamedAfterTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task ShouldNameTheSnapshotAfterTheNamesAndValuesOfTheArguments()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string title = "Abbey Road";
				int year = 1969;

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).NamedAfter(title, year);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that "foo"
					             matches the snapshot "MyTests.MyTest.title=Abbey Road_year=1969",
					             but the snapshot did not exist

					             Actual:
					             foo
					             """);
				await That(store.Mismatches.Keys).IsEqualTo(new[]
				{
					Id with { Name = "title=Abbey Road_year=1969", },
				}).InAnyOrder();
			}

			[Fact]
			public async Task WhenAccepting_ShouldAcceptTheNamedSnapshot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string title = "Let It Be";

				async Task Act()
					=> await That("id=42").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text("42", "<id>").NamedAfter(title).AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with { Name = "title=Let It Be", }] = "id=<id>",
				});
			}

			[Fact]
			public async Task WhenArgumentIsAMemberAccess_ShouldUseTheNameOfTheMember()
			{
				Album album = new("Abbey Road", 1969);

				await ShouldBeNamed("Title=Abbey Road_Year=1969",
					result => result.NamedAfter(album.Title, album.Year),
					"the name of a member access is the one that nameof returns, so that it holds no dots");
			}

			[Fact]
			public async Task WhenArgumentIsAnotherExpression_ShouldUseTheExpression()
			{
				int year = 1969;

				await ShouldBeNamed("year + 1=1970_\"Let It Be\"=Let It Be",
					result => result.NamedAfter(year + 1, "Let It Be"),
					"only a name or member access has a shorter name");
			}

			[Fact]
			public async Task WhenArgumentIsFormattable_ShouldUseTheInvariantCulture()
			{
				CultureInfo previousCulture = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("de-DE");
				try
				{
					decimal price = 12.99m;
					DateTime released = new(1969, 9, 26);

					await ShouldBeNamed("price=12.99_released=09/26/1969 00:00:00",
						result => result.NamedAfter(price, released),
						"the snapshot must have the same name on every machine");
				}
				finally
				{
					CultureInfo.CurrentCulture = previousCulture;
				}
			}

			[Fact]
			public async Task WhenArgumentIsNull_ShouldWriteNull()
			{
				string? title = null;

				await ShouldBeNamed("title=null",
					result => result.NamedAfter(title),
					"a missing value has no text of its own");
			}

			[Fact]
			public async Task WhenExpressionIsEmpty_ShouldNameTheArgumentByItsPosition()
			{
				string title = "Abbey Road";
				int year = 1969;

				await ShouldBeNamed("argument1=Abbey Road_argument2=1969",
					result => result.NamedAfter(title, year, doNotPopulateThisValue1: "", doNotPopulateThisValue2: ""),
					"a compiler before C# 10 does not set the expressions");
			}

			[Fact]
			public async Task WhenExpressionSpansSeveralLines_ShouldCollapseTheWhitespace()
			{
				int year = 1969;

				await ShouldBeNamed("year + 1=1970",
					result => result.NamedAfter(year
					                            + 1),
					"the line ending of the test file can differ between checkouts");
			}

			[Fact]
			public async Task WhenNamedAfterFourArguments_ShouldUseAllOfThem()
			{
				string artist = "The Beatles";
				string title = "Abbey Road";
				int year = 1969;
				int tracks = 17;

				await ShouldBeNamed("artist=The Beatles_title=Abbey Road_year=1969_tracks=17",
					result => result.NamedAfter(artist, title, year, tracks),
					"each argument adds its name and value");
			}

			[Fact]
			public async Task WhenNamedAfterOneArgument_ShouldUseIt()
			{
				string title = "Abbey Road";

				await ShouldBeNamed("title=Abbey Road",
					result => result.NamedAfter(title),
					"the name of the argument is taken from the call");
			}

			[Fact]
			public async Task WhenNamedAfterSeveralStrings_ShouldNotTakeAStringAsTheExpressionOfAnotherArgument()
			{
				string artist = "AC/DC";
				string title = "Highway to Hell";
				string label = "Atlantic";

				await ShouldBeNamed("artist=AC/DC_title=Highway to Hell",
					result => result.NamedAfter(artist, title),
					"the second string is an argument and not the expression of the first one");
				await ShouldBeNamed("artist=AC/DC_title=Highway to Hell_label=Atlantic",
					result => result.NamedAfter(artist, title, label),
					"the third string is an argument and not the expression of the first one");
			}

			[Fact]
			public async Task WhenNamedAfterThreeArguments_ShouldUseAllOfThem()
			{
				string title = "Let It Be";
				int year = 1970;
				bool isLive = false;

				await ShouldBeNamed("title=Let It Be_year=1970_isLive=False",
					result => result.NamedAfter(title, year, isLive),
					"each argument adds its name and value");
			}

			[Fact]
			public async Task WhenNamedBefore_ShouldReplaceTheName()
			{
				string title = "Abbey Road";

				await ShouldBeNamed("title=Abbey Road",
					result => result.Named("login").NamedAfter(title),
					"the last name wins, like for .Named(…)");
			}

			[Fact]
			public async Task WhenSnapshotIsUsedForAnotherValueWithOtherArguments_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = "title=Abbey Road", }] = "foo";
				store.Snapshots[Id with { Name = "title=Let It Be", }] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act(string title, string subject)
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).NamedAfter(title);

				await That(() => Act("Abbey Road", "foo")).DoesNotThrow();
				await That(() => Act("Let It Be", "bar")).DoesNotThrow()
					.Because("the cases of a parameterized test are kept apart by their arguments");
			}

			private async Task ShouldBeNamed(string name,
				Func<SnapshotResult<string?, IThat<string?>>, SnapshotResult<string?, IThat<string?>>> nameSnapshot,
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
		}
	}
}
