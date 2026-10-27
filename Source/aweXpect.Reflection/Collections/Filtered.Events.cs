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
	///     Container for a filterable collection of <see cref="EventInfo" />.
	/// </summary>
	public class Events : Filtered<EventInfo, Events>, IDescribableSubject
	{
		private readonly Func<string> _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="EventInfo" />.
		/// </summary>
		internal Events(Types types, string description, MemberScopes memberScope)
			: base(types.SelectMany(type => type.GetDeclaredEvents(memberScope)))
		{
			_types = types;
			_description = () => description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="events" />.
		/// </summary>
		internal Events(IEnumerable<EventInfo?> events, Func<string> description)
			: base(events.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="EventInfo" />.
		/// </summary>
		private protected Events(Events inner) : this(inner, inner.Filters)
		{
		}

		private Events(Events inner, IReadOnlyList<IFilter<EventInfo>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_types = inner._types;
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			// Suffix filters end with a space that separates them from the outer description. A source without one,
			// e.g. In.Type, needs a space before them instead and drops the trailing one.
			string description = _types is null ? _description() + " " : _description();
			foreach (IFilter<EventInfo> filter in Filters)
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
		private protected override Events CloneWith(IReadOnlyList<IFilter<EventInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Get all declaring types of the filtered events.
		/// </summary>
		public Types DeclaringTypes() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="EventInfo" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Events, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, StringEqualityResult>
		{
			private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<EventInfo>> _createFilter;
			private readonly OptionsHolder<StringEqualityOptions> _options;
			private readonly Events _source;

			internal StringEqualityResult(Events source, Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<EventInfo>> createFilter,
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
		///     A container for the events declared on types, that allows adding the inherited events via
		///     <see cref="IncludingInherited" /> and also those declared on <see cref="object" />,
		///     <see cref="System.ValueType" /> and <see cref="System.Enum" /> via <see cref="IncludingObjectMembers" />.
		/// </summary>
		/// <remarks>
		///     Set these options directly after selecting the events, as a filter returns a plain
		///     <see cref="Filtered.Events" />.
		/// </remarks>
		public sealed class MemberScopeResult : Events
		{
			private readonly IFilter<EventInfo>? _memberFilter;
			private readonly MemberScopes _scope;
			private readonly string _sourceDescription;
			private readonly Types _sourceTypes;

			internal MemberScopeResult(Types types, string description, IFilter<EventInfo>? filter,
				MemberScopes memberScope)
				: base(Create(types, description, filter, memberScope))
			{
				_sourceTypes = types;
				_sourceDescription = description;
				_memberFilter = filter;
				_scope = memberScope;
			}

			/// <summary>
			///     Also includes the inherited events, but neither those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> or <see cref="System.Enum" /> nor those a derived type cannot access.
			/// </summary>
			/// <remarks>
			///     An event inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingInherited()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, _scope & ~MemberScopes.DeclaredOnly);

			/// <summary>
			///     Also includes the inherited events, including those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> and <see cref="System.Enum" />.
			/// </summary>
			/// <remarks>
			///     An event inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingObjectMembers()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, MemberScopes.IncludingObjectMembers);

			private static Events Create(Types types, string description, IFilter<EventInfo>? filter,
				MemberScopes memberScope)
			{
				Events events = new(types, memberScope.DescribeListing(description), memberScope);
				return filter is null ? events : events.Which(filter);
			}
		}
	}
}
