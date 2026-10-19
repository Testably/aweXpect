using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>?>> HaveParameterCount(
		this IThat<IEnumerable<MethodInfo?>?> subject, int expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<int, IEnumerable<MethodInfo?>>(expected,
				static (expected, it, grammars)
				=> new HaveParameterCountConstraint(it, grammars, expected)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>?>> HaveParameterCount(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject, int expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<int, IAsyncEnumerable<MethodInfo?>>(expected,
				static (expected, it, grammars)
				=> new HaveParameterCountConstraint(it, grammars, expected)),
			subject);
#endif

	private static string ParameterCountDescription(int count)
		=> count switch
		{
			0 => "no parameters",
			1 => "one parameter",
			_ => $"{count} parameters",
		};

	private sealed class HaveParameterCountConstraint(string it, ExpectationGrammars grammars, int expected)
		: CollectionConstraintResult<MethodInfo?>(it, grammars),
			IContextConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				method => method?.GetParameters().Length == expected, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, method => method?.GetParameters().Length == expected);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have ").Append(ParameterCountDescription(expected));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained methods with a different number of parameters");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have ").Append(ParameterCountDescription(expected));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained methods with ")
				.Append(ParameterCountDescription(expected));
		}
	}
}
