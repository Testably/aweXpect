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
		private readonly string _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="FieldInfo" />.
		/// </summary>
		internal Fields(Types types, string description,
			MemberScope memberScope = MemberScope.DeclaredOnly) : base(types.SelectMany(type =>
			type.GetDeclaredFields(memberScope)))
		{
			_types = types;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="fields" />.
		/// </summary>
		internal Fields(IEnumerable<FieldInfo?> fields, string description)
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
			string description = _description;
			foreach (IFilter<FieldInfo> filter in Filters)
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
		public class StringEqualityResult : Fields, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Fields inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
		}
	}
}
