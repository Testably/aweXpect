using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

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
	///     <see cref="ImplementDirectly{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> Implement<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectImplement(subject, typeof(TInterface), false);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="ImplementDirectly(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> Implement(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectImplement(subject, interfaceType, false);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that a type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also accept an indirectly implemented <typeparamref name="TInterface" /> use
	///     <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> ImplementDirectly<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectImplement(subject, typeof(TInterface), true);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that a type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also accept an indirectly implemented <paramref name="interfaceType" /> use
	///     <see cref="Implement(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check direct implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> ImplementDirectly(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectImplement(subject, interfaceType, true);

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
	///     <see cref="ImplementDirectly{TInterface}(IThat{IAsyncEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> Implement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectImplement(subject, typeof(TInterface), false);
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
	///     <see cref="ImplementDirectly(IThat{IAsyncEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> Implement(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectImplement(subject, interfaceType, false);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that a type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also accept an indirectly implemented <typeparamref name="TInterface" /> use
	///     <see cref="Implement{TInterface}(IThat{IAsyncEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> ImplementDirectly<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectImplement(subject, typeof(TInterface), true);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that a type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also accept an indirectly implemented <paramref name="interfaceType" /> use
	///     <see cref="Implement(IThat{IAsyncEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check direct implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> ImplementDirectly(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectImplement(subject, interfaceType, true);
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
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotImplement<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectDoNotImplement(subject, typeof(TInterface), false);

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
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotImplement(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectDoNotImplement(subject, interfaceType, false);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     Types that implement <typeparamref name="TInterface" /> only through a base class or through another
	///     implemented interface satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotImplementDirectly<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectDoNotImplement(subject, typeof(TInterface), true);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     Types that implement <paramref name="interfaceType" /> only through a base class or through another
	///     implemented interface satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check direct implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotImplementDirectly(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectDoNotImplement(subject, interfaceType, true);

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
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotImplement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectDoNotImplement(subject, typeof(TInterface), false);
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
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotImplement(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectDoNotImplement(subject, interfaceType, false);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     Types that implement <typeparamref name="TInterface" /> only through a base class or through another
	///     implemented interface satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotImplementDirectly<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectDoNotImplement(subject, typeof(TInterface), true);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     Types that implement <paramref name="interfaceType" /> only through a base class or through another
	///     implemented interface satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check direct implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotImplementDirectly(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
		=> ExpectDoNotImplement(subject, interfaceType, true);
#endif

	private static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> ExpectImplement(
		IThat<IEnumerable<Type?>?> subject,
		Type interfaceType,
		bool directly)
	{
		interfaceType.EnsureIsInterface();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, bool Directly), IEnumerable<Type?>>((interfaceType, directly),
				static (s, it, grammars)
				=> new ImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Directly)),
			subject);
	}

#if NET8_0_OR_GREATER
	private static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> ExpectImplement(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType,
		bool directly)
	{
		interfaceType.EnsureIsInterface();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, bool Directly), IAsyncEnumerable<Type?>>((interfaceType, directly),
				static (s, it, grammars)
				=> new ImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Directly)),
			subject);
	}
#endif

	private static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> ExpectDoNotImplement(
		IThat<IEnumerable<Type?>?> subject,
		Type interfaceType,
		bool directly)
	{
		interfaceType.EnsureIsInterface();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, bool Directly), IEnumerable<Type?>>((interfaceType, directly),
				static (s, it, grammars)
				=> new DoNotImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Directly)),
			subject);
	}

#if NET8_0_OR_GREATER
	private static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> ExpectDoNotImplement(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType,
		bool directly)
	{
		interfaceType.EnsureIsInterface();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type InterfaceType, bool Directly), IAsyncEnumerable<Type?>>((interfaceType, directly),
				static (s, it, grammars)
				=> new DoNotImplementConstraint(it, grammars | ExpectationGrammars.Plural, s.InterfaceType, s.Directly)),
			subject);
	}
#endif

	private sealed class ImplementConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		bool directly)
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
				type => type?.Implements(interfaceType, directly) == true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.Implements(interfaceType, directly) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(directly ? "all directly implement " : "all implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(directly
				? " contained types that do not directly implement "
				: " contained types that do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(directly ? "not all directly implement " : "not all implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(directly
				? " only contained types that directly implement "
				: " only contained types that implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}
	}

	private sealed class DoNotImplementConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		bool directly)
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
				type => type?.Implements(interfaceType, directly) != true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.Implements(interfaceType, directly) != true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(directly ? "all do not directly implement " : "all do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(directly
				? " contained types that directly implement "
				: " contained types that implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(directly ? "at least one directly implements " : "at least one implements ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(directly
				? " only contained types that do not directly implement "
				: " only contained types that do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}
	}
}
