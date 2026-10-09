using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> HaveParameterCount(
		this IThat<IEnumerable<ConstructorInfo?>?> subject, int expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveParameterCountConstraint(it, grammars, expected)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     the <paramref name="expected" /> number of parameters.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>> HaveParameterCount(
		this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject, int expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
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
		: CollectionConstraintResult<ConstructorInfo?>(it, grammars),
			IContextConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				constructor => constructor?.GetParameters().Length == expected, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, constructor => constructor?.GetParameters().Length == expected);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have ").Append(ParameterCountDescription(expected));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained constructors with a different number of parameters");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have ").Append(ParameterCountDescription(expected));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained constructors with ")
				.Append(ParameterCountDescription(expected));
		}
	}
}
