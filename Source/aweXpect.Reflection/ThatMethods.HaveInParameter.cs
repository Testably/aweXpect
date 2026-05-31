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
	///     an <see langword="in" /> parameter.
	/// </summary>
	public static AndOrResult<IEnumerable<MethodInfo?>, IThat<IEnumerable<MethodInfo?>>> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<MethodInfo?>>((it, grammars)
				=> new HaveInParameterConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<MethodInfo?>, IThat<IAsyncEnumerable<MethodInfo?>>> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<MethodInfo?>>((it, grammars)
				=> new HaveInParameterConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveInParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveInParameter<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveInParameter(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveInParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveInParameter<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveInParameter(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsInParameter(), "with in modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveInParameterExactly(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<MethodInfo?>, object?> HaveInParameterExactly(
		this IThat<IEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveInParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, TParameter> HaveInParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsInParameter(), "with in modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="MethodInfo" /> have
	///     an <see langword="in" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<MethodInfo?>, object?> HaveInParameterExactly(
		this IThat<IAsyncEnumerable<MethodInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsInParameter(), "with in modifier");
#endif

	private sealed class HaveInParameterConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<MethodInfo?>(grammars),
			IValueConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, method => method?.GetParameters().Any(p => p.IsInParameter()) == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual)
			=> SetValue(actual, method => method?.GetParameters().Any(p => p.IsInParameter()) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have an in parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained methods without an in parameter ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have an in parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained methods with an in parameter ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
