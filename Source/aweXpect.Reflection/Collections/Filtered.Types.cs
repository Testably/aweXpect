using System;
using System.Linq;
using aweXpect.Core;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="Type" />.
	/// </summary>
	public class Types : Filtered<Type, Types>, IDescribableSubject
	{
		private const string TypesSuffix = " types";

		private readonly Assemblies? _assemblies;
		private readonly string _description;

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Assemblies assemblies, string description) : base(
			assemblies.SelectMany(assembly => assembly.GetTypes()))
		{
			_assemblies = assemblies;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Constructors constructors) : base(constructors
			.Select(constructorInfo => constructorInfo.DeclaringType)
			.Where(x => x is not null)
			.Cast<Type>()
			.Distinct())
		{
			_description = constructors.GetDescription() + TypesSuffix;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Properties properties) : base(properties
			.Select(propertyInfo => propertyInfo.DeclaringType)
			.Where(x => x is not null)
			.Cast<Type>()
			.Distinct())
		{
			_description = properties.GetDescription() + TypesSuffix;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Methods methods) : base(methods
			.Select(methodInfo => methodInfo.DeclaringType)
			.Where(x => x is not null)
			.Cast<Type>()
			.Distinct())
		{
			_description = methods.GetDescription() + TypesSuffix;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Fields fields) : base(fields
			.Select(fieldInfo => fieldInfo.DeclaringType)
			.Where(x => x is not null)
			.Cast<Type>()
			.Distinct())
		{
			_description = fields.GetDescription() + TypesSuffix;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Events events) : base(events
			.Select(eventInfo => eventInfo.DeclaringType)
			.Where(x => x is not null)
			.Cast<Type>()
			.Distinct())
		{
			_description = events.GetDescription() + TypesSuffix;
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<Type> filter in Filters)
			{
				description = filter.Describes(description);
			}

			if (_assemblies is not null)
			{
				return description + _assemblies.GetDescription();
			}

			return description;
		}

		/// <summary>
		///     Get all assemblies of the filtered types.
		/// </summary>
		public Assemblies Assemblies() => new(this);

		/// <summary>
		///     Get all constructors in the filtered types.
		/// </summary>
		public Constructors Constructors() => new(this, "constructors ");

		/// <summary>
		///     Get all events in the filtered types.
		/// </summary>
		public Events Events() => new(this, "events ");

		/// <summary>
		///     Get all fields in the filtered types.
		/// </summary>
		public Fields Fields() => new(this, "fields ");

		/// <summary>
		///     Get all methods in the filtered types.
		/// </summary>
		public Methods Methods() => new(this, "methods ");

		/// <summary>
		///     Get all properties in the filtered types.
		/// </summary>
		public Properties Properties() => new(this, "properties ");
	}
}
