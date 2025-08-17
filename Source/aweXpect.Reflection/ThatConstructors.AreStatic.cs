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

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> are static.
	/// </summary>
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>>> AreStatic(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreStaticConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> are not static.
	/// </summary>
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>>> AreNotStatic(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotStaticConstraint(it, grammars)),
			subject);

	private sealed class AreStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<ConstructorInfo?>>(grammars),
			IValueConstraint<IEnumerable<ConstructorInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(constructor => constructor?.IsStatic == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-static constructors ");
			Formatter.Format(stringBuilder, Actual?.Where(constructor => constructor?.IsStatic == false),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all static");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained static constructors ");
			Formatter.Format(stringBuilder, Actual?.Where(constructor => constructor?.IsStatic == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStaticConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<ConstructorInfo?>>(grammars),
			IValueConstraint<IEnumerable<ConstructorInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(constructor => constructor?.IsStatic == false) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not static");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained static constructors ");
			Formatter.Format(stringBuilder, Actual?.Where(constructor => constructor?.IsStatic == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a static constructor");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-static constructors ");
			Formatter.Format(stringBuilder, Actual?.Where(constructor => constructor?.IsStatic == false),
				FormattingOptions.Indented(indentation));
		}
	}
}
