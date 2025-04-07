using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Extensions;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are static.
	/// </summary>
	public static AndOrResult<IEnumerable<Type>, IThat<IEnumerable<Type>>> AreStatic(
		this IThat<IEnumerable<Type>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not static.
	/// </summary>
	public static AndOrResult<IEnumerable<Type>, IThat<IEnumerable<Type>>> AreNotStatic(
		this IThat<IEnumerable<Type>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type>>(grammars),
			IValueConstraint<IEnumerable<Type>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type.IsReallyStatic()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-static types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained static types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type>>(grammars),
			IValueConstraint<IEnumerable<Type>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => !type.IsReallyStatic()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained static types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an static type");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-static types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.IsReallyStatic()),
				FormattingOptions.Indented(indentation));
		}
	}
}
