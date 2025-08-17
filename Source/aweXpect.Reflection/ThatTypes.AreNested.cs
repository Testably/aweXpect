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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are nested.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNested(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNestedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not nested.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotNested(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotNestedConstraint(it, grammars)),
			subject);

	private sealed class AreNestedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsNested == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all nested");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-nested types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsNested != true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all nested");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained nested types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsNested == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotNestedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsNested != true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not nested");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained nested types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsNested == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a nested type");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-nested types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsNested != true),
				FormattingOptions.Indented(indentation));
		}
	}
}
