using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> is private protected.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsPrivateProtected<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsPrivateProtectedConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> is not private protected.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsNotPrivateProtected<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsPrivateProtectedConstraint<TMember>(it, grammars).Invert()),
			subject);

	private sealed class IsPrivateProtectedConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TMember>(it, grammars),
			IValueConstraint<TMember>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(TMember actual)
		{
			Actual = actual;
			Outcome = actual.HasAccessModifier(AccessModifiers.PrivateProtected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is private protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was ").Append(Actual.GetAccessModifier().GetString());

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not private protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was");
	}
}
