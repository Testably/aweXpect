using System.Collections.Generic;
using System.Linq;
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
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are protected internal.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreProtectedInternal<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreProtectedInternalConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not protected internal.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreNotProtectedInternal<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotProtectedInternalConstraint<TMember>(it, grammars)),
			subject);

	private sealed class AreProtectedInternalConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IValueConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
		{
			Actual = actual;
			Outcome = actual.All(member => member.HasAccessModifier(AccessModifiers.ProtectedInternal))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are protected internal");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder,
				Actual?.Where(member => !member.HasAccessModifier(AccessModifiers.ProtectedInternal)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all are protected internal");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were");
	}

	private sealed class AreNotProtectedInternalConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IValueConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
		{
			Actual = actual;
			Outcome = actual.All(member => !member.HasAccessModifier(AccessModifiers.ProtectedInternal))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are not protected internal");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained protected internal items ");
			Formatter.Format(stringBuilder, Actual?.Where(member => member.HasAccessModifier(AccessModifiers.ProtectedInternal)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("at least one is protected internal");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("none were");
	}
}
