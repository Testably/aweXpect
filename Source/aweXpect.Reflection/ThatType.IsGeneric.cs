using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> is generic.
	/// </summary>
	public static GenericArgumentCollectionResult<Type?> IsGeneric(
		this IThat<Type?> subject)
	{
		GenericArgumentsFilterOptions genericFilterOptions = new();
		return new GenericArgumentCollectionResult<Type?>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsGenericConstraint(it, grammars, genericFilterOptions)),
			subject,
			genericFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> is not generic.
	/// </summary>
	public static AndOrResult<Type?, IThat<Type?>> IsNotGeneric(
		this IThat<Type?> subject)
	{
		GenericArgumentsFilterOptions genericFilterOptions = new();
		return new AndOrResult<Type?, IThat<Type?>>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsGenericConstraint(it, grammars, genericFilterOptions).Invert()),
			subject);
	}

	private sealed class IsGenericConstraint(
		string it,
		ExpectationGrammars grammars,
		GenericArgumentsFilterOptions options)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IAsyncConstraint<Type?>
	{
		public async Task<ConstraintResult> IsMetBy(Type? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			Outcome = actual?.IsGenericType == true &&
			          await options.Matches(actual)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is generic");
			stringBuilder.Append(options.GetDescription());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.IsGenericType == true)
			{
				stringBuilder.Append(It).Append(" was generic ");
			}
			else
			{
				stringBuilder.Append(It).Append(" was non-generic ");
			}

			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not generic");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was generic ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
