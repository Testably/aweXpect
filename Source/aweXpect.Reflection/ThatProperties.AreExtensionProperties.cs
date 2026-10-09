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

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are extension properties
	///     (declared with the C# extension block syntax).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreExtensionProperties(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreExtensionPropertiesConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are extension properties
	///     (declared with the C# extension block syntax).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>>
		AreExtensionProperties(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreExtensionPropertiesConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not extension properties
	///     (not declared with the C# extension block syntax).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreNotExtensionProperties(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreNotExtensionPropertiesConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not extension properties
	///     (not declared with the C# extension block syntax).
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>>
		AreNotExtensionProperties(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreNotExtensionPropertiesConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreExtensionPropertiesConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(it, grammars),
			IContextConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, property => property.IsExtensionProperty(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, property => property.IsExtensionProperty());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all extension properties");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained non-extension properties");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all extension properties");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained extension properties");
	}

	private sealed class AreNotExtensionPropertiesConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(it, grammars),
			IContextConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, property => !property.IsExtensionProperty(), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, property => !property.IsExtensionProperty());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not extension properties");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained extension properties");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an extension property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained non-extension properties");
	}
}
