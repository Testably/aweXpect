using System.Collections.Generic;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Collections;

public sealed class QuantifiedCollectionConstraintBaseTests
{
	[Test]
	public async Task IsMetBy_WhenTheEvaluationIsNotCompleted_ShouldFail()
	{
		int[] subject = [2, 4,];
		IEnumerableElements<int> elements = That(subject).All();

		async Task Act()
			=> await new AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>>(
				((IExpectThat<IEnumerable<int>?>)elements.Subject).ExpectationBuilder.AddConstraint((it, grammars)
					=> new DoesNotCompleteConstraint(it, grammars, elements.Quantifier)),
				elements.Subject);

		await That(Act).ThrowsExactly<FailException>()
			.WithMessage("""
			             Expected that subject
			             is even for all items,
			             but it could not be verified, because the expectation did not decide its outcome
			             """)
			.Because("a constraint that forgets to complete the evaluation must not pass as inconclusive");
	}

	[Test]
	public async Task Record_WhenItemIsUnansweredAfterTheOutcomeIsDetermined_ShouldNotCountTheRemainingItems()
	{
		InvalidOperationException exception = new("boom");
		int[] subject = [1, 1, 2, 1,];

		async Task Act()
			=> await That(subject).AtMost(1)
				.AreVerifiedBy(x => x.Satisfies(y => y == 1 ? true : throw exception), false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             satisfies y => y == 1 ? true : throw exception for at most one item,
			             but at least 2 of at least 2 did

			             Matching items:
			             [1, 1, (… and maybe more)]
			             """)
			.Because("the evaluation stops at the unanswered item, so the number of items is not known");
	}

	[Test]
	[Arguments(true)]
	[Arguments(false)]
	public async Task Record_WhenItemIsUnansweredAfterTheOutcomeIsDetermined_ShouldNotDecideIt(bool completesEarly)
	{
		InvalidOperationException exception = new("boom");
		int[] subject = [1, 2,];

		async Task Act()
			=> await That(subject).AtLeast(1)
				.AreVerifiedBy(x => x.Satisfies(y => y == 1 ? true : throw exception), completesEarly);

		await That(Act).DoesNotThrow()
			.Because("the first item already determines the outcome, whether the remaining items are read or not");
	}

	[Test]
	public async Task Record_WhenItemIsUnansweredForAll_ShouldFailWithTheItemResult()
	{
		InvalidOperationException exception = new("boom");
		int[] subject = [1, 2, 3,];

		async Task Act()
			=> await That(subject).All().AreVerifiedBy(x => x.Satisfies(y => y < 2 ? true : throw exception));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             satisfies y => y < 2 ? true : throw exception for all items,
			             but for the item at index 1, the predicate did throw an InvalidOperationException:
			               boom
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception))
			.Because("an item that the nested expectations did not answer is neither matching nor not matching");
	}

	[Test]
	public async Task Record_WhenItemIsUnansweredForNone_ShouldFailWithTheItemResult()
	{
		InvalidOperationException exception = new("boom");
		int[] subject = [1, 2,];

		async Task Act()
			=> await That(subject).None().AreVerifiedBy(x => x.Satisfies(_ => throw exception));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             satisfies _ => throw exception for no items,
			             but for the item at index 0, the predicate did throw an InvalidOperationException:
			               boom
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception))
			.Because("an item that the nested expectations did not answer must not count as not matching");
	}

	[Test]
	public async Task Record_WhenItemIsUnansweredUnderNegation_ShouldFailWithTheItemResult()
	{
		InvalidOperationException exception = new("boom");
		int[] subject = [1, 2,];

		async Task Act()
			=> await That(subject)
				.DoesNotComplyWith(it => it.All().AreVerifiedBy(x => x.Satisfies(_ => throw exception)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             satisfies _ => throw exception not for all items,
			             but for the item at index 0, the predicate did throw an InvalidOperationException:
			               boom
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception))
			.Because("an item that the nested expectations did not answer fails the negation as well");
	}

	[Test]
	public async Task Record_WhenItemIsUndecided_ShouldNotDecideTheOutcome()
	{
		int[] subject = [1, 2,];
		using CancellationTokenSource cts = new();
		cts.Cancel();

		async Task Act()
			=> await That(subject).None().AreVerifiedBy(x => x.IsEqualTo(3), undecidedItem: 2)
				.WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 3 for no items,
			             but it could not be verified, because the evaluation was already canceled
			             """)
			.Because("a canceled item must not count as not matching, which would let the expectation succeed");
	}

	[Test]
	public async Task Record_WhenUnansweredItemHasContexts_ShouldShowThemForTheItem()
	{
		InvalidOperationException exception = new("boom");
		int[][] subject = [[1,], [2, 3,],];

		async Task Act()
			=> await That(subject).All()
				.AreVerifiedBy(x => x.None().ComplyWith(y => y.Satisfies(z => z < 3 ? false : throw exception)));

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             satisfies z => z < 3 ? false : throw exception for no items for all items,
			             but for the item at index 1, for the item at index 1, the predicate did throw an InvalidOperationException:
			               boom

			             Collection (item [1]):
			             [2, 3]
			             """).And
			.Whose(e => e.InnerException, i => i.IsSameAs(exception));
	}

	[Test]
	public async Task StartEvaluation_AfterACompletedEvaluation_ShouldNotShowItsItems()
	{
		AreEvenConstraint sut = new(EnumerableQuantifier.All());
		sut.IsMetBy([1, 3,]);

		ConstraintResult result = sut.IsMetBy(null);

		await That(ResultContextCollector.Capture(result)).IsNull()
			.Because("the items of the earlier evaluation do not describe the null subject");
	}

	[Test]
	public async Task StartEvaluation_AfterAnEvaluationThatStoppedEarly_ShouldNotCountItsItems()
	{
		AreEvenConstraint sut = new(EnumerableQuantifier.Exactly(2));
		sut.IsMetBy([2, 4, -1,]);

		ConstraintResult result = sut.IsMetBy([2, 4,]);

		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the items that the earlier evaluation recorded before it stopped do not count");
	}

	private sealed class AreEvenConstraint(EnumerableQuantifier quantifier)
		: QuantifiedCollectionConstraint<int[]?, int>("it", ExpectationGrammars.None, quantifier,
			_ => "is even", "were")
	{
		public AreEvenConstraint IsMetBy(int[]? actual)
		{
			StartEvaluation();
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			foreach (int item in actual)
			{
				if (item < 0)
				{
					Outcome = Outcome.FailureBothWays;
					return this;
				}

				Record(item, item % 2 == 0);
			}

			Complete();
			return this;
		}
	}

	private sealed class DoesNotCompleteConstraint(string it, ExpectationGrammars grammars, EnumerableQuantifier quantifier)
		: QuantifiedCollectionConstraint<IEnumerable<int>?, int>(it, grammars, quantifier, _ => "is even", "were"),
			IValueConstraint<IEnumerable<int>?>
	{
		public ConstraintResult IsMetBy(IEnumerable<int>? actual)
		{
			StartEvaluation();
			Actual = actual;
			foreach (int item in actual ?? [])
			{
				Record(item, item % 2 == 0);
			}

			return this;
		}
	}
}

internal static class AreVerifiedByExtensions
{
	/// <remarks>
	///     The <paramref name="undecidedItem" /> gets an undecided item result instead of being verified, as by a
	///     canceled evaluation.
	/// </remarks>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreVerifiedBy<TItem>(
		this IEnumerableElements<TItem> elements, Action<IThat<TItem>> expectations, bool completesEarly = true,
		TItem? undecidedItem = default)
		=> new(((IExpectThat<IEnumerable<TItem>?>)elements.Subject).ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreVerifiedByConstraint<TItem>(it, grammars, elements.Quantifier, expectations, completesEarly,
					undecidedItem)),
			elements.Subject);

	private sealed class AreVerifiedByConstraint<TItem>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Action<IThat<TItem>> expectations,
		bool completesEarly,
		TItem? undecidedItem)
		: QuantifiedCollectionConstraintBase<IEnumerable<TItem>?, TItem>(it, grammars, quantifier),
			IAsyncConstraint<IEnumerable<TItem>?>
	{
		private readonly ManualExpectationBuilder<TItem> _builder = Create(expectations);

		protected override string Verb => _builder.GetResultVerb();

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual,
			CancellationToken cancellationToken)
		{
			StartEvaluation();
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			foreach (TItem item in actual)
			{
				Record(item, EqualityComparer<TItem?>.Default.Equals(item, undecidedItem)
					? new DummyConstraintResult(Outcome.Undecided)
					: await _builder.IsMetBy(item, cancellationToken));
				if (completesEarly && IsDetermined)
				{
					CompleteEarly();
					return this;
				}
			}

			Complete();
			return this;
		}

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> _builder.AppendExpectation(stringBuilder, indentation);

		private static ManualExpectationBuilder<TItem> Create(Action<IThat<TItem>> expectations)
		{
			ManualExpectationBuilder<TItem> builder = new();
			expectations.Invoke(new ThatSubject<TItem>(builder));
			return builder;
		}
	}
}
