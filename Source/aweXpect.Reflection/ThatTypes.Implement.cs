using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

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
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> Implement<TInterface>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.Implement(typeof(TInterface));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> Implement(
		this IThat<IEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     …implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <inheritdoc cref="Implement{TInterface}(IThat{IEnumerable{Type}})" path="/remarks" />
	/// <inheritdoc cref="Implement{TInterface}(IThat{IEnumerable{Type}})" path="/exception" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> Implement<TInterface>(
		this IEnumerableItems<Type?> items)
		=> items.Implement(typeof(TInterface));

	/// <summary>
	///     …implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <inheritdoc cref="Implement(IThat{IEnumerable{Type}}, Type)" path="/remarks" />
	/// <inheritdoc cref="Implement(IThat{IEnumerable{Type}}, Type)" path="/param[@name='interfaceType']" />
	/// <inheritdoc cref="Implement(IThat{IEnumerable{Type}}, Type)" path="/exception" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> Implement(
		this IEnumerableItems<Type?> items,
		Type interfaceType)
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="Implement(IEnumerableItems{Type}, Type)" />
	public static InheritanceScopeResult<TCollection, TCollection> Implement<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Type interfaceType)
		where TCollection : struct, IEnumerable<Type?>
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> Implement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.Implement(typeof(TInterface));

	/// <inheritdoc cref="Implement(IThat{IEnumerable{Type}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> Implement(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="Implement{TInterface}(IEnumerableItems{Type})" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> Implement<TInterface>(
		this IAsyncEnumerableItems<Type?> items)
		=> items.Implement(typeof(TInterface));

	/// <inheritdoc cref="Implement(IEnumerableItems{Type}, Type)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> Implement(
		this IAsyncEnumerableItems<Type?> items,
		Type interfaceType)
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}
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
		=> subject.DoNotImplement(typeof(TInterface));

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
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotImplement{TInterface}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotImplement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotImplement(typeof(TInterface));

	/// <inheritdoc cref="DoNotImplement(IThat{IEnumerable{Type}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotImplement(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type interfaceType)
	{
		ImplementExpectation expectation = new(interfaceType);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}
#endif

	private sealed class ImplementExpectation(Type interfaceType) : ItemExpectation<Type>
	{
		private readonly Type _interfaceType = interfaceType.EnsureIsInterface();

		public OptionsHolder<MemberScopes> Scope { get; } = new(MemberScopes.None);

		public override ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> new(item.Implements(_interfaceType, Scope.Value.IsDirect()));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			if (Scope.Value.IsDirect())
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append(grammars.IsPlural() ? "implement " : "implements ");
			Formatter.Format(stringBuilder, _interfaceType);
		}
	}
}
