using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		StartsWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected.ToArray(),
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		StartsWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			params TItem[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		StartsWith(
			this IThat<IEnumerable<string?>?> source,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected.ToArray(),
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		StartsWith(
			this IThat<IEnumerable<string?>?> source,
			params string[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<string, string>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		StartsWith<TItem>(
			this IThat<IEnumerable> source,
			IEnumerable<TItem> expected)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected.ToArray(),
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		StartsWith<TItem>(
			this IThat<IEnumerable> source,
			params TItem[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		StartsWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected.ToArray(),
					options)),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		StartsWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			params TItem[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		StartsWith(
			this IThat<ImmutableArray<string?>> source,
			IEnumerable<string?> expected)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<string?>, string?>(expectationBuilder, it,
					grammars,
					Formatter.Format(expected),
					expected.ToArray(),
					options)),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection starts with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		StartsWith(
			this IThat<ImmutableArray<string?>> source,
			params string[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<string?>, string>(expectationBuilder, it,
					grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}
#endif

	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotStartWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotStartWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			params TItem[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		DoesNotStartWith(
			this IThat<IEnumerable<string?>?> source,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		DoesNotStartWith(
			this IThat<IEnumerable<string?>?> source,
			params string[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithConstraint<string, string>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		DoesNotStartWith<TItem>(
			this IThat<IEnumerable> source,
			IEnumerable<TItem> unexpected)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		DoesNotStartWith<TItem>(
			this IThat<IEnumerable> source,
			params TItem[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		DoesNotStartWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		DoesNotStartWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			params TItem[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		DoesNotStartWith(
			this IThat<ImmutableArray<string?>> source,
			IEnumerable<string?> unexpected)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<string?>, string?>(expectationBuilder, it,
					grammars,
					Formatter.Format(unexpected),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not start with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		DoesNotStartWith(
			this IThat<ImmutableArray<string?>> source,
			params string[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new StartsWithForEnumerableConstraint<ImmutableArray<string?>, string>(expectationBuilder, it,
					grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}
#endif

	private sealed class StartsWithConstraint<TItem, TMatch>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TItem[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private TItem? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;

		public StartsWithConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TItem[] expected,
			IOptionsEquality<TMatch> options) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
		}

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_index = 0;
			foreach (TItem item in materializedEnumerable)
			{
				TItem expectedItem = _expected[_index++];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					Outcome = Outcome.Failure;
					return this;
				}

				if (_expected.Length == _index)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			_expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("starts with ", "start with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index - 1).Append(" instead of ");
				Formatter.Format(stringBuilder, _expected[_index - 1]);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").Append(_index).Append(" items and misses ")
					.Append(_expected.Length - _index).Append(" items: ");
				Formatter.Format(stringBuilder, _expected.Skip(_index), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not start with ", "do not start with "))
				.Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_expected.Length == 0)
			{
				stringBuilder.Append(_it).Append(Grammars.SubjectVerb(_it, " was ", " were "));
				Formatter.Format(stringBuilder, Actual, FormattingOptions.MultipleLines);
			}
			else
			{
				stringBuilder.Append(_it).Append(" did start with ");
				Formatter.Format(stringBuilder, Actual?.Take(_index), FormattingOptions.MultipleLines);
			}
		}
	}

	private sealed class StartsWithForEnumerableConstraint<TEnumerable, TMatch>
		: ConstraintResult.WithNotNullValue<TEnumerable?>,
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TMatch[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private object? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;

		public StartsWithForEnumerableConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TMatch[] expected,
			IOptionsEquality<TMatch> options) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
		}

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_index = 0;
			foreach (object? item in materializedEnumerable)
			{
				object? expectedItem = _expected[_index++];
				if (item is not TMatch matchedItem || !await _options.AreConsideredEqual(matchedItem, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					Outcome = Outcome.Failure;
					return this;
				}

				if (_expected.Length == _index)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			_expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("starts with ", "start with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index - 1).Append(" instead of ");
				Formatter.Format(stringBuilder, _expected[_index - 1]);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").Append(_index).Append(" items and misses ")
					.Append(_expected.Length - _index).Append(" items: ");
				Formatter.Format(stringBuilder, _expected.Skip(_index), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not start with ", "do not start with "))
				.Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_expected.Length == 0)
			{
				stringBuilder.Append(_it).Append(Grammars.SubjectVerb(_it, " was ", " were "));
				Formatter.Format(stringBuilder, Actual, FormattingOptions.MultipleLines);
			}
			else
			{
				stringBuilder.Append(_it).Append(" did start with ");
				Formatter.Format(stringBuilder, Actual?.Cast<object?>().Take(_index), FormattingOptions.MultipleLines);
			}
		}
	}
}
