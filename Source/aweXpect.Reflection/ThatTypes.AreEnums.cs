using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are enums.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreEnums(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreEnumsConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not enums.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotEnums(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotEnumsConstraint(it, grammars)),
			subject);

	private sealed class AreEnumsConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsEnum == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all enums");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained other types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsEnum != true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all enums");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained enums ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsEnum == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotEnumsConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsEnum != true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not enums");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained enums ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsEnum == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an enum");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained not enums ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsEnum != true),
				FormattingOptions.Indented(indentation));
		}
	}
}
