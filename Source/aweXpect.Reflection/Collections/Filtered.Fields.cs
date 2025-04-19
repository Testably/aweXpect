using System.Linq;
using System.Reflection;
using aweXpect.Core;

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
		internal Fields(Types types, string description) : base(types.SelectMany(type =>
			type.GetFields(BindingFlags.DeclaredOnly |
			               BindingFlags.NonPublic |
			               BindingFlags.Public |
			               BindingFlags.Instance)))
		{
			_types = types;
			_description = description;
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
				return description + "in " + _types.GetDescription();
			}

			return description;
		}

		/// <summary>
		///     Get all types of the filtered fields.
		/// </summary>
		public Types Types() => new(this);
	}
}
