using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are sealed.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> AreSealed(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreSealedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are not sealed.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> AreNotSealed(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotSealedConstraint(it, grammars)),
			subject);

	private sealed class AreSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<MethodInfo?>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(method => method.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-sealed methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => !method.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all sealed");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained sealed methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<MethodInfo?>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(method => !method.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained sealed methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a sealed method");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-sealed methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => !method.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}
}
