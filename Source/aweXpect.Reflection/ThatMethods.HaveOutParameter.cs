using System;
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

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> HaveOutParameter(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<MethodInfo?>>((it, grammars)
				=> new HaveOutParameterConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>>> HaveOutParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<MethodInfo?>>((it, grammars)
				=> new HaveOutParameterConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveOutParameter(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveOutParameter(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveOutParameter(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveOutParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveOutParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveOutParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveOutParameterExactly(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveOutParameterExactly(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveOutParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveOutParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");
#endif

	private sealed class HaveOutParameterConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<MethodInfo?>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, method => method?.GetParameters().Any(p => p.IsOutParameter()) == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
			=> SetValue(actual, method => method?.GetParameters().Any(p => p.IsOutParameter()) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have an out parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained methods without an out parameter ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have an out parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained methods with an out parameter ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
