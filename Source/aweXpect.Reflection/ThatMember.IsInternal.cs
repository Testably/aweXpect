using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> is internal.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsInternal<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsInternalConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> is not internal.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsNotInternal<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsInternalConstraint<TMember>(it, grammars).Invert()),
			subject);

	private sealed class IsInternalConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TMember>(it, grammars),
			IValueConstraint<TMember>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(TMember actual)
		{
			Actual = actual;
			Outcome = actual.HasAccessModifier(AccessModifiers.Internal) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is internal");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was ").Append(Actual.GetAccessModifier().GetString());

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not internal");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was");
	}
}
