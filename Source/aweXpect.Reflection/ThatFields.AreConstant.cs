using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> are constant.
	/// </summary>
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreConstant(
		this IThat<IEnumerable<FieldInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<FieldInfo?>>((it, grammars)
				=> new AreConstantConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> are constant.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>> AreConstant(
		this IThat<IAsyncEnumerable<FieldInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<FieldInfo?>>((it, grammars)
				=> new AreConstantConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> are not constant.
	/// </summary>
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> AreNotConstant(
		this IThat<IEnumerable<FieldInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<FieldInfo?>>((it, grammars)
				=> new AreNotConstantConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="FieldInfo" /> are not constant.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>> AreNotConstant(
		this IThat<IAsyncEnumerable<FieldInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<FieldInfo?>>((it, grammars)
				=> new AreNotConstantConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreConstantConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<FieldInfo?>(grammars),
			IValueConstraint<IEnumerable<FieldInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<FieldInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<FieldInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, field => field?.IsLiteral == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual)
			=> SetValue(actual, field => field?.IsLiteral == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all constant");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-constant fields ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all constant");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained constant fields ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotConstantConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<FieldInfo?>(grammars),
			IValueConstraint<IEnumerable<FieldInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<FieldInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<FieldInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, field => field?.IsLiteral == false);
#endif

		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual)
			=> SetValue(actual, field => field?.IsLiteral == false);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not constant");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained constant fields ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a constant field");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-constant fields ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
