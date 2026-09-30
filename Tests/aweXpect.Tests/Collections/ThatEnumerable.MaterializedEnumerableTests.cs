using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class MaterializedEnumerableTests
	{
		[Fact]
		public async Task WhenExtensionIsChainedWithBuiltInExpectation_ShouldEnumerateTheSubjectOnlyOnce()
		{
			OneShotEnumerable subject = new(2, 4, 6);

			async Task Act()
				=> await That(subject).Contains(2).And.HasEvenItems(3);

			await That(Act).DoesNotThrow()
				.Because("the extension continues the materialized subject instead of enumerating it again");
			await That(subject.Enumerations).IsEqualTo(1);
		}

		[Fact]
		public async Task WhenExtensionIsChainedWithBuiltInExpectation_ShouldFailWithTheItemsOfTheSubject()
		{
			OneShotEnumerable subject = new(2, 3, 4);

			async Task Act()
				=> await That(subject).Contains(2).And.HasEvenItems(3);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 2 at least once and has 3 even items,
				             but it had 2 even items
				             """)
				.Because("the extension sees all items of the subject, although the source yields them only once");
		}
	}

	private sealed class OneShotEnumerable(params int[] items) : IEnumerable<int>
	{
		public int Enumerations { get; private set; }

		public IEnumerator<int> GetEnumerator()
		{
			Enumerations++;
			return (Enumerations == 1 ? items : []).AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}

internal static class ThatEnumerableOfEvenItems
{
	public static AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>> HasEvenItems(
		this IThat<IEnumerable<int>?> subject, int expected)
		=> new(((IExpectThat<IEnumerable<int>?>)subject).ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasEvenItemsConstraint(it, grammars, expected)),
			subject);

	private sealed class HasEvenItemsConstraint(string it, ExpectationGrammars grammars, int expected)
		: ConstraintResult.WithNotNullValue<IEnumerable<int>>(it, grammars),
			IContextConstraint<IEnumerable<int>?>
	{
		private int _count;

		public ConstraintResult IsMetBy(IEnumerable<int>? actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			_count = context.UseMaterializedEnumerable(actual).Count(item => item % 2 == 0);
			Outcome = _count == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has ").Append(expected).Append(" even items");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had ").Append(_count).Append(" even items");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have ").Append(expected).Append(" even items");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
