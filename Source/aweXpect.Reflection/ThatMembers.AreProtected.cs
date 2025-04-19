using System.Collections.Generic;
using System.Linq;
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
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are protected.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreProtected<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreProtectedConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not protected.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreNotProtected<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotProtectedConstraint<TMember>(it, grammars)),
			subject);

	private sealed class AreProtectedConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IValueConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
		{
			Actual = actual;
			Outcome = actual.All(member => member.HasAccessModifier(AccessModifiers.Protected))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder,
				Actual?.Where(member => !member.HasAccessModifier(AccessModifiers.Protected)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all are protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were");
	}

	private sealed class AreNotProtectedConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IValueConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
		{
			Actual = actual;
			Outcome = actual.All(member => !member.HasAccessModifier(AccessModifiers.Protected))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are not protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained protected items ");
			Formatter.Format(stringBuilder,
				Actual?.Where(member => member.HasAccessModifier(AccessModifiers.Protected)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("at least one is protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("none were");
	}
}
