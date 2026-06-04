using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is assignable to <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers the type itself as assignable, mirroring
	///     the inverse of <see cref="Type.IsAssignableFrom(Type)" />. Closed generic variance is honored, but open generic
	///     type definitions are not supported.<br />
	///     For base-class inheritance use <see cref="InheritsFrom{TBaseType}(IThat{Type}, bool)" />, for interfaces use
	///     <see cref="Implements{TInterface}(IThat{Type}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	public static AndOrResult<Type?, IThat<Type?>> IsAssignableTo<TType>(
		this IThat<Type?> subject)
		=> subject.IsAssignableTo(typeof(TType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> is assignable to <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers the type itself as assignable, mirroring
	///     the inverse of <see cref="Type.IsAssignableFrom(Type)" />. Closed generic variance is honored, but open generic
	///     type definitions are not supported.<br />
	///     For base-class inheritance use <see cref="InheritsFrom(IThat{Type}, Type, bool)" />, for interfaces use
	///     <see cref="Implements(IThat{Type}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="type">The type the subject should be assignable to.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<Type?, IThat<Type?>> IsAssignableTo(
		this IThat<Type?> subject,
		Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new IsAssignableToConstraint(expectationBuilder, it, grammars, type)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not assignable to <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers the type itself as assignable, mirroring
	///     the inverse of <see cref="Type.IsAssignableFrom(Type)" />. Closed generic variance is honored, but open generic
	///     type definitions are not supported.<br />
	///     For base-class inheritance use <see cref="DoesNotInheritFrom{TBaseType}(IThat{Type}, bool)" />, for interfaces
	///     use <see cref="DoesNotImplement{TInterface}(IThat{Type}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	public static AndOrResult<Type?, IThat<Type?>> IsNotAssignableTo<TType>(
		this IThat<Type?> subject)
		=> subject.IsNotAssignableTo(typeof(TType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not assignable to <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers the type itself as assignable, mirroring
	///     the inverse of <see cref="Type.IsAssignableFrom(Type)" />. Closed generic variance is honored, but open generic
	///     type definitions are not supported.<br />
	///     For base-class inheritance use <see cref="DoesNotInheritFrom(IThat{Type}, Type, bool)" />, for interfaces
	///     use <see cref="DoesNotImplement(IThat{Type}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="type">The type the subject should not be assignable to.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<Type?, IThat<Type?>> IsNotAssignableTo(
		this IThat<Type?> subject,
		Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new IsAssignableToConstraint(expectationBuilder, it, grammars, type).Invert()),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> is assignable from <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="IsAssignableTo{TType}(IThat{Type})" />: it checks that
	///     <typeparamref name="TType" /> is assignable to the subject, mirroring <see cref="Type.IsAssignableFrom(Type)" />.
	///     The type itself is considered assignable and closed generic variance is honored, but open generic type
	///     definitions are not supported.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	public static AndOrResult<Type?, IThat<Type?>> IsAssignableFrom<TType>(
		this IThat<Type?> subject)
		=> subject.IsAssignableFrom(typeof(TType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> is assignable from <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="IsAssignableTo(IThat{Type}, Type)" />: it checks that
	///     <paramref name="type" /> is assignable to the subject, mirroring <see cref="Type.IsAssignableFrom(Type)" />.
	///     The type itself is considered assignable and closed generic variance is honored, but open generic type
	///     definitions are not supported.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="type">The type that should be assignable to the subject.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<Type?, IThat<Type?>> IsAssignableFrom(
		this IThat<Type?> subject,
		Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new IsAssignableFromConstraint(expectationBuilder, it, grammars, type)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not assignable from <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="IsNotAssignableTo{TType}(IThat{Type})" />: it checks that
	///     <typeparamref name="TType" /> is not assignable to the subject, mirroring the inverse of
	///     <see cref="Type.IsAssignableFrom(Type)" />. The type itself is considered assignable and closed generic variance
	///     is honored, but open generic type definitions are not supported.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	public static AndOrResult<Type?, IThat<Type?>> IsNotAssignableFrom<TType>(
		this IThat<Type?> subject)
		=> subject.IsNotAssignableFrom(typeof(TType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not assignable from <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="IsNotAssignableTo(IThat{Type}, Type)" />: it checks that
	///     <paramref name="type" /> is not assignable to the subject, mirroring the inverse of
	///     <see cref="Type.IsAssignableFrom(Type)" />. The type itself is considered assignable and closed generic variance
	///     is honored, but open generic type definitions are not supported.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="type">The type that should not be assignable to the subject.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static AndOrResult<Type?, IThat<Type?>> IsNotAssignableFrom(
		this IThat<Type?> subject,
		Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new IsAssignableFromConstraint(expectationBuilder, it, grammars, type).Invert()),
			subject);
	}

	private sealed class IsAssignableToConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual is not null && type.IsAssignableFrom(actual) ? Outcome.Success : Outcome.Failure;
			if (actual is not null)
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual", Formatter.Format(actual)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was not assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not assignable to ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was assignable to ");
			Formatter.Format(stringBuilder, type);
		}
	}

	private sealed class IsAssignableFromConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type type)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual.IsAssignableFrom(type) ? Outcome.Success : Outcome.Failure;
			if (actual is not null)
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual", Formatter.Format(actual)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was not assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is not assignable from ");
			Formatter.Format(stringBuilder, type);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was assignable from ");
			Formatter.Format(stringBuilder, type);
		}
	}
}
