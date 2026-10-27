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
	///     Container for a filterable collection of <see cref="FieldInfo" />.
	/// </summary>
	public class Fields : Filtered<FieldInfo, Fields>, IDescribableSubject
	{
		private readonly Func<string> _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="FieldInfo" />.
		/// </summary>
		internal Fields(Types types, string description, MemberScopes memberScope)
			: base(types.SelectMany(type => type.GetDeclaredFields(memberScope)))
		{
			_types = types;
			_description = () => description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="fields" />.
		/// </summary>
		internal Fields(IEnumerable<FieldInfo?> fields, Func<string> description)
			: base(fields.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="FieldInfo" />.
		/// </summary>
		private protected Fields(Fields inner) : this(inner, inner.Filters)
		{
		}

		private Fields(Fields inner, IReadOnlyList<IFilter<FieldInfo>> filters) : base(inner, filters)
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
			foreach (IFilter<FieldInfo> filter in Filters)
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
		private protected override Fields CloneWith(IReadOnlyList<IFilter<FieldInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Get all declaring types of the filtered fields.
		/// </summary>
		public Types DeclaringTypes() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="FieldInfo" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Fields, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, StringEqualityResult>
		{
			private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<FieldInfo>> _createFilter;
			private readonly OptionsHolder<StringEqualityOptions> _options;
			private readonly Fields _source;

			internal StringEqualityResult(Fields source, Func<IReadOnlyOptionsHolder<StringEqualityOptions>, IFilter<FieldInfo>> createFilter,
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
		///     A container for the fields declared on types, that allows adding the inherited fields via
		///     <see cref="IncludingInherited" /> and also those declared on <see cref="object" />,
		///     <see cref="System.ValueType" /> and <see cref="System.Enum" /> via <see cref="IncludingObjectMembers" />.
		/// </summary>
		/// <remarks>
		///     Set these options directly after selecting the fields, as a filter returns a plain
		///     <see cref="Filtered.Fields" />.
		/// </remarks>
		public sealed class MemberScopeResult : Fields
		{
			private readonly IFilter<FieldInfo>? _memberFilter;
			private readonly MemberScopes _scope;
			private readonly string _sourceDescription;
			private readonly Types _sourceTypes;

			internal MemberScopeResult(Types types, string description, IFilter<FieldInfo>? filter,
				MemberScopes memberScope)
				: base(Create(types, description, filter, memberScope))
			{
				_sourceTypes = types;
				_sourceDescription = description;
				_memberFilter = filter;
				_scope = memberScope;
			}

			/// <summary>
			///     Also includes the inherited fields, but neither those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> or <see cref="System.Enum" /> nor those a derived type cannot access.
			/// </summary>
			/// <remarks>
			///     A field inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingInherited()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, _scope & ~MemberScopes.DeclaredOnly);

			/// <summary>
			///     Also includes the inherited fields, including those declared on <see cref="object" />,
			///     <see cref="System.ValueType" /> and <see cref="System.Enum" />.
			/// </summary>
			/// <remarks>
			///     A field inherited from a base type is included once for each type that inherits it.
			/// </remarks>
			public MemberScopeResult IncludingObjectMembers()
				=> new(_sourceTypes, _sourceDescription, _memberFilter, MemberScopes.IncludingObjectMembers);

			private static Fields Create(Types types, string description, IFilter<FieldInfo>? filter,
				MemberScopes memberScope)
			{
				Fields fields = new(types, memberScope.DescribeListing(description), memberScope);
				return filter is null ? fields : fields.Which(filter);
			}
		}
	}
}
