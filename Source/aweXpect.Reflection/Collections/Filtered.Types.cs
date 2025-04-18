using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;

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

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		protected Types(Types inner) : base(inner, inner.Filters)
		{
			_description = inner._description;
			_assemblies = inner._assemblies;
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

		/// <summary>
		///     A Container for a filterable collection of <see cref="Type" />,
		///     that also allows specifying string equality options.
		/// </summary>
		public class StringEqualityResult : Types
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Types inner, StringEqualityOptions options) : base(inner)
			{
				_options = options;
			}

			/// <summary>
			///     Ignores casing when comparing the <see langword="string" />s,
			///     according to the <paramref name="ignoreCase" /> parameter.
			/// </summary>
			public StringEqualityResult IgnoringCase(bool ignoreCase = true)
			{
				_options.IgnoringCase(ignoreCase);
				return this;
			}

			/// <summary>
			///     Ignores leading white-space when comparing <see langword="string" />s,
			///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
			/// </summary>
			/// <remarks>
			///     Note:<br />
			///     This affects the index of first mismatch, as the removed whitespace is also ignored for the index calculation!
			/// </remarks>
			public StringEqualityResult IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
			{
				_options.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);
				return this;
			}

			/// <summary>
			///     Ignores trailing white-space when comparing <see langword="string" />s,
			///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
			/// </summary>
			public StringEqualityResult IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
			{
				_options.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);
				return this;
			}

			/// <summary>
			///     Uses the provided <paramref name="comparer" /> for comparing <see langword="string" />s.
			/// </summary>
			public StringEqualityResult Using(IEqualityComparer<string> comparer)
			{
				_options.UsingComparer(comparer);
				return this;
			}
		}

		/// <summary>
		///     A Container for a filterable collection of <see cref="Type" />,
		///     that also allows specifying string equality options and types.
		/// </summary>
		public class StringEqualityResultType : StringEqualityResult
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResultType(Types inner, StringEqualityOptions options) : base(inner, options)
			{
				_options = options;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> to be exactly equal.
			/// </summary>
			public StringEqualityResult Exactly()
			{
				_options.Exactly();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as a prefix, so that the actual value starts with it.
			/// </summary>
			public StringEqualityResult AsPrefix()
			{
				_options.AsPrefix();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as <see cref="Regex" /> pattern.
			/// </summary>
			public StringEqualityResult AsRegex()
			{
				_options.AsRegex();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as a suffix, so that the actual value ends with it.
			/// </summary>
			public StringEqualityResult AsSuffix()
			{
				_options.AsSuffix();
				return this;
			}

			/// <summary>
			///     Interprets the expected <see langword="string" /> as wildcard pattern.<br />
			///     Supports * to match zero or more characters and ? to match exactly one character.
			/// </summary>
			public StringEqualityResult AsWildcard()
			{
				_options.AsWildcard();
				return this;
			}
		}
	}
}
