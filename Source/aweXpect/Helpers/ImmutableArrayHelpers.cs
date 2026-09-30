using System.Diagnostics.CodeAnalysis;
using System.Text;
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
	///     almost every member of it throws.
	/// </remarks>
	public static bool IsDefaultImmutableArray<T>(this T value)
#if NET8_0_OR_GREATER
		=> IsImmutableArray<T>.Value && EqualityComparer<T>.Default.Equals(value, default!);
#else
		=> false;
#endif

	/// <summary>
	///     Returns a result that fails the expectation of the <paramref name="constraint" /> and its negation alike,
	///     like a <see langword="null" /> subject.
	/// </summary>
	public static ConstraintResult AsNullSubject(this ConstraintResult constraint, string it)
		=> new NullSubjectResult(constraint, it);

#if NET8_0_OR_GREATER
	private static class IsImmutableArray<T>
	{
		public static readonly bool Value =
			typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(ImmutableArray<>);
	}
#endif

	private sealed class NullSubjectResult : ConstraintResult
	{
		private readonly ConstraintResult _inner;
		private readonly string _it;

		public NullSubjectResult(ConstraintResult inner, string it) : base(inner.FurtherProcessingStrategy)
		{
			_inner = inner;
			_it = it;
			Grammars = inner.Grammars;
			Outcome = Outcome.Failure;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_it).Append(Grammars.SubjectVerb(_it, " was", " were")).Append(" <null>");

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
			=> _inner.TryGetValue(out value);

		public override ConstraintResult Negate()
		{
			_inner.Negate();
			Grammars = _inner.Grammars;
			return this;
		}
	}
}
