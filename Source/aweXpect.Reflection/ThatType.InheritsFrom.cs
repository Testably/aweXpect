using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> inherits from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; a type is not regarded as inheriting from itself.<br />
	///     To check for an implemented interface use <see cref="Implements{TInterface}(IThat{Type})" />,
	///     or use <see cref="IsAssignableTo{TType}(IThat{Type})" /> to cover base classes and interfaces in one step.<br />
	///     To require <typeparamref name="TBaseType" /> to be the direct parent use
	///     <see cref="InheritsDirectlyFrom{TBaseType}(IThat{Type})" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> InheritsFrom<TBaseType>(
		this IThat<Type?> subject)
		=> subject.InheritsFrom(typeof(TBaseType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> inherits from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; a type is not regarded as inheriting from itself.<br />
	///     To check for an implemented interface use <see cref="Implements(IThat{Type}, Type)" />,
	///     or use <see cref="IsAssignableTo(IThat{Type}, Type)" /> to cover base classes and interfaces in one step.<br />
	///     To require <paramref name="baseType" /> to be the direct parent use
	///     <see cref="InheritsDirectlyFrom(IThat{Type}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> InheritsFrom(
		this IThat<Type?> subject,
		Type baseType)
		=> ExpectInheritsFrom(subject, baseType, false);

	/// <summary>
	///     Verifies that the <see cref="Type" /> inherits directly from the base class <typeparamref name="TBaseType" />,
	///     i.e. that <typeparamref name="TBaseType" /> is its direct parent.
	/// </summary>
	/// <remarks>
	///     To also accept <typeparamref name="TBaseType" /> anywhere in the base-class chain use
	///     <see cref="InheritsFrom{TBaseType}(IThat{Type})" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> InheritsDirectlyFrom<TBaseType>(
		this IThat<Type?> subject)
		=> ExpectInheritsFrom(subject, typeof(TBaseType), true);

	/// <summary>
	///     Verifies that the <see cref="Type" /> inherits directly from the base class <paramref name="baseType" />,
	///     i.e. that <paramref name="baseType" /> is its direct parent.
	/// </summary>
	/// <remarks>
	///     To also accept <paramref name="baseType" /> anywhere in the base-class chain use
	///     <see cref="InheritsFrom(IThat{Type}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check direct inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> InheritsDirectlyFrom(
		this IThat<Type?> subject,
		Type baseType)
		=> ExpectInheritsFrom(subject, baseType, true);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use <see cref="DoesNotImplement{TInterface}(IThat{Type})" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotInheritFrom<TBaseType>(
		this IThat<Type?> subject)
		=> subject.DoesNotInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use <see cref="DoesNotImplement(IThat{Type}, Type)" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotInheritFrom(
		this IThat<Type?> subject,
		Type baseType)
		=> ExpectDoesNotInheritFrom(subject, baseType, false);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit directly from the base class
	///     <typeparamref name="TBaseType" />, i.e. that <typeparamref name="TBaseType" /> is not its direct parent.
	/// </summary>
	/// <remarks>
	///     A type that inherits from <typeparamref name="TBaseType" /> only indirectly satisfies this expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotInheritDirectlyFrom<TBaseType>(
		this IThat<Type?> subject)
		=> ExpectDoesNotInheritFrom(subject, typeof(TBaseType), true);

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit directly from the base class
	///     <paramref name="baseType" />, i.e. that <paramref name="baseType" /> is not its direct parent.
	/// </summary>
	/// <remarks>
	///     A type that inherits from <paramref name="baseType" /> only indirectly satisfies this expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check direct inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<Type, IThat<Type?>> DoesNotInheritDirectlyFrom(
		this IThat<Type?> subject,
		Type baseType)
		=> ExpectDoesNotInheritFrom(subject, baseType, true);

	private static AndOrResult<Type, IThat<Type?>> ExpectInheritsFrom(
		IThat<Type?> subject,
		Type baseType,
		bool directly)
	{
		baseType.EnsureIsClass();
		return new AndOrResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((BaseType: baseType, Directly: directly),
				static (s, it, grammars)
				=> new InheritsFromConstraint(it, grammars, s.BaseType, s.Directly)),
			subject);
	}

	private static AndOrResult<Type, IThat<Type?>> ExpectDoesNotInheritFrom(
		IThat<Type?> subject,
		Type baseType,
		bool directly)
	{
		baseType.EnsureIsClass();
		return new AndOrResult<Type, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((BaseType: baseType, Directly: directly),
				static (s, it, grammars)
				=> new InheritsFromConstraint(it, grammars, s.BaseType, s.Directly).Invert()),
			subject);
	}

	private sealed class InheritsFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		bool directly)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.InheritsFromClass(baseType, directly) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Actual is { } actual)
			{
				contexts.Add(new ResultContext.SyncCallback("Actual", () => Formatter.Format(actual)));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("inherits ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(directly && Actual?.InheritsFromClass(baseType) == true
				? " did only indirectly"
				: " did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not inherit ");
			AppendDirectlyFrom(stringBuilder, directly);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");

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
