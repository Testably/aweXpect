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
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		EndsWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected.ToArray(),
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		EndsWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			params TItem[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		EndsWith(
			this IThat<IEnumerable<string?>?> source,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected.ToArray(),
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		EndsWith(
			this IThat<IEnumerable<string?>?> source,
			params string[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<string, string>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		EndsWith<TItem>(
			this IThat<IEnumerable> source,
			IEnumerable<TItem> expected)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected.ToArray(),
					options)),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		EndsWith<TItem>(
			this IThat<IEnumerable> source,
			params TItem[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		EndsWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected.ToArray(),
					options)),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		EndsWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			params TItem[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		EndsWith(
			this IThat<ImmutableArray<string?>> source,
			IEnumerable<string?> expected)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<string?>, string?>(expectationBuilder, it,
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
	///     Verifies that the collection ends with the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		EndsWith(
			this IThat<ImmutableArray<string?>> source,
			params string[] expected)
	{
		_ = expected ?? throw new ArgumentNullException(nameof(expected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<string?>, string>(expectationBuilder, it,
					grammars,
					Formatter.Format(expected),
					expected,
					options)),
			source,
			options);
	}
#endif

	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotEndWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		DoesNotEndWith<TItem>(
			this IThat<IEnumerable<TItem>?> source,
			params TItem[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		DoesNotEndWith(
			this IThat<IEnumerable<string?>?> source,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<string?, string?>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		DoesNotEndWith(
			this IThat<IEnumerable<string?>?> source,
			params string[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithConstraint<string, string>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		DoesNotEndWith<TItem>(
			this IThat<IEnumerable> source,
			IEnumerable<TItem> unexpected)
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>
		DoesNotEndWith<TItem>(
			this IThat<IEnumerable> source,
			params TItem[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<IEnumerable, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		DoesNotEndWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected.ToArray(),
					options).Invert()),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>
		DoesNotEndWith<TItem>(
			this IThat<ImmutableArray<TItem>> source,
			params TItem[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectEqualityResult<ImmutableArray<TItem>, IThat<ImmutableArray<TItem>>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<TItem>, TItem>(expectationBuilder, it, grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		DoesNotEndWith(
			this IThat<ImmutableArray<string?>> source,
			IEnumerable<string?> unexpected)
	{
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<string?>, string?>(expectationBuilder, it,
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
	///     Verifies that the collection does not end with the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>
		DoesNotEndWith(
			this IThat<ImmutableArray<string?>> source,
			params string[] unexpected)
	{
		_ = unexpected ?? throw new ArgumentNullException(nameof(unexpected));
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityResult<ImmutableArray<string?>, IThat<ImmutableArray<string?>>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new EndsWithForEnumerableConstraint<ImmutableArray<string?>, string>(expectationBuilder, it,
					grammars,
					Formatter.Format(unexpected),
					unexpected,
					options).Invert()),
			source,
			options);
	}
#endif

	private sealed class EndsWithConstraint<TItem, TMatch>
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
		private int _itemsCount;
		private int _offset;

		public EndsWithConstraint(
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
			List<TItem> items = materializedEnumerable.ToList();

			_itemsCount = items.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					_expectationBuilder.AddCollectionContext(materializedEnumerable);
					return this;
				}

				TItem item = items[_index + _offset];
				TItem expectedItem = _expected[_index];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					Outcome = Outcome.Failure;
					return this;
				}
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("ends with ", "end with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
				Formatter.Format(stringBuilder, _expected[_index]);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").Append(_itemsCount).Append(" items and misses ")
					.Append(_expected.Length - _itemsCount).Append(" items: ");
				Formatter.Format(stringBuilder, _expected.Take(-_offset), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not end with ", "do not end with ")).Append(_expectedExpression);
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
				stringBuilder.Append(_it).Append(" did end with ");
				Formatter.Format(stringBuilder, Actual?.Skip(_offset), FormattingOptions.MultipleLines);
			}
		}
	}

	private sealed class EndsWithForEnumerableConstraint<TEnumerable, TMatch>
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
		private int _itemsCount;
		private int _offset;

		public EndsWithForEnumerableConstraint(
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
			List<object?> items = materializedEnumerable.Cast<object?>().ToList();

			_itemsCount = items.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					_expectationBuilder.AddCollectionContext(materializedEnumerable);
					return this;
				}

				object? item = items[_index + _offset];
				TMatch expectedItem = _expected[_index];
				if (item is not TMatch matchedItem || !await _options.AreConsideredEqual(matchedItem, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					Outcome = Outcome.Failure;
					return this;
				}
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("ends with ", "end with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
				Formatter.Format(stringBuilder, _expected[_index]);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").Append(_itemsCount).Append(" items and misses ")
					.Append(_expected.Length - _itemsCount).Append(" items: ");
				Formatter.Format(stringBuilder, _expected.Take(-_offset), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not end with ", "do not end with ")).Append(_expectedExpression);
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
				stringBuilder.Append(_it).Append(" did end with ");
				Formatter.Format(stringBuilder, Actual?.Cast<object?>().Skip(_offset), FormattingOptions.MultipleLines);
			}
		}
	}
}
