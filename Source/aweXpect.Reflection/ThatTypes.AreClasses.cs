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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are classes.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreClasses(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreClassesConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not classes.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotClasses(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotClassesConstraint(it, grammars)),
			subject);

	private sealed class AreClassesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type.IsReallyClass()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all classes");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained other types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.IsReallyClass()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all classes");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained classes ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.IsReallyClass()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotClassesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsClass != true || type.IsRecordClass()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not classes");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained classes ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.IsReallyClass()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a class");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained not classes ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.IsReallyClass()),
				FormattingOptions.Indented(indentation));
		}
	}
}
