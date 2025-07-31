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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are interfaces.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreInterfaces(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreInterfacesConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not interfaces.
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotInterfaces(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new AreNotInterfacesConstraint(it, grammars)),
			subject);

	private sealed class AreInterfacesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsInterface == true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all interfaces");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained other types ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsInterface != true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all interfaces");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained interfaces ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsInterface == true),
				FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotInterfacesConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IValueConstraint<IEnumerable<Type?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			Actual = actual;
			Outcome = actual.All(type => type?.IsInterface != true) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not interfaces");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained interfaces ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsInterface == true),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an interface");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained not interfaces ");
			Formatter.Format(stringBuilder, Actual?.Where(type => type?.IsInterface != true),
				FormattingOptions.Indented(indentation));
		}
	}
}
