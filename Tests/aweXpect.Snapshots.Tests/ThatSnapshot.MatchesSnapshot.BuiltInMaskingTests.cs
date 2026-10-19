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

			[Test]
			public async Task WhenAccepting_ShouldAcceptTheNumberedPlaceholders()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} 2026-10-17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Guids().Mask().For.Dates().Mask().Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<FailException>();
				await That(store.Accepted[Id]).IsEqualTo("{Guid_1} {Date_1}");
			}

			[Test]
			[Arguments("2026-10-17")]
			[Arguments("2026-10-17T09:30")]
			[Arguments("2026-10-17T09:30:15")]
			[Arguments("2026-10-17T09:30:15.1")]
			[Arguments("2026-10-17T09:30:15.1234567")]
			[Arguments("2026-10-17T09:30:15.1234567Z")]
			[Arguments("2026-10-17T09:30:15.1234567+02:00")]
			[Arguments("2026-10-17T09:30-05:30")]
			public async Task WhenMaskingDates_ShouldMaskIso8601(string date)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"created: {date}.";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Dates().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("created: {Date_1}.");
			}

			[Test]
			public async Task WhenMaskingDates_ShouldMaskTheRoundTripFormatOfDateTimeAndDateTimeOffset()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = string.Join(" ",
					new DateTime(2026, 10, 17, 9, 30, 15, DateTimeKind.Unspecified).ToString("o"),
					new DateTime(2026, 10, 17, 9, 30, 15, DateTimeKind.Utc).ToString("o"),
					new DateTimeOffset(2026, 10, 17, 9, 30, 15, TimeSpan.FromHours(-7)).ToString("o"));

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Dates().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Date_1} {Date_2} {Date_3}")
					.Because("the aweXpect formatter writes dates with the round-trip format \"o\"");
			}

			[Test]
			public async Task WhenMaskingDates_ShouldNumberTheSameTextTheSame()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "from 2026-10-17 to 2026-10-18, created 2026-10-17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Dates().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("from {Date_1} to {Date_2}, created {Date_1}");
			}

			[Test]
			[Arguments("2026-13-01")]
			[Arguments("2026-00-01")]
			[Arguments("2026-10-32")]
			[Arguments("12026-10-17")]
			[Arguments("2026-10-170")]
			[Arguments("17.10.2026")]
			[Arguments("10/17/2026")]
			[Arguments("09:30:15")]
			[Arguments("v2026.10.17")]
			public async Task WhenMaskingDates_ShouldNotMaskSimilarText(string text)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"value: {text}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Dates().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Test]
			public async Task WhenMaskingDates_WithSpaceInsteadOfT_ShouldOnlyMaskTheDate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "2026-10-17 09:30:15";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Dates().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Date_1} 09:30:15")
					.Because("only ISO 8601 with a 'T' between the date and the time is matched as one");
			}

			[Test]
			public async Task WhenMaskingGuids_InBracesOrParentheses_ShouldKeepThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{{{FirstGuid}}} ({SecondGuid})";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{{Guid_1}} ({Guid_2})");
			}

			[Test]
			public async Task WhenMaskingGuids_InUpperAndLowerCase_ShouldNumberThemAsTheSameGuid()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} {FirstGuid.ToUpperInvariant()}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1} {Guid_1}");
			}

			[Test]
			public async Task WhenMaskingGuids_ShouldMaskThemNextToOtherWordCharacters()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order_{FirstGuid}_x";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("order_{Guid_1}_x");
			}

			[Test]
			public async Task WhenMaskingGuids_ShouldNumberThemInTheOrderOfAppearance()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{SecondGuid}, {FirstGuid}, {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1}, {Guid_2}, {Guid_1}")
					.Because("the same Guid keeps the same number, so relations stay visible");
			}

			[Test]
			[Arguments("3f2504e04f8911d39a0c0305e82c3301")]
			[Arguments("a3f2504e0-4f89-11d3-9a0c-0305e82c3301")]
			[Arguments("3f2504e0-4f89-11d3-9a0c-0305e82c33011")]
			[Arguments("3f2504e0-4f89-11d3-9a0c-0305e82c330")]
			[Arguments("3f2504g0-4f89-11d3-9a0c-0305e82c3301")]
			public async Task WhenMaskingGuids_ShouldNotMaskSimilarText(string text)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"value: {text}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject)
					.Because("only the format \"D\" that is not part of a longer run of hexadecimal digits is a Guid");
			}

			[Test]
			public async Task WhenMaskingGuidsAndDates_ShouldCountThemSeparately()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} 2026-10-17 {SecondGuid} 2026-10-18";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask().For.Dates().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1} {Date_1} {Guid_2} {Date_2}");
			}

			[Test]
			public async Task WhenMaskingGuidsAfterAMaskingOfAGuid_ShouldNotCountTheMaskedGuid()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order {FirstGuid}, customer {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(FirstGuid).Mask("<order-id>").For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("order <order-id>, customer {Guid_1}");
			}

			[Test]
			public async Task WhenMaskingGuidsBeforeAMaskingOfAGuid_ShouldMaskAllGuidsFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order {FirstGuid}, customer {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Guids().Mask().For.Text(FirstGuid).Mask("<order-id>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("order {Guid_1}, customer {Guid_2}")
					.Because("the maskings are applied in the order in which they are added");
			}

			[Test]
			public async Task WhenNegated_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "{Guid_1}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = FirstGuid;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Test]
			public async Task WhenSnapshotContainsTheNumberedPlaceholders_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "{Guid_1} created {Date_1}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{Guid.NewGuid()} created {DateTime.UtcNow:o}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask().For.Dates().Mask();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
				await That(store.Mismatches).IsEmpty();
			}

			[Test]
			public async Task WhenTwoValuesOnlyDifferInTheirGuids_ShouldNotConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id={Guid_1}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That($"id={FirstGuid}").MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();
					await That($"id={SecondGuid}").MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the masked values");
			}
		}
	}
}
