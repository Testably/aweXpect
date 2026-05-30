using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
	public class Types : Filtered<Type, Types>,
		IDescribableSubject,
		IMembers.IPrivate,
		IMembers.IProtected,
		ILimitedAbstractSealedMembers
	{
		private const string DeclaringTypesPrefix = "declaring types of ";

		private readonly Assemblies? _assemblies;
		private readonly string _description;
		private readonly MemberFilterState _memberState = new();

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		public Types(IEnumerable<Type?> source, string description)
			: base(source.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Assemblies assemblies, string description) : base(
			assemblies.SelectMany(GetLoadableTypes))
		{
			_assemblies = assemblies;
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Constructors constructors) : base(constructors
			.Select(constructorInfo => constructorInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + constructors.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Properties properties) : base(properties
			.Select(propertyInfo => propertyInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + properties.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Methods methods) : base(methods
			.Select(methodInfo => methodInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + methods.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Fields fields) : base(fields
			.Select(fieldInfo => fieldInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + fields.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		internal Types(Events events) : base(events
			.Select(eventInfo => eventInfo.DeclaringType)
			.WhereNotNull()
			.Distinct())
		{
			_description = DeclaringTypesPrefix + events.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Type" />.
		/// </summary>
		protected Types(Types inner) : base(inner, inner.Filters)
		{
			_description = inner._description;
			_assemblies = inner._assemblies;
		}

		/// <summary>
		///     Filters for public members.
		/// </summary>
		public IMembers Public
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.Public);
				return this;
			}
		}

		/// <summary>
		///     Filters for private members.
		/// </summary>
		public IMembers.IPrivate Private
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.Private);
				return this;
			}
		}

		/// <summary>
		///     Filters for protected members.
		/// </summary>
		public IMembers.IProtected Protected
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.Protected);
				return this;
			}
		}

		/// <summary>
		///     Filters for internal members.
		/// </summary>
		public IMembers Internal
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.Internal);
				return this;
			}
		}

		/// <summary>
		///     Filters for static members.
		/// </summary>
		public IMemberSelectors Static
		{
			get
			{
				_memberState.SetStatic();
				return this;
			}
		}

		/// <summary>
		///     Filters for abstract members.
		/// </summary>
		public ILimitedAbstractSealedMembers Abstract
		{
			get
			{
				_memberState.SetAbstract();
				return this;
			}
		}

		/// <summary>
		///     Filters for sealed members.
		/// </summary>
		public ILimitedAbstractSealedMembers Sealed
		{
			get
			{
				_memberState.SetSealed();
				return this;
			}
		}

		/// <inheritdoc cref="IMembers.IPrivate.Protected" />
		IMembers IMembers.IPrivate.Protected
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.PrivateProtected);
				return this;
			}
		}

		/// <inheritdoc cref="IMembers.IProtected.Internal" />
		IMembers IMembers.IProtected.Internal
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.ProtectedInternal);
				return this;
			}
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
		///     Gets the types of the <paramref name="assembly" />, ignoring those that fail to load.
		/// </summary>
		/// <remarks>
		///     <see cref="Assembly.GetTypes()" /> throws a <see cref="ReflectionTypeLoadException" /> when any type cannot be
		///     loaded (e.g. an unresolvable dependency). The exception still exposes the types that loaded successfully via
		///     <see cref="ReflectionTypeLoadException.Types" />, so we fall back to those instead of failing the whole query.
		/// </remarks>
		private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException exception)
			{
				return exception.Types.Where(type => type is not null)!;
			}
		}

		/// <summary>
		///     Get all assemblies of the filtered types.
		/// </summary>
		public Assemblies Assemblies() => new(this);

		/// <summary>
		///     Get all constructors in the filtered types.
		/// </summary>
		public Constructors Constructors()
		{
			Constructors constructors = new(this, "constructors ");
			IFilter<ConstructorInfo>? filter = _memberState.BuildConstructorFilter();
			_memberState.Reset();
			return filter is null ? constructors : constructors.Which(filter);
		}

		/// <summary>
		///     Get all events in the filtered types.
		/// </summary>
		public Events Events()
		{
			Events events = new(this, "events ");
			IFilter<EventInfo>? filter = _memberState.BuildEventFilter();
			_memberState.Reset();
			return filter is null ? events : events.Which(filter);
		}

		/// <summary>
		///     Get all fields in the filtered types.
		/// </summary>
		public Fields Fields()
		{
			Fields fields = new(this, "fields ");
			IFilter<FieldInfo>? filter = _memberState.BuildFieldFilter();
			_memberState.Reset();
			return filter is null ? fields : fields.Which(filter);
		}

		/// <summary>
		///     Get all methods in the filtered types.
		/// </summary>
		public Methods Methods()
		{
			Methods methods = new(this, "methods ");
			IFilter<MethodInfo>? filter = _memberState.BuildMethodFilter();
			_memberState.Reset();
			return filter is null ? methods : methods.Which(filter);
		}

		/// <summary>
		///     Get all properties in the filtered types.
		/// </summary>
		public Properties Properties()
		{
			Properties properties = new(this, "properties ");
			IFilter<PropertyInfo>? filter = _memberState.BuildPropertyFilter();
			_memberState.Reset();
			return filter is null ? properties : properties.Which(filter);
		}

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
