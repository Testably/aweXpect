using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace aweXpect.Reflection.Collections;

public static partial class Filtered
{
	/// <summary>
	///     Container for a filterable collection of <see cref="Assembly" />.
	/// </summary>
	public class Assemblies : Filtered<Assembly, Assemblies>,
		IDescribableSubject,
		ITypeAssemblies.IProtected,
		ITypeAssemblies.IPrivate
	{
		private readonly string _description;
		private readonly List<Func<Type, bool>> _typeFilters = [];
		private AccessModifiers? _implicitAccessModifier;
		private string? _typeFilterDescription;

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		public Assemblies(IEnumerable<Assembly?> source, string description)
			: base(source.WhereNotNull())
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		public Assemblies(Assembly? source, string description) : this([source,], description)
		{
			_description = description;
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		internal Assemblies(Types types) : base(types.Select(type => type.Assembly).Distinct())
		{
			_description = "assemblies containing " + types.GetDescription();
		}

		/// <summary>
		///     Container for a filterable collection of <see cref="Assembly" />.
		/// </summary>
		protected Assemblies(Assemblies inner) : base(inner, inner.Filters)
		{
			_description = inner._description;
		}

		/// <summary>
		///     Filters for public types.
		/// </summary>
		public ITypeAssemblies Public
		{
			get
			{
				_implicitAccessModifier = AccessModifiers.Public;
				return this;
			}
		}

		/// <summary>
		///     Filters for private types.
		/// </summary>
		public ITypeAssemblies.IPrivate Private
		{
			get
			{
				_implicitAccessModifier = AccessModifiers.Private;
				return this;
			}
		}

		/// <summary>
		///     Filters for protected types.
		/// </summary>
		public ITypeAssemblies.IProtected Protected
		{
			get
			{
				_implicitAccessModifier = AccessModifiers.Protected;
				return this;
			}
		}

		/// <summary>
		///     Filters for internal types.
		/// </summary>
		public ITypeAssemblies Internal
		{
			get
			{
				_implicitAccessModifier = AccessModifiers.Internal;
				return this;
			}
		}

		/// <inheritdoc />
		public string GetDescription()
		{
			string description = _description;
			foreach (IFilter<Assembly> filter in Filters)
			{
				description = filter.Describes(description);
			}

			return description;
		}

		/// <inheritdoc cref="ITypeAssemblies.IPrivate.Protected" />
		ITypeAssemblies ITypeAssemblies.IPrivate.Protected
		{
			get
			{
				_implicitAccessModifier = AccessModifiers.PrivateProtected;
				return this;
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.IProtected.Internal" />
		ITypeAssemblies ITypeAssemblies.IProtected.Internal
		{
			get
			{
				_implicitAccessModifier = AccessModifiers.ProtectedInternal;
				return this;
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.Abstract" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Abstract
		{
			get
			{
				_typeFilterDescription = _typeFilterDescription switch
				{
					null => "abstract ",
					_ => _typeFilterDescription + "abstract ",
				};
				_typeFilters.Add(type => type.IsReallyAbstract());
				return this;
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.Sealed" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Sealed
		{
			get
			{
				_typeFilterDescription = _typeFilterDescription switch
				{
					null => "sealed ",
					_ => _typeFilterDescription + "sealed ",
				};
				_typeFilters.Add(type => type.IsReallySealed());
				return this;
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.Static" />
		public ILimitedStaticTypeAssemblies<ILimitedStaticTypeAssemblies> Static
		{
			get
			{
				_typeFilterDescription = _typeFilterDescription switch
				{
					null => "static ",
					_ => _typeFilterDescription + "static ",
				};
				_typeFilters.Add(type => type.IsReallyStatic());
				return this;
			}
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Generic" />
		public ITypeAssemblies Generic
		{
			get
			{
				_typeFilterDescription = _typeFilterDescription switch
				{
					null => "generic ",
					_ => _typeFilterDescription + "generic ",
				};
				_typeFilters.Add(type => type.IsGenericType);
				return this;
			}
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Nested" />
		public ITypeAssemblies Nested
		{
			get
			{
				_typeFilterDescription = _typeFilterDescription switch
				{
					null => "nested ",
					_ => _typeFilterDescription + "nested ",
				};
				_typeFilters.Add(type => type.IsNested);
				return this;
			}
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Types(AccessModifiers)" />
		public Types Types(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			if (_typeFilters.Any())
			{
				return new Types(this, "types ")
					.Which(Filter.Prefix<Type>(
						type => _typeFilters.All(predicate => predicate.Invoke(type)),
						_typeFilterDescription ?? ""));
			}

			return new Types(this, "types ");
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Classes(AccessModifiers)" />
		public Types Classes(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			_typeFilters.Add(type => type.IsReallyClass());
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			return new Types(this, "classes ")
				.Which(Filter.Prefix<Type>(
					type => _typeFilters.All(predicate => predicate.Invoke(type)),
					_typeFilterDescription ?? ""));
		}

		/// <inheritdoc cref="ILimitedAbstractSealedTypeAssemblies.Records(AccessModifiers)" />
		public Types Records(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			_typeFilters.Add(type => type.IsRecordClass());
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			return new Types(this, "records ")
				.Which(Filter.Prefix<Type>(
					type => _typeFilters.All(predicate => predicate.Invoke(type)),
					_typeFilterDescription ?? ""));
		}

		/// <inheritdoc cref="ITypeAssemblies.RecordStructs(AccessModifiers)" />
		public Types RecordStructs(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			_typeFilters.Add(type => type.IsRecordStruct());
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			return new Types(this, "record structs ")
				.Which(Filter.Prefix<Type>(
					type => _typeFilters.All(predicate => predicate.Invoke(type)),
					_typeFilterDescription ?? ""));
		}

		/// <inheritdoc cref="ITypeAssemblies.Structs(AccessModifiers)" />
		public Types Structs(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			_typeFilters.Add(type => type.IsReallyStruct());
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			return new Types(this, "structs ")
				.Which(Filter.Prefix<Type>(
					type => _typeFilters.All(predicate => predicate.Invoke(type)),
					_typeFilterDescription ?? ""));
		}

		/// <inheritdoc cref="ITypeAssemblies.Interfaces(AccessModifiers)" />
		public Types Interfaces(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			_typeFilters.Add(type => type.IsInterface);
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			return new Types(this, "interfaces ")
				.Which(Filter.Prefix<Type>(
					type => _typeFilters.All(predicate => predicate.Invoke(type)),
					_typeFilterDescription ?? ""));
		}

		/// <inheritdoc cref="ITypeAssemblies.Enums(AccessModifiers)" />
		public Types Enums(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			_typeFilters.Add(type => type.IsEnum);
			if (_implicitAccessModifier is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(_implicitAccessModifier.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + _implicitAccessModifier.Value.GetString(" ");
			}

			if (accessModifier != AccessModifiers.Any)
			{
				_typeFilters.Add(type => type.HasAccessModifier(accessModifier));
				_typeFilterDescription = accessModifier.GetString(" ") + (_typeFilterDescription ?? "");
			}

			return new Types(this, "enums ")
				.Which(Filter.Prefix<Type>(
					type => _typeFilters.All(predicate => predicate.Invoke(type)),
					_typeFilterDescription ?? ""));
		}

		/// <inheritdoc cref="ITypeAssemblies.Constructors()" />
		public Constructors Constructors()
		{
			if (_implicitAccessModifier is not null)
			{
				List<Func<ConstructorInfo, bool>> filters = [];
				filters.Add(constructor => constructor.HasAccessModifier(_implicitAccessModifier.Value));
				string filterDescription = _implicitAccessModifier.Value.GetString(" ");

				return new Constructors(new Types(this, ""), "constructors ")
					.Which(Filter.Prefix<ConstructorInfo>(
						constructor => filters.All(predicate => predicate.Invoke(constructor)),
						filterDescription));
			}

			return new Constructors(new Types(this, ""), "constructors ");
		}

		/// <inheritdoc cref="ITypeAssemblies.Events()" />
		public Events Events()
		{
			if (_implicitAccessModifier is not null)
			{
				List<Func<EventInfo, bool>> filters = [];
				filters.Add(constructor => constructor.HasAccessModifier(_implicitAccessModifier.Value));
				string filterDescription = _implicitAccessModifier.Value.GetString(" ");

				return new Events(new Types(this, ""), "events ")
					.Which(Filter.Prefix<EventInfo>(
						@event => filters.All(predicate => predicate.Invoke(@event)),
						filterDescription));
			}

			return new Events(new Types(this, ""), "events ");
		}

		/// <inheritdoc cref="ITypeAssemblies.Fields()" />
		public Fields Fields()
		{
			if (_implicitAccessModifier is not null)
			{
				List<Func<FieldInfo, bool>> filters = [];
				filters.Add(constructor => constructor.HasAccessModifier(_implicitAccessModifier.Value));
				string filterDescription = _implicitAccessModifier.Value.GetString(" ");

				return new Fields(new Types(this, ""), "fields ")
					.Which(Filter.Prefix<FieldInfo>(
						field => filters.All(predicate => predicate.Invoke(field)),
						filterDescription));
			}

			return new Fields(new Types(this, ""), "fields ");
		}

		/// <inheritdoc cref="ITypeAssemblies.Methods()" />
		public Methods Methods()
		{
			if (_implicitAccessModifier is not null)
			{
				List<Func<MethodInfo, bool>> filters = [];
				filters.Add(constructor => constructor.HasAccessModifier(_implicitAccessModifier.Value));
				string filterDescription = _implicitAccessModifier.Value.GetString(" ");

				return new Methods(new Types(this, ""), "methods ")
					.Which(Filter.Prefix<MethodInfo>(
						method => filters.All(predicate => predicate.Invoke(method)),
						filterDescription));
			}

			return new Methods(new Types(this, ""), "methods ");
		}

		/// <inheritdoc cref="ITypeAssemblies.Properties()" />
		public Properties Properties()
		{
			if (_implicitAccessModifier is not null)
			{
				List<Func<PropertyInfo, bool>> filters = [];
				filters.Add(constructor => constructor.HasAccessModifier(_implicitAccessModifier.Value));
				string filterDescription = _implicitAccessModifier.Value.GetString(" ");

				return new Properties(new Types(this, ""), "properties ")
					.Which(Filter.Prefix<PropertyInfo>(
						property => filters.All(predicate => predicate.Invoke(property)),
						filterDescription));
			}

			return new Properties(new Types(this, ""), "properties ");
		}

		/// <summary>
		///     A Container for a filterable collection of <see cref="Assembly" />,
		///     that also allows specifying string equality options.
		/// </summary>
		public class StringEqualityResult : Assemblies
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResult(Assemblies inner, StringEqualityOptions options) : base(inner)
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
		///     A Container for a filterable collection of <see cref="Assembly" />,
		///     that also allows specifying string equality options and types.
		/// </summary>
		public class StringEqualityResultType : StringEqualityResult
		{
			private readonly StringEqualityOptions _options;

			internal StringEqualityResultType(Assemblies inner, StringEqualityOptions options) : base(inner, options)
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
