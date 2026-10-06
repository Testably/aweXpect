using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class Triggered
	{
		public sealed class WithParameterTests
		{
			[Test]
			[Arguments(0, false)]
			[Arguments(1, true)]
			[Arguments(2, false)]
			[Arguments(3, false)]
			public async Task ShouldSupportPositionalParameterFilters(int position, bool expectSuccess)
			{
				CustomEventWithParametersClass<string, string, string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, string, string>> recording =
					sut.Record().Events();

				sut.NotifyCustomEvent("p0", "p1", "p2");
				sut.NotifyCustomEvent("p0", "p1", "p2");

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, string, string>.CustomEvent))
						.WithParameter<string>(position, s => s == "p1")
						.AtLeast(2.Times());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut with string parameter [{position}] s => s == "p1" at least twice,
					              but it was never recorded in [
					                CustomEvent("p0", "p1", "p2"),
					                CustomEvent("p0", "p1", "p2")
					              ]
					              """);
			}

			[Test]
			public async Task WhenCustomEventWithParameters_WhenFilterResultsInTooFewRecordings_ShouldFail()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");
				sut.NotifyCustomEvent("bar");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(s => s == "foo")
						.AtLeast(2.Times());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut with string parameter s => s == "foo" at least twice,
					             but it was only recorded once in [
					               CustomEvent("foo"),
					               CustomEvent("bar")
					             ]
					             """)
					.Because("too few recordings are phrased like too few signals");
			}

			[Test]
			public async Task WhenCustomEventWithParametersIsTriggeredOftenEnough_ShouldSucceed()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");
				sut.NotifyCustomEvent("foo");
				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(s => s == "foo")
						.AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMultipleFiltersAreSpecified_ShouldVerifyAllFilters_ShouldFail()
			{
				CustomEventWithParametersClass<string, int> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo", 1);
				sut.NotifyCustomEvent("bar", 2);

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string, int>.CustomEvent))
						.WithParameter<string>(s => s == "foo")
						.WithParameter<int>(i => i > 1)
						.AtLeast(1.Times());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut with string parameter s => s == "foo" and with int parameter i => i > 1 at least once,
					             but it was never recorded in [
					               CustomEvent("foo", 1),
					               CustomEvent("bar", 2)
					             ]
					             """);
			}

			[Test]
			public async Task WhenMultiplePositionFiltersAreSpecified_ShouldVerifyAllFilters_ShouldFail()
			{
				CustomEventWithParametersClass<string, string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo1", "bar2");
				sut.NotifyCustomEvent("bar1", "foo2");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string, int>.CustomEvent))
						.WithParameter<string>(0, s => s == "foo1")
						.WithParameter<string>(1, s => s == "foo2");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut with string parameter [0] s => s == "foo1" and with string parameter [1] s => s == "foo2" at least once,
					             but it was never recorded in [
					               CustomEvent("foo1", "bar2"),
					               CustomEvent("bar1", "foo2")
					             ]
					             """);
			}

			[Test]
			public async Task WhenPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(-1, _ => true);

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("position").And
					.WithMessage("The position must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(_ => throw exception);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut with string parameter _ => throw exception at least once,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenPredicateThrows_WhenNegated_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(_ => throw exception);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut with string parameter _ => throw exception,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a predicate that threw answered nothing, so the negation fails as well");
			}

			[Test]
			public async Task WhenPredicateWithPositionThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(0, _ => throw exception)
						.Within(10.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut with string parameter [0] _ => throw exception at least once within 0:10,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenPredicateWithPositionThrows_WhenNegated_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(0, _ => throw exception));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut with string parameter [0] _ => throw exception,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a predicate that threw answered nothing, so the negation fails as well");
			}

			[Test]
			public async Task WhenTypeIsNotUnique_ShouldCheckAllMatchingParameters()
			{
				CustomEventWithParametersClass<string, string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo", "bar");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string, int>.CustomEvent))
						.WithParameter<string>(s => s == "bar");

				await That(Act).DoesNotThrow();
			}
		}
	}
}
