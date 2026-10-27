using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> has the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TMember, IThat<TMember?>> Is<TMember>(
		this IThat<TMember?> subject,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(subject.Get().ExpectationBuilder.AddConstraint(accessModifier,
				static (accessModifier, it, grammars)
					=> new HasAccessModifierConstraint<TMember?>(it, grammars, accessModifier)),
			subject);

	/// <summary>
	///     Verifies that the <typeparamref name="TMember" /> does not have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by none of them.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TMember, IThat<TMember?>> IsNot<TMember>(
		this IThat<TMember?> subject,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(subject.Get().ExpectationBuilder.AddConstraint(accessModifier,
				static (accessModifier, it, grammars)
					=> new HasAccessModifierConstraint<TMember?>(it, grammars, accessModifier).Invert()),
			subject);

	private sealed class HasAccessModifierConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		AccessModifiers accessModifier)
		: ConstraintResult.WithNotNullValue<TMember>(it, grammars),
			IValueConstraint<TMember>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(TMember actual)
		{
			Actual = actual;
			Outcome = actual.HasAccessModifier(accessModifier) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.AppendExpectation(accessModifier, false, false);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was ").Append(Actual.GetAccessModifier().GetString());

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.AppendExpectation(accessModifier, true, false);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it was ").Append(Actual.GetAccessModifier().GetString());
	}
}
