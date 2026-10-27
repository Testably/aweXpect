using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="MethodInfo" />.
	/// </summary>
	public class Methods : Filtered<MethodInfo, Methods>, IDescribableSubject
	{
		private readonly Func<string> _description;
		private readonly bool _includeOperators;
		private readonly MemberScopes _memberScope;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="MethodInfo" />.
		/// </summary>
		internal Methods(Types types, string description, MemberScopes memberScope)
			: this(types, () => description, memberScope, false)
		{
		}

		private Methods(Types types, Func<string> description, MemberScopes memberScope, bool includeOperators)
			: base(types.SelectMany(type => type.GetDeclaredMethods(memberScope, includeOperators)))
		{
			_types = types;
			_description = description;
			_memberScope = memberScope;
			_includeOperators = includeOperators;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="methods" />.
		/// </summary>
		internal Methods(IEnumerable<MethodInfo?> methods, Func<string> description)
			: base(methods.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="MethodInfo" />.
		/// </summary>
		private protected Methods(Methods inner) : this(inner, inner.Filters)
		{
		}

		private Methods(Methods inner, IReadOnlyList<IFilter<MethodInfo>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_types = inner._types;
			_memberScope = inner._memberScope;
			_includeOperators = inner._includeOperators;
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			// Suffix filters end with a space that separates them from the outer description. A source without one,
			// e.g. In.Type, needs a space before them instead and drops the trailing one.
			string description = _types is null ? _description() + " " : _description();
			foreach (IFilter<MethodInfo> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_types is not null)
			{
				return description + _types.GetDescription().PrefixIn();
			}

			return description.TrimEnd();
		}

		/// <inheritdoc />
		private protected override Methods CloneWith(IReadOnlyList<IFilter<MethodInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Returns a copy of this collection that additionally includes operator special-name members, so that operator
		///     filters work even when operators are not opted in via <c>IncludedSpecialNameMembers</c>.
		/// </summary>
		/// <remarks>
		///     Operator inclusion is fixed when the underlying methods are gathered, so it cannot be toggled in place after
		///     construction. This rebuilds the collection from its source types - re-applying any filters already composed -
		///     with operators included. Collections that already include operators, or that were created from an explicit
		///     method list (and therefore have no source types to rebuild from), are returned unchanged.
		/// </remarks>
		internal Methods WithOperatorsIncluded()
		{
			if (_includeOperators || _types is null)
			{
				return this;
			}

			Methods rebuilt = new(_types, _description, _memberScope, true);
			return rebuilt.CloneWith([.. Filters,]);
		}

		/// <summary>
		///     Get all declaring types of the filtered methods.
		/// </summary>
		public Types DeclaringTypes() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="MethodInfo" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Methods, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, StringEqualityResult>
		{
			private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<MethodInfo>> _createFilter;
			private readonly OptionsHolder<StringEqualityOptions> _options;
			private readonly Methods _source;

			internal StringEqualityResult(Methods source, Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<MethodInfo>> createFilter,
				OptionsHolder<StringEqualityOptions> options) : base(source.Which(createFilter(options)))
			{
				_source = source;
				_createFilter = createFilter;
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;

			/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
			StringEqualityResult ICopyOnOption<StringEqualityOptions, StringEqualityResult>.WithOptions(
				StringEqualityOptions options)
				=> new(_source, _createFilter, new OptionsHolder<StringEqualityOptions>(options));
		}

		/// <summary>
		///     A container for the methods declared on types, that allows adding the inherited methods via
		///     <see cref="IncludingInherited" /> and also those declared on <see cref="object" />,
		///     <see cref="System.ValueType" /> and <see cref="System.Enum" /> via <see cref="IncludingObjectMembers" />.
		/// </summary>
		/// <remarks>
		///     Set these options directly after selecting the methods, as a filter returns a plain
		///     <see cref="Filtered.Methods" />.
		/// </remarks>
		public sealed class MemberScopeResult : Methods
		{
			private readonly IFilter<MethodInfo>? _memberFilter;
			private readonly MemberScopes _scope;
			private readonly string _sourceDescription;
			private readonly Types _sourceTypes;

			internal MemberScopeResult(Types types, string description, IFilter<MethodInfo>? filter,
				MemberScopes memberScope)
				: base(Create(types, description, filter, memberScope))
			{
				_sourceTypes = types;
				_sourceDescription = description;
				_memberFilter = filter;
				_scope = memberScope;
			}

			/// <summary>
			///     Also includes the inherited methods, but neither those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> or <see cref="System.Enum" /> nor those a derived type cannot access.
			/// </summary>
			/// <remarks>
			///     A method inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingInherited()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, _scope & ~MemberScopes.DeclaredOnly);

			/// <summary>
			///     Also includes the inherited methods, including those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> and <see cref="System.Enum" />.
			/// </summary>
			/// <remarks>
			///     A method inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingObjectMembers()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, MemberScopes.IncludingObjectMembers);

			private static Methods Create(Types types, string description, IFilter<MethodInfo>? filter,
				MemberScopes memberScope)
			{
				Methods methods = new(types, memberScope.DescribeListing(description), memberScope);
				return filter is null ? methods : methods.Which(filter);
			}
		}
	}
}
