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
	///     Verifies that the <see cref="Type" /> inherits from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; a type is not regarded as inheriting from itself.<br />
	///     To check for an implemented interface use <see cref="Implements{TInterface}(IThat{Type}, bool)" />,
	///     or use <see cref="IsAssignableTo{TType}(IThat{Type})" /> to cover base classes and interfaces in one step.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> InheritsFrom<TBaseType>(
		this IThat<Type?> subject,
		bool forceDirect = false)
		=> subject.InheritsFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that the <see cref="Type" /> inherits from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; a type is not regarded as inheriting from itself.<br />
	///     To check for an implemented interface use <see cref="Implements(IThat{Type}, Type, bool)" />,
	///     or use <see cref="IsAssignableTo(IThat{Type}, Type)" /> to cover base classes and interfaces in one step.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> InheritsFrom(
		this IThat<Type?> subject,
		Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new InheritsFromConstraint(expectationBuilder, it, grammars, baseType, forceDirect)),
			subject);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use <see cref="DoesNotImplement{TInterface}(IThat{Type}, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotInheritFrom<TBaseType>(
		this IThat<Type?> subject,
		bool forceDirect = false)
		=> subject.DoesNotInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use <see cref="DoesNotImplement(IThat{Type}, Type, bool)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotInheritFrom(
		this IThat<Type?> subject,
		Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((expectationBuilder, it, grammars)
				=> new InheritsFromConstraint(expectationBuilder, it, grammars, baseType, forceDirect).Invert()),
			subject);
	}

	private sealed class InheritsFromConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		bool forceDirect)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.InheritsFromClass(baseType, forceDirect) == true ? Outcome.Success : Outcome.Failure;
			if (actual is not null)
			{
				expectationBuilder.AddContext(new ResultContext.Fixed("Actual", Formatter.Format(actual)));
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("inherits ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (forceDirect && Actual?.InheritsFromClass(baseType) == true)
			{
				stringBuilder.Append(It).Append(" inherited from ");
				Formatter.Format(stringBuilder, baseType);
				stringBuilder.Append(" only indirectly");
				return;
			}

			stringBuilder.Append(It).Append(" did not inherit from ");
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
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
