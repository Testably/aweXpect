using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Recording;
using aweXpect.Results;

namespace aweXpect.Internal.Tests.Results;

public sealed class EventTriggerResultTests
{
	[Test]
	public async Task ShouldBeOptionsProvider_ForQuantifier()
	{
		Quantifier quantifier = new();
		RepeatedCheckOptions options = new();
		EventTriggerResult<EventTriggerResultTests> sut = CreateSut(new EventTriggerResultTests(), quantifier, options);

		await That(sut).Is<IOptionsProvider<Quantifier>>()
			.Whose(x => x.Options, it => it.IsSameAs(quantifier));
	}

	[Test]
	public async Task ShouldBeOptionsProvider_ForRepeatedCheckOptions()
	{
		Quantifier quantifier = new();
		RepeatedCheckOptions options = new();
		EventTriggerResult<EventTriggerResultTests> sut = CreateSut(new EventTriggerResultTests(), quantifier, options);

		await That(sut).Is<IOptionsProvider<RepeatedCheckOptions>>()
			.Whose(x => x.Options, it => it.IsSameAs(options));
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_CanSpecifyNameOfParameter()
	{
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		sut.NotifyCustomEvent("foo");
		sut.NotifyCustomEvent("bar");

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent)))
				.WithParameter<string>(" with my parameter", null, s => s == "foo")
				.AtLeast().Twice();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least twice,
			             but it was only recorded once in [
			               CustomEvent("foo"),
			               CustomEvent("bar")
			             ]
			             """);
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsNull_WithoutPosition_ShouldIgnoreTheArgument()
	{
		CustomEventWithParametersClass<string?> sut = new();
		IEventRecording<CustomEventWithParametersClass<string?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(null);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string?>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string?>.CustomEvent)))
				.WithParameter<string?>(" with my parameter", null, p => p is null);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least once,
			             but it was never recorded in [
			               CustomEvent(<null>)
			             ]
			             """)
			.Because("a null argument has no type that could match at any position");
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsNull_WithPosition_ForNonNullableValueType_ShouldNotInvokeThePredicate()
	{
		bool isInvoked = false;
		CustomEventWithParametersClass<int?> sut = new();
		IEventRecording<CustomEventWithParametersClass<int?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(null);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<int?>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<int?>.CustomEvent)))
				.WithParameter<int>(" with my parameter", 0, _ =>
				{
					isInvoked = true;
					return true;
				});

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least once,
			             but it was never recorded in [
			               CustomEvent(<null>)
			             ]
			             """);
		await That(isInvoked).IsFalse();
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsNull_WithPosition_ForNullableValueType_ShouldPassNullToThePredicate()
	{
		CustomEventWithParametersClass<int?> sut = new();
		IEventRecording<CustomEventWithParametersClass<int?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(null);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<int?>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<int?>.CustomEvent)))
				.WithParameter<int?>(" with my parameter", 0, p => p is null);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsNull_WithPosition_ShouldPassNullToThePredicate()
	{
		CustomEventWithParametersClass<string?> sut = new();
		IEventRecording<CustomEventWithParametersClass<string?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(null);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string?>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string?>.CustomEvent)))
				.WithParameter<string?>(" with my parameter", 0, p => p is null);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsNull_WithPosition_WhenNegated_ShouldFail()
	{
		CustomEventWithParametersClass<string?> sut = new();
		IEventRecording<CustomEventWithParametersClass<string?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(null);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string?>>.ICustomParameterFilter)That(recording)
					.DidNotTrigger(nameof(CustomEventWithParametersClass<string?>.CustomEvent)))
				.WithParameter<string?>(" with my parameter", 0, p => p is null);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has never recorded the CustomEvent event on sut with my parameter,
			             but it was recorded once in [
			               CustomEvent(<null>)
			             ]
			             """);
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsNull_WithPosition_WhenPredicateIsNotNullSafe_ShouldFailWithTheExceptionAsInnerException()
	{
		CustomEventWithParametersClass<string?> sut = new();
		IEventRecording<CustomEventWithParametersClass<string?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(null);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string?>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string?>.CustomEvent)))
				.WithParameter<string>(" with my parameter", 0, p => p.Length > 3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least once,
			             but the predicate did throw a NullReferenceException:
			               *
			             """).AsWildcard().And
			.Whose(e => e.InnerException, i => i.Is<NullReferenceException>());
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenArgumentIsOfAnotherType_WithPosition_ShouldFail()
	{
		CustomEventWithParametersClass<object?> sut = new();
		IEventRecording<CustomEventWithParametersClass<object?>> recording = sut.Record().Events();

		sut.NotifyCustomEvent(42);

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<object?>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<object?>.CustomEvent)))
				.WithParameter<string?>(" with my parameter", 0, _ => true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least once,
			             but it was never recorded in [
			               CustomEvent(42)
			             ]
			             """);
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenPositionIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		sut.NotifyCustomEvent("foo");

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent)))
				.WithParameter<string>(" with my parameter", -1, _ => true);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("position").And
			.WithMessage("The position must not be negative.").AsPrefix();
	}

	[Test]
	public async Task WhenCastingToICustomParameterFilter_WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
	{
		InvalidOperationException exception = new("predicate failed");
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		sut.NotifyCustomEvent("foo");

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent)))
				.WithParameter<string>(" with my parameter", null, _ => throw exception);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least once,
			             but the predicate did throw an InvalidOperationException:
			               predicate failed
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task WhenPredicateIsNull_ForEventArgs_ShouldThrowArgumentNullException()
	{
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		async Task Act() =>
			await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
				.With<EventArgs>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WhenPredicateIsNull_ForParameter_ShouldThrowArgumentNullException()
	{
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		async Task Act() =>
			await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
				.WithParameter<string>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WhenPredicateIsNull_ForParameter_WithPosition_ShouldThrowArgumentNullException()
	{
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		async Task Act() =>
			await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
				.WithParameter<string>(1, null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WhenPredicateIsNull_ForSender_ShouldThrowArgumentNullException()
	{
		CustomEventWithParametersClass<string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

		async Task Act() =>
			await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
				.WithSender(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task WithMultipleMatchingParameters_PredicateChecksForAnyOne()
	{
		CustomEventWithParametersClass<string, int, string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string, int, string>> recording = sut.Record().Events();

		sut.NotifyCustomEvent("foo", 1, "bar");
		sut.NotifyCustomEvent("bar", 2, "foo");

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string, int, string>>.ICustomParameterFilter)That(recording)
					.Triggered(nameof(CustomEventWithParametersClass<string, int, string>.CustomEvent)))
				.WithParameter<string>(" with my parameter", null, s => s == "foo")
				.AtLeast(2.Times());

		await That(Act).DoesNotThrow();
	}

	[Test]
	[Arguments(0, false)]
	[Arguments(1, true)]
	[Arguments(2, false)]
	[Arguments(3, true)]
	[Arguments(4, true)]
	public async Task WithPosition_ShouldConsiderPosition(int position, bool expectFailure)
	{
		CustomEventWithParametersClass<string, string, string> sut = new();
		IEventRecording<CustomEventWithParametersClass<string, string, string>> recording = sut.Record().Events();

		sut.NotifyCustomEvent("foo", "bar", "baz");
		sut.NotifyCustomEvent("bar", "baz", "foo");

		async Task Act() =>
			await ((EventTriggerResult<CustomEventWithParametersClass<string, string, string>>.ICustomParameterFilter)
					That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, string, string>.CustomEvent)))
				.WithParameter<string>(" with my parameter", position, s => s == "foo");

		await That(Act).Throws<FailException>().OnlyIf(expectFailure)
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut with my parameter at least once,
			             but it was never recorded in [
			               CustomEvent("foo", "bar", "baz"),
			               CustomEvent("bar", "baz", "foo")
			             ]
			             """);
	}

	private static EventTriggerResult<T> CreateSut<T>(T subject, Quantifier quantifier, RepeatedCheckOptions options,
		TriggerEventFilter? filter = null)
		where T : notnull
	{
		RecordingFactory<T> recording = new(subject, nameof(subject));
		filter ??= new TriggerEventFilter();
#pragma warning disable aweXpect0001
		IThat<IEventRecording<T>> source = That(recording.Events());
#pragma warning restore aweXpect0001
		return new EventTriggerResult<T>(source.Get().ExpectationBuilder,
			source,
			filter,
			quantifier,
			options);
	}

	private sealed class CustomEventWithParametersClass<T1>
	{
		public delegate void CustomEventDelegate(T1 arg1);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(T1 arg1)
			=> CustomEvent?.Invoke(arg1);
	}

	private sealed class CustomEventWithParametersClass<T1, T2, T3>
	{
		public delegate void CustomEventDelegate(T1 arg1, T2 arg2, T3 arg3);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(T1 arg1, T2 arg2, T3 arg3)
			=> CustomEvent?.Invoke(arg1, arg2, arg3);
	}
}
