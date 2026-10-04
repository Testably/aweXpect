using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique, Options: options),
					static (state, it, grammars) =>
					{
						SubjectEqualityOptions<string?, string?> itemOptions =
							new(state.Options);
						return new AreUniqueConstraint<IEnumerable<string?>?, string?, string?>(
							it, grammars,
							state.Quantifier,
							a => a,
							memberAccessorExpression: null,
							state.Options,
							state.ExpectUnique,
							itemOptions);
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<IEnumerable<string?>?, string?, TMember>(
							it, grammars,
							state.Quantifier,
							state.MemberAccessor,
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<IEnumerable<string?>?, string?, string>(
							it, grammars,
							state.Quantifier,
							state.MemberAccessor,
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique, Options: options),
					static (state, it, grammars) =>
					{
						SubjectEqualityOptions<TItem, TItem> itemOptions =
							new(state.Options);
						return new AreUniqueConstraint<IEnumerable<TItem>?, TItem, TItem>(
							it, grammars,
							state.Quantifier,
							a => a,
							memberAccessorExpression: null,
							state.Options,
							state.ExpectUnique,
							itemOptions);
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<IEnumerable<TItem>?, TItem, TMember>(
							it, grammars,
							state.Quantifier,
							state.MemberAccessor,
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<IEnumerable<TItem>?, TItem, string>(
							it, grammars,
							state.Quantifier,
							state.MemberAccessor,
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique, Options: options),
					static (state, it, grammars) =>
					{
						SubjectEqualityOptions<object?, object?> itemOptions =
							new(state.Options);
						return new AreUniqueConstraint<TEnumerable, object?, object?>(
							it, grammars,
							state.Quantifier,
							a => a,
							memberAccessorExpression: null,
							state.Options,
							state.ExpectUnique,
							itemOptions);
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, TMember>(
							it, grammars,
							state.Quantifier,
							state.MemberAccessor,
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, string>(
							it, grammars,
							state.Quantifier,
							state.MemberAccessor,
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique, Options: options),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, TItem>(
							it, grammars,
							state.Quantifier,
							a => (TItem)a!,
							memberAccessorExpression: null,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, TMember>(
							it, grammars,
							state.Quantifier,
							a => state.MemberAccessor((TItem)a!),
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, string>(
							it, grammars,
							state.Quantifier,
							a => state.MemberAccessor((TItem)a!),
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique, Options: options),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, string?>(
							it, grammars,
							state.Quantifier,
							a => (string?)a,
							memberAccessorExpression: null,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, TMember>(
							it, grammars,
							state.Quantifier,
							a => state.MemberAccessor((string?)a),
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
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
				expectationBuilder.AddConstraint(
					(Quantifier: _quantifier, ExpectUnique: expectUnique,
						MemberAccessorExpression: memberAccessorExpression, Options: options,
						MemberAccessor: memberAccessor),
					static (state, it, grammars)
						=> new AreUniqueConstraint<TEnumerable, object?, string>(
							it, grammars,
							state.Quantifier,
							a => state.MemberAccessor((string?)a),
							state.MemberAccessorExpression,
							state.Options,
							state.ExpectUnique)),
				_subject,
				options);
		}
	}
}
