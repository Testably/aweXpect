using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class MemberRuleTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private const string SecondGuid = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task IgnoringMember_ShouldLeaveOutTheMemberWithItsName()
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
						.For.Members().WithName("Id").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Name = "Ann"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task IgnoringMember_ShouldMatchAtAnyDepthAndInCollectionsAndDictionaries()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             ByCode = {
				                                               ["p"] = RuleLine {
				                                                 Product = "Pen"
				                                               }
				                                             },
				                                             Customer = RuleCustomer {
				                                               Name = "Ann"
				                                             },
				                                             Lines = [
				                                               RuleLine {
				                                                 Product = "Pen"
				                                               }
				                                             ],
				                                             OrderId = 7
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the name is compared with the name of the member itself at any depth, so 'OrderId' is kept");
			}

			[Fact]
			public async Task IgnoringMember_WithNameInAnotherCase_ShouldKeepTheMember()
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
						.For.Members().WithName("id").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Id = 1,
				                                             Name = "Ann"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the name is compared case-sensitive by default");
			}

			[Fact]
			public async Task IgnoringMember_WithPath_ShouldOnlyMatchThatPath()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithPath("Customer.Id").Ignore()
						.For.Members().WithName("ByCode").Ignore()
						.For.Members().WithName("Lines").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             Customer = RuleCustomer {
				                                               Name = "Ann"
				                                             },
				                                             Id = 42,
				                                             OrderId = 7
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task IgnoringMember_WithPath_ShouldMatchWholeSegmentsAtTheEndOfThePath()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithPath("[0].Id").Ignore()
						.For.Members().WithPath("rId").Ignore()
						.For.Members().WithPath("customer.Id").Ignore()
						.For.Members().WithName("ByCode").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             Customer = RuleCustomer {
				                                               Id = 9,
				                                               Name = "Ann"
				                                             },
				                                             Id = 42,
				                                             Lines = [
				                                               RuleLine {
				                                                 Product = "Pen"
				                                               }
				                                             ],
				                                             OrderId = 7
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("an index starts a segment, 'rId' is no whole segment of 'OrderId', and the segments are compared case-sensitive");
			}

			[Fact]
			public async Task IgnoringMemberOfType_ShouldMatchDeclaredNullableRuntimeAndDerivedTypes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTypes subject = new()
				{
					Created = new DateTime(2026, 10, 17, 9, 30, 0, DateTimeKind.Utc),
					Shipped = new DateTime(2026, 10, 18, 9, 30, 0, DateTimeKind.Utc),
					Extra = new DateTime(2026, 10, 19, 9, 30, 0, DateTimeKind.Utc),
					Error = new InvalidOperationException("boom"),
					Name = "kept",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<DateTime>().Ignore()
						.For.Members().OfType<Exception>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTypes {
				                                             Name = "kept"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a DateTime? member, a DateTime in an object member and a derived exception match as well");
			}

			[Fact]
			public async Task IgnoringMemberOfType_WhenAnObjectMemberHoldsAnotherType_ShouldKeepIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTypes subject = new()
				{
					Extra = 5,
					Name = "kept",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<DateTime>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTypes {
				                                             Error = <null>,
				                                             Extra = 5,
				                                             Name = "kept"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("an object member is only left out when its value is of the type, while a DateTime? member is left out by its declared type, also when it is null");
			}

			[Fact]
			public async Task IgnoringMemberOfExactType_ShouldNotMatchDerivedTypes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTypes subject = new()
				{
					Created = new DateTime(2026, 10, 17, 9, 30, 0, DateTimeKind.Utc),
					Extra = new DateTime(2026, 10, 19, 9, 30, 0, DateTimeKind.Utc),
					Error = new InvalidOperationException("boom"),
					Name = "kept",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfExactType<DateTime>().Ignore()
						.For.Members().OfExactType<Exception>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTypes {
				                                             Error = *boom*,
				                                             Name = "kept"
				                                           }
				                                           """).AsWildcard().IgnoringNewlineStyle()
					.Because("a DateTime, a null DateTime? and a DateTime in an object member are exactly of the type, but an InvalidOperationException is no exact Exception");
			}

			[Fact]
			public async Task IgnoringMemberOfType_ShouldKeepTheItemsOfCollectionsAndDictionaries()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleCollections subject = RuleCollections.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<DateTime>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCollections {
				                                             Dates = [
				                                               2026-10-17T00:00:00.0000000,
				                                               2026-10-18T00:00:00.0000000
				                                             ],
				                                             Releases = {
				                                               ["Abbey Road"] = 1969-09-26T00:00:00.0000000
				                                             }
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("only members are left out by their type, so a list of DateTime is not written as an empty list");
			}

			[Fact]
			public async Task IgnoringValuesOfType_ShouldAlsoLeaveOutTheItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<object> subject =
				[
					new DateTime(2026, 10, 17),
					"Let It Be",
					new DateTime(2026, 10, 18),
				];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<DateTime>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             "Let It Be"
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("the values are the members and the items, so the items of the type are left out");
			}

			[Fact]
			public async Task IgnoringMemberOfType_WhenTheItemsAreMaskedByType_ShouldMaskThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleCollections subject = RuleCollections.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<DateTime>().Ignore()
						.For.Values().OfType<DateTime>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCollections {
				                                             Dates = [
				                                               {DateTime_1},
				                                               {DateTime_2}
				                                             ],
				                                             Releases = {
				                                               ["Abbey Road"] = {DateTime_3}
				                                             }
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the items keep their place and are masked instead");
			}

			[Fact]
			public async Task IgnoringValue_WithThePathOfAnItem_ShouldLeaveOutTheItem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleCollections subject = RuleCollections.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().WithPath("Dates[0]").Ignore()
						.For.Members().WithName("Created").Ignore()
						.For.Members().WithName("Releases").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCollections {
				                                             Dates = [
				                                               2026-10-18T00:00:00.0000000
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("an item can still be left out on purpose by its path");
			}

			[Fact]
			public async Task IgnoringMember_WithThePathOfAnItem_ShouldKeepTheItem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleCollections subject = RuleCollections.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithPath("Dates[0]").Ignore()
						.For.Members().WithName("Created").Ignore()
						.For.Members().WithName("Releases").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCollections {
				                                             Dates = [
				                                               2026-10-17T00:00:00.0000000,
				                                               2026-10-18T00:00:00.0000000
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("an item is no member");
			}

			[Fact]
			public async Task WhenAGetterThrows_ShouldNeitherIgnoreNorMaskTheValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleThrowing subject = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<DateTime>().Ignore()
						.For.Members().WithName("Value").Mask("{value}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("*Value = *boom*").AsWildcard()
					.Because("a value that cannot be read is written with its exception by the formatter");
			}

			[Fact]
			public async Task IgnoringWinsOverMasking()
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
						.For.Members().WithName("Id").Mask()
						.For.Members().WithName("Id").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Name = "Ann"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task MaskingMember_ShouldNumberTheValuesAcrossTheSubject()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             ByCode = {
				                                               ["p"] = RuleLine {
				                                                 Id = {Id_1},
				                                                 Product = "Pen"
				                                               }
				                                             },
				                                             Customer = RuleCustomer {
				                                               Id = {Id_2},
				                                               Name = "Ann"
				                                             },
				                                             Id = {Id_3},
				                                             Lines = [
				                                               RuleLine {
				                                                 Id = {Id_1},
				                                                 Product = "Pen"
				                                               }
				                                             ],
				                                             OrderId = 7
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the same value keeps the same number in the order the values are written, without quotes");
			}

			[Fact]
			public async Task MaskingMember_WhenValueIsNull_ShouldKeepIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleNote subject = new()
				{
					Text = null,
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Text").Mask("{text}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleNote {
				                                             Text = <null>
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a null value does not change from run to run, so it stays visible");
			}

			[Fact]
			public async Task MaskingMember_WithReplacement_ShouldWriteItWithoutQuotes()
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
						.For.Members().WithName("Name").Mask("{name}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Id = 1,
				                                             Name = {name}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task MaskingType_ShouldShareTheNumbersWithGuids()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleReference subject = new()
				{
					Key = Guid.Parse(FirstGuid),
					Note = $"see {FirstGuid.ToUpperInvariant()} and {SecondGuid}",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<Guid>().Mask()
						.For.Guids().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleReference {
				                                             Key = {Guid_1},
				                                             Note = "see {Guid_1} and {Guid_2}"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a Guid member and the same Guid in a string get the same number");
			}

			[Fact]
			public async Task MaskingType_InACollection_ShouldMaskTheItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<Guid> subject = [Guid.Parse(FirstGuid), Guid.Parse(SecondGuid), Guid.Parse(FirstGuid),];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<Guid>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             {Guid_1},
				                                             {Guid_2},
				                                             {Guid_1}
				                                           ]
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task MaskingMembersOfType_InACollection_ShouldKeepTheItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<Guid> subject = [Guid.Parse(FirstGuid),];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<Guid>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo($$"""
				                                           [
				                                             {{FirstGuid}}
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("an item is no member");
			}

			[Fact]
			public async Task MaskingType_WithReplacement_ShouldWriteItWithoutQuotes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleLink subject = new()
				{
					Target = new Uri("https://example.com/orders/42"),
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<Uri>().Mask("{uri}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleLink {
				                                             Target = {uri}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task TheFirstMatchingMaskingWins()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleReference subject = new()
				{
					Key = Guid.Parse(FirstGuid),
					Note = "n",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Key").Mask("{key}")
						.For.Values().OfType<Guid>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleReference {
				                                             Key = {key},
				                                             Note = "n"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenGlobalAndLocal_ShouldApplyTheLocalRuleFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().WithName("Name").Mask("{global}"),
					SnapshotRule.For.Members().WithName("Id").Mask("{id}"),
				]);
				RuleCustomer subject = new()
				{
					Id = 1,
					Name = "Ann",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Name").Mask("{local}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Id = {id},
				                                             Name = {local}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenGlobalIgnoringRules_ShouldLeaveOutTheValues()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().WithName("Id").Ignore(),
				]);
				RuleCustomer subject = new()
				{
					Id = 1,
					Name = "Ann",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Name = "Ann"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenGlobalRulesAndTheSubjectIsAString_ShouldIgnoreThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().OfType<string>().Ignore(),
					SnapshotRule.For.Values().OfType<string>().Mask("{text}"),
				]);
				string subject = "Id = 1";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Id = 1")
					.Because("a string is compared as raw text and has no members");
			}

			[Fact]
			public async Task WhenOnlyMaskedValuesDiffer_ShouldNotConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = """
				                      RuleCustomer {
				                        Id = {Id_1},
				                        Name = "Ann"
				                      }
				                      """;
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That(new RuleCustomer { Id = 1, Name = "Ann", }).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Mask();
					await That(new RuleCustomer { Id = 2, Name = "Ann", }).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Mask();
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the masked value");
			}

			[Fact]
			public async Task WhenNameIsEmpty_ShouldThrowArgumentException()
			{
				void MembersAct()
					=> SnapshotRule.For.Members().WithName("");

				void ValuesAct()
					=> SnapshotRule.For.Values().WithName("");

				await That(MembersAct).Throws<ArgumentException>()
					.WithParamName("name").And
					.WithMessage("The 'name' cannot be empty.*").AsWildcard();
				await That(ValuesAct).Throws<ArgumentException>().WithParamName("name");
			}

			[Fact]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Members().WithName(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name");
			}

			[Fact]
			public async Task WhenPathIsNullOrEmpty_ShouldThrow()
			{
				void NullAct()
					=> SnapshotRule.For.Values().WithPath(null!);

				void EmptyAct()
					=> SnapshotRule.For.Members().WithPath("");

				await That(NullAct).Throws<ArgumentNullException>().WithParamName("path");
				await That(EmptyAct).Throws<ArgumentException>().WithParamName("path");
			}

			[Fact]
			public async Task WhenReplacementIsNull_ShouldThrowArgumentNullException()
			{
				void MemberAct()
					=> SnapshotRule.For.Members().WithName("Id").Mask((string)null!);

				void TypeAct()
					=> SnapshotRule.For.Values().OfType<Guid>().Mask((string)null!);

				void TemplateAct()
					=> SnapshotRule.For.Values().Mask((Func<FormattedValue, string?>)null!);

				await That(MemberAct).Throws<ArgumentNullException>().WithParamName("template");
				await That(TypeAct).Throws<ArgumentNullException>().WithParamName("template");
				await That(TemplateAct).Throws<ArgumentNullException>().WithParamName("template");
			}

			[Fact]
			public async Task WhenSubjectIsAString_MemberRule_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Ignore();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object, but a string is compared as raw text.*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsAString_ValueRule_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<Guid>().Mask();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object*").AsWildcard();
			}

			[Fact]
			public async Task Rules_ShouldBeEmptyByDefault()
			{
				IReadOnlyList<SnapshotRule> rules = Customize.aweXpect.Snapshots().Rules.Get();

				await That(rules).IsEmpty();
			}

			[Fact]
			public async Task Rules_WhenSetToNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Rules.Set(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("value");
			}

			[Fact]
			public async Task Rules_WhenContainingNull_ShouldThrowArgumentException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Rules.Set([null!,]);

				await That(Act).Throws<ArgumentException>().WithParamName("value");
			}

			[Fact]
			public async Task Rules_WhenTheListIsChangedAfterSetting_ShouldKeepTheSetting()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<SnapshotRule> rules = [SnapshotRule.For.Members().WithName("Id").Ignore(),];
				using CustomizationLifetime setting = Customize.aweXpect.Snapshots().Rules.Set(rules);
				rules.Add(SnapshotRule.For.Members().WithName("Name").Ignore());
				RuleCustomer subject = new()
				{
					Id = 1,
					Name = "Ann",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Name = "Ann"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task For_ShouldOfferTheSameRulesAsForAllSnapshots()
			{
				string[] global = ApiParity.GetSurface(
					typeof(SnapshotRule).GetProperty(nameof(SnapshotRule.For))!.PropertyType, typeof(SnapshotRule));
				string[] expectation = ApiParity.GetSurface(
					typeof(SnapshotResult<object, IThat<object>>)
						.GetProperty(nameof(SnapshotResult<object, IThat<object>>.For))!.PropertyType,
					typeof(SnapshotResult<object, IThat<object>>));

				await That(expectation).IsEqualTo(global)
					.Because("every selection, filter and action of the expectation exists for all snapshots, and vice versa");
				await That(global.Any(signature => signature.Contains(".Convert`1(")) &&
				           global.Any(signature => signature.Contains(".Keep`0()"))).IsTrue()
					.Because("the comparison follows the selections down to their actions");
			}
		}
	}
}

public sealed class RuleCustomer
{
	public int Id { get; set; }
	public string Name { get; set; } = "";
}

public sealed class RuleLine
{
	public int Id { get; set; }
	public string Product { get; set; } = "";
}

public sealed class RuleOrder
{
	public Dictionary<string, RuleLine> ByCode { get; set; } = new();
	public RuleCustomer Customer { get; set; } = new();
	public int Id { get; set; }
	public List<RuleLine> Lines { get; set; } = new();
	public int OrderId { get; set; }

	public static RuleOrder Create()
	{
		RuleLine line = new()
		{
			Id = 5,
			Product = "Pen",
		};
		return new RuleOrder
		{
			ByCode = new Dictionary<string, RuleLine>
			{
				["p"] = line,
			},
			Customer = new RuleCustomer
			{
				Id = 9,
				Name = "Ann",
			},
			Id = 42,
			Lines = [line,],
			OrderId = 7,
		};
	}
}

public sealed class RuleTypes
{
	public DateTime Created { get; set; }
	public Exception? Error { get; set; }
	public object? Extra { get; set; }
	public string Name { get; set; } = "";
	public DateTime? Shipped { get; set; }
}

public sealed class RuleNote
{
	public string? Text { get; set; }
}

public sealed class RuleReference
{
	public Guid Key { get; set; }
	public string Note { get; set; } = "";
}

public sealed class RuleLink
{
	public Uri? Target { get; set; }
}

public sealed class RuleThrowing
{
	private readonly string _message = "boom";

	public object? Value => throw new InvalidOperationException(_message);
}

public sealed class RuleCollections
{
	public DateTime Created { get; set; }
	public List<DateTime> Dates { get; set; } = [];
	public Dictionary<string, DateTime> Releases { get; set; } = [];

	public static RuleCollections Create()
		=> new()
		{
			Created = new DateTime(2026, 10, 16),
			Dates = [new DateTime(2026, 10, 17), new DateTime(2026, 10, 18),],
			Releases = new Dictionary<string, DateTime>
			{
				["Abbey Road"] = new(1969, 9, 26),
			},
		};
}
