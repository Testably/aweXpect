using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have dependencies only on
	///     the <paramref name="allowed" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name starts with one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>> HaveDependenciesOnlyOn(
		this IThat<IEnumerable<Assembly?>> subject, params string[] allowed)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new HaveDependenciesOnlyOnConstraint(it, grammars, allowed, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have dependencies only on
	///     the <paramref name="allowed" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name starts with one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>>
		HaveDependenciesOnlyOn(
			this IThat<IAsyncEnumerable<Assembly?>> subject, params string[] allowed)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new HaveDependenciesOnlyOnConstraint(it, grammars, allowed, options)),
			subject,
			options);
	}
#endif

	private sealed class HaveDependenciesOnlyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] allowed,
		StringEqualityOptions options)
		: CollectionConstraintResult<Assembly?>(grammars),
			IAsyncConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
		private readonly Dictionary<Assembly, string?[]> _disallowedDependencies = new();

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, HasDependenciesOnlyOnAllowed);
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, CancellationToken cancellationToken)
			=> await SetValue(actual, HasDependenciesOnlyOnAllowed);

#if NET8_0_OR_GREATER
		private async ValueTask<bool> HasDependenciesOnlyOnAllowed(Assembly? assembly)
#else
		private async Task<bool> HasDependenciesOnlyOnAllowed(Assembly? assembly)
#endif
		{
			if (assembly is null)
			{
				return false;
			}

			string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
			List<string?> violations = [];
			foreach (AssemblyName dependency in assembly.GetReferencedAssemblies())
			{
				if (prefixes.Any(prefix => dependency.Name?.StartsWith(prefix, StringComparison.Ordinal) == true))
				{
					continue;
				}

				if (!await allowed.AnyAsync(expected => options.AreConsideredEqual(dependency.Name, expected)))
				{
					violations.Add(dependency.Name);
				}
			}

			if (violations.Count > 0)
			{
				_disallowedDependencies[assembly] = violations.ToArray();
			}

			return violations.Count == 0;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			string itemIndentation = (indentation ?? string.Empty) + "  ";
			stringBuilder.Append(it).Append(" contained assemblies with disallowed dependencies [");
			for (int index = 0; index < NotMatching.Length; index++)
			{
				Assembly? assembly = NotMatching[index];
				string?[] violations =
					assembly is not null && _disallowedDependencies.TryGetValue(assembly, out string?[]? value)
						? value
						: [];
				stringBuilder.Append(Environment.NewLine).Append(itemIndentation)
					.Append(Formatter.Format(assembly))
					.Append(" depends on ")
					.Append(Formatter.Format(violations));
				if (index < NotMatching.Length - 1)
				{
					stringBuilder.Append(',');
				}
			}

			stringBuilder.Append(Environment.NewLine).Append(indentation).Append(']');
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained assemblies depending only on the allowed assemblies ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}

		private string DescribeAllowed()
			=> allowed.Length == 0
				? "no assemblies"
				: $"assemblies {string.Join(" or ", allowed.Select(expected => options.GetExpectation(expected, ExpectationGrammars.None)))}";
	}
}
