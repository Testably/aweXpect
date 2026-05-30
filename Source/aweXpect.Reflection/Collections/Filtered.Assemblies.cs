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
		private readonly MemberFilterState _memberState = new();
		private readonly List<Func<Type, bool>> _typeFilters = [];
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
				_memberState.SetAccess(AccessModifiers.Public);
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
				_memberState.SetAccess(AccessModifiers.Private);
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
				_memberState.SetAccess(AccessModifiers.Protected);
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
				_memberState.SetAccess(AccessModifiers.Internal);
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
				_memberState.SetAccess(AccessModifiers.PrivateProtected);
				return this;
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.IProtected.Internal" />
		ITypeAssemblies ITypeAssemblies.IProtected.Internal
		{
			get
			{
				_memberState.SetAccess(AccessModifiers.ProtectedInternal);
				return this;
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.Abstract" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Abstract
		{
			get
			{
				_typeFilterDescription = (_typeFilterDescription ?? "") + "abstract ";
				_typeFilters.Add(type => type.IsReallyAbstract());
				_memberState.SetAbstract();
				return new LimitedAbstractSealedAssemblies(this);
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.Sealed" />
		public ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Sealed
		{
			get
			{
				_typeFilterDescription = (_typeFilterDescription ?? "") + "sealed ";
				_typeFilters.Add(type => type.IsReallySealed());
				_memberState.SetSealed();
				return new LimitedAbstractSealedAssemblies(this);
			}
		}

		/// <inheritdoc cref="ITypeAssemblies.Static" />
		public ILimitedStaticTypeAssemblies<ILimitedStaticTypeAssemblies> Static
		{
			get
			{
				_typeFilterDescription = (_typeFilterDescription ?? "") + "static ";
				_typeFilters.Add(type => type.IsReallyStatic());
				_memberState.SetStatic();
				return this;
			}
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Generic" />
		public ITypeAssemblies Generic
		{
			get
			{
				_typeFilterDescription = (_typeFilterDescription ?? "") + "generic ";
				_typeFilters.Add(type => type.IsGenericType);
				return this;
			}
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies{TLimitedTypeAssemblies}.Nested" />
		public ITypeAssemblies Nested
		{
			get
			{
				_typeFilterDescription = (_typeFilterDescription ?? "") + "nested ";
				_typeFilters.Add(type => type.IsNested);
				return this;
			}
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Types(AccessModifiers)" />
		public Types Types(AccessModifiers accessModifier = AccessModifiers.Any)
		{
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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
			AccessModifiers? implicitAccess = _memberState.AccessModifier;
			if (implicitAccess is not null)
			{
				_typeFilters.Add(type => type.HasAccessModifier(implicitAccess.Value));
				_typeFilterDescription = (_typeFilterDescription ?? "") + implicitAccess.Value.GetString(" ");
			}

			_memberState.Reset();

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

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Constructors()" />
		public Constructors Constructors()
		{
			Constructors constructors = new(new Types(this, ""), "constructors ");
			IFilter<ConstructorInfo>? filter = _memberState.BuildConstructorFilter();
			_memberState.Reset();
			return filter is null ? constructors : constructors.Which(filter);
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Events()" />
		public Events Events()
		{
			Events events = new(new Types(this, ""), "events ");
			IFilter<EventInfo>? filter = _memberState.BuildEventFilter();
			_memberState.Reset();
			return filter is null ? events : events.Which(filter);
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Fields()" />
		public Fields Fields()
		{
			Fields fields = new(new Types(this, ""), "fields ");
			IFilter<FieldInfo>? filter = _memberState.BuildFieldFilter();
			_memberState.Reset();
			return filter is null ? fields : fields.Which(filter);
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Methods()" />
		public Methods Methods()
		{
			Methods methods = new(new Types(this, ""), "methods ");
			IFilter<MethodInfo>? filter = _memberState.BuildMethodFilter();
			_memberState.Reset();
			return filter is null ? methods : methods.Which(filter);
		}

		/// <inheritdoc cref="ILimitedStaticTypeAssemblies.Properties()" />
		public Properties Properties()
		{
			Properties properties = new(new Types(this, ""), "properties ");
			IFilter<PropertyInfo>? filter = _memberState.BuildPropertyFilter();
			_memberState.Reset();
			return filter is null ? properties : properties.Which(filter);
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

		private sealed class LimitedAbstractSealedAssemblies
			: ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies>
		{
			private readonly Assemblies _inner;

			public LimitedAbstractSealedAssemblies(Assemblies inner)
			{
				_inner = inner;
			}

			public ILimitedAbstractSealedTypeAssemblies Generic
			{
				get
				{
					_ = _inner.Generic;
					return this;
				}
			}

			public ILimitedAbstractSealedTypeAssemblies Nested
			{
				get
				{
					_ = _inner.Nested;
					return this;
				}
			}

			public Types Types(AccessModifiers accessModifier = AccessModifiers.Any)
				=> _inner.Types(accessModifier);

			public Types Classes(AccessModifiers accessModifier = AccessModifiers.Any)
				=> _inner.Classes(accessModifier);

			public Types Records(AccessModifiers accessModifier = AccessModifiers.Any)
				=> _inner.Records(accessModifier);

			public Events Events() => _inner.Events();
			public Methods Methods() => _inner.Methods();
			public Properties Properties() => _inner.Properties();
		}
	}
}
