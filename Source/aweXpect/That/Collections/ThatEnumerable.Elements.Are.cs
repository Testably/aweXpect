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
		///     …are of type <typeparamref name="TType" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			Are<TType>()
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are of type {Formatter.Format(typeof(TType))}",
								(false, false) => $"is of type {Formatter.Format(typeof(TType))}",
								(true, true) => $"are not of type {Formatter.Format(typeof(TType))}",
								(false, true) => $"is not of type {Formatter.Format(typeof(TType))}",
							},
						a => a is TType,
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
			Are(Type type)
		{
			type.ThrowIfNull();
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new CollectionConstraint<TItem>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => (g.IsPlural(),
								g.IsNegated()) switch
							{
								(true, false) => $"are of type {Formatter.Format(type)}",
								(false, false) => $"is of type {Formatter.Format(type)}",
								(true, true) => $"are not of type {Formatter.Format(type)}",
								(false, true) => $"is not of type {Formatter.Format(type)}",
							},
						a => type.IsInstanceOfType(a),
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
			Are<TType>()
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
								(true, false) => $"are of type {Formatter.Format(typeof(TType))}",
								(false, false) => $"is of type {Formatter.Format(typeof(TType))}",
								(true, true) => $"are not of type {Formatter.Format(typeof(TType))}",
								(false, true) => $"is not of type {Formatter.Format(typeof(TType))}",
							},
						a => a is TType,
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>
			Are(Type type)
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
								(true, false) => $"are of type {Formatter.Format(type)}",
								(false, false) => $"is of type {Formatter.Format(type)}",
								(true, true) => $"are not of type {Formatter.Format(type)}",
								(false, true) => $"is not of type {Formatter.Format(type)}",
							},
						a => type.IsInstanceOfType(a),
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
			Are<TType>()
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
								(true, false) => $"are of type {Formatter.Format(typeof(TType))}",
								(false, false) => $"is of type {Formatter.Format(typeof(TType))}",
								(true, true) => $"are not of type {Formatter.Format(typeof(TType))}",
								(false, true) => $"is not of type {Formatter.Format(typeof(TType))}",
							},
						a => a is TType,
						"were")),
				_subject,
				options);
		}

		/// <summary>
		///     …are of type <paramref name="type" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>
			Are(Type type)
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
								(true, false) => $"are of type {Formatter.Format(type)}",
								(false, false) => $"is of type {Formatter.Format(type)}",
								(true, true) => $"are not of type {Formatter.Format(type)}",
								(false, true) => $"is not of type {Formatter.Format(type)}",
							},
						a => type.IsInstanceOfType(a),
						"were")),
				_subject,
				options);
		}
	}
}
