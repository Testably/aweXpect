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
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are generic.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> AreGeneric(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreGenericConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> are not generic.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> AreNotGeneric(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotGenericConstraint(it, grammars)),
			subject);

	private sealed class AreGenericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<MethodInfo?>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(method => method?.IsGenericMethod == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all generic");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-generic methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsGenericMethod != true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all generic");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained generic methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsGenericMethod == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotGenericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<MethodInfo?>>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(method => method?.IsGenericMethod != true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not generic");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained generic methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsGenericMethod == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a generic method");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-generic methods ");
			Formatter.Format(stringBuilder, Actual?.Where(method => method?.IsGenericMethod != true),
				FormattingOptions.Indented(indentation));
		}
	}
}
