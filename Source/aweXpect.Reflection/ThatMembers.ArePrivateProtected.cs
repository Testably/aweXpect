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
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are private protected.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> ArePrivateProtected<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new ArePrivateProtectedConstraint<TMember>(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not private protected.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreNotPrivateProtected<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotPrivateProtectedConstraint<TMember>(it, grammars)),
			subject);

	private sealed class ArePrivateProtectedConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IValueConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
		{
			Actual = actual;
			Outcome = actual.All(member => member.HasAccessModifier(AccessModifiers.PrivateProtected))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are private protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder,
				Actual?.Where(member => !member.HasAccessModifier(AccessModifiers.PrivateProtected)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all are private protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were");
	}

	private sealed class AreNotPrivateProtectedConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<TMember>>(grammars),
			IValueConstraint<IEnumerable<TMember>>
		where TMember : MemberInfo?
	{
		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
		{
			Actual = actual;
			Outcome = actual.All(member => !member.HasAccessModifier(AccessModifiers.PrivateProtected))
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are not private protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained private protected items ");
			Formatter.Format(stringBuilder, Actual?.Where(member => member.HasAccessModifier(AccessModifiers.PrivateProtected)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("at least one is private protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("none were");
	}
}
