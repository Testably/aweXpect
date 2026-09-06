using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are exactly of type <typeparamref name="TType" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			AreExactly<TType>()
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are exactly of type {Formatter.Format(typeof(TType))}",
								(false, false) => $"is exactly of type {Formatter.Format(typeof(TType))}",
								(true, true) => $"are not exactly of type {Formatter.Format(typeof(TType))}",
								(false, true) => $"is not exactly of type {Formatter.Format(typeof(TType))}",
							},
						a => a?.GetType() == typeof(TType),
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are exactly of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder,
						it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are exactly of type {Formatter.Format(type)}",
								(false, false) => $"is exactly of type {Formatter.Format(type)}",
								(true, true) => $"are not exactly of type {Formatter.Format(type)}",
								(false, true) => $"is not exactly of type {Formatter.Format(type)}",
							},
						a => a?.GetType() == type,
						"were")),
				_subject,
				options);
		}
	}

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>
			AreExactly<TType>()
		{
			ObjectEqualityOptions<object?> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are exactly of type {Formatter.Format(typeof(TType))}",
								(false, false) => $"is exactly of type {Formatter.Format(typeof(TType))}",
								(true, true) => $"are not exactly of type {Formatter.Format(typeof(TType))}",
								(false, true) => $"is not exactly of type {Formatter.Format(typeof(TType))}",
							},
						a => a?.GetType() == typeof(TType),
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ObjectEqualityOptions<object?> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are exactly of type {Formatter.Format(type)}",
								(false, false) => $"is exactly of type {Formatter.Format(type)}",
								(true, true) => $"are not exactly of type {Formatter.Format(type)}",
								(false, true) => $"is not exactly of type {Formatter.Format(type)}",
							},
						a => a?.GetType() == type,
						"were")),
				_subject,
				options);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			AreExactly<TType>()
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are exactly of type {Formatter.Format(typeof(TType))}",
								(false, false) => $"is exactly of type {Formatter.Format(typeof(TType))}",
								(true, true) => $"are not exactly of type {Formatter.Format(typeof(TType))}",
								(false, true) => $"is not exactly of type {Formatter.Format(typeof(TType))}",
							},
						a => a?.GetType() == typeof(TType),
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			AreExactly(Type type)
		{
			type.ThrowIfNull();
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionForEnumerableConstraint<TEnumerable>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are exactly of type {Formatter.Format(type)}",
								(false, false) => $"is exactly of type {Formatter.Format(type)}",
								(true, true) => $"are not exactly of type {Formatter.Format(type)}",
								(false, true) => $"is not exactly of type {Formatter.Format(type)}",
							},
						a => a?.GetType() == type,
						"were")),
				_subject,
				options);
		}
	}
}
