using System;
using System.Collections;
using aweXpect.Core;
using aweXpect.Core.Constraints;
#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
#endif

namespace aweXpect.Helpers;

internal static class ImmutableArrayHelpers
{
	/// <summary>
	///     Checks if the <paramref name="value" /> is a default <c>ImmutableArray&lt;T&gt;</c>.
	/// </summary>
	/// <remarks>
	///     A default <c>ImmutableArray&lt;T&gt;</c> is the struct's version of a <see langword="null" /> collection, as
	///     almost every member of it throws. It is also recognized when it is boxed, e.g. as an
	///     <see cref="IEnumerable" />.
	/// </remarks>
	public static bool IsDefaultImmutableArray<T>(this T value)
#if NET8_0_OR_GREATER
		=> IsImmutableArray<T>.Value
			? EqualityComparer<T>.Default.Equals(value, default!)
			// A value of any other struct type is not boxed for the check.
			: default(T) is null && IsBoxedDefaultImmutableArray(value);
#else
		=> IsBoxedDefaultImmutableArray(value);
#endif

	/// <summary>
	///     Returns <see langword="null" /> for a default <c>ImmutableArray&lt;T&gt;</c>, and the
	///     <paramref name="value" /> otherwise.
	/// </summary>
	public static T? NullIfDefaultImmutableArray<T>(this T? value)
		where T : class
		=> value.IsDefaultImmutableArray() ? null : value;

	/// <summary>
	///     Returns a result that renders the subject of the <paramref name="constraint" /> as <see langword="null" />,
	///     with the <paramref name="outcome" /> of the expectation that is not negated.
	/// </summary>
	/// <remarks>
	///     By default it fails the expectation and its negation alike, like a <see langword="null" /> subject of an
	///     expectation that inspects the items.
	/// </remarks>
	public static ConstraintResult AsNullSubject(this ConstraintResult constraint, string it,
		Outcome outcome = Outcome.FailureBothWays)
		=> new NullSubjectResult(constraint, it, outcome);

	private static bool IsBoxedDefaultImmutableArray(object? value)
		=> value is ValueType and ICollection collection && IsImmutableArrayType(value.GetType()) &&
		   IsDefault(collection);

	private static bool IsImmutableArrayType(Type type)
#if NET8_0_OR_GREATER
		=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);
#else
		=> type.IsGenericType &&
		   type.GetGenericTypeDefinition().FullName == "System.Collections.Immutable.ImmutableArray`1";
#endif

	/// <summary>
	///     The count is the cheapest member that tells a default <c>ImmutableArray&lt;T&gt;</c> apart without knowing
	///     its item type: it throws only for a default one.
	/// </summary>
	private static bool IsDefault(ICollection immutableArray)
	{
		try
		{
			_ = immutableArray.Count;
			return false;
		}
		catch (InvalidOperationException)
		{
			return true;
		}
	}

#if NET8_0_OR_GREATER
	private static class IsImmutableArray<T>
	{
		public static readonly bool Value = IsImmutableArrayType(typeof(T));
	}
#endif

	private sealed class NullSubjectResult : ConstraintResult
	{
		private readonly ConstraintResult _inner;
		private readonly string _it;
		private readonly Outcome _outcome;

		public NullSubjectResult(ConstraintResult inner, string it, Outcome outcome)
			: base(inner.FurtherProcessingStrategy)
		{
			_inner = inner;
			_it = it;
			_outcome = outcome;
			Grammars = inner.Grammars;
		}

		/// <inheritdoc />
		public override Outcome Outcome
			=> (_outcome, Grammars.IsNegated()) switch
			{
				(Outcome.Failure, true) => Outcome.Success,
				(Outcome.Success, true) => Outcome.Failure,
				(_, _) => _outcome,
			};

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _inner.AppendContexts(contexts);

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_it).Append(Grammars.SubjectVerb(_it, " was", " were")).Append(" <null>");

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> _inner.TryGetStoredValue(out value);

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			Grammars = _inner.Grammars;
			return this;
		}
	}
}
