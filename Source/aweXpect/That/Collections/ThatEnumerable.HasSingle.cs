using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	public static SingleItemResult<IEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IEnumerable<TItem>?> source)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasSingleConstraint<TItem>(expectationBuilder, it, grammars, options)),
			options,
			f => f.FirstOrDefault(item => options.Matches(item))
		);
	}

	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static SingleItemResult<IEnumerable, object?> HasSingle(
		this IThat<IEnumerable> source)
	{
		PredicateOptions<object?> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new SingleItemResult<IEnumerable, object?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasSingleForEnumerableConstraint<IEnumerable, object?>(expectationBuilder, it, grammars,
					options)),
			options,
			f =>
			{
				return f.Cast<object?>().FirstOrDefault(item => options.Matches(item));
			});
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	public static SingleItemResult<ImmutableArray<TItem>, TItem> HasSingle<TItem>(
		this IThat<ImmutableArray<TItem>> source)
	{
		PredicateOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new SingleItemResult<ImmutableArray<TItem>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasSingleForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					options)),
			options,
			f =>
			{
				return f.FirstOrDefault(item => options.Matches(item));
			});
	}
#endif

	private sealed class HasSingleConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		PredicateOptions<TItem> options)
		: ConstraintResult.WithValue<TItem?>(grammars),
			IContextConstraint<IEnumerable<TItem>?>
	{
		private IEnumerable<TItem>? _actual;
		private int _count;
		private bool _isEmpty;

		public ConstraintResult IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_count = 0;
			_isEmpty = true;

			foreach (TItem item in materialized)
			{
				_isEmpty = false;
				if (!options.Matches(item))
				{
					continue;
				}

				Actual = item;
				if (++_count > 1)
				{
					break;
				}
			}

			Outcome = _count == 1 ? Outcome.Success : Outcome.Failure;
			if (_count > 1)
			{
				expectationBuilder.AddCollectionContext(materialized);
			}

			return this;
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			return base.TryGetValue(out value);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a single item", "have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_count == 0)
			{
				stringBuilder.Append(it).Append(_isEmpty
					? Grammars.SubjectVerb(it, " was empty", " were empty")
					: " did not contain any matching item");
			}
			else
			{
				stringBuilder.Append(it).Append(" contained more than one item");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a single item", "do not have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}

	private sealed class HasSingleForEnumerableConstraint<TEnumerable, TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		PredicateOptions<TItem> options)
		: ConstraintResult.WithValue<TItem>(grammars),
			IContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private TEnumerable? _actual;
		private int _count;
		private bool _isEmpty;

		public ConstraintResult IsMetBy(TEnumerable actual, IEvaluationContext context)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			_count = 0;
			_isEmpty = true;

			foreach (TItem item in materialized.Cast<TItem>())
			{
				_isEmpty = false;
				if (!options.Matches(item))
				{
					continue;
				}

				Actual = item;
				if (++_count > 1)
				{
					break;
				}
			}

			Outcome = _count == 1 ? Outcome.Success : Outcome.Failure;
			if (_count > 1)
			{
				expectationBuilder.AddCollectionContext(materialized);
			}

			return this;
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (typeof(TValue) != typeof(object) &&
			    _actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			return base.TryGetValue(out value);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a single item", "have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_count == 0)
			{
				stringBuilder.Append(it).Append(_isEmpty
					? Grammars.SubjectVerb(it, " was empty", " were empty")
					: " did not contain any matching item");
			}
			else
			{
				stringBuilder.Append(it).Append(" contained more than one item");
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a single item", "do not have a single item"))
				.Append(options.GetDescription());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				stringBuilder.Append(it).Append(" did");
			}
		}
	}
}
