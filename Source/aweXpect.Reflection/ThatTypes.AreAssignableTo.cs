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
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreAssignableTo<TType>(
		this IThat<IEnumerable<Type?>> subject)
		=> subject.AreAssignableTo(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable to
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     for interfaces use <see cref="Implement(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type the items should be assignable to.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreAssignableTo(
		this IThat<IEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreAssignableToConstraint(it, grammars, type)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreAssignableTo<TType>(
		this IThat<IAsyncEnumerable<Type?>> subject)
		=> subject.AreAssignableTo(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable to
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     for interfaces use <see cref="Implement(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type the items should be assignable to.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreAssignableTo(
		this IThat<IAsyncEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreAssignableToConstraint(it, grammars, type)),
			subject);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotAssignableTo<TType>(
		this IThat<IEnumerable<Type?>> subject)
		=> subject.AreNotAssignableTo(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable to
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     for interfaces use <see cref="Implement(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type the items should not be assignable to.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotAssignableTo(
		this IThat<IEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreNotAssignableToConstraint(it, grammars, type)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}}, bool)" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreNotAssignableTo<TType>(
		this IThat<IAsyncEnumerable<Type?>> subject)
		=> subject.AreNotAssignableTo(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable to
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom(IThat{IEnumerable{Type}}, Type, bool)" />,
	///     for interfaces use <see cref="Implement(IThat{IEnumerable{Type}}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type the items should not be assignable to.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreNotAssignableTo(
		this IThat<IAsyncEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreNotAssignableToConstraint(it, grammars, type)),
			subject);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks that
	///     <typeparamref name="TType" /> is assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreAssignableFrom<TType>(
		this IThat<IEnumerable<Type?>> subject)
		=> subject.AreAssignableFrom(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable from
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" />: it checks that
	///     <paramref name="type" /> is assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type that should be assignable to each item.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreAssignableFrom(
		this IThat<IEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreAssignableFromConstraint(it, grammars, type)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks that
	///     <typeparamref name="TType" /> is assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreAssignableFrom<TType>(
		this IThat<IAsyncEnumerable<Type?>> subject)
		=> subject.AreAssignableFrom(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable from
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreAssignableTo(IThat{IEnumerable{Type}}, Type)" />: it checks that
	///     <paramref name="type" /> is assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type that should be assignable to each item.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreAssignableFrom(
		this IThat<IAsyncEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreAssignableFromConstraint(it, grammars, type)),
			subject);
	}
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreNotAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks
	///     that <typeparamref name="TType" /> is not assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotAssignableFrom<TType>(
		this IThat<IEnumerable<Type?>> subject)
		=> subject.AreNotAssignableFrom(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable from
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreNotAssignableTo(IThat{IEnumerable{Type}}, Type)" />: it checks
	///     that <paramref name="type" /> is not assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type that should not be assignable to each item.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotAssignableFrom(
		this IThat<IEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreNotAssignableFromConstraint(it, grammars, type)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreNotAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks
	///     that <typeparamref name="TType" /> is not assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreNotAssignableFrom<TType>(
		this IThat<IAsyncEnumerable<Type?>> subject)
		=> subject.AreNotAssignableFrom(typeof(TType));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable from
	///     <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreNotAssignableTo(IThat{IEnumerable{Type}}, Type)" />: it checks
	///     that <paramref name="type" /> is not assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	/// <param name="type">The type that should not be assignable to each item.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreNotAssignableFrom(
		this IThat<IAsyncEnumerable<Type?>> subject, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreNotAssignableFromConstraint(it, grammars, type)),
			subject);
	}
#endif

	private sealed class AreAssignableToConstraint(
		string it,
		ExpectationGrammars grammars,
		Type type)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, t => t is not null && type.IsAssignableFrom(t));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, t => t is not null && type.IsAssignableFrom(t));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are not all assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotAssignableToConstraint(
		string it,
		ExpectationGrammars grammars,
		Type type)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, t => t is null || !type.IsAssignableFrom(t));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, t => t is null || !type.IsAssignableFrom(t));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all not assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("also contain a type assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreAssignableFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type type)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, t => t is not null && t.IsAssignableFrom(type));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, t => t is not null && t.IsAssignableFrom(type));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are not all assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotAssignableFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type type)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, t => t is null || !t.IsAssignableFrom(type));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, t => t is null || !t.IsAssignableFrom(type));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("are all not assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("also contain a type assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
