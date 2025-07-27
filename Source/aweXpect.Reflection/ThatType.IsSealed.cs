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
	///     Verifies that the <see cref="Type" /> is sealed.
	/// </summary>
	/// <remarks>
	///     Static types are not considered sealed, even though they
	///     have <see cref="Type.IsSealed" /> set to <see langword="true" />.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> IsSealed(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsSealedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not sealed.
	/// </summary>
	/// <remarks>
	///     Static types are considered not sealed, even though they
	///     have <see cref="Type.IsSealed" /> set to <see langword="true" />.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> IsNotSealed(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsSealedConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.IsReallySealed() == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was non-sealed ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not sealed");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was sealed ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
