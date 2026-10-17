using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class BuiltInMaskingTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private const string SecondGuid = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task WhenAccepting_ShouldAcceptTheNumberedPlaceholders()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} 2026-10-17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Guids().Masking.Dates().AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted[Id]).IsEqualTo("{Guid_1} {Date_1}");
			}

			[Theory]
			[InlineData("2026-10-17")]
			[InlineData("2026-10-17T09:30")]
			[InlineData("2026-10-17T09:30:15")]
			[InlineData("2026-10-17T09:30:15.1")]
			[InlineData("2026-10-17T09:30:15.1234567")]
			[InlineData("2026-10-17T09:30:15.1234567Z")]
			[InlineData("2026-10-17T09:30:15.1234567+02:00")]
			[InlineData("2026-10-17T09:30-05:30")]
			public async Task WhenMaskingDates_ShouldMaskIso8601(string date)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"created: {date}.";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Dates();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("created: {Date_1}.");
			}

			[Fact]
			public async Task WhenMaskingDates_ShouldMaskTheRoundTripFormatOfDateTimeAndDateTimeOffset()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = string.Join(" ",
					new DateTime(2026, 10, 17, 9, 30, 15, DateTimeKind.Unspecified).ToString("o"),
					new DateTime(2026, 10, 17, 9, 30, 15, DateTimeKind.Utc).ToString("o"),
					new DateTimeOffset(2026, 10, 17, 9, 30, 15, TimeSpan.FromHours(-7)).ToString("o"));

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Dates();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Date_1} {Date_2} {Date_3}")
					.Because("the aweXpect formatter writes dates with the round-trip format \"o\"");
			}

			[Fact]
			public async Task WhenMaskingDates_ShouldNumberTheSameTextTheSame()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "from 2026-10-17 to 2026-10-18, created 2026-10-17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Dates();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("from {Date_1} to {Date_2}, created {Date_1}");
			}

			[Theory]
			[InlineData("2026-13-01")]
			[InlineData("2026-00-01")]
			[InlineData("2026-10-32")]
			[InlineData("12026-10-17")]
			[InlineData("2026-10-170")]
			[InlineData("17.10.2026")]
			[InlineData("10/17/2026")]
			[InlineData("09:30:15")]
			[InlineData("v2026.10.17")]
			public async Task WhenMaskingDates_ShouldNotMaskSimilarText(string text)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"value: {text}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Dates();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Fact]
			public async Task WhenMaskingDates_WithSpaceInsteadOfT_ShouldOnlyMaskTheDate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "2026-10-17 09:30:15";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Dates();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Date_1} 09:30:15")
					.Because("only ISO 8601 with a 'T' between the date and the time is matched as one");
			}

			[Fact]
			public async Task WhenMaskingGuids_InBracesOrParentheses_ShouldKeepThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{{{FirstGuid}}} ({SecondGuid})";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{{Guid_1}} ({Guid_2})");
			}

			[Fact]
			public async Task WhenMaskingGuids_InUpperAndLowerCase_ShouldNumberThemAsTheSameGuid()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} {FirstGuid.ToUpperInvariant()}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1} {Guid_1}");
			}

			[Fact]
			public async Task WhenMaskingGuids_ShouldMaskThemNextToOtherWordCharacters()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order_{FirstGuid}_x";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("order_{Guid_1}_x");
			}

			[Fact]
			public async Task WhenMaskingGuids_ShouldNumberThemInTheOrderOfAppearance()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{SecondGuid}, {FirstGuid}, {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1}, {Guid_2}, {Guid_1}")
					.Because("the same Guid keeps the same number, so relations stay visible");
			}

			[Theory]
			[InlineData("3f2504e04f8911d39a0c0305e82c3301")]
			[InlineData("a3f2504e0-4f89-11d3-9a0c-0305e82c3301")]
			[InlineData("3f2504e0-4f89-11d3-9a0c-0305e82c33011")]
			[InlineData("3f2504e0-4f89-11d3-9a0c-0305e82c330")]
			[InlineData("3f2504g0-4f89-11d3-9a0c-0305e82c3301")]
			public async Task WhenMaskingGuids_ShouldNotMaskSimilarText(string text)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"value: {text}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject)
					.Because("only the format \"D\" that is not part of a longer run of hexadecimal digits is a Guid");
			}

			[Fact]
			public async Task WhenMaskingGuidsAndDates_ShouldCountThemSeparately()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} 2026-10-17 {SecondGuid} 2026-10-18";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids().Masking.Dates();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1} {Date_1} {Guid_2} {Date_2}");
			}

			[Fact]
			public async Task WhenMaskingGuidsAfterAMaskingOfAGuid_ShouldNotCountTheMaskedGuid()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order {FirstGuid}, customer {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text(FirstGuid, "<order-id>").Masking.Guids();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("order <order-id>, customer {Guid_1}");
			}

			[Fact]
			public async Task WhenMaskingGuidsBeforeAMaskingOfAGuid_ShouldMaskAllGuidsFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order {FirstGuid}, customer {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Guids().Masking.Text(FirstGuid, "<order-id>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("order {Guid_1}, customer {Guid_2}")
					.Because("the maskings are applied in the order in which they are added");
			}

			[Fact]
			public async Task WhenNegated_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "{Guid_1}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = FirstGuid;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Fact]
			public async Task WhenSnapshotContainsTheNumberedPlaceholders_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "{Guid_1} created {Date_1}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{Guid.NewGuid()} created {DateTime.UtcNow:o}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids().Masking.Dates();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenTwoValuesOnlyDifferInTheirGuids_ShouldNotConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id={Guid_1}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That($"id={FirstGuid}").MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();
					await That($"id={SecondGuid}").MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the masked values");
			}
		}
	}
}
