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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TBaseType" /> to be the direct parent use
	///     <see cref="InheritDirectlyFrom{TBaseType}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> InheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectInheritFrom(subject, typeof(TBaseType), false);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="baseType" /> to be the direct parent use
	///     <see cref="InheritDirectlyFrom(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> InheritFrom(
		this IThat<IEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectInheritFrom(subject, baseType, false);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit directly from the base
	///     class <typeparamref name="TBaseType" />, i.e. that <typeparamref name="TBaseType" /> is their direct parent.
	/// </summary>
	/// <remarks>
	///     To also accept <typeparamref name="TBaseType" /> anywhere in the base-class chain use
	///     <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> InheritDirectlyFrom<TBaseType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectInheritFrom(subject, typeof(TBaseType), true);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit directly from the base
	///     class <paramref name="baseType" />, i.e. that <paramref name="baseType" /> is their direct parent.
	/// </summary>
	/// <remarks>
	///     To also accept <paramref name="baseType" /> anywhere in the base-class chain use
	///     <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check direct inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> InheritDirectlyFrom(
		this IThat<IEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectInheritFrom(subject, baseType, true);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TBaseType" /> to be the direct parent use
	///     <see cref="InheritDirectlyFrom{TBaseType}(IThat{IAsyncEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> InheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectInheritFrom(subject, typeof(TBaseType), false);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement(IThat{IEnumerable{Type}}, Type)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="baseType" /> to be the direct parent use
	///     <see cref="InheritDirectlyFrom(IThat{IAsyncEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> InheritFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectInheritFrom(subject, baseType, false);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit directly from the base
	///     class <typeparamref name="TBaseType" />, i.e. that <typeparamref name="TBaseType" /> is their direct parent.
	/// </summary>
	/// <remarks>
	///     To also accept <typeparamref name="TBaseType" /> anywhere in the base-class chain use
	///     <see cref="InheritFrom{TBaseType}(IThat{IAsyncEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> InheritDirectlyFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectInheritFrom(subject, typeof(TBaseType), true);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit directly from the base
	///     class <paramref name="baseType" />, i.e. that <paramref name="baseType" /> is their direct parent.
	/// </summary>
	/// <remarks>
	///     To also accept <paramref name="baseType" /> anywhere in the base-class chain use
	///     <see cref="InheritFrom(IThat{IAsyncEnumerable{Type}}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check direct inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> InheritDirectlyFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectInheritFrom(subject, baseType, true);
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
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotInheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectDoNotInheritFrom(subject, typeof(TBaseType), false);

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
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotInheritFrom(
		this IThat<IEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectDoNotInheritFrom(subject, baseType, false);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> inherit directly from the
	///     base class <typeparamref name="TBaseType" />, i.e. that <typeparamref name="TBaseType" /> is not their direct
	///     parent.
	/// </summary>
	/// <remarks>
	///     Types that inherit from <typeparamref name="TBaseType" /> only indirectly satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotInheritDirectlyFrom<TBaseType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> ExpectDoNotInheritFrom(subject, typeof(TBaseType), true);

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> inherit directly from the
	///     base class <paramref name="baseType" />, i.e. that <paramref name="baseType" /> is not their direct parent.
	/// </summary>
	/// <remarks>
	///     Types that inherit from <paramref name="baseType" /> only indirectly satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check direct inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotInheritDirectlyFrom(
		this IThat<IEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectDoNotInheritFrom(subject, baseType, true);

#if NET8_0_OR_GREATER
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
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotInheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectDoNotInheritFrom(subject, typeof(TBaseType), false);
#endif

#if NET8_0_OR_GREATER
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
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotInheritFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectDoNotInheritFrom(subject, baseType, false);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> inherit directly from the
	///     base class <typeparamref name="TBaseType" />, i.e. that <typeparamref name="TBaseType" /> is not their direct
	///     parent.
	/// </summary>
	/// <remarks>
	///     Types that inherit from <typeparamref name="TBaseType" /> only indirectly satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotInheritDirectlyFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectDoNotInheritFrom(subject, typeof(TBaseType), true);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> inherit directly from the
	///     base class <paramref name="baseType" />, i.e. that <paramref name="baseType" /> is not their direct parent.
	/// </summary>
	/// <remarks>
	///     Types that inherit from <paramref name="baseType" /> only indirectly satisfy this expectation.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check direct inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotInheritDirectlyFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectDoNotInheritFrom(subject, baseType, true);
#endif

	private static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> ExpectInheritFrom(
		IThat<IEnumerable<Type?>?> subject,
		Type baseType,
		bool directly)
	{
		baseType.EnsureIsClass();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, bool Directly), IEnumerable<Type?>>((baseType, directly),
				static (s, it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Directly)),
			subject);
	}

#if NET8_0_OR_GREATER
	private static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> ExpectInheritFrom(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType,
		bool directly)
	{
		baseType.EnsureIsClass();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, bool Directly), IAsyncEnumerable<Type?>>((baseType, directly),
				static (s, it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Directly)),
			subject);
	}
#endif

	private static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> ExpectDoNotInheritFrom(
		IThat<IEnumerable<Type?>?> subject,
		Type baseType,
		bool directly)
	{
		baseType.EnsureIsClass();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, bool Directly), IEnumerable<Type?>>((baseType, directly),
				static (s, it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Directly)),
			subject);
	}

#if NET8_0_OR_GREATER
	private static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> ExpectDoNotInheritFrom(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType,
		bool directly)
	{
		baseType.EnsureIsClass();
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, bool Directly), IAsyncEnumerable<Type?>>((baseType, directly),
				static (s, it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Directly)),
			subject);
	}
#endif

	private sealed class InheritFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
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
				type => type?.InheritsFromClass(baseType, directly) == true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.InheritsFromClass(baseType, directly) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);

		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		private static void AppendDirectlyFrom(StringBuilder stringBuilder, bool directly)
		{
			if (directly)
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append("from ");
		}
	}

	private sealed class DoNotInheritFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
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
				type => type?.InheritsFromClass(baseType, directly) != true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.InheritsFromClass(baseType, directly) != true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all do not inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);

		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("at least one inherits ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);

		}

		private static void AppendDirectlyFrom(StringBuilder stringBuilder, bool directly)
		{
			if (directly)
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append("from ");
		}
	}
}
