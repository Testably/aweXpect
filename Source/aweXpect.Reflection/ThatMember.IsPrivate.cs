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
	///     Verifies that the <typeparamref name="TMember" /> is private.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsPrivate<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsPrivateConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> is not private.
	/// </summary>
	public static AndOrResult<TMember, IThat<TMember>> IsNotPrivate<TMember>(
		this IThat<TMember> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsPrivateConstraint<TMember>(it, grammars).Invert()),
			subject);

	private sealed class IsPrivateConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<TMember>(it, grammars),
			IValueConstraint<TMember>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(TMember actual)
		{
			Actual = actual;
			Outcome = actual.HasAccessModifier(AccessModifiers.Private) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is private");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was ").Append(Actual.GetAccessModifier().GetString());

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not private");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was");
	}
}
