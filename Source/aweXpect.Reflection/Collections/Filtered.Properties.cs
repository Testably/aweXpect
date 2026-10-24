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
		private readonly string _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="PropertyInfo" />.
		/// </summary>
		internal Properties(Types types, string description,
			MemberScope memberScope = MemberScope.DeclaredOnly) : base(types.SelectMany(type =>
			type.GetDeclaredProperties(memberScope)))
		{
			_types = types;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="properties" />.
		/// </summary>
		internal Properties(IEnumerable<PropertyInfo?> properties, string description)
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
			string description = _description;
			foreach (IFilter<PropertyInfo> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_types is not null)
			{
				return description + _types.GetDescription().PrefixIn();
			}

			return description;
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
		public class StringEqualityResult : Properties, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Properties inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
		}
	}
}
