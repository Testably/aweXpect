using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class ValueMaskTemplateTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private const string SecondGuid = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

			private const string LinkedIds = """
			                                 Ticket {
			                                   Fan = Fan {
			                                     Id = {Fan_1},
			                                     Name = "Ann"
			                                   },
			                                   FanId = {Fan_1},
			                                   Id = {Ticket_1}
			                                 }
			                                 """;

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task Mask_WithNumber_ShouldNumberTheValues()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Mask("<id {0}>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             ByCode = {
				                                               ["p"] = RuleLine {
				                                                 Id = <id 1>,
				                                                 Product = "Pen"
				                                               }
				                                             },
				                                             Customer = RuleCustomer {
				                                               Id = <id 2>,
				                                               Name = "Ann"
				                                             },
				                                             Id = <id 3>,
				                                             Lines = [
				                                               RuleLine {
				                                                 Id = <id 1>,
				                                                 Product = "Pen"
				                                               }
				                                             ],
				                                             OrderId = 7
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("{0} is replaced with the number of the value, and equal values get the same number");
			}

			[Test]
			public async Task Mask_WithTheSameTemplate_ShouldShareTheCounterAcrossRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleLine>().WithName("Id").Mask("#{0}")
						.For.Members().DeclaredIn<RuleCustomer>().WithName("Id").Mask("#{0}")
						.For.Members().WithName("OrderId").Mask("#{0}")
						.For.Members().WithName("Id").Mask("order {0}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             ByCode = {
				                                               ["p"] = RuleLine {
				                                                 Id = #1,
				                                                 Product = "Pen"
				                                               }
				                                             },
				                                             Customer = RuleCustomer {
				                                               Id = #2,
				                                               Name = "Ann"
				                                             },
				                                             Id = order 1,
				                                             Lines = [
				                                               RuleLine {
				                                                 Id = #1,
				                                                 Product = "Pen"
				                                               }
				                                             ],
				                                             OrderId = #3
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("every value masked with the same template shares one counter, and another template counts on its own");
			}

			[Test]
			public async Task Mask_WithoutNumber_ShouldWriteTheFixedText()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleOrder subject = RuleOrder.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Mask("<id>")
						.For.Members().WithName("ByCode").Ignore()
						.For.Members().WithName("Lines").Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleOrder {
				                                             Customer = RuleCustomer {
				                                               Id = <id>,
				                                               Name = "Ann"
				                                             },
				                                             Id = <id>,
				                                             OrderId = 7
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task Mask_WithBracesAroundTheTemplate_ShouldKeepThemAsTheyAre()
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
						.For.Members().WithName("Name").Mask("{{Customer_{0}}}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Id = 1,
				                                             Name = {{Customer_1}}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("{0} is replaced as plain text, not as a composite format, so braces are not escaped");
			}

			[Test]
			public async Task Mask_ShouldBeTheTemplateNamedAfterTheSelection()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleReference subject = new()
				{
					Key = Guid.Parse(FirstGuid),
					Note = $"see {FirstGuid} and {SecondGuid}",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Key").Mask("{Guid_{0}}")
						.For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleReference {
				                                             Key = {Guid_1},
				                                             Note = "see {Guid_1} and {Guid_2}"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("'.For.Guids().Mask()' is the shorthand for the template '{Guid_{0}}', so both share the counter");
			}

			[Test]
			public async Task Mask_OnItems_ShouldNumberTheItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> subject = ["Let It Be", "Abbey Road", "Let It Be",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<string>().Mask("<track {0}>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             <track 1>,
				                                             <track 2>,
				                                             <track 1>
				                                           ]
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task Mask_WithTemplateFunction_ShouldLinkTheIds()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				Ticket subject = Ticket.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<int>().WithName("Id").AsSuffix()
						.Mask(value => "{" + (value.Name == "Id"
							? value.DeclaringType.Name
							: value.Name.Substring(0, value.Name.Length - 2)) + "_{0}}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(LinkedIds).IgnoringNewlineStyle()
					.Because("the id of the fan and the fan id of the ticket get the same template and value, so they share a number");
			}

			[Test]
			public async Task Mask_WithTemplateFunctionForAllSnapshots_ShouldLinkTheIds()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().OfType<int>().WithName("Id").AsSuffix()
						.Mask(value => "{" + (value.Name == "Id"
							? value.DeclaringType.Name
							: value.Name.Substring(0, value.Name.Length - 2)) + "_{0}}"),
				]);
				Ticket subject = Ticket.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(LinkedIds).IgnoringNewlineStyle()
					.Because("a rule for all snapshots takes the same template function");
			}

			[Test]
			public async Task Mask_WhenTheTemplateFunctionReturnsNull_ShouldKeepTheValue()
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
						.For.Members().Mask(value => value.Name == "Name" ? "{name}" : null)
						.For.Members().WithName("Id").Mask("{id}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCustomer {
				                                             Id = {id},
				                                             Name = {name}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a value without a template is not masked by that rule, so the next one applies");
			}

			[Test]
			public async Task Mask_WhenTheTemplateFunctionThrows_ShouldThrowTheException()
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
						.For.Members().Mask(_ => throw new InvalidOperationException("template failed"));

				await That(Act).Throws<InvalidOperationException>().WithMessage("template failed")
					.Because("an exception of the template function is a mistake in the test, which a placeholder would hide");
			}
		}
	}
}

public sealed class Fan
{
	public int Id { get; set; }
	public string Name { get; set; } = "";
}

public sealed class Ticket
{
	public Fan Fan { get; set; } = new();
	public int FanId { get; set; }
	public int Id { get; set; }

	public static Ticket Create()
		=> new()
		{
			Fan = new Fan
			{
				Id = 1023,
				Name = "Ann",
			},
			FanId = 1023,
			Id = 7,
		};
}
