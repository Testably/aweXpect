using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> has at least one dependency (a type referenced in its signature)
	///     outside the allowed <paramref name="namespaces" /> (including sub-namespaces), its own namespace and
	///     framework assemblies — the positive counterpart of
	///     <see cref="DependsOnlyOn(IThat{Type?}, IEnumerable{string})" /> for finding violators of an allowed set.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static NamespaceDependencyOutsideResult<Type?> HasDependenciesOutside(
		this IThat<Type?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyOutsideResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasDependenciesOutsideConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> has at least one dependency (a type referenced in its signature)
	///     outside the allowed set formed by the filtered collections of types <paramref name="target" /> and
	///     <paramref name="additional" />, its own namespace and framework assemblies — the positive counterpart of
	///     <see cref="DependsOnlyOn(IThat{Type?}, Filtered.Types, Filtered.Types[])" /> for finding violators of an
	///     allowed set.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency is inside the allowed set when it
	///     is a member of the union of the resolved collections (by <see cref="Type" /> identity; a generic type
	///     definition in a collection matches any construction of it). The type's own namespace never counts as
	///     outside, including its sub-namespaces unless
	///     <see cref="TypeSetDependencyOutsideResult{TThat}.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so that framework dependencies never count as outside the
	///     allowed set. The default prefixes include <c>Microsoft</c>, so e.g. a dependency on
	///     <c>Microsoft.EntityFrameworkCore</c> is also ignored; customize the prefixes to make such a dependency
	///     count.
	/// </remarks>
	public static TypeSetDependencyOutsideResult<Type?> HasDependenciesOutside(
		this IThat<Type?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyOutsideResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasDependenciesOutsideTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}

	private sealed class HasDependenciesOutsideConstraint(
		string it,
		ExpectationGrammars grammars,
		NamespaceDependencyOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		private IReadOnlyList<string> _violations = [];

		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_violations = actual.GetDependencyNamespaceViolations(options);
			Outcome = _violations.Count > 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has dependencies outside ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only depended on the allowed namespaces");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have dependencies outside ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" also depended on ");
			Formatter.Format(stringBuilder, _violations);
		}
	}

	private sealed class HasDependenciesOutsideTypeSetConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeSetDependencyOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncConstraint<Type?>
	{
		private IReadOnlyList<string> _violations = [];

		public async Task<ConstraintResult> IsMetBy(Type? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ResolvedTypeSet allowed = await options.Resolve(cancellationToken);
			_violations = actual.GetDependencyTypeSetViolations(allowed);
			Outcome = _violations.Count > 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has dependencies outside ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only depended on the allowed types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have dependencies outside ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" also depended on ");
			Formatter.Format(stringBuilder, _violations);
		}
	}
}
