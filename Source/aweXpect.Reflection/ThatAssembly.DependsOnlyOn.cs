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

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has dependencies only on the <paramref name="allowed" /> assemblies.
	/// </summary>
	/// <remarks>
	///     References to assemblies whose name starts with one of the
	///     <see cref="AwexpectCustomization.ReflectionCustomizationValue.ExcludedAssemblyPrefixes" /> are ignored,
	///     so that framework assemblies do not have to be listed explicitly.
	/// </remarks>
	public static StringEqualityTypeResult<Assembly?, IThat<Assembly?>> DependsOnlyOn(
		this IThat<Assembly?> subject, params string[] allowed)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<Assembly?, IThat<Assembly?>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DependsOnlyOnConstraint(it, grammars, allowed, options)),
			subject,
			options);
	}

	private sealed class DependsOnlyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string[] allowed,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IAsyncConstraint<Assembly?>
	{
		private string?[] _violations = [];

		public async Task<ConstraintResult> IsMetBy(Assembly? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
			List<string?> violations = [];
			foreach (AssemblyName dependency in actual.GetReferencedAssemblies())
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

			_violations = violations.ToArray();
			Outcome = _violations.Length == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("it also had dependencies on ");
			Formatter.Format(stringBuilder, _violations);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have dependencies only on ").Append(DescribeAllowed());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("it only had dependencies on the allowed assemblies");

		private string DescribeAllowed()
			=> allowed.Length == 0
				? "no assemblies"
				: $"assemblies {string.Join(" or ", allowed.Select(expected => options.GetExpectation(expected, ExpectationGrammars.None)))}";
	}
}
