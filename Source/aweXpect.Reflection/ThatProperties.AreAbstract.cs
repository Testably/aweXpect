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
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are abstract.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreAbstract(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreAbstractConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not abstract.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreNotAbstract(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotAbstractConstraint(it, grammars)),
			subject);

	private sealed class AreAbstractConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(property => property.IsReallyAbstract()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all abstract");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-abstract properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => !property.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all abstract");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained abstract properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => property.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotAbstractConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(property => !property.IsReallyAbstract()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not abstract");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained abstract properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => property.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an abstract property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-abstract properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => !property.IsReallyAbstract()),
				FormattingOptions.Indented(indentation));
		}
	}
}
