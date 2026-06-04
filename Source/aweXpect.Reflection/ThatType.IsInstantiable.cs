using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is instantiable.
	/// </summary>
	/// <remarks>
	///     Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> IsInstantiable(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsInstantiableConstraint(it, grammars)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not instantiable.
	/// </summary>
	/// <remarks>
	///     Abstract types, static types, interfaces and open generic type definitions are not considered instantiable.
	/// </remarks>
	public static AndOrResult<Type?, IThat<Type?>> IsNotInstantiable(
		this IThat<Type?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsInstantiableConstraint(it, grammars).Invert()),
			subject);

	private sealed class IsInstantiableConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual.IsReallyInstantiable() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is instantiable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ").Append(GetNonInstantiableDescription(Actual!)).Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not instantiable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was instantiable ");
			Formatter.Format(stringBuilder, Actual);
		}

		private static string GetNonInstantiableDescription(Type type)
		{
			if (type.IsInterface)
			{
				return "an interface";
			}

			if (type.IsReallyStatic())
			{
				return "static";
			}

			if (type.IsReallyAbstract())
			{
				return "abstract";
			}

			return "a generic type definition";
		}
	}
}
