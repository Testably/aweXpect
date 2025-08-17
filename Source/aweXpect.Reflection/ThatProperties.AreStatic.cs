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

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are static.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreStatic(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not static.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreNotStatic(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(property => property.IsReallyStatic()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-static properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => !property.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained static properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => property.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(property => !property.IsReallyStatic()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained static properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => property.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a static property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-static properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => !property.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}
	}
}
