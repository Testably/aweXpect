using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		/// <remarks>
		///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
		///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
		/// </remarks>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember> AreUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		/// <remarks>
		///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
		///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
		/// </remarks>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember> AreNotUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreNotUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUniqueCore(
			bool expectUnique)
		{
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
				expectationBuilder.AddConstraint((it, grammars) =>
				{
					SubjectEqualityOptions<string?, string?> itemOptions =
						new(options, () => options.ComparesByOrdinalEquality);
					return new AreUniqueConstraint<string?, string?>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), itemOptions),
						a => a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						itemOptions.UseComparerOf,
						createGetHashCode: () => MemberHashing.For(options));
				}),
				_subject,
				options);
		}

		private ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember>
			AreUniqueCore<TMember>(
				Func<string?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ItemEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<string?, TMember>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUniqueCore(
			Func<string?, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<string?, string>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}
	}

	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		/// <remarks>
		///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
		///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
		/// </remarks>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		/// <remarks>
		///     A set with a custom comparer never holds two items that its comparer considers equal, so its items count as
		///     unique without being compared, unless the comparison is changed, e.g. with <c>Using(…)</c>.
		/// </remarks>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreNotUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreNotUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreUniqueCore(
			bool expectUnique)
		{
			ItemEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars) =>
				{
					SubjectEqualityOptions<TItem, TItem> itemOptions = new(options, () => options.HasDefaultMatchType);
					return new AreUniqueConstraint<TItem, TItem>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), itemOptions),
						a => a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						itemOptions.UseComparerOf,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options));
				}),
				_subject,
				options);
		}

		private ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreUniqueCore<TMember>(
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ItemEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<TItem, TMember>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreUniqueCore(
			Func<TItem, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<TItem, string>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		/// <remarks>
		///     A set of <see langword="object" />s with a custom comparer never holds two items that its comparer considers
		///     equal, so its items count as unique without being compared, unless the comparison is changed, e.g. with
		///     <c>Using(…)</c>. The comparer of a set of another item type cannot be read without reflection and is not used.
		/// </remarks>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember> AreUnique<TMember>(
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable?>> AreUnique(
			Func<object?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		/// <remarks>
		///     A set of <see langword="object" />s with a custom comparer never holds two items that its comparer considers
		///     equal, so its items count as unique without being compared, unless the comparison is changed, e.g. with
		///     <c>Using(…)</c>. The comparer of a set of another item type cannot be read without reflection and is not used.
		/// </remarks>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember> AreNotUnique<TMember>(
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable?>> AreNotUnique(
			Func<object?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?> AreUniqueCore(bool expectUnique)
		{
			ItemEqualityOptions<object?> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>(
				expectationBuilder.AddConstraint((it, grammars) =>
				{
					SubjectEqualityOptions<object?, object?> itemOptions =
						new(options, () => options.HasDefaultMatchType);
					return new AreUniqueForEnumerableConstraint<TEnumerable, object?>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), itemOptions),
						a => a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						itemOptions.UseComparerOf,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options));
				}),
				_subject,
				options);
		}

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember> AreUniqueCore<TMember>(
			Func<object?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ItemEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private StringEqualityResult<TEnumerable, IThat<TEnumerable?>> AreUniqueCore(
			Func<object?, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable?>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreNotUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreNotUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem> AreUniqueCore(bool expectUnique)
		{
			ItemEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TItem>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => (TItem)a!,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUniqueCore<TMember>(
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ItemEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((TItem)a!),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUniqueCore(
			Func<TItem, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((TItem)a!),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreNotUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreNotUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUniqueCore(bool expectUnique)
		{
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string?>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => (string?)a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUniqueCore<TMember>(
			Func<string?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ItemEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((string?)a),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						appendOptionsContexts: options.AppendContexts,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}

		private StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUniqueCore(
			Func<string?, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new("expected");
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string>(
						it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((string?)a),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique,
						createGetHashCode: () => MemberHashing.For(options))),
				_subject,
				options);
		}
	}

	/// <remarks>
	///     When <paramref name="isUniqueBySubject" /> holds for the subject, e.g. for a set with a custom comparer
	///     whose comparison was not changed, every item is unique without being compared, because a set never holds two
	///     items that its comparer considers equal.
	/// </remarks>
	private sealed class AreUniqueConstraint<TItem, TMember>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, TMember> memberAccessor,
		Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
		bool expectUnique,
		Func<IEnumerable<TItem>, bool>? isUniqueBySubject = null,
		Action<ResultContextCollector>? appendOptionsContexts = null,
		Func<Func<TMember, int>?>? createGetHashCode = null)
		: QuantifiedCollectionConstraint<IEnumerable<TItem>?, TItem>(it, grammars, quantifier,
				expectationText, "were"),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private CollectionContext _collectionContext;

		public async Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem>(actual);
			if (isUniqueBySubject?.Invoke(actual) == true)
			{
				foreach (TItem item in materialized)
				{
					Record(item, expectUnique);
				}

				Complete();
				_collectionContext.Set(materialized);
				return this;
			}

			bool cancelEarly = actual is not ICollection<TItem>;
			OccurrenceCounter<TMember> occurrences = new(areConsideredEqual, createGetHashCode?.Invoke());
			List<(TItem Item, int MemberIndex)> items = [];
			foreach (TItem item in materialized)
			{
				items.Add((item, await occurrences.Add(UserCode.Invoke(memberAccessor, item, "the member selector"))));
				if (cancelEarly && occurrences.Determines(Quantifier, expectUnique))
				{
					RecordAll(items, occurrences);
					CompleteEarly();
					_collectionContext.Set(materialized);
					return this;
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return this;
				}
			}

			RecordAll(items, occurrences);
			Complete();
			_collectionContext.Set(materialized);
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
			appendOptionsContexts?.Invoke(contexts);
		}

		private void RecordAll(List<(TItem Item, int MemberIndex)> items, OccurrenceCounter<TMember> occurrences)
		{
			foreach ((TItem item, int memberIndex) in items)
			{
				Record(item, occurrences.IsUnique(memberIndex) == expectUnique);
			}
		}
	}

	/// <remarks>
	///     When <paramref name="isUniqueBySubject" /> holds for the subject, every item is unique without being compared,
	///     as in <see cref="AreUniqueConstraint{TItem,TMember}" />.
	/// </remarks>
	private sealed class AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<object?, TMember> memberAccessor,
		Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
		bool expectUnique,
		Func<object?, bool>? isUniqueBySubject = null,
		Action<ResultContextCollector>? appendOptionsContexts = null,
		Func<Func<TMember, int>?>? createGetHashCode = null)
		: QuantifiedCollectionConstraint<TEnumerable, object?>(it, grammars, quantifier,
				expectationText, "were"),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private CollectionContext _collectionContext;
		private Type? _itemType;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			_collectionContext.AppendTo(contexts);
			base.AppendContexts(contexts);
			appendOptionsContexts?.Invoke(contexts);
		}

		protected override Type ItemType => _itemType ?? typeof(object);

		public async Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			if (isUniqueBySubject?.Invoke(actual) == true)
			{
				foreach (object? item in materialized)
				{
					_itemType ??= item?.GetType();
					Record(item, expectUnique);
				}

				Complete();
				_collectionContext.Set(materialized);
				return this;
			}

			bool cancelEarly = actual is not ICollection;
			OccurrenceCounter<TMember> occurrences = new(areConsideredEqual, createGetHashCode?.Invoke());
			List<(object? Item, int MemberIndex)> items = [];
			foreach (object? item in materialized)
			{
				_itemType ??= item?.GetType();
				items.Add((item, await occurrences.Add(UserCode.Invoke(memberAccessor, item, "the member selector"))));
				if (cancelEarly && occurrences.Determines(Quantifier, expectUnique))
				{
					RecordAll(items, occurrences);
					CompleteEarly();
					_collectionContext.Set(materialized);
					return this;
				}

				if (cancellationToken.IsCanceledBeforeTheEndOf(materialized))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materialized, true);
					return this;
				}
			}

			RecordAll(items, occurrences);
			Complete();
			_collectionContext.Set(materialized);
			return this;
		}

		private void RecordAll(List<(object? Item, int MemberIndex)> items, OccurrenceCounter<TMember> occurrences)
		{
			foreach ((object? item, int memberIndex) in items)
			{
				Record(item, occurrences.IsUnique(memberIndex) == expectUnique);
			}
		}
	}
}
