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
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are sealed.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreSealed(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreSealedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not sealed.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreNotSealed(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotSealedConstraint(it, grammars)),
			subject);

	private sealed class AreSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(property => property.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-sealed properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => !property.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all sealed");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained sealed properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => property.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<PropertyInfo?>>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(property => !property.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained sealed properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => property.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a sealed property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-sealed properties ");
			Formatter.Format(stringBuilder, Actual?.Where(property => !property.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}
}
