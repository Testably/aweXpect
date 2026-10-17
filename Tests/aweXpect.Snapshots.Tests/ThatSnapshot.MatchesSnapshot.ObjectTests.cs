using System.Collections.Generic;
using System.Globalization;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class ObjectTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task WhenSubjectContainsAHashSet_ShouldWriteItsItemsSorted()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				HashSet<string> subject = ["gift", "express", "fragile",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             "express",
				                                             "fragile",
				                                             "gift"
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("the order of a hash set depends on the randomized hash codes of strings, so it is sorted");
			}

			[Fact]
			public async Task ShouldWriteTheObjectCompletelyAndIndented()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotOrder subject = SnapshotOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           SnapshotOrder {
				                                             Created = 2026-10-17T09:30:00.0000000Z,
				                                             Id = 42,
				                                             Lines = [
				                                               SnapshotLine {
				                                                 Price = 1.5,
				                                                 Product = "Pen",
				                                                 Quantity = 2
				                                               },
				                                               SnapshotLine {
				                                                 Price = 12.99,
				                                                 Product = "Book",
				                                                 Quantity = 1
				                                               }
				                                             ],
				                                             Note = "multi\nline \"quoted\"",
				                                             Rate = 0.30000000000000004,
				                                             Shipping = <null>,
				                                             Status = Closed,
				                                             Tags = {
				                                               ["b"] = 2,
				                                               ["a"] = 1
				                                             }
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("members are written in alphabetical order, nothing is shortened, and the items of a collection in the order it enumerates them");
			}

			[Fact]
			public async Task WhenCurrentCultureIsNotInvariant_ShouldWriteNumbersAndDatesTheSame()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				CultureInfo previousCulture = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("de-DE");
				try
				{
					SnapshotLine subject = new("Pen", 1234, 1234.5m);

					async Task Act()
						=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

					await That(Act).Throws<XunitException>();
				}
				finally
				{
					CultureInfo.CurrentCulture = previousCulture;
				}

				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           SnapshotLine {
				                                             Price = 1234.5,
				                                             Product = "Pen",
				                                             Quantity = 1234
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the snapshot must not depend on the culture of the machine that runs the test");
			}

			[Fact]
			public async Task WhenObjectContainsItself_ShouldWriteItAsRecursive()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotNode subject = new()
				{
					Name = "root",
				};
				subject.Children.Add(new SnapshotNode
				{
					Name = "child",
					Parent = subject,
				});

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           SnapshotNode {
				                                             Children = [
				                                               SnapshotNode {
				                                                 Children = [],
				                                                 Name = "child",
				                                                 Parent = SnapshotNode { *recursive* }
				                                               }
				                                             ],
				                                             Name = "root",
				                                             Parent = <null>
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenObjectDiffers_ShouldShowTheChangedLineAndTheWrittenObject()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = """
				                      SnapshotLine {
				                        Price = 1.5,
				                        Product = "Pen",
				                        Quantity = 2
				                      }
				                      """;
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotLine subject = new("Pen", 3, 1.5m);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "SnapshotLine {\n  Price =…", which differs on line 4 and column 14:
					                             ↓ (actual)
					               "… Quantity = 3\n}"
					               "… Quantity = 2\n}"
					                             ↑ (expected)

					             Actual:
					             SnapshotLine {
					               Price = 1.5,
					               Product = "Pen",
					               Quantity = 3
					             }

					             Snapshot:
					             SnapshotLine {
					               Price = 1.5,
					               Product = "Pen",
					               Quantity = 2
					             }
					             """);
			}

			[Fact]
			public async Task WhenObjectIsNestedTooDeeply_ShouldFailWithoutRecordingMismatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotNode subject = new();
				for (int i = 0; i < 100_000; i++)
				{
					subject = new SnapshotNode
					{
						Parent = subject,
					};
				}

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it could not be written: The value is nested too deeply to be formatted completely on the remaining stack at a depth of *.
					             """).AsWildcard();
				await That(exception.InnerException).IsExactly<InsufficientExecutionStackException>();
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenObjectIsNull_ShouldFailWithoutRecordingMismatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotLine? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenObjectMatches_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "SnapshotLine {\n  Price = 1.5,\n  Product = \"Pen\",\n  Quantity = 2\n}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotLine subject = new("Pen", 2, 1.5m);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("the newline style of the snapshot does not matter, so a snapshot written on another OS matches");
				await That(store.Matches).IsEqualTo(new[] { Id, });
			}

			[Fact]
			public async Task WhenObjectIsUsedAgainWithEqualContent_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "SnapshotLine {\n  Price = 1.5,\n  Product = \"Pen\",\n  Quantity = 2\n}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That(new SnapshotLine("Pen", 2, 1.5m)).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow();
				await That(Act).DoesNotThrow()
					.Because("another instance with the same content is written the same, so it is the same value");
			}

			[Fact]
			public async Task WhenObjectIsUsedForAnotherValue_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "SnapshotLine {\n  Price = 1.5,\n  Product = \"Pen\",\n  Quantity = 2\n}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(new SnapshotLine("Pen", 2, 1.5m)).MatchesSnapshot(Id.SourceFile, Id.MemberName);
				SnapshotLine subject = new("Pen", 3, 1.5m);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot was already used for another value in this test run*").AsWildcard();
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenObjectIsNegated_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "42";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				int subject = 42;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Fact]
			public async Task WhenAcceptingChanges_ShouldAcceptTheWrittenObject()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotLine subject = new("Pen", 2, 1.5m);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("pen")
						.AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(store.Accepted[Id with { Name = "pen", }]).IsEqualTo("""
				                                                              SnapshotLine {
				                                                                Price = 1.5,
				                                                                Product = "Pen",
				                                                                Quantity = 2
				                                                              }
				                                                              """).IgnoringNewlineStyle();
				await That(Act).DoesNotThrow()
					.Because("the accepted snapshot is the written object");
			}

			[Fact]
			public async Task WhenMasking_ShouldMaskTheWrittenObject()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotShipping subject = new(Guid.NewGuid().ToString());

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(@"Address = ""[0-9a-f-]{36}""", @"Address = ""<guid>""");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           SnapshotShipping {
				                                             Address = "<guid>"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the masking applies to the written object, including the quotes of the formatter");
			}

			[Fact]
			public async Task WhenMaskingGuidsAndDatesGlobally_ShouldMaskTheWrittenObject()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Guids(),
					SnapshotMasking.Dates(),
				]);
				Guid parent = Guid.NewGuid();
				SnapshotAudit subject = new(Guid.NewGuid(), parent, parent, DateTime.UtcNow);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           SnapshotAudit {
				                                             Created = {Date_1},
				                                             Id = {Guid_1},
				                                             Owner = {Guid_2},
				                                             Parent = {Guid_2}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the global maskings apply to the written object, and the same Guid keeps its number");
			}

			[Fact]
			public async Task WhenSubjectIsAValueType_ShouldWriteIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             1,
				                                             2,
				                                             3
				                                           ]
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenSubjectIsAnObjectHoldingAString_ShouldWriteItInQuotes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				object subject = "line 1\nline 2";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("\"line 1\\nline 2\"")
					.Because("the static type decides, and only a string subject is compared as raw text");
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public async Task WhenSubjectIsAString_ShouldCompareTheRawText(bool isNullableString)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string nonNullableSubject = "line 1\nline 2";
				string? nullableSubject = "line 1\nline 2";

				async Task Act()
				{
					if (isNullableString)
					{
						await That(nullableSubject).MatchesSnapshot(Id.SourceFile, Id.MemberName);
					}
					else
					{
						await That(nonNullableSubject).MatchesSnapshot(Id.SourceFile, Id.MemberName);
					}
				}

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("line 1\nline 2")
					.Because("a string subject is compared as raw text, without quotes and escaping");
			}
		}
	}
}

public enum SnapshotStatus
{
	Open,
	Closed,
}

public sealed record SnapshotLine(string Product, int Quantity, decimal Price);

public sealed record SnapshotShipping(string Address);

public sealed record SnapshotAudit(Guid Id, Guid Parent, Guid Owner, DateTime Created);

public sealed record SnapshotOrder(
	int Id,
	SnapshotStatus Status,
	DateTime Created,
	double Rate,
	string? Note,
	List<SnapshotLine> Lines,
	Dictionary<string, int> Tags,
	SnapshotShipping? Shipping)
{
	public static SnapshotOrder Create()
		=> new(42, SnapshotStatus.Closed, new DateTime(2026, 10, 17, 9, 30, 0, DateTimeKind.Utc), 0.1 + 0.2,
			"multi\nline \"quoted\"",
			[new SnapshotLine("Pen", 2, 1.5m), new SnapshotLine("Book", 1, 12.99m),],
			new Dictionary<string, int>
			{
				["b"] = 2,
				["a"] = 1,
			},
			null);
}

public sealed class SnapshotNode
{
	public List<SnapshotNode> Children { get; } = new();
	public string Name { get; set; } = "";
	public SnapshotNode? Parent { get; set; }
}
