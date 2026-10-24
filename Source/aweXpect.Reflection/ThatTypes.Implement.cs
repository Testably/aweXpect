using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TInterface" /> to be implemented directly use
	///     <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> Implement<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectImplement(subject, typeof(TInterface));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> Implement(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectImplement(subject, interfaceType);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TInterface" /> to be implemented directly use
	///     <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> Implement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectImplement(subject, typeof(TInterface));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="InheritanceScopeResult{TType, TThat}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> Implement(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectImplement(subject, interfaceType);
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotImplement<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectDoNotImplement(subject, typeof(TInterface));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotImplement(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectDoNotImplement(subject, interfaceType);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotImplement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectDoNotImplement(subject, typeof(TInterface));
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotImplement(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectDoNotImplement(subject, interfaceType);
#endif

	private static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ExpectImplement(
		IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		ScopeOptions scope = new();
		return new InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, ScopeOptions Scope), IEnumerable<Type?>>((interfaceType, scope),
				static (s, it, grammars)
				=> new ImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	private static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ExpectImplement(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, ScopeOptions Scope), IAsyncEnumerable<Type?>>((interfaceType, scope),
				static (s, it, grammars)
				=> new ImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Scope)),
			subject,
			scope);
	}
#endif

	private static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ExpectDoNotImplement(
		IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		ScopeOptions scope = new();
		return new InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, ScopeOptions Scope), IEnumerable<Type?>>((interfaceType, scope),
				static (s, it, grammars)
				=> new DoNotImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	private static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ExpectDoNotImplement(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		ScopeOptions scope = new();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, ScopeOptions Scope), IAsyncEnumerable<Type?>>((interfaceType, scope),
				static (s, it, grammars)
				=> new DoNotImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Scope)),
			subject,
			scope);
	}
#endif

	private sealed class ImplementConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		ScopeOptions scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				type => type?.Implements(interfaceType, scope.IsDirect) == true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.Implements(interfaceType, scope.IsDirect) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(scope.IsDirect ? "all directly implement " : "all implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.IsDirect
				? " contained types that do not directly implement "
				: " contained types that do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(scope.IsDirect ? "not all directly implement " : "not all implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.IsDirect
				? " only contained types that directly implement "
				: " only contained types that implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}
	}

	private sealed class DoNotImplementConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		ScopeOptions scope)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				type => type?.Implements(interfaceType, scope.IsDirect) != true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.Implements(interfaceType, scope.IsDirect) != true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(scope.IsDirect ? "all do not directly implement " : "all do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.IsDirect
				? " contained types that directly implement "
				: " contained types that implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(scope.IsDirect ? "at least one directly implements " : "at least one implements ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.IsDirect
				? " only contained types that do not directly implement "
				: " only contained types that do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}
	}
}
