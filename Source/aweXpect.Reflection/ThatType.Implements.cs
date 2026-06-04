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
	///     Verifies that the <see cref="Type" /> implements the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored and a type does not
	///     implement itself.<br />
	///     To check for base-class inheritance use <see cref="InheritsFrom{TBaseType}(IThat{Type}, bool)" />,
	///     or use <see cref="IsAssignableTo{TType}(IThat{Type})" /> to cover base classes and interfaces in one step.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TInterface" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TInterface" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> Implements<TInterface>(
		this IThat<Type?> subject,
		bool forceDirect = false)
		=> subject.Implements(typeof(TInterface), forceDirect);

	/// <summary>
	///     Verifies that the <see cref="Type" /> implements the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored and a type does not
	///     implement itself.<br />
	///     To check for base-class inheritance use <see cref="InheritsFrom(IThat{Type}, Type, bool)" />,
	///     or use <see cref="IsAssignableTo(IThat{Type}, Type)" /> to cover base classes and interfaces in one step.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> Implements(
		this IThat<Type?> subject,
		Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new ImplementsConstraint(expectationBuilder, it, grammars, interfaceType, forceDirect)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use <see cref="DoesNotInheritFrom{TBaseType}(IThat{Type}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TInterface" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TInterface" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotImplement<TInterface>(
		this IThat<Type?> subject,
		bool forceDirect = false)
		=> subject.DoesNotImplement(typeof(TInterface), forceDirect);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use <see cref="DoesNotInheritFrom(IThat{Type}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotImplement(
		this IThat<Type?> subject,
		Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return new(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new ImplementsConstraint(expectationBuilder, it, grammars, interfaceType, forceDirect).Invert()),
			subject);
	}

	private sealed class ImplementsConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		bool forceDirect)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.Implements(interfaceType, forceDirect) == true ? Outcome.Success : Outcome.Failure;
			if (actual is not null)
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual", Formatter.Format(actual)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(forceDirect ? "directly implements " : "implements ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (forceDirect && Actual?.Implements(interfaceType) == true)
			{
				stringBuilder.Append(It).Append(" implemented ");
				Formatter.Format(stringBuilder, interfaceType);
				stringBuilder.Append(" only indirectly");
				return;
			}

			stringBuilder.Append(It).Append(" did not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(forceDirect ? "does not directly implement " : "does not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(forceDirect ? " did directly implement " : " did implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}
	}
}
