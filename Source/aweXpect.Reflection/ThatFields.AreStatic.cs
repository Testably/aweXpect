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

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> are static.
	/// </summary>
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreStatic(
		this IThat<IEnumerable<FieldInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> are not static.
	/// </summary>
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreNotStatic(
		this IThat<IEnumerable<FieldInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<FieldInfo?>>(grammars),
			IValueConstraint<IEnumerable<FieldInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(field => field?.IsStatic == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-static fields ");
			Formatter.Format(stringBuilder, Actual?.Where(field => field?.IsStatic == false),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained static fields ");
			Formatter.Format(stringBuilder, Actual?.Where(field => field?.IsStatic == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<FieldInfo?>>(grammars),
			IValueConstraint<IEnumerable<FieldInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(field => field?.IsStatic == false) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained static fields ");
			Formatter.Format(stringBuilder, Actual?.Where(field => field?.IsStatic == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a static field");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-static fields ");
			Formatter.Format(stringBuilder, Actual?.Where(field => field?.IsStatic == false),
				FormattingOptions.Indented(indentation));
		}
	}
}
