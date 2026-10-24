using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

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
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> InheritsFrom<TBaseType>(
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
	///     <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> InheritsFrom(
		this IThat<Type?> subject,
		Type baseType)
	{
		baseType.EnsureIsClass();
		ScopeOptions scope = new();
		return new InheritanceScopeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(
				(BaseType: baseType, Scope: scope),
				static (s, it, grammars)
				=> new InheritsFromConstraint(it, grammars, s.BaseType, s.Scope)),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use <see cref="DoesNotImplement{TInterface}(IThat{Type})" />.<br />
	///     With <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> only <typeparamref name="TBaseType" /> as the
	///     direct parent fails, so a type that inherits from it only indirectly satisfies the expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotInheritFrom<TBaseType>(
		this IThat<Type?> subject)
		=> subject.DoesNotInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To check that an interface is not implemented use <see cref="DoesNotImplement(IThat{Type}, Type)" />.<br />
	///     With <see cref="InheritanceScopeResult{TType, TValue}.Directly" /> only <paramref name="baseType" /> as the
	///     direct parent fails, so a type that inherits from it only indirectly satisfies the expectation.
	/// </remarks>
	/// <param name="subject">The type subject.</param>
	/// <param name="baseType">The base type to check inheritance from.</param>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotInheritFrom(
		this IThat<Type?> subject,
		Type baseType)
	{
		baseType.EnsureIsClass();
		ScopeOptions scope = new();
		return new InheritanceScopeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(
				(BaseType: baseType, Scope: scope),
				static (s, it, grammars)
				=> new InheritsFromConstraint(it, grammars, s.BaseType, s.Scope).Invert()),
			subject,
			scope);
	}

	private sealed class InheritsFromConstraint(
		string it,
		ExpectationGrammars grammars,
		Type baseType,
		ScopeOptions scope)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual?.InheritsFromClass(baseType, scope.IsDirect) == true ? Outcome.Success : Outcome.Failure;
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
			AppendDirectlyFrom(stringBuilder, scope.IsDirect);
			Formatter.Format(stringBuilder, baseType);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(scope.IsDirect && Actual?.InheritsFromClass(baseType) == true
				? " did only indirectly"
				: " did not");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not inherit ");
			AppendDirectlyFrom(stringBuilder, scope.IsDirect);
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
