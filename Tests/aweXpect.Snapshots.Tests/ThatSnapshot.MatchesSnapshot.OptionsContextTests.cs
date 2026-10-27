using System.Text.RegularExpressions;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed partial class OptionsContextTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task ComparerForAllSnapshots_ShouldBeListedAsSuch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.When(_ => true, FakeSnapshotComparer.Different("the tempo changed")),
				]);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed: the tempo changed

					             Actual:
					             Abbey Road

					             Snapshot:
					             Abbey Road

					             Snapshot options:
					              - compare with FakeSnapshotComparer (for all snapshots)
					             """)
					.Because("a comparer for all snapshots is usually registered far away from the test");
			}

			[Test]
			public async Task ComparerOfTheExpectation_WhenSnapshotIsMissing_ShouldBeListed()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Using(FakeSnapshotComparer.Equal());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             Abbey Road

					             Snapshot options:
					              - compare with FakeSnapshotComparer
					             """);
			}

			[Test]
			public async Task LineRules_ShouldBeDescribedByTheirFiltersAndActions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").IgnoringCase().Ignore()
						.For.Lines().Containing("v*.*").AsWildcard().Mask("<version>")
						.For.Lines().Which(line => line.StartsWith('#')).Except(line => line.Length > 80)
						.Convert(line => line.Trim())
						.For.Lines().Mask("<line>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             <line>

					             Snapshot options:
					              - ignore the lines containing "elapsed" ignoring case
					              - mask the lines containing "v*.*" as wildcard with "<version>"
					              - convert the lines satisfying line => line.StartsWith('#') and not satisfying line => line.Length > 80 using line => line.Trim()
					              - mask the lines with "<line>"
					             """);
			}

			[Test]
			public async Task MemberAndValueRules_ShouldBeDescribedByTheirFiltersAndActions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleCustomer subject = new()
				{
					Id = 1,
					Name = "Ann",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Na").AsPrefix().IgnoringCase().Ignore()
						.For.Members().OfType<int>().OrOfExactType<long>().Mask()
						.For.Members().DeclaredIn<RuleCustomer>().WithPath("Customer.Name").Mask("<name>")
						.For.Members().WhichAreEmptyCollections().Ignore()
						.For.Members().Which(member => member.Name == "Id").Mask(member => "{" + member.Name + "}")
						.For.Values().Except(value => value.Path == "Id").OfExactType<DateTime>()
						.Convert(date => date.Year);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             RuleCustomer {
					               Id = {Int32_1}
					             }

					             Snapshot options:
					              - ignore the members whose name starts with "Na" ignoring case
					              - mask the members of type int or of exact type long
					              - mask the members declared in RuleCustomer and whose path ends with "Customer.Name" with "<name>"
					              - ignore the members whose value is an empty collection
					              - mask the members satisfying member => member.Name == "Id" using member => "{" + member.Name + "}"
					              - convert the values not satisfying value => value.Path == "Id" and of exact type DateTime using date => date.Year
					             """);
			}

			[Test]
			public async Task RulesForAllSnapshots_OnBinaryData_ShouldNotBeListed()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Guids().Mask(),
				]);
				byte[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("bin");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist
					             """)
					.Because("rules do not apply to binary data");
			}

			[Test]
			public async Task RulesForAllSnapshots_OnObject_ShouldNotListLineRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().WithName("PlayCount").Ignore(),
					SnapshotRule.For.Lines().Containing("elapsed").Ignore(),
				]);
				RuleCustomer subject = new()
				{
					Id = 1,
					Name = "Ann",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             RuleCustomer {
					               Id = 1,
					               Name = "Ann"
					             }

					             Snapshot options:
					              - ignore the members whose name is equal to "PlayCount" (for all snapshots)
					             """)
					.Because("line rules do not apply to an object");
			}

			[Test]
			public async Task RulesForAllSnapshots_OnString_ShouldNotListMemberAndValueRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().WithName("PlayCount").Ignore(),
					SnapshotRule.For.Values().OfType<Guid>().Mask(),
					SnapshotRule.For.Lines().Containing("elapsed").Ignore(),
				]);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             Abbey Road

					             Snapshot options:
					              - ignore the lines containing "elapsed" (for all snapshots)
					             """)
					.Because("member and value rules do not apply to a string");
			}

			[Test]
			public async Task RulesForAllSnapshots_ShouldBeListedAfterTheRulesOfTheExpectation()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().WithName("PlayCount").Ignore(),
					SnapshotRule.For.Guids().Mask(),
				]);
				RuleCustomer subject = new()
				{
					Id = 1,
					Name = "Ann",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Ignore();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             RuleCustomer {
					               Name = "Ann"
					             }

					             Snapshot options:
					              - ignore the members whose name is equal to "Id"
					              - ignore the members whose name is equal to "PlayCount" (for all snapshots)
					              - mask the Guids (for all snapshots)
					             """)
					.Because("a rule for all snapshots is usually set far away from the test");
			}

			[Test]
			public async Task TextRules_ShouldBeDescribedByTheirSelectionsAndActions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Abbey").Mask("<title>")
						.For.Text("r*d").AsWildcard().IgnoringCase().Mask("<road>")
						.For.Text(@"\d+").AsRegex().Mask("<number>")
						.For.Regex(TokenRegex()).Mask("token=<token>")
						.For.Guids().Mask()
						.For.Dates().Mask("<date>")
						.For.MachineName().Mask()
						.For.UserName().Mask("<user>")
						.For.Paths().Keep();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             <title> <road>

					             Snapshot options:
					              - mask the text "Abbey" with "<title>"
					              - mask the text "r*d" as wildcard ignoring case with "<road>"
					              - mask the text "\\d+" as regex with "<number>"
					              - mask the matches of the regex "token=\\w+" with "token=<token>"
					              - mask the Guids
					              - mask the dates with "<date>"
					              - mask the machine name
					              - mask the user name with "<user>"
					              - keep the paths of the environment
					             """);
			}

			[Test]
			public async Task WhenAllOptionsAreTheDefaults_ShouldNotAddTheContext()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             Abbey Road
					             """);
			}

#if NET8_0_OR_GREATER
			[GeneratedRegex("token=\\w+")]
			private static partial Regex TokenRegex();
#else
			private static readonly Regex TokenRegexInstance = new("token=\\w+");

			private static Regex TokenRegex() => TokenRegexInstance;
#endif
		}
	}
}
