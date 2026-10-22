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
		private readonly string _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="EventInfo" />.
		/// </summary>
		internal Events(Types types, string description,
			MemberScope memberScope = MemberScope.DeclaredOnly) : base(types.SelectMany(type =>
			type.GetDeclaredEvents(memberScope)))
		{
			_types = types;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="events" />.
		/// </summary>
		internal Events(IEnumerable<EventInfo> events, string description)
			: base(events.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="EventInfo" />.
		/// </summary>
		protected Events(Events inner) : this(inner, inner.Filters)
		{
		}

		private Events(Events inner, List<IFilter<EventInfo>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_types = inner._types;
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<EventInfo> filter in Filters)
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
		private protected override Events CloneWith(List<IFilter<EventInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Get all declaring types of the filtered events.
		/// </summary>
		public Types DeclaringTypes() => new(this);

		/// <summary>
		///     A Container for a filterable collection of <see cref="EventInfo" />,
		///     that also allows specifying string equality options via <see cref="StringEqualityOptionsExtensions" />.
		/// </summary>
		public class StringEqualityResult : Events, IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Events inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
			StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
		}
	}
}
