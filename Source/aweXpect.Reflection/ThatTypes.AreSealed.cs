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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are sealed.
	/// </summary>
	/// <remarks>
	///     Static types are not considered sealed, even though they
	///     have <see cref="Type.IsSealed" /> set to <see langword="true" />.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type>, IThat<IEnumerable<Type>>> AreSealed(
		this IThat<IEnumerable<Type>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreSealedConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not sealed.
	/// </summary>
	/// <remarks>
	///     Static types are considered not sealed, even though they
	///     have <see cref="Type.IsSealed" /> set to <see langword="true" />.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type>, IThat<IEnumerable<Type>>> AreNotSealed(
		this IThat<IEnumerable<Type>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotSealedConstraint(it, grammars)),
			subject);

	private sealed class AreSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type>>(grammars),
			IValueConstraint<IEnumerable<Type>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-sealed types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all sealed");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained sealed types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotSealedConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type>>(grammars),
			IValueConstraint<IEnumerable<Type>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => !type.IsReallySealed()) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not sealed");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained sealed types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an sealed type");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-sealed types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => !type.IsReallySealed()),
				FormattingOptions.Indented(indentation));
		}
	}
}
