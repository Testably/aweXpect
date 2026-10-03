using System.Collections;
using System.Collections.Generic;
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
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> IsEmpty<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<IEnumerable<TItem>?, TItem>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the collection is empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnumerable, IThat<TEnumerable?>> IsEmpty<TEnumerable>(
		this IThat<TEnumerable?> subject)
		where TEnumerable : IEnumerable
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<TEnumerable, object?>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> IsNotEmpty<TItem>(
		this IThat<IEnumerable<TItem>?> subject)
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<IEnumerable<TItem>?, TItem>(it, grammars).Invert()),
			subject);

	/// <summary>
	///     Verifies that the collection is not empty.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TEnumerable, IThat<TEnumerable?>> IsNotEmpty<TEnumerable>(
		this IThat<TEnumerable?> subject)
		where TEnumerable : IEnumerable
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new IsEmptyConstraint<TEnumerable, object?>(it, grammars).Invert()),
			subject);

	private sealed class IsEmptyConstraint<TEnumerable, TItem>(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private object? _materializedEnumerable;

		public ConstraintResult IsMetBy(TEnumerable actual, IEvaluationContext context)
		{
			Actual = actual;
			_materializedEnumerable = null;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (CollectionItems<TItem>.CountOf(actual) is { } count)
			{
				_materializedEnumerable = actual;
				Outcome = count > 0 ? Outcome.Failure : Outcome.Success;
				return this;
			}

			CollectionItems<TItem> materialized = CollectionItems<TItem>.Materialize(actual, context);
			_materializedEnumerable = materialized.Value;
			using IEnumerator<TItem> enumerator = materialized.Items.GetEnumerator();
			Outcome = enumerator.MoveNext() ? Outcome.Failure : Outcome.Success;
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
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was empty", " were empty"));
	}
}
