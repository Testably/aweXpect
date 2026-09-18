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
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T?, IThat<T?>>(expectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(expectationBuilder, it, grammars, type)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is not of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<T?, IThat<T?>> IsNot<T>(
		this IThat<T?> subject,
		Type type)
		where T : class
	{
		type.ThrowIfNull();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new AndOrResult<T?, IThat<T?>>(expectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(expectationBuilder, it, grammars, type).Invert()),
			subject);
	}

	private sealed class IsOfTypeConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<object>(it, grammars),
			IValueConstraint<object?>
	{
		public ConstraintResult IsMetBy(object? actual)
		{
			Actual = actual;
			Outcome = IsOrImplements(type, actual) ? Outcome.Success : Outcome.Failure;
			if (Outcome == Outcome.Failure && actual is not null)
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual",
					Formatter.Format(actual, FormattingOptions.MultipleLines)));
			}

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

				if (!ReflectionFallback.IsSupported)
				{
					throw new NotSupportedException(
						$"The interfaces of {Formatter.Format(actualType)} cannot be found by reflection, which is switched off when publishing with trimming or Native AOT enabled. Check against a constructed interface instead of its generic definition. Alternatively, set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.");
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
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual!.GetType());
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
