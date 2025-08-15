using System.Linq;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has a dependency on the <paramref name="expected" /> assembly.
	/// </summary>
	public static StringEqualityTypeResult<Assembly?, IThat<Assembly?>> HasADependencyOn(
		this IThat<Assembly?> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<Assembly?, IThat<Assembly?>>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasADependencyOnConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class HasADependencyOnConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IValueConstraint<Assembly?>
	{
		public ConstraintResult IsMetBy(Assembly? actual)
		{
			Actual = actual;
			Outcome = actual?.GetReferencedAssemblies().Any(dep => options.AreConsideredEqual(dep.Name, expected)) ==
			          true
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has a dependency on assembly ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("it did not have the required dependency in ");
			Formatter.Format(stringBuilder, Actual?.GetReferencedAssemblies().Select(dep => dep.Name));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has no dependency on assembly ")
				.Append(options.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("it had the unexpected dependency in ");
			Formatter.Format(stringBuilder, Actual?.GetReferencedAssemblies().Select(dep => dep.Name));
		}
	}
}
