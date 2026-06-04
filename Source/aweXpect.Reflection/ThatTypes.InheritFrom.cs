using System;
using System.Collections.Generic;
using System.Text;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

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
	///     To check for an implemented interface use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}}, bool)" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> InheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.InheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> InheritFrom(
		this IThat<IEnumerable<Type?>> subject,
		Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, baseType, forceDirect)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}}, bool)" />,
	///     or use <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> InheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.InheritFrom(typeof(TBaseType), forceDirect);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check for an implemented interface use <see cref="Implement(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     or use <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> InheritFrom(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new InheritFromConstraint(it, grammars | ExpectationGrammars.Plural, baseType, forceDirect)),
			subject);
	}
#endif

	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use
	///     <see cref="DoNotImplement{TInterface}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotInheritFrom<TBaseType>(
		this IThat<IEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.DoNotInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use
	///     <see cref="DoNotImplement(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> DoNotInheritFrom(
		this IThat<IEnumerable<Type?>> subject,
		Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, baseType, forceDirect)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use
	///     <see cref="DoNotImplement{TInterface}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotInheritFrom<TBaseType>(
		this IThat<IAsyncEnumerable<Type?>> subject,
		bool forceDirect = false)
		=> subject.DoNotInheritFrom(typeof(TBaseType), forceDirect);
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that not all items in the filtered collection of <see cref="Type" /> inherit from the base class
	///     <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use
	///     <see cref="DoNotImplement(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> DoNotInheritFrom(
		this IThat<IAsyncEnumerable<Type?>> subject,
		Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new DoNotInheritFromConstraint(it, grammars | ExpectationGrammars.Plural, baseType, forceDirect)),
			subject);
	}
#endif

	private sealed class InheritFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		bool forceDirect)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type?.InheritsFromClass(baseType, forceDirect) == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type?.InheritsFromClass(baseType, forceDirect) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);

			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}

		private static void AppendDirectlyFrom(StringBuilder stringBuilder, bool forceDirect)
		{
			if (forceDirect)
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
		bool forceDirect)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type?.InheritsFromClass(baseType, forceDirect) != true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type?.InheritsFromClass(baseType, forceDirect) != true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all do not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained types that inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);

			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("at least one inherits ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained types that do not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);

			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}

		private static void AppendDirectlyFrom(StringBuilder stringBuilder, bool forceDirect)
		{
			if (forceDirect)
			{
				stringBuilder.Append("directly ");
			}

			stringBuilder.Append("from ");
		}
	}
}
