using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     A typed <see cref="ConstraintResult" /> which is used to split a collection of <typeparamref name="T" /> in a
///     matching and not matching group.
/// </summary>
internal abstract class CollectionConstraintResult<T>(ExpectationGrammars grammars) : ConstraintResult(grammars)
{
#if NET8_0_OR_GREATER
	private IAsyncEnumerable<T>? _asyncElements;
#endif
	private IEnumerable<T>? _elements;
	private Outcome _outcome = Outcome.Undecided;

	/// <summary>
	///     Flag indicating if the constraint is negated.
	/// </summary>
	protected bool IsNegated { get; private set; }

	/// <inheritdoc />
	public override Outcome Outcome
	{
		get => (_outcome, _isNegated: IsNegated) switch
		{
			(Outcome.Failure, true) => Outcome.Success,
			(Outcome.Success, true) => Outcome.Failure,
			(_, _) => _outcome,
		};
		protected set => _outcome = value;
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     The matching elements after calling <c>SetValue</c> or <c>SetValueAsync</c>.
	/// </summary>
#else
	/// <summary>
	/// The matching elements after calling <c>SetValue</c>.
	/// </summary>
#endif
	protected T[] Matching { get; private set; } = [];

#if NET8_0_OR_GREATER
	/// <summary>
	///     The not matching elements after calling <c>SetValue</c> or <c>SetValueAsync</c>.
	/// </summary>
#else
	/// <summary>
	/// The not matching elements after calling <c>SetValue</c>.
	/// </summary>
#endif
	protected T[] NotMatching { get; private set; } = [];

	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into <see cref="Matching" />
	///     and <see cref="NotMatching" />.
	/// </summary>
	protected ConstraintResult SetValue(IEnumerable<T> elements, Func<T, bool> predicate)
	{
		_elements = elements;
		(Matching, NotMatching) = elements.Split(predicate);
		Outcome = NotMatching.Length == 0 ? Outcome.Success : Outcome.Failure;
		return this;
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into <see cref="Matching" />
	///     and <see cref="NotMatching" />.
	/// </summary>
	protected async Task<ConstraintResult> SetValue(IEnumerable<T> elements, Func<T, ValueTask<bool>> predicate)
	{
		_elements = elements;
		(Matching, NotMatching) = await elements.SplitAsync(predicate);
		Outcome = NotMatching.Length == 0 ? Outcome.Success : Outcome.Failure;
		return this;
	}
#else
	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into <see cref="Matching" />
	///     and <see cref="NotMatching" />.
	/// </summary>
	protected async Task<ConstraintResult> SetValue(IEnumerable<T> elements, Func<T, Task<bool>> predicate)
	{
		_elements = elements;
		(Matching, NotMatching) = await elements.SplitAsync(predicate);
		Outcome = NotMatching.Length == 0 ? Outcome.Success : Outcome.Failure;
		return this;
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into <see cref="Matching" />
	///     and <see cref="NotMatching" />.
	/// </summary>
	protected async Task<ConstraintResult> SetAsyncValue(IAsyncEnumerable<T> elements, Func<T, bool> predicate)
	{
		_asyncElements = elements;
		(Matching, NotMatching) = await elements.SplitAsync(predicate);
		Outcome = NotMatching.Length == 0 ? Outcome.Success : Outcome.Failure;
		return this;
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Splits the <paramref name="elements" /> according to the <paramref name="predicate" /> into <see cref="Matching" />
	///     and <see cref="NotMatching" />.
	/// </summary>
	protected async Task<ConstraintResult> SetAsyncValue(IAsyncEnumerable<T> elements,
		Func<T, ValueTask<bool>> predicate)
	{
		_asyncElements = elements;
		(Matching, NotMatching) = await elements.SplitAsync(predicate);
		Outcome = NotMatching.Length == 0 ? Outcome.Success : Outcome.Failure;
		return this;
	}
#endif

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     not negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are not
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="ExpectationGrammars" /> are
	///     negated.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected abstract void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Appends the result to the <paramref name="stringBuilder" /> when the <see cref="Outcome" />
	///     is <see cref="Outcome.Undecided" />.
	/// </summary>
	protected virtual void AppendUndecidedResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append("could not verify, because it was already cancelled");

	/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (IsNegated)
		{
			AppendNegatedExpectation(stringBuilder, indentation);
		}
		else
		{
			AppendNormalExpectation(stringBuilder, indentation);
		}
	}

	/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (Outcome == Outcome.Undecided)
		{
			AppendUndecidedResult(stringBuilder, indentation);
		}
		else if (IsNegated)
		{
			AppendNegatedResult(stringBuilder, indentation);
		}
		else
		{
			AppendNormalResult(stringBuilder, indentation);
		}
	}

	/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
	public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
	{
		if (_elements is TValue typedValue)
		{
			value = typedValue;
			return true;
		}

#if NET8_0_OR_GREATER
		if (_asyncElements is TValue asyncTypedValue)
		{
			value = asyncTypedValue;
			return true;
		}
#endif

		value = default;
		return typeof(TValue).IsAssignableFrom(typeof(T));
	}

	/// <inheritdoc cref="ConstraintResult.Negate()" />
	public override ConstraintResult Negate()
	{
		IsNegated = !IsNegated;
		return this;
	}
}
