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
	///     Verifies that the <see cref="Type" /> inherits from <typeparamref name="TBaseType" />.
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> InheritsFrom<TBaseType>(
		this IThat<Type?> subject,
		bool forceDirect = false)
		=> subject.InheritsFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that the <see cref="Type" /> inherits from <paramref name="baseType" />.
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> InheritsFrom(
		this IThat<Type?> subject,
		Type baseType,
		bool forceDirect = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new InheritsFromConstraint(it, grammars, baseType, forceDirect)),
			subject);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from <typeparamref name="TBaseType" />.
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <typeparamref name="TBaseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <typeparamref name="TBaseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotInheritFrom<TBaseType>(
		this IThat<Type?> subject,
		bool forceDirect = false)
		=> subject.DoesNotInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from <paramref name="baseType" />.
	/// </summary>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <param name="forceDirect">
	///     If set to <see langword="false" /> (default value), the <paramref name="baseType" />
	///     can be anywhere in the inheritance tree, otherwise if set to <see langword="true" /> requires the
	///     <paramref name="baseType" /> to be the direct parent.
	/// </param>
	public static AndOrResult<Type?, IThat<Type?>> DoesNotInheritFrom(
		this IThat<Type?> subject,
		Type baseType,
		bool forceDirect = false)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new InheritsFromConstraint(it, grammars, baseType, forceDirect).Invert()),
			subject);

	private sealed class InheritsFromConstraint(
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
			Outcome = actual?.InheritsFrom(baseType, forceDirect) == true ? Outcome.Success : Outcome.Failure;
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
			stringBuilder.Append(It).Append(" did not inherit ");
			AppendDirectlyFrom(stringBuilder, forceDirect);
			Formatter.Format(stringBuilder, baseType);
			stringBuilder.Append(", but was ");
			Formatter.Format(stringBuilder, Actual);
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
