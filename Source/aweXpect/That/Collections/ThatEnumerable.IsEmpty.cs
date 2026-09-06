using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> IsEmpty<TItem>(
		this IThat<IEnumerable<TItem>?> source)
		=> new(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<TItem>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	public static AndOrResult<TEnumerable, IThat<TEnumerable?>> IsEmpty<TEnumerable>(
		this IThat<TEnumerable?> source)
		where TEnumerable : IEnumerable
		=> new(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyForEnumerableConstraint<TEnumerable>(it, grammars)),
			source);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> IsNotEmpty<TItem>(
		this IThat<IEnumerable<TItem>?> source)
		=> new(source.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<TItem>(it, grammars).Invert()),
			source);

	private sealed class IsEmptyConstraint<TItem>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IContextConstraint<IEnumerable<TItem>?>
	{
		private IEnumerable<TItem>? _materializedEnumerable;

		public ConstraintResult IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (actual is ICollection<TItem> collectionOfT)
			{
				_materializedEnumerable = actual;
				if (collectionOfT.Count > 0)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				Outcome = Outcome.Success;
				return this;
			}

			_materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			using IEnumerator<TItem> enumerator = _materializedEnumerable.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is empty", "are empty"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, _materializedEnumerable, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not empty", "are not empty"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}

	private sealed class IsEmptyForEnumerableConstraint<TEnumerable>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private IEnumerable? _materializedEnumerable;

		public ConstraintResult IsMetBy(TEnumerable actual, IEvaluationContext context)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (actual is ICollection collectionOfT)
			{
				_materializedEnumerable = actual;
				if (collectionOfT.Count > 0)
				{
					Outcome = Outcome.Failure;
					return this;
				}

				Outcome = Outcome.Success;
				return this;
			}

			_materializedEnumerable = context.UseMaterializedEnumerable(actual);
			if (_materializedEnumerable.Cast<object?>().Any())
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is empty", "are empty"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, _materializedEnumerable, FormattingOptions.MultipleLines);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not empty", "are not empty"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}
}
