using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter.
	/// </summary>
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>>> HaveParamsParameter(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveParamsParameterConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>>> HaveParamsParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveParamsParameterConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, object?> HaveParamsParameter(
		this IThat<IEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveParamsParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     a <see langword="params" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, object?> HaveParamsParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsParamsParameter(), "with params modifier");
#endif

	private sealed class HaveParamsParameterConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<ConstructorInfo?>(grammars),
			IValueConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, constructor => constructor?.GetParameters().Any(p => p.IsParamsParameter()) == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual)
			=> SetValue(actual, constructor => constructor?.GetParameters().Any(p => p.IsParamsParameter()) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have a params parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained constructors without a params parameter ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have a params parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained constructors with a params parameter ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
