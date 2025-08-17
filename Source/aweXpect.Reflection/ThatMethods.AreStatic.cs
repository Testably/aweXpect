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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are static.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> AreStatic(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are not static.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> AreNotStatic(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<MethodInfo?>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(method => method?.IsStatic == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-static methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsStatic == false),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained static methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsStatic == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<MethodInfo?>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(method => method?.IsStatic == false) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained static methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsStatic == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a static method");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-static methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsStatic == false),
				FormattingOptions.Indented(indentation));
		}
	}
}
