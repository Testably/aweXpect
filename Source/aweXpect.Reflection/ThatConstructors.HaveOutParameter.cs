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

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter.
	/// </summary>
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>>> HaveOutParameter(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveOutParameterConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>>> HaveOutParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveOutParameterConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, object?> HaveOutParameter(
		this IThat<IEnumerable<ConstructorInfo?>> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, object?> HaveOutParameter(
		this IThat<IEnumerable<ConstructorInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, object?> HaveOutParameter(
		this IThat<IEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject)
		=> subject.HaveParameter<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, object?> HaveOutParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, Type parameterType)
		=> subject.HaveParameter(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveOutParameter<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, object?> HaveOutParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameter(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, object?> HaveOutParameter(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameter(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IEnumerable<ConstructorInfo?>, object?> HaveOutParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IEnumerable<ConstructorInfo?>, object?> HaveOutParameterExactly(
		this IThat<IEnumerable<ConstructorInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject)
		=> subject.HaveParameterExactly<TParameter>().WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, object?> HaveOutParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, Type parameterType)
		=> subject.HaveParameterExactly(parameterType).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <typeparamref name="TParameter" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, TParameter> HaveOutParameterExactly<TParameter>(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, string expected)
		=> subject.HaveParameterExactly<TParameter>(expected).WithModifier(p => p.IsOutParameter(), "with out modifier");

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     an <see langword="out" /> parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<IAsyncEnumerable<ConstructorInfo?>, object?> HaveOutParameterExactly(
		this IThat<IAsyncEnumerable<ConstructorInfo?>> subject, Type parameterType, string expected)
		=> subject.HaveParameterExactly(parameterType, expected).WithModifier(p => p.IsOutParameter(), "with out modifier");
#endif

	private sealed class HaveOutParameterConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<ConstructorInfo?>(grammars),
			IValueConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, constructor => constructor?.GetParameters().Any(p => p.IsOutParameter()) == true);
#endif

		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual)
			=> SetValue(actual, constructor => constructor?.GetParameters().Any(p => p.IsOutParameter()) == true);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have an out parameter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained constructors without an out parameter ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have an out parameter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained constructors with an out parameter ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
