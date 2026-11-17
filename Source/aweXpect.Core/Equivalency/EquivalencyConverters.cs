using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Core.Metadata;

namespace aweXpect.Equivalency;

/// <summary>
///     Converts an expected value to the type of the actual value with the <see cref="TypeConverter" /> that the type of
///     the expected value declares with a <see cref="TypeConverterAttribute" />.
/// </summary>
/// <remarks>
///     Only a converter declared on the type itself is used, never one that <see cref="TypeDescriptor" /> provides, and
///     the types of the framework are never converted, so that existing comparisons keep their meaning, for example a
///     number does not become equivalent to a number of another type. A conversion to <see cref="string" /> is never
///     used, as every converter supports it.
/// </remarks>
internal static class EquivalencyConverters
{
	private static readonly ConcurrentDictionary<Type, Lazy<TypeConverter>?> Reflected = new();

	public static bool TryConvert(object expected, Type actualType, [NotNullWhen(true)] out object? converted)
	{
		converted = null;
		Type expectedType = expected.GetType();
		if (expectedType == actualType || actualType == typeof(string) ||
		    EquivalencyMembers.IsFramework(expectedType) ||
		    GetConverter(expectedType) is not { } converter)
		{
			return false;
		}

		converted = UserCode.Invoke(
			static values => values.Converter.Value.CanConvertTo(values.ActualType)
				? values.Converter.Value.ConvertTo(null, CultureInfo.InvariantCulture, values.Expected,
					values.ActualType)
				: null,
			(Converter: converter, Expected: expected, ActualType: actualType),
			"the type converter of " + Formatter.Format(expectedType));
		return converted is not null && converted.GetType() == actualType;
	}

	/// <summary>
	///     Creates the converter with <paramref name="create" /> when it is first used.
	/// </summary>
	/// <remarks>
	///     The converter is not created when it is registered, which happens while the test assembly is loaded, so that
	///     an exception of its constructor fails only the comparison that uses it. An exception that reflection wraps is
	///     unwrapped, so that it is reported like the one of the converter itself.
	/// </remarks>
	public static Lazy<TypeConverter> CreateLazily(Func<TypeConverter> create)
		=> new(() =>
		{
			try
			{
				return create();
			}
			catch (TargetInvocationException exception) when (exception.InnerException is not null)
			{
				ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
				throw;
			}
		});

	/// <remarks>
	///     A registered converter comes from the source generator and works when publishing with trimming or Native AOT
	///     enabled; the attribute is only read by reflection for a type the generator did not see.
	/// </remarks>
	private static Lazy<TypeConverter>? GetConverter(Type type)
	{
		if (TypeMetadataRegistry.Instance.TryGet(type, out TypeMetadataRegistry.TypeMetadata? metadata) &&
		    metadata.Converter is not null)
		{
			return metadata.Converter;
		}

		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		if (!Reflected.TryGetValue(type, out Lazy<TypeConverter>? converter))
		{
			converter = ReflectConverter(type);
			Reflected.TryAdd(type, converter);
		}

		return converter;
	}

	/// <remarks>
	///     Like the source generator, only a converter that can be created without arguments is used.
	/// </remarks>
#if NET8_0_OR_GREATER
	[RequiresUnreferencedCode("Creates the converter named by the attribute, which the trimmer may remove.")]
#endif
	private static Lazy<TypeConverter>? ReflectConverter(Type type)
	{
		if (Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), false) is not TypeConverterAttribute
		    attribute ||
		    Type.GetType(attribute.ConverterTypeName, false) is not { } converterType ||
		    converterType.IsAbstract || converterType.ContainsGenericParameters ||
		    !typeof(TypeConverter).IsAssignableFrom(converterType) ||
		    converterType.GetConstructor(Type.EmptyTypes) is null)
		{
			return null;
		}

		return CreateLazily(() => (TypeConverter)Activator.CreateInstance(converterType)!);
	}
}
