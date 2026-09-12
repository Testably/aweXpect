using System;
using System.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatObject
{
	/// <summary>
	///     Verifies that the subject is of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T?, IThat<T?>> Is<T>(
		this IThat<T?> source,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		return new AndOrResult<T?, IThat<T?>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, type)),
			source);
	}

	/// <summary>
	///     Verifies that the subject is not of type <paramref name="type" />.
	/// </summary>
	public static AndOrResult<T?, IThat<T?>> IsNot<T>(
		this IThat<T?> source,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		return new AndOrResult<T?, IThat<T?>>(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, type).Invert()),
			source);
	}

	private sealed class IsOfTypeConstraint(string it, ExpectationGrammars grammars, Type type)
		: ConstraintResult.WithValue<object>(grammars),
			IValueConstraint<object?>
	{
		public ConstraintResult IsMetBy(object? actual)
		{
			Actual = actual;
			Outcome = IsOrImplements(type, actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		private static bool IsOrImplements(Type type, object? actual)
		{
			if (type.IsInstanceOfType(actual))
			{
				return true;
			}

			Type? actualType = actual?.GetType();
			if (type.IsGenericTypeDefinition && actualType?.IsGenericType == true)
			{
				Type actualGenericType = actualType.GetGenericTypeDefinition();
				if (!type.IsInterface)
				{
					return type.IsAssignableFrom(actualGenericType);
				}

				Type[] interfaces = actualGenericType.GetInterfaces();
				return interfaces
					.Any(childInterface =>
					{
						Type currentInterface = childInterface.IsGenericType
							? childInterface.GetGenericTypeDefinition()
							: childInterface;

						return currentInterface == type;
					});
			}

			return false;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is type ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" was ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.Indented(indentation, true));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not type ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
