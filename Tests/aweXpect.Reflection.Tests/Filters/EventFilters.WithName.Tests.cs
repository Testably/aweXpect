using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WithName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesWithName()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName(nameof(SomeClassToVerifyTheEventNameOfIt.SomeEventToVerifyTheNameOfIt));

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo("events with name equal to \"SomeEventToVerifyTheNameOfIt\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName("SomeEventToVerifyTheNameOf").AsPrefix();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo("events with name starting with \"SomeEventToVerifyTheNameOf\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsRegex()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName("[a-zA-Z]*EventToVerifyTheName").AsRegex();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo("events with name matching regex \"[a-zA-Z]*EventToVerifyTheName\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName("EventToVerifyTheNameOfIt").AsSuffix();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo("events with name ending with \"EventToVerifyTheNameOfIt\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName("*EventToVerifyTheNameOf*").AsWildcard();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo("events with name matching \"*EventToVerifyTheNameOf*\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportExactly()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName(nameof(SomeClassToVerifyTheEventNameOfIt.SomeEventToVerifyTheNameOfIt))
					.Exactly();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo("events with name equal to \"SomeEventToVerifyTheNameOfIt\" in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName(nameof(SomeClassToVerifyTheEventNameOfIt.SomeEventToVerifyTheNameOfIt)
						.ToLowerInvariant()).IgnoringCase();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo(
						"events with name equal to \"someeventtoverifythenameofit\" ignoring case in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringLeadingWhiteSpace()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName(
						"\t " + nameof(SomeClassToVerifyTheEventNameOfIt.SomeEventToVerifyTheNameOfIt))
					.IgnoringLeadingWhiteSpace();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo(
						"events with name equal to \"\\t SomeEventToVerifyTheNameOfIt\" ignoring leading white-space in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportIgnoringTrailingWhiteSpace()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName(
						nameof(SomeClassToVerifyTheEventNameOfIt.SomeEventToVerifyTheNameOfIt) + "\t ")
					.IgnoringTrailingWhiteSpace();

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo(
						"events with name equal to \"SomeEventToVerifyTheNameOfIt\\t \" ignoring trailing white-space in")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportUsingCustomComparer()
			{
				Filtered.Events events = In.Type<SomeClassToVerifyTheEventNameOfIt>()
					.Events().WithName("SOmEEventTOVErIfyThENAmEofit")
					.Using(new IgnoreCaseForVocalsComparer());

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo(
						"events with name equal to \"SOmEEventTOVErIfyThENAmEofit\" using IgnoreCaseForVocalsComparer in")
					.AsPrefix();
			}
		}
	}
}
