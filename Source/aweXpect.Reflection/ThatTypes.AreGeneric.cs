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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are generic.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreGeneric(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreGenericConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not generic.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotGeneric(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotGenericConstraint(it, grammars)),
			subject);

	private sealed class AreGenericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsGenericType == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all generic");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-generic types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsGenericType != true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all generic");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained generic types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsGenericType == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotGenericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsGenericType != true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not generic");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained generic types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsGenericType == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a generic type");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-generic types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsGenericType != true),
				FormattingOptions.Indented(indentation));
		}
	}
}
