using System.Collections.Generic;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;
using aweXpect.Customization;
using aweXpect.Options;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core;

public sealed class CollectionMatcherExtensionsTests
{
	[Test]
	public async Task VerifyItems_WhenAnItemExpectationIsCanceled_ShouldBeUndecided()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		CollectionMatchOptions.ExpectationItem<int>[] expected =
		[
			new(x => x.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new UndecidedConstraint(it, grammars)),
				ExpectationGrammars.None, new Context(), cts.Token),
		];
		ICollectionMatcher<int, int> matcher = new CollectionMatchOptions().GetCollectionMatcher<int, int>(expected);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", [1,], new ObjectEqualityOptions<int>(), cts.Token);

		await That(outcome).IsEqualTo(Outcome.Undecided)
			.Because("the canceled item expectation did not decide whether the item matches");
		await That(failure).IsNull();
	}

	[Test]
	public async Task VerifyItems_WhenCanceledBeforeTheEndOfASequence_ShouldBeUndecided()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		ICollectionMatcher<int, int> matcher = new CollectionMatchOptions().GetCollectionMatcher<int, int>([1, 2,]);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", ToEnumerable(1, 3), new ObjectEqualityOptions<int>(), cts.Token);

		await That(outcome).IsEqualTo(Outcome.Undecided)
			.Because("the remaining items of the sequence were not read");
		await That(failure).IsNull();
	}

	[Test]
	public async Task VerifyItems_WhenCanceledForACompleteCollection_ShouldVerifyTheItems()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		ICollectionMatcher<int, int> matcher = new CollectionMatchOptions().GetCollectionMatcher<int, int>([1, 2,]);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", new List<int> { 1, 3, }, new ObjectEqualityOptions<int>(), cts.Token);

		await That(outcome).IsEqualTo(Outcome.Failure)
			.Because("a collection that is complete in memory is judged like any other value");
		await That(failure).IsEqualTo("it contained item 3 at index 1 instead of 2");
	}

	[Test]
	public async Task VerifyItems_WhenItemsAreNull_ShouldThrowArgumentNullException()
	{
		ICollectionMatcher<int, int> matcher = new CollectionMatchOptions().GetCollectionMatcher<int, int>([1,]);

		async Task Act()
			=> await matcher.VerifyItems("it", null!, new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("items").And
			.WithMessage("The 'items' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task VerifyItems_WhenItemsDiffer_ShouldReturnTheFailureOfTheCollectionIsEqualTo()
	{
		ICollectionMatcher<string, string> matcher = new CollectionMatchOptions().InAnyOrder()
			.GetCollectionMatcher<string, string>(["rock", "jazz",]);

		(Outcome outcome, string? failure) = await matcher.VerifyItems("it", ["rock", "pop",],
			new ObjectEqualityOptions<string>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Failure);
		await That(failure).IsEqualTo("""
		                              it
		                                contained item "pop" at index 1 that was not expected and
		                                lacked 1 of 2 expected items: "jazz"
		                              """);
	}

	[Test]
	public async Task VerifyItems_WhenItemsMatch_ShouldSucceed()
	{
		ICollectionMatcher<int, int> matcher =
			new CollectionMatchOptions().GetCollectionMatcher<int, int>([1, 2, 3,]);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", ToEnumerable(1, 2, 3), new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Success);
		await That(failure).IsNull();
	}

	[Test]
	public async Task VerifyItems_WhenMatcherAnswersAsynchronouslyAndAnItemFails_ShouldStopAtIt()
	{
		AwaitingMatcher matcher = new(failingValue: 3);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", ToEnumerable(1, 2, 3, 4), new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Failure);
		await That(failure).IsEqualTo("it had 3");
		await That(matcher.VerifiedValues).IsEqualTo([1, 2, 3,])
			.Because("the items after the failure need not be verified");
	}

	[Test]
	public async Task VerifyItems_WhenMatcherAnswersAsynchronouslyAndNoItemFails_ShouldComplete()
	{
		AwaitingMatcher matcher = new(failingValue: null);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", [1, 2,], new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Failure);
		await That(failure).IsEqualTo("it was completed");
		await That(matcher.VerifiedValues).IsEqualTo([1, 2,]);
	}

	[Test]
	public async Task VerifyItems_WhenMatcherIsDetermined_ShouldStopReadingTheItems()
	{
		int readItems = 0;

		IEnumerable<int> Source()
		{
			for (int i = 1; i <= 10; i++)
			{
				readItems++;
				yield return i;
			}
		}

		ICollectionMatcher<int, int> matcher =
			new CollectionMatchOptions(CollectionMatchOptions.EquivalenceRelations.Contains)
				.GetCollectionMatcher<int, int>([1, 2,]);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", Source(), new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Success);
		await That(failure).IsNull();
		await That(readItems).IsEqualTo(2).Because("additional items cannot change the result of the containment");
	}

	[Test]
	public async Task VerifyItems_WhenMatcherIsNull_ShouldThrowArgumentNullException()
	{
		ICollectionMatcher<int, int> matcher = null!;

		async Task Act()
			=> await matcher.VerifyItems("it", [1,], new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("matcher").And
			.WithMessage("The 'matcher' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task VerifyItems_WhenMatcherReportsTooManyDeviations_ShouldNameTheirLimit()
	{
		AwaitingMatcher matcher = new(failingValue: 1, failure: null, isAwaited: false);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", [1,], new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Failure);
		await That(failure).IsEqualTo(
				$"it had more than {2 * Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get()} deviations")
			.Because("the limit is twice the maximum number of collection items");
	}

	[Test]
	public async Task VerifyItems_WhenOptionsAreNull_ShouldThrowArgumentNullException()
	{
		ICollectionMatcher<int, int> matcher = new CollectionMatchOptions().GetCollectionMatcher<int, int>([1,]);

		async Task Act()
			=> await matcher.VerifyItems("it", [1,], null!, CancellationToken.None);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("options").And
			.WithMessage("The 'options' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task VerifyItems_WhenTheEnumerationThrows_ShouldThrowAsCodeOfTheCaller()
	{
		NotSupportedException exception = new("thrown by the enumeration");

		IEnumerable<int> Source()
		{
			yield return 1;
			throw exception;
		}

		ICollectionMatcher<int, int> matcher = new CollectionMatchOptions().GetCollectionMatcher<int, int>([1, 2,]);

		async Task Act()
			=> await matcher.VerifyItems("it", Source(), new ObjectEqualityOptions<int>(), CancellationToken.None);

		UserCodeException failure = await That(Act).Throws<UserCodeException>()
			.WithMessage("The code of the caller threw an exception while the expectation was evaluated.");
		await That(failure.InnerException).IsSameAs(exception);
	}

	[Test]
	public async Task VerifyItems_WithAMaterializedSequence_ShouldReadItsItems()
	{
		IEnumerable<int> materialized = new Context().UseMaterializedEnumerable(ToEnumerable(1, 2, 4));
		ICollectionMatcher<int, int> matcher =
			new CollectionMatchOptions().GetCollectionMatcher<int, int>([1, 2, 3,]);

		(Outcome outcome, string? failure) =
			await matcher.VerifyItems("it", materialized, new ObjectEqualityOptions<int>(), CancellationToken.None);

		await That(outcome).IsEqualTo(Outcome.Failure);
		await That(failure).IsEqualTo("it contained item 4 at index 2 instead of 3");
	}

	private static IEnumerable<int> ToEnumerable(params int[] items)
	{
		foreach (int item in items)
		{
			yield return item;
		}
	}

	/// <summary>
	///     Fails for the <c>failingValue</c>, and otherwise when it is completed.
	/// </summary>
	private sealed class AwaitingMatcher(int? failingValue, string? failure = "", bool isAwaited = true)
		: ICollectionMatcher<int, int>
	{
		public List<int> VerifiedValues { get; } = [];

		public bool IsDetermined => false;

		public ValueTask<(bool IsFailure, string? Failure)> Verify(string it, int value,
			IOptionsEquality<int> options, int maximumNumber)
		{
			VerifiedValues.Add(value);
			(bool, string?) result = value == failingValue
				? (true, failure == "" ? $"{it} had {value}" : failure)
				: (false, null);
			return isAwaited ? Await(result) : new ValueTask<(bool, string?)>(result);
		}

		public ValueTask<(bool IsFailure, string? Failure)> VerifyComplete(string it, IOptionsEquality<int> options,
			int maximumNumber)
			=> Await((true, $"{it} was completed"));

		private static async ValueTask<(bool, string?)> Await((bool, string?) result)
		{
			await Task.Yield();
			return result;
		}
	}

	private sealed class UndecidedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<int>(it, grammars), IAsyncConstraint<int>
	{
		public ValueTask<ConstraintResult> IsMetBy(int actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = cancellationToken.IsCancellationRequested ? Outcome.Undecided : Outcome.Success;
			return new ValueTask<ConstraintResult>(this);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is decided");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was undecided");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not decided");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was decided");
	}
}
