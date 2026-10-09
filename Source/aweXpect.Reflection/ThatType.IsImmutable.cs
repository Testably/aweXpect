using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> IsImmutable(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsImmutableConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> IsNotImmutable(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsImmutableConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsImmutableConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual.IsImmutable() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is immutable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was mutable ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not immutable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was immutable ");
			Formatter.Format(stringBuilder, Actual);
		}

		/// <inheritdoc />
		/// <remarks>
		///     The mutable members are only needed for the failure message, so they are collected here instead of on
		///     every (typically succeeding) evaluation.
		/// </remarks>
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Outcome == Outcome.Failure && !IsNegated)
			{
				contexts.AddMembersContext("Mutable members", Actual!.GetMutableMembers());
			}
		}
	}
}
