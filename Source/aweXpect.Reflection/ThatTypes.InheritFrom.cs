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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TBaseType" /> to be the direct parent use
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> InheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.InheritFrom(typeof(TBaseType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="baseType" /> to be the direct parent use
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> InheritFrom(
		this IThat<IEnumerable<Type?>?> subject,
		Type baseType)
	{
		InheritFromExpectation expectation = new(baseType);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <summary>
	///     …inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <inheritdoc cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" path="/remarks" />
	/// <inheritdoc cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" path="/exception" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> InheritFrom<TBaseType>(
		this IEnumerableItems<Type?> items)
		=> items.InheritFrom(typeof(TBaseType));

	/// <summary>
	///     …inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <inheritdoc cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" path="/remarks" />
	/// <inheritdoc cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" path="/param[@name='baseType']" />
	/// <inheritdoc cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" path="/exception" />
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> InheritFrom(
		this IEnumerableItems<Type?> items,
		Type baseType)
	{
		InheritFromExpectation expectation = new(baseType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

	/// <inheritdoc cref="InheritFrom(IEnumerableItems{Type}, Type)" />
	public static InheritanceScopeResult<TCollection, TCollection> InheritFrom<TCollection>(
		this IStructEnumerableItems<TCollection, Type?> items,
		Type baseType)
		where TCollection : struct, IEnumerable<Type?>
	{
		InheritFromExpectation expectation = new(baseType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> InheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.InheritFrom(typeof(TBaseType));

	/// <inheritdoc cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> InheritFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
	{
		InheritFromExpectation expectation = new(baseType);
		return new(subject.ExpectAll(expectation), subject, expectation.Scope);
	}

	/// <inheritdoc cref="InheritFrom{TBaseType}(IEnumerableItems{Type})" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> InheritFrom<TBaseType>(
		this IAsyncEnumerableItems<Type?> items)
		=> items.InheritFrom(typeof(TBaseType));

	/// <inheritdoc cref="InheritFrom(IEnumerableItems{Type}, Type)" />
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> InheritFrom(
		this IAsyncEnumerableItems<Type?> items,
		Type baseType)
	{
		InheritFromExpectation expectation = new(baseType);
		return new(items.ExpectItems(expectation), items.Subject, expectation.Scope);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> inherit from the base
	///     class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use
	///     <see cref="DoNotImplement{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotInheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.DoNotInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> inherit from the base
	///     class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use
	///     <see cref="DoNotImplement(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> DoNotInheritFrom(
		this IThat<IEnumerable<Type?>?> subject,
		Type baseType)
	{
		InheritFromExpectation expectation = new(baseType);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="DoNotInheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotInheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.DoNotInheritFrom(typeof(TBaseType));

	/// <inheritdoc cref="DoNotInheritFrom(IThat{IEnumerable{Type}}, Type)" />
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotInheritFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
	{
		InheritFromExpectation expectation = new(baseType);
		return new(subject.ExpectNone(expectation), subject, expectation.Scope);
	}
#endif

	private sealed class InheritFromExpectation(Type baseType) : ItemExpectation<Type>
	{
		private readonly Type _baseType = baseType.EnsureIsClass();

		public OptionsHolder<MemberScopes> Scope { get; } = new(MemberScopes.None);

		public override ValueTask<PredicateResult> IsMatch(Type item, CancellationToken cancellationToken)
			=> new(item.InheritsFromClass(_baseType, Scope.Value.IsDirect()));

		public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		{
			stringBuilder.Append(grammars.IsPlural() ? "inherit " : "inherits ");
			if (Scope.Value.IsDirect())
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append("from ");
			Formatter.Format(stringBuilder, _baseType);
		}
	}
}
