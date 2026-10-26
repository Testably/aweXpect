using System;
using System.Collections.Generic;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
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
		=> ExpectInheritFrom(subject, typeof(TBaseType));

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
		=> ExpectInheritFrom(subject, baseType);

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
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> InheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectInheritFrom(subject, typeof(TBaseType));
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
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> InheritFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectInheritFrom(subject, baseType);
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
		=> ExpectDoNotInheritFrom(subject, typeof(TBaseType));

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
		=> ExpectDoNotInheritFrom(subject, baseType);

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
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotInheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> ExpectDoNotInheritFrom(subject, typeof(TBaseType));
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
	public static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> DoNotInheritFrom(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
		=> ExpectDoNotInheritFrom(subject, baseType);
#endif

	private static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ExpectInheritFrom(
		IThat<IEnumerable<Type?>?> subject,
		Type baseType)
	{
		baseType.EnsureIsClass();
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, OptionsHolder<MemberScopes> Scope), IEnumerable<Type?>>((baseType, scope),
				static (s, it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	private static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ExpectInheritFrom(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
	{
		baseType.EnsureIsClass();
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, OptionsHolder<MemberScopes> Scope), IAsyncEnumerable<Type?>>((baseType, scope),
				static (s, it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Scope)),
			subject,
			scope);
	}
#endif

	private static InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?> ExpectDoNotInheritFrom(
		IThat<IEnumerable<Type?>?> subject,
		Type baseType)
	{
		baseType.EnsureIsClass();
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new InheritanceScopeResult<IEnumerable<Type?>, IEnumerable<Type?>?>(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, OptionsHolder<MemberScopes> Scope), IEnumerable<Type?>>((baseType, scope),
				static (s, it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Scope)),
			subject,
			scope);
	}

#if NET8_0_OR_GREATER
	private static InheritanceScopeResult<IAsyncEnumerable<Type?>, IAsyncEnumerable<Type?>?> ExpectDoNotInheritFrom(
		IThat<IAsyncEnumerable<Type?>?> subject,
		Type baseType)
	{
		baseType.EnsureIsClass();
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Type BaseType, OptionsHolder<MemberScopes> Scope), IAsyncEnumerable<Type?>>((baseType, scope),
				static (s, it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, s.BaseType, s.Scope)),
			subject,
			scope);
	}
#endif

	private sealed class InheritFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		OptionsHolder<MemberScopes> scope)
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
				type => type?.InheritsFromClass(baseType, scope.Value.IsDirect()) == true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.InheritsFromClass(baseType, scope.Value.IsDirect()) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
			Formatter.Format(stringBuilder, baseType);

		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
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
		OptionsHolder<MemberScopes> scope)
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
				type => type?.InheritsFromClass(baseType, scope.Value.IsDirect()) != true, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, type => type?.InheritsFromClass(baseType, scope.Value.IsDirect()) != true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all do not inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
			Formatter.Format(stringBuilder, baseType);

		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("at least one inherits ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, scope.Value.IsDirect());
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
