using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
	///     To check for base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TInterface" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TInterface" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> Implement<TInterface>(
		this IThat<IEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.Implement(typeof(TInterface), forceDirect);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> Implement(
		this IThat<IEnumerable<Type?>> subject,
		Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new ImplementConstraint(it, grammars | ExpectationGrammars.Plural, interfaceType, forceDirect)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TInterface" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TInterface" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> Implement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.Implement(typeof(TInterface), forceDirect);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check for base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> Implement(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new ImplementConstraint(it, grammars | ExpectationGrammars.Plural, interfaceType, forceDirect)),
			subject);
	}
#endif

	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TInterface" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TInterface" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotImplement<TInterface>(
		this IThat<IEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.DoNotImplement(typeof(TInterface), forceDirect);

	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotImplement(
		this IThat<IEnumerable<Type?>> subject,
		Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new DoNotImplementConstraint(it, grammars | ExpectationGrammars.Plural, interfaceType, forceDirect)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TInterface" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TInterface" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotImplement<TInterface>(
		this IThat<IAsyncEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.DoNotImplement(typeof(TInterface), forceDirect);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> implement the interface
	///     <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To check that a base class is not inherited use
	///     <see cref="DoNotInheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="interfaceType">The interface to check implementation of.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="interfaceType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="interfaceType" /> to be implemented directly.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotImplement(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new DoNotImplementConstraint(it, grammars | ExpectationGrammars.Plural, interfaceType, forceDirect)),
			subject);
	}
#endif

	private sealed class ImplementConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		bool forceDirect)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type?.Implements(interfaceType, forceDirect) == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type?.Implements(interfaceType, forceDirect) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(forceDirect ? "all directly implement " : "all implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(forceDirect
				? " contained types that do not directly implement "
				: " contained types that do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(forceDirect ? "not all directly implement " : "not all implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(forceDirect
				? " only contained types that directly implement "
				: " only contained types that implement ");
			Formatter.Format(stringBuilder, interfaceType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class DoNotImplementConstraint(
		string it,
		ExpectationGrammars grammars,
		Type interfaceType,
		bool forceDirect)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type?.Implements(interfaceType, forceDirect) != true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type?.Implements(interfaceType, forceDirect) != true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(forceDirect ? "all do not directly implement " : "all do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(forceDirect
				? " contained types that directly implement "
				: " contained types that implement ");
			Formatter.Format(stringBuilder, interfaceType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(forceDirect ? "at least one directly implements " : "at least one implements ");
			Formatter.Format(stringBuilder, interfaceType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(forceDirect
				? " only contained types that do not directly implement "
				: " only contained types that do not implement ");
			Formatter.Format(stringBuilder, interfaceType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
