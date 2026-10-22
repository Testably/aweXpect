using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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
	///     To check for base-class inheritance use <see cref="InheritsFrom{TBaseType}(IThat{Type})" />,
	///     or use <see cref="IsAssignableTo{TType}(IThat{Type})" /> to cover base classes and interfaces in one step.<br />
	///     To require <typeparamref name="TInterface" /> to be implemented directly use
	///     <see cref="ImplementsDirectly{TInterface}(IThat{Type})" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> Implements<TInterface>(
		this IThat<Type?> subject)
		=> subject.Implements(typeof(TInterface));

	/// <summary>
	///     Verifies that the <see cref="Type" /> implements the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored and a type does not
	///     implement itself.<br />
	///     To check for base-class inheritance use <see cref="InheritsFrom(IThat{Type}, Type)" />,
	///     or use <see cref="IsAssignableTo(IThat{Type}, Type)" /> to cover base classes and interfaces in one step.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="ImplementsDirectly(IThat{Type}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> Implements(
		this IThat<Type?> subject,
		Type interfaceType)
		=> ExpectImplements(subject, interfaceType, false);

	/// <summary>
	///     Verifies that the <see cref="Type" /> implements the interface <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that the type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also accept an indirectly implemented <typeparamref name="TInterface" /> use
	///     <see cref="Implements{TInterface}(IThat{Type})" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> ImplementsDirectly<TInterface>(
		this IThat<Type?> subject)
		=> ExpectImplements(subject, typeof(TInterface), true);

	/// <summary>
	///     Verifies that the <see cref="Type" /> implements the interface <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that the type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also accept an indirectly implemented <paramref name="interfaceType" /> use
	///     <see cref="Implements(IThat{Type}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check direct implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> ImplementsDirectly(
		this IThat<Type?> subject,
		Type interfaceType)
		=> ExpectImplements(subject, interfaceType, true);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use <see cref="DoesNotInheritFrom{TBaseType}(IThat{Type})" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotImplement<TInterface>(
		this IThat<Type?> subject)
		=> subject.DoesNotImplement(typeof(TInterface));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use <see cref="DoesNotInheritFrom(IThat{Type}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotImplement(
		this IThat<Type?> subject,
		Type interfaceType)
		=> ExpectDoesNotImplement(subject, interfaceType, false);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <typeparamref name="TInterface" />
	///     directly.
	/// </summary>
	/// <remarks>
	///     A type that implements <typeparamref name="TInterface" /> only through a base class or through another
	///     implemented interface satisfies this expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotImplementDirectly<TInterface>(
		this IThat<Type?> subject)
		=> ExpectDoesNotImplement(subject, typeof(TInterface), true);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <paramref name="interfaceType" />
	///     directly.
	/// </summary>
	/// <remarks>
	///     A type that implements <paramref name="interfaceType" /> only through a base class or through another
	///     implemented interface satisfies this expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check direct implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotImplementDirectly(
		this IThat<Type?> subject,
		Type interfaceType)
		=> ExpectDoesNotImplement(subject, interfaceType, true);

	private static AndOrResult<Type, IThat<Type?>> ExpectImplements(
		IThat<Type?> subject,
		Type interfaceType,
		bool directly)
	{
		interfaceType.EnsureIsInterface();
		return new AndOrResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((InterfaceType: interfaceType, Directly: directly),
				static (s, it, grammars)
				=> new ImplementsConstraint(it, grammars, s.InterfaceType, s.Directly)),
			subject);
	}

	private static AndOrResult<Type, IThat<Type?>> ExpectDoesNotImplement(
		IThat<Type?> subject,
		Type interfaceType,
		bool directly)
	{
		interfaceType.EnsureIsInterface();
		return new AndOrResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((InterfaceType: interfaceType, Directly: directly),
				static (s, it, grammars)
				=> new ImplementsConstraint(it, grammars, s.InterfaceType, s.Directly).Invert()),
			subject);
	}

	private sealed class ImplementsConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		bool directly)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.Implements(interfaceType, directly) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Actual is { } actual)
			{
				contexts.Add(new ResultContext.SyncCallback("Actual", () => Formatter.Format(actual)));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(directly ? "directly implements " : "implements ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(directly && Actual?.Implements(interfaceType) == true
				? " did only indirectly"
				: " did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(directly ? "does not directly implement " : "does not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
