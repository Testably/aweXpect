using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Signaling;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Equivalency;

/// <summary>
///     The expectation of an <c>It.Is…</c> in an expected object is evaluated as part of the evaluation that compares
///     the objects, so the timeout and the cancellation of that evaluation also end it.
/// </summary>
/// <remarks>
///     Everything a nested expectation waits for ends on its own after half a minute, so that a regression fails the
///     test instead of hanging the test run.
/// </remarks>
public sealed class NestedExpectationEvaluationTests
{
	/// <summary>
	///     Where the objects are compared for equivalency.
	/// </summary>
	public enum Usage
	{
		Object,
		DictionaryValue,
		CollectionItem,
	}

	/// <summary>
	///     The expectations that can compare for equivalency.
	/// </summary>
	public enum Comparison
	{
		ObjectIsEquivalentTo,
		ObjectIsEqualTo,
		ObjectIsOneOf,
		DictionaryIsEqualTo,
		DictionaryContains,
		DictionaryContainsValue,
		DictionaryContainsValues,
		CollectionIsEqualTo,
		CollectionContains,
		CollectionStartsWith,
		CollectionEndsWith,
		CollectionHasItem,
		CollectionAllAreEqualTo,
		CollectionAllAreEquivalentTo,
		CollectionAllAreUnique,
#if NET8_0_OR_GREATER
		AsyncCollectionIsEqualTo,
		AsyncCollectionContains,
		AsyncCollectionStartsWith,
		AsyncCollectionEndsWith,
		AsyncCollectionHasItem,
		AsyncCollectionAllAreEqualTo,
		AsyncCollectionAllAreEquivalentTo,
		AsyncCollectionAllAreUnique,
#endif
	}

	private static readonly TimeSpan SafetyNet = 30.Seconds();

	[Test]
	[Arguments(Comparison.ObjectIsEquivalentTo)]
	[Arguments(Comparison.ObjectIsEqualTo)]
	[Arguments(Comparison.ObjectIsOneOf)]
	[Arguments(Comparison.DictionaryIsEqualTo)]
	[Arguments(Comparison.DictionaryContains)]
	[Arguments(Comparison.DictionaryContainsValue)]
	[Arguments(Comparison.DictionaryContainsValues)]
	[Arguments(Comparison.CollectionIsEqualTo)]
	[Arguments(Comparison.CollectionContains)]
	[Arguments(Comparison.CollectionStartsWith)]
	[Arguments(Comparison.CollectionEndsWith)]
	[Arguments(Comparison.CollectionHasItem)]
	[Arguments(Comparison.CollectionAllAreEqualTo)]
	[Arguments(Comparison.CollectionAllAreEquivalentTo)]
	[Arguments(Comparison.CollectionAllAreUnique)]
#if NET8_0_OR_GREATER
	[Arguments(Comparison.AsyncCollectionIsEqualTo)]
	[Arguments(Comparison.AsyncCollectionContains)]
	[Arguments(Comparison.AsyncCollectionStartsWith)]
	[Arguments(Comparison.AsyncCollectionEndsWith)]
	[Arguments(Comparison.AsyncCollectionHasItem)]
	[Arguments(Comparison.AsyncCollectionAllAreEqualTo)]
	[Arguments(Comparison.AsyncCollectionAllAreEquivalentTo)]
	[Arguments(Comparison.AsyncCollectionAllAreUnique)]
#endif
	public async Task EveryComparison_ShouldEvaluateTheNestedExpectationWithTheTokenOfTheEvaluation(
		Comparison comparison)
	{
		using CancellationTokenSource cts = new();
		CapturingConstraint constraint = new(comparison.ToString().EndsWith("AreUnique")
			? Outcome.Failure
			: Outcome.Success);
		MyClass subject = new()
		{
			Value = 1,
		};
		object expected = new
		{
			Value = ItIs(constraint),
		};

		async Task Act()
			=> await Compare(comparison, subject, expected, cts.Token);

		await That(Act).DoesNotThrow()
			.Because("the nested expectation is met, except for AreUnique, whose items must not be equivalent");
		await That(constraint.CancellationToken).IsEqualTo(cts.Token)
			.Because("the expectation has to compare with the options of its evaluation");
	}

	[Test]
	public async Task ForEvaluation_ShouldEvaluateTheNestedExpectationInAContextOfTheEvaluation()
	{
		using CancellationTokenSource cts = new();
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			Cancellation = EvaluationCancellation.Create(null, cts.Token),
			TimeSystem = timeSystem,
		};
		CapturingConstraint constraint = new(Outcome.Success);
		ObjectEqualityOptions<MyClass> sut = Equivalent();

		ObjectEqualityOptions<MyClass> options = sut.ForEvaluation(context, cts.Token);
		bool result = await options.AreConsideredEqual(new MyClass
		{
			Value = 1,
		}, new
		{
			Value = ItIs(constraint),
		});

		await That(result).IsTrue();
		await That(options).IsNotSameAs(sut);
		await That(constraint.Context).IsNotSameAs(context)
			.Because("the collections and the reasons of the nested expectation are released after the comparison");
		await That(constraint.Context?.Cancellation).IsSameAs(context.Cancellation);
		await That((constraint.Context as Context)?.TimeSystem).IsSameAs(timeSystem);
		await That(constraint.CancellationToken).IsEqualTo(cts.Token);
	}

	[Test]
	public async Task ForEvaluation_ShouldNotShareTheDifferencesBetweenEvaluations()
	{
		ObjectEqualityOptions<MyClass> sut = Equivalent();
		MyClass actual = new()
		{
			Value = 1,
		};

		IObjectMatchResult first = await sut.ForEvaluation(new Context(), CancellationToken.None)
			.AreConsideredEqualWithExplanation(actual, new
			{
				Value = 2,
			});
		IObjectMatchResult second = await sut.ForEvaluation(new Context(), CancellationToken.None)
			.AreConsideredEqualWithExplanation(actual, new
			{
				Value = 3,
			});

		await That(second).IsNotSameAs(first);
		await That(first.GetExtendedFailure("it", ExpectationGrammars.None, actual, actual))
			.IsEqualTo("""
			           it was not:
			             Property Value differed:
			                 Actual: 1
			               Expected: 2
			           """)
			.Because("each evaluation keeps the differences of its own comparison");
	}

	[Test]
	public async Task ForEvaluation_WhenTheNestedExpectationFails_ShouldLeaveNothingInTheContextOfTheEvaluation()
	{
		Context context = new();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		CapturingConstraint constraint = new(Outcome.Failure, source);
		It.IsEquivalent<int> expectation = ItIs(constraint);
		_ = expectation.Because(Task.FromResult<string?>("of a reason"));

		IObjectMatchResult result = await Equivalent().ForEvaluation(context, CancellationToken.None)
			.AreConsideredEqualWithExplanation(new MyClass
			{
				Value = 1,
			}, new
			{
				Value = expectation,
			});
		int disposeCountAfterTheComparison = source.DisposeCount;
		await context.ReleaseMaterializations();

		await That(result.IsMatch).IsFalse();
		await That(disposeCountAfterTheComparison).IsEqualTo(1)
			.Because("the source that the nested expectation materialized is released with its comparison");
		await That(source.DisposeCount).IsEqualTo(1)
			.Because("the evaluation must not release the source again");
		await That(context.HasPendingReasons).IsFalse();
	}

	[Test]
	public async Task ForEvaluation_WithoutEquivalency_ShouldReturnTheSameOptions()
	{
		ObjectEqualityOptions<MyClass> sut = new();

		ObjectEqualityOptions<MyClass> options = sut.ForEvaluation(new Context(), CancellationToken.None);

		await That(options).IsSameAs(sut)
			.Because("a comparison that evaluates nothing needs no options of its own for an evaluation");
	}

	[Test]
	public async Task WithoutForEvaluation_ShouldEvaluateTheNestedExpectationOnItsOwn()
	{
		CapturingConstraint constraint = new(Outcome.Success);

		bool result = await Equivalent().AreConsideredEqual(new MyClass
		{
			Value = 1,
		}, new
		{
			Value = ItIs(constraint),
		});

		await That(result).IsTrue();
		await That(constraint.Context?.Cancellation).IsSameAs(EvaluationCancellation.None);
		await That(constraint.CancellationToken).IsEqualTo(CancellationToken.None);
	}

	[Test]
	public async Task NestedWait_ShouldWaitOnTheTimeSystemOfTheEvaluation()
	{
		VirtualTimeSystem timeSystem = new();
		MyClass subject = new()
		{
			Value = new Signaler(),
		};
		var expected = new
		{
			Value = It.Is<Signaler>().That.Signaled().Within(30.Seconds()),
		};

		async Task Act()
			=> await That(subject).IsEquivalentTo(expected).UseTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equivalent to expected,
			             but it was not:
			               Property Value differed:
			                   Actual: Signaler { }
			                 Expected: is Signaler that has recorded the callback at least once within 0:30

			             Equivalency options:
			              - include public fields and properties
			             """);
		await That(timeSystem.Now).IsEqualTo(30.Seconds())
			.Because("the nested expectation waited on the virtual clock of the evaluation");
	}

	[Test]
	public async Task NestedWait_WhenTheTimeoutIsShorter_ShouldFailWithTheTimeoutOnTheTimeSystemOfTheEvaluation()
	{
		VirtualTimeSystem timeSystem = new();
		MyClass subject = new()
		{
			Value = new Signaler(),
		};
		var expected = new
		{
			Value = It.Is<Signaler>().That.Signaled().Within(30.Seconds()),
		};

		async Task Act()
			=> await That(subject).IsEquivalentTo(expected).WithTimeout(10.Seconds()).UseTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("*but it did not finish within 0:10*").AsWildcard().And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:10."));
		await That(timeSystem.Now).IsEqualTo(10.Seconds())
			.Because("the timeout cuts the wait of the nested expectation short on the virtual clock");
	}

	[Test]
	[Arguments(Usage.Object)]
	[Arguments(Usage.DictionaryValue)]
	[Arguments(Usage.CollectionItem)]
	public async Task NestedWait_WhenCanceled_ShouldBeInconclusive(Usage usage)
	{
		using CancellationTokenSource cts = new();

		async Task Act()
			=> await Evaluate(usage, 1, It.Is<int>().That.Satisfies(_ =>
			{
				cts.Cancel();
				return false;
			}).Within(SafetyNet), cancellationToken: cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("*but it could not be verified, because the evaluation was already canceled*").AsWildcard()
			.Because("the cancellation has to end the wait of the nested expectation");
	}

	[Test]
	[Arguments(Usage.Object)]
	[Arguments(Usage.DictionaryValue)]
	[Arguments(Usage.CollectionItem)]
	public async Task NestedWait_WhenTimeoutElapses_ShouldFailWithTheTimeout(Usage usage)
	{
		async Task Act()
			=> await Evaluate(usage, 1, It.Is<int>().That.Satisfies(_ => false).Within(SafetyNet),
				50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("*but it did not finish within 0:00.050*").AsWildcard().And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the timeout has to end the wait of the nested expectation");
	}

	[Test]
	[Arguments(Usage.Object)]
	[Arguments(Usage.DictionaryValue)]
	[Arguments(Usage.CollectionItem)]
	public async Task PendingReason_WhenCanceled_ShouldStillReportTheDifference(Usage usage)
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await Evaluate(usage, 1, It.Is<int>().That.IsEqualTo(2).Because(PendingTask.Of<string?>()),
				cancellationToken: cts.Token);

		await That(Act).Throws<FailException>()
			.WithMessage("*is int that is equal to 2, because the reason was not available in time*").AsWildcard()
			.Because("the cancellation has to stop waiting for the reason of the nested expectation");
	}

	[Test]
	[Arguments(Usage.Object)]
	[Arguments(Usage.DictionaryValue)]
	[Arguments(Usage.CollectionItem)]
	public async Task PendingReason_WhenTimeoutElapses_ShouldStillReportTheDifference(Usage usage)
	{
		async Task Act()
			=> await Evaluate(usage, 1, It.Is<int>().That.IsEqualTo(2).Because(PendingTask.Of<string?>()),
				50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("*is int that is equal to 2, because the reason was not available in time*").AsWildcard()
			.Because("the timeout has to stop waiting for the reason of the nested expectation");
	}

#if NET8_0_OR_GREATER
	[Test]
	[Arguments(Usage.Object)]
	[Arguments(Usage.DictionaryValue)]
	[Arguments(Usage.CollectionItem)]
	public async Task StalledAsyncSource_WhenCanceled_ShouldBeInconclusive(Usage usage)
	{
		using CancellationTokenSource cts = new();

		async Task Act()
			=> await Evaluate(usage, StallsAfterTheFirstItem(cts.Cancel),
				It.Is<IAsyncEnumerable<int>>().That.Contains(2), cancellationToken: cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("*but it could not be verified, because the evaluation was already canceled*").AsWildcard()
			.Because("the cancellation has to end the enumeration by the nested expectation");
	}

	[Test]
	[Arguments(Usage.Object)]
	[Arguments(Usage.DictionaryValue)]
	[Arguments(Usage.CollectionItem)]
	public async Task StalledAsyncSource_WhenTimeoutElapses_ShouldFailWithTheTimeout(Usage usage)
	{
		async Task Act()
			=> await Evaluate(usage, StallsAfterTheFirstItem(),
				It.Is<IAsyncEnumerable<int>>().That.Contains(2), 50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("*but it did not finish within 0:00.050*").AsWildcard().And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the timeout has to end the enumeration by the nested expectation");
	}

	/// <summary>
	///     Yields one item and then stalls without observing a cancellation.
	/// </summary>
	private static async IAsyncEnumerable<int> StallsAfterTheFirstItem(Action? whenStalled = null)
	{
		yield return 1;
		whenStalled?.Invoke();
		await Task.Delay(SafetyNet);
	}
#endif

	/// <summary>
	///     Compares an object whose <c>Value</c> is the <paramref name="value" /> for equivalency with one whose
	///     <c>Value</c> is the <paramref name="expectedValue" />.
	/// </summary>
	private static async Task Evaluate(Usage usage, object? value, object? expectedValue, TimeSpan? timeout = null,
		CancellationToken? cancellationToken = null)
	{
		MyClass subject = new()
		{
			Value = value,
		};
		var expected = new
		{
			Value = expectedValue,
		};
		switch (usage)
		{
			case Usage.Object:
				await Within(That(subject).IsEquivalentTo(expected), timeout, cancellationToken);
				break;
			case Usage.DictionaryValue:
				Dictionary<string, object?> dictionary = new()
				{
					["key"] = subject,
				};
				await Within(That(dictionary).ContainsValue(expected).Equivalent(), timeout, cancellationToken);
				break;
			default:
				object?[] collection = [subject,];
				await Within(That(collection).Contains(expected).Equivalent(), timeout, cancellationToken);
				break;
		}
	}

	/// <summary>
	///     Compares the <paramref name="subject" /> for equivalency with the <paramref name="expected" /> object in an
	///     evaluation with the <paramref name="cancellationToken" />.
	/// </summary>
	private static async Task Compare(Comparison comparison, object subject, object expected,
		CancellationToken cancellationToken)
	{
		Dictionary<string, object?> dictionary = new()
		{
			["key"] = subject,
		};
		object?[] collection = [subject,];
		switch (comparison)
		{
			case Comparison.ObjectIsEquivalentTo:
				await That(subject).IsEquivalentTo(expected).WithCancellation(cancellationToken);
				break;
			case Comparison.ObjectIsEqualTo:
				await That(subject).IsEqualTo(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.ObjectIsOneOf:
				await That(subject).IsOneOf(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.DictionaryIsEqualTo:
				await That(dictionary).IsEqualTo(new Dictionary<string, object?>
				{
					["key"] = expected,
				}).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.DictionaryContains:
				await That(dictionary).Contains(new KeyValuePair<string, object?>("key", expected)).Equivalent()
					.WithCancellation(cancellationToken);
				break;
			case Comparison.DictionaryContainsValue:
				await That(dictionary).ContainsValue(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.DictionaryContainsValues:
				await That(dictionary).ContainsValues(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionIsEqualTo:
				await That(collection).IsEqualTo([expected,]).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionContains:
				await That(collection).Contains(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionStartsWith:
				await That(collection).StartsWith(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionEndsWith:
				await That(collection).EndsWith(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionHasItem:
				await That(collection).HasItem(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionAllAreEqualTo:
				await That(collection).All().AreEqualTo(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionAllAreEquivalentTo:
				await That(collection).All().AreEquivalentTo(expected).WithCancellation(cancellationToken);
				break;
			case Comparison.CollectionAllAreUnique:
				await That(new[]
				{
					expected, subject,
				}).All().AreUnique().Equivalent().WithCancellation(cancellationToken);
				break;
#if NET8_0_OR_GREATER
			default:
				await CompareAsyncCollection(comparison, subject, expected, cancellationToken);
				break;
#endif
		}
	}

#if NET8_0_OR_GREATER
	private static async Task CompareAsyncCollection(Comparison comparison, object subject, object expected,
		CancellationToken cancellationToken)
	{
		IAsyncEnumerable<object?> collection = ToAsyncEnumerable(subject);
		switch (comparison)
		{
			case Comparison.AsyncCollectionIsEqualTo:
				await That(collection).IsEqualTo([expected,]).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.AsyncCollectionContains:
				await That(collection).Contains(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.AsyncCollectionStartsWith:
				await That(collection).StartsWith(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.AsyncCollectionEndsWith:
				await That(collection).EndsWith(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.AsyncCollectionHasItem:
				await That(collection).HasItem(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.AsyncCollectionAllAreEqualTo:
				await That(collection).All().AreEqualTo(expected).Equivalent().WithCancellation(cancellationToken);
				break;
			case Comparison.AsyncCollectionAllAreEquivalentTo:
				await That(collection).All().AreEquivalentTo(expected).WithCancellation(cancellationToken);
				break;
			default:
				await That(ToAsyncEnumerable(expected, subject)).All().AreUnique().Equivalent()
					.WithCancellation(cancellationToken);
				break;
		}
	}

	private static async IAsyncEnumerable<object?> ToAsyncEnumerable(params object?[] items)
	{
		foreach (object? item in items)
		{
			await Task.Yield();
			yield return item;
		}
	}
#endif

	private static TSelf Within<TType, TSelf>(ExpectationResult<TType, TSelf> result, TimeSpan? timeout,
		CancellationToken? cancellationToken)
		where TSelf : ExpectationResult<TType, TSelf>
	{
		TSelf limited = (TSelf)result;
		if (timeout is not null)
		{
			limited = limited.WithTimeout(timeout.Value);
		}

		return cancellationToken is null ? limited : limited.WithCancellation(cancellationToken.Value);
	}

	private static ObjectEqualityOptions<MyClass> Equivalent()
	{
		ObjectEqualityOptions<MyClass> options = new();
		options.SetMatchType(new EquivalencyMatchType(new EquivalencyOptions()));
		return options;
	}

	private static It.IsEquivalent<int> ItIs(CapturingConstraint constraint)
	{
		It.IsEquivalent<int> expectation = It.Is<int>();
		((IExpectThat<int>)expectation).ExpectationBuilder.AddConstraint((_, _) => constraint);
		return expectation;
	}

	/// <summary>
	///     Keeps the context and the token it is evaluated with, and reads the first item of the
	///     <paramref name="source" /> through the context.
	/// </summary>
	private sealed class CapturingConstraint(Outcome outcome, IEnumerable<int>? source = null)
		: IAsyncContextConstraint<int>
	{
		public CancellationToken CancellationToken { get; private set; }
		public IEvaluationContext? Context { get; private set; }

		public ValueTask<ConstraintResult> IsMetBy(int actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Context = context;
			CancellationToken = cancellationToken;
			if (source is not null)
			{
				_ = context.UseMaterializedEnumerable(source).First();
			}

			return new ValueTask<ConstraintResult>(new DummyConstraintResult(outcome, "captures"));
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("captures");
	}

	private sealed class MyClass
	{
		public object? Value { get; set; }
	}
}
