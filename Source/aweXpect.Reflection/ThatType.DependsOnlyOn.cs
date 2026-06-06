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
	///     Verifies that the <see cref="Type" /> depends on (references in its signature) only types in the
	///     <paramref name="namespaces" /> (including sub-namespaces), its own namespace or framework assemblies.
	/// </summary>
	/// <remarks>
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework namespaces do not have to be listed explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>DoesNotDependOn</c> or customize the prefixes.
	/// </remarks>
	public static NamespaceDependencyOnlyOnResult<Type?> DependsOnlyOn(
		this IThat<Type?> subject, params IEnumerable<string> namespaces)
	{
		NamespaceDependencyOptions options = new(namespaces);
		return new NamespaceDependencyOnlyOnResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnlyOnConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> depends on (references in its signature) only types in the filtered
	///     collections of types <paramref name="target" /> or <paramref name="additional" />, its own namespace or
	///     framework assemblies.
	/// </summary>
	/// <remarks>
	///     The target collections are resolved once per assertion; a dependency is allowed when it is a member of the
	///     union of the resolved collections (by <see cref="Type" /> identity; a generic type definition in a
	///     collection matches any construction of it). The type's own namespace is always allowed, including its
	///     sub-namespaces unless
	///     <see cref="TypeSetDependencyOnlyOnResult{TThat}.ExcludingOwnSubNamespaces" /> is used.
	///     <para />
	///     Dependencies on types whose assembly name matches one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> at a
	///     name-segment boundary (<c>System</c> covers <c>System.Text.Json</c>, but not
	///     <c>SystemsBiology.Core</c>) are ignored, so
	///     that framework types do not have to be included explicitly. The default prefixes include
	///     <c>Microsoft</c>, so e.g. <c>Microsoft.EntityFrameworkCore</c> is also ignored; forbid such a dependency
	///     explicitly via <c>DoesNotDependOn</c> or customize the prefixes.
	/// </remarks>
	public static TypeSetDependencyOnlyOnResult<Type?> DependsOnlyOn(
		this IThat<Type?> subject, Filtered.Types target, params Filtered.Types[] additional)
	{
		TypeSetDependencyOptions options = new(target, additional);
		return new TypeSetDependencyOnlyOnResult<Type?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnlyOnTypeSetConstraint(it, grammars, options)),
			subject,
			options);
	}

	private sealed class DependsOnlyOnConstraint(
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
			Outcome = _violations.Count == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("depends only on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" also depended on ");
			Formatter.Format(stringBuilder, _violations);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not depend only on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only depended on the allowed namespaces");
	}

	private sealed class DependsOnlyOnTypeSetConstraint(
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
			Outcome = _violations.Count == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("depends only on ").Append(options.Describe());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" also depended on ");
			Formatter.Format(stringBuilder, _violations);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not depend only on ").Append(options.Describe());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only depended on the allowed types");
	}
}
