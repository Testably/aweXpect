using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using aweXpect.Options;
using aweXpect.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatMembers
{
	/// <summary>
	///     Verifies that all items have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TMember?>, IThat<IEnumerable<TMember?>?>> Are<TMember>(
		this IThat<IEnumerable<TMember?>?> subject,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(subject.Get().ExpectationBuilder.AddConstraint(accessModifier,
				static (accessModifier, it, grammars)
					=> new AreConstraint<IEnumerable<TMember?>?, TMember>(it, grammars,
						EnumerableQuantifier.All(), true, accessModifier)),
			subject);

	/// <summary>
	///     …have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	public static AndOrResult<IEnumerable<TMember?>, IThat<IEnumerable<TMember?>?>> Are<TMember>(
		this IEnumerableItems<TMember?> items,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(items.Subject.Get().ExpectationBuilder.AddConstraint((items.Quantifier, accessModifier),
				static (state, it, grammars)
					=> new AreConstraint<IEnumerable<TMember?>?, TMember>(it, grammars,
						state.Quantifier, false, state.accessModifier)),
			items.Subject);

	/// <summary>
	///     …have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	public static AndOrResult<TCollection, IThat<TCollection>> Are<TCollection, TMember>(
		this IStructEnumerableItems<TCollection, TMember?> items,
		AccessModifiers accessModifier)
		where TCollection : struct, IEnumerable<TMember?>
		where TMember : MemberInfo
		=> new(items.Subject.Get().ExpectationBuilder.AddConstraint((items.Quantifier, accessModifier),
				static (state, it, grammars)
					=> new AreConstraint<TCollection, TMember>(it, grammars,
						state.Quantifier, false, state.accessModifier)),
			items.Subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TMember?>, IThat<IAsyncEnumerable<TMember?>?>> Are<TMember>(
		this IThat<IAsyncEnumerable<TMember?>?> subject,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(subject.Get().ExpectationBuilder.AddConstraint(accessModifier,
				static (accessModifier, it, grammars)
					=> new AreAsyncConstraint<TMember>(it, grammars,
						EnumerableQuantifier.All(), true, accessModifier)),
			subject);

	/// <summary>
	///     …have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<TMember?>, IThat<IAsyncEnumerable<TMember?>?>> Are<TMember>(
		this IAsyncEnumerableItems<TMember?> items,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(items.Subject.Get().ExpectationBuilder.AddConstraint((items.Quantifier, accessModifier),
				static (state, it, grammars)
					=> new AreAsyncConstraint<TMember>(it, grammars,
						state.Quantifier, false, state.accessModifier)),
			items.Subject);
#endif

	/// <summary>
	///     Verifies that no items have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<TMember?>, IThat<IEnumerable<TMember?>?>> AreNot<TMember>(
		this IThat<IEnumerable<TMember?>?> subject,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(subject.Get().ExpectationBuilder.AddConstraint(accessModifier,
				static (accessModifier, it, grammars)
					=> new AreConstraint<IEnumerable<TMember?>?, TMember>(it, grammars,
						EnumerableQuantifier.None(ExpectationGrammars.Nested), true, accessModifier)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that no items have the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Combined flags, e.g. <c>AccessModifiers.Public | AccessModifiers.Internal</c>, are met by any of them.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<TMember?>, IThat<IAsyncEnumerable<TMember?>?>> AreNot<TMember>(
		this IThat<IAsyncEnumerable<TMember?>?> subject,
		AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> new(subject.Get().ExpectationBuilder.AddConstraint(accessModifier,
				static (accessModifier, it, grammars)
					=> new AreAsyncConstraint<TMember>(it, grammars,
						EnumerableQuantifier.None(ExpectationGrammars.Nested), true, accessModifier)),
			subject);
#endif

	private static PredicateResult HasAccessModifierMember<TMember>(TMember? member, AccessModifiers accessModifier)
		where TMember : MemberInfo
		=> member is null ? ItemResults.Null : member.HasAccessModifier(accessModifier);

	private static string AccessModifierVerb(AccessModifiers accessModifier)
		=> accessModifier == AccessModifiers.Any ? "did" : "were";

	private static void AppendAccessModifierExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		AccessModifiers accessModifier)
		=> stringBuilder.AppendExpectation(accessModifier, false, grammars.IsPlural());

	private static string FormatMembers<TMember>(IEnumerable<TMember?> items, FormattingOptions options)
		where TMember : MemberInfo
		=> ValueFormatters.Format(Formatter, items.Select(item => Formattable(item)), options);

	private sealed class AreConstraint<TCollection, TMember>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		bool isOnCollection,
		AccessModifiers accessModifier)
		: GeneratedCollectionConstraint<TCollection, TMember?>(it, grammars, quantifier, isOnCollection)
		where TCollection : IEnumerable<TMember?>?
		where TMember : MemberInfo
	{
		protected override string Verb => AccessModifierVerb(accessModifier);

		protected override ValueTask<PredicateResult> IsMatch(TMember? item, CancellationToken cancellationToken)
			=> new(HasAccessModifierMember(item, accessModifier));

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> AppendAccessModifierExpectation(stringBuilder, grammars, accessModifier);

		protected override string FormatItems(IEnumerable<TMember?> items, FormattingOptions options)
			=> FormatMembers(items, options);
	}

#if NET8_0_OR_GREATER
	private sealed class AreAsyncConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		bool isOnCollection,
		AccessModifiers accessModifier)
		: GeneratedAsyncCollectionConstraint<TMember?>(it, grammars, quantifier, isOnCollection)
		where TMember : MemberInfo
	{
		protected override string Verb => AccessModifierVerb(accessModifier);

		protected override ValueTask<PredicateResult> IsMatch(TMember? item, CancellationToken cancellationToken)
			=> new(HasAccessModifierMember(item, accessModifier));

		protected override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
			string? indentation)
			=> AppendAccessModifierExpectation(stringBuilder, grammars, accessModifier);

		protected override string FormatItems(IEnumerable<TMember?> items, FormattingOptions options)
			=> FormatMembers(items, options);
	}
#endif
}
