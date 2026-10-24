using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

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
	///     <see cref="InheritanceScopeResult{TType, TThat}.Directly" />: an interface that the type only receives
	///     through a base class or through another implemented interface is not implemented directly. As the compiler
	///     emits all inherited interfaces into the metadata, an interface that is declared on the type and also
	///     inherited cannot be told apart and counts as not direct.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> Implements<TInterface>(
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
	///     <see cref="InheritanceScopeResult{TType, TThat}.Directly" />: an interface that the type only receives
	///     through a base class or through another implemented interface is not implemented directly. As the compiler
	///     emits all inherited interfaces into the metadata, an interface that is declared on the type and also
	///     inherited cannot be told apart and counts as not direct.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> Implements(
		this IThat<Type?> subject,
		Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		ScopeOptions scope = new();
		return new InheritanceScopeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(
				(InterfaceType: interfaceType, Scope: scope),
				static (s, it, grammars)
				=> new ImplementsConstraint(it, grammars, s.InterfaceType, s.Scope)),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use <see cref="DoesNotInheritFrom{TBaseType}(IThat{Type})" />.<br />
	///     With <see cref="InheritanceScopeResult{TType, TThat}.Directly" /> a type that implements
	///     <typeparamref name="TInterface" /> only through a base class or through another implemented interface
	///     satisfies this expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotImplement<TInterface>(
		this IThat<Type?> subject)
		=> subject.DoesNotImplement(typeof(TInterface));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use <see cref="DoesNotInheritFrom(IThat{Type}, Type)" />.<br />
	///     With <see cref="InheritanceScopeResult{TType, TThat}.Directly" /> a type that implements
	///     <paramref name="interfaceType" /> only through a base class or through another implemented interface
	///     satisfies this expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotImplement(
		this IThat<Type?> subject,
		Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		ScopeOptions scope = new();
		return new InheritanceScopeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(
				(InterfaceType: interfaceType, Scope: scope),
				static (s, it, grammars)
				=> new ImplementsConstraint(it, grammars, s.InterfaceType, s.Scope).Invert()),
			subject,
			scope);
	}

	private sealed class ImplementsConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		ScopeOptions scope)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.Implements(interfaceType, scope.IsDirect) == true ? Outcome.Success : Outcome.Failure;
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
			stringBuilder.Append(scope.IsDirect ? "directly implements " : "implements ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(scope.IsDirect && Actual?.Implements(interfaceType) == true
				? " did only indirectly"
				: " did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(scope.IsDirect ? "does not directly implement " : "does not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
