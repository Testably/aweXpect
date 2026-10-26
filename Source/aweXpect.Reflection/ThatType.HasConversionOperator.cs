using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> has an implicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasImplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject)
		=> subject.HasImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that the <see cref="Type" /> has an implicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasImplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint((Source: source, Target: target, Scope: scope),
				static (s, it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, true, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have an implicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveImplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject)
		=> subject.DoesNotHaveImplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have an implicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveImplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint((Source: source, Target: target, Scope: scope),
				static (s, it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, true, s.Source, s.Target, s.Scope).Invert()),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> has an explicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasExplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject)
		=> subject.HasExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that the <see cref="Type" /> has an explicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> HasExplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint((Source: source, Target: target, Scope: scope),
				static (s, it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, false, s.Source, s.Target, s.Scope)),
			subject,
			scope);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have an explicit conversion operator from
	///     <typeparamref name="TSource" /> to <typeparamref name="TTarget" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveExplicitConversionOperator<TSource, TTarget>(
		this IThat<Type?> subject)
		=> subject.DoesNotHaveExplicitConversionOperator(typeof(TSource), typeof(TTarget));

	/// <summary>
	///     Verifies that the <see cref="Type" /> does not have an explicit conversion operator from
	///     <paramref name="source" /> to <paramref name="target" />.
	/// </summary>
	/// <remarks>
	///     Operators inherited from base types are considered as well; restrict the check to the operators declared
	///     on the type with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.<br />
	///     Matches the non-checked <c>op_Explicit</c> conversion. Because C# requires a <c>checked</c> explicit
	///     conversion to be declared alongside its non-checked counterpart, this also holds for types that additionally
	///     declare a <c>checked</c> conversion with the same signature.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<Type, Type?> DoesNotHaveExplicitConversionOperator(
		this IThat<Type?> subject,
		Type source,
		Type target)
	{
		OptionsHolder<MemberScopes> scope = new(MemberScopes.None);
		return new(subject.Get().ExpectationBuilder.AddConstraint((Source: source, Target: target, Scope: scope),
				static (s, it, grammars)
				=> new HasConversionOperatorConstraint(it, grammars, false, s.Source, s.Target, s.Scope).Invert()),
			subject,
			scope);
	}

	private sealed class HasConversionOperatorConstraint(
		string it,
		ExpectationGrammars grammars,
		bool isImplicit,
		Type source,
		Type target,
		OptionsHolder<MemberScopes> scope)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = actual.GetConversionOperator(isImplicit, source, target, !scope.Value.IsDirect()) is not null
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendConversion(stringBuilder, scope.Value.IsDirect() ? "declares " : "has ");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.Value.IsDirect() ? " did not declare " : " did not have ");
			AppendConversionDescription(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendConversion(stringBuilder, scope.Value.IsDirect() ? "does not declare " : "does not have ");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(scope.Value.IsDirect() ? " declared " : " had ");
			AppendConversionDescription(stringBuilder);
			stringBuilder.Append(' ');
			Formatter.Format(stringBuilder, Actual);
		}

		private void AppendConversion(StringBuilder stringBuilder, string verb)
		{
			stringBuilder.Append(verb);
			AppendConversionDescription(stringBuilder);
		}

		private void AppendConversionDescription(StringBuilder stringBuilder)
		{
			stringBuilder.Append(isImplicit ? "an implicit conversion operator from " : "an explicit conversion operator from ");
			Formatter.Format(stringBuilder, source);
			stringBuilder.Append(" to ");
			Formatter.Format(stringBuilder, target);
		}
	}
}
