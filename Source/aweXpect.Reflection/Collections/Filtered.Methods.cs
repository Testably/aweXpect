using System.Linq;
using System.Reflection;
using aweXpect.Core;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="MethodInfo" />.
	/// </summary>
	public class Methods : Filtered<MethodInfo, Methods>, IDescribableSubject
	{
		private readonly string _description;
		private readonly Types? _types;

		/// <summary>
		///     Container for a filterable collection of <see cref="MethodInfo" />.
		/// </summary>
		internal Methods(Types types, string description) : base(types.SelectMany(type =>
			type.GetMethods(BindingFlags.DeclaredOnly |
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
			foreach (IFilter<MethodInfo> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_types is not null)
			{
				return description + _types.GetDescription();
			}

			return description;
		}

		/// <summary>
		///     Get all types of the filtered methods.
		/// </summary>
		public Types Types() => new(this);
	}
}
