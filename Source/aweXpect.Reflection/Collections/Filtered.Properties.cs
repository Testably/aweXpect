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
	///     Container for a filterable collection of <see cref="PropertyInfo" />.
	/// </summary>
	public class Properties : Filtered<PropertyInfo, Properties>, IDescribableSubject
	{
		private readonly Func<string> _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="PropertyInfo" />.
		/// </summary>
		internal Properties(Types types, string description, MemberScopes memberScope)
			: base(types.SelectMany(type => type.GetDeclaredProperties(memberScope)))
		{
			_types = types;
			_description = () => description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="properties" />.
		/// </summary>
		internal Properties(IEnumerable<PropertyInfo?> properties, Func<string> description)
			: base(properties.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="PropertyInfo" />.
		/// </summary>
		private protected Properties(Properties inner) : this(inner, inner.Filters)
		{
		}

		private Properties(Properties inner, IReadOnlyList<IFilter<PropertyInfo>> filters) : base(inner, filters)
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
			foreach (IFilter<PropertyInfo> filter in Filters)
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
		private protected override Properties CloneWith(IReadOnlyList<IFilter<PropertyInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Get all declaring types of the filtered properties.
		/// </summary>
		public Types DeclaringTypes() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="PropertyInfo" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Properties, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, StringEqualityResult>
		{
			private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<PropertyInfo>> _createFilter;
			private readonly OptionsHolder<StringEqualityOptions> _options;
			private readonly Properties _source;

			internal StringEqualityResult(Properties source, Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<PropertyInfo>> createFilter,
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
		///     A container for the properties declared on types, that allows adding the inherited properties via
		///     <see cref="IncludingInherited" /> and also those declared on <see cref="object" />,
		///     <see cref="System.ValueType" /> and <see cref="System.Enum" /> via <see cref="IncludingObjectMembers" />.
		/// </summary>
		/// <remarks>
		///     Set these options directly after selecting the properties, as a filter returns a plain
		///     <see cref="Filtered.Properties" />.
		/// </remarks>
		public sealed class MemberScopeResult : Properties
		{
			private readonly IFilter<PropertyInfo>? _memberFilter;
			private readonly MemberScopes _scope;
			private readonly string _sourceDescription;
			private readonly Types _sourceTypes;

			internal MemberScopeResult(Types types, string description, IFilter<PropertyInfo>? filter,
				MemberScopes memberScope)
				: base(Create(types, description, filter, memberScope))
			{
				_sourceTypes = types;
				_sourceDescription = description;
				_memberFilter = filter;
				_scope = memberScope;
			}

			/// <summary>
			///     Also includes the inherited properties, but neither those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> or <see cref="System.Enum" /> nor those a derived type cannot access.
			/// </summary>
			/// <remarks>
			///     A property inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingInherited()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, _scope & ~MemberScopes.DeclaredOnly);

			/// <summary>
			///     Also includes the inherited properties, including those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> and <see cref="System.Enum" />.
			/// </summary>
			/// <remarks>
			///     A property inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingObjectMembers()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, MemberScopes.IncludingObjectMembers);

			private static Properties Create(Types types, string description, IFilter<PropertyInfo>? filter,
				MemberScopes memberScope)
			{
				Properties properties = new(types, memberScope.DescribeListing(description), memberScope);
				return filter is null ? properties : properties.Which(filter);
			}
		}
	}
}
