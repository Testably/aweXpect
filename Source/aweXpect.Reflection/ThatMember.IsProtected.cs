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
	///     Verifies that the <typeparamref name="TMember" /> is protected.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsProtected<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsProtectedConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> is not protected.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsNotProtected<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsProtectedConstraint<TMember>(it, grammars).Invert()),
			subject);

	private sealed class IsProtectedConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TMember>(it, grammars),
			IValueConstraint<TMember>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(TMember actual)
		{
			Actual = actual;
			Outcome = actual.HasAccessModifier(AccessModifiers.Protected) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was ").Append(Actual.GetAccessModifier().GetString());

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was");
	}
}
