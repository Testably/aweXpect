using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="ConstructorInfo" />.
	/// </summary>
	public class Constructors : Filtered<ConstructorInfo, Constructors>, IDescribableSubject
	{
		private readonly string _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="ConstructorInfo" />.
		/// </summary>
		internal Constructors(Types types, string description) : base(types.SelectMany(type =>
			type.GetDeclaredConstructors()))
		{
			_types = types;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of the given <paramref name="constructors" />.
		/// </summary>
		internal Constructors(IEnumerable<ConstructorInfo> constructors, string description)
			: base(constructors.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="ConstructorInfo" />.
		/// </summary>
		protected Constructors(Constructors inner) : this(inner, inner.Filters)
		{
		}

		private Constructors(Constructors inner, List<IFilter<ConstructorInfo>> filters) : base(inner, filters)
		{
			_description = inner._description;
			_types = inner._types;
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<ConstructorInfo> filter in Filters)
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
		private protected override Constructors CloneWith(List<IFilter<ConstructorInfo>> filters)
			=> new(this, filters);

		/// <summary>
		///     Get all declaring types of the filtered constructors.
		/// </summary>
		public Types DeclaringTypes() => new(this);
	}
}
