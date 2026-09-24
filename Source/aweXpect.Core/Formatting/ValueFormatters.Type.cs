using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Formatting;

public static partial class ValueFormatters
{
	private static readonly Dictionary<Type, string> Aliases = new()
	{
		{
			typeof(byte), "byte"
		},
		{
			typeof(sbyte), "sbyte"
		},
		{
			typeof(short), "short"
		},
		{
			typeof(ushort), "ushort"
		},
		{
			typeof(int), "int"
		},
		{
			typeof(uint), "uint"
		},
		{
			typeof(long), "long"
		},
		{
			typeof(ulong), "ulong"
		},
		{
			typeof(float), "float"
		},
		{
			typeof(double), "double"
		},
		{
			typeof(decimal), "decimal"
		},
		{
			typeof(object), "object"
		},
		{
			typeof(bool), "bool"
		},
		{
			typeof(char), "char"
		},
		{
			typeof(string), "string"
		},
		{
			typeof(void), "void"
		},
		{
			typeof(nint), "nint"
		},
		{
			typeof(nuint), "nuint"
		},
	};

	/// <summary>
	///     Returns the formatted <paramref name="value" /> according to the <paramref name="options" />.
	/// </summary>
	public static string Format(
		this ValueFormatter formatter,
		Type? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			return ValueFormatter.NullString;
		}

		StringBuilder stringBuilder = new();
		FormatType(value, stringBuilder);
		return stringBuilder.ToString();
	}

	/// <summary>
	///     Appends the formatted <paramref name="value" /> according to the <paramref name="options" />
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	public static void Format(
		this ValueFormatter formatter,
		StringBuilder stringBuilder,
		Type? value,
		FormattingOptions? options = null)
	{
		if (value == null)
		{
			stringBuilder.Append(ValueFormatter.NullString);
			return;
		}

		FormatType(value, stringBuilder);
	}

	private static void FormatType(
		Type value,
		StringBuilder stringBuilder)
		=> FormatType(value, stringBuilder, null);

#pragma warning disable S3776 // https://rules.sonarsource.com/csharp/RSPEC-3776
	private static void FormatType(
		Type value,
		StringBuilder stringBuilder,
		Type[]? genericArguments)
	{
		if (value == typeof(void))
		{
			stringBuilder.Append("void");
		}
		else if (value.IsGenericParameter)
		{
			stringBuilder.Append(value.Name);
		}
		else if (value.IsArray)
		{
			FormatType(value.GetElementType()!, stringBuilder);
			stringBuilder.Append("[]");
		}
		else if (!AppendedPrimitiveAlias(value, stringBuilder))
		{
			if (value.IsNested && value.DeclaringType is not null)
			{
				Type[]? declaringTypeGenericArguments = null;
				if (value.IsGenericType)
				{
					int arity = GetArityOfGenericParameters(value.DeclaringType);
					declaringTypeGenericArguments = [..value.GenericTypeArguments.Take(arity),];
					genericArguments = [..(genericArguments ?? value.GenericTypeArguments).Skip(arity),];
				}

				FormatType(value.DeclaringType, stringBuilder, declaringTypeGenericArguments);
				stringBuilder.Append('.');
			}

			if (value.IsGenericType)
			{
				Type genericTypeDefinition = value.GetGenericTypeDefinition();
				stringBuilder.Append(genericTypeDefinition.Name.SubstringUntilFirst('`'));
				if (genericArguments?.Length == 0)
				{
					return;
				}

				stringBuilder.Append('<');
				bool isFirstArgument = true;
				bool previousWasWritten = false;
				genericArguments ??= value.GetGenericArguments();
				foreach (Type? argument in genericArguments)
				{
					bool isWritten = !argument.ContainsGenericParameters;
					if (!isFirstArgument)
					{
						// An unbound argument writes nothing, so a space after its comma would dangle.
						stringBuilder.Append(previousWasWritten && isWritten ? ", " : ",");
					}

					isFirstArgument = false;
					previousWasWritten = isWritten;
					if (isWritten)
					{
						FormatType(argument, stringBuilder);
					}
				}

				stringBuilder.Append('>');
			}
			else
			{
				stringBuilder.Append(value.Name);
			}
		}
	}
#pragma warning restore S3776

	private static int GetArityOfGenericParameters(Type type)
	{
		int tickIndex = type.Name.LastIndexOf('`');
		if (tickIndex != -1)
		{
			string? arityStr = type.Name[(tickIndex + 1)..];
			if (int.TryParse(arityStr, NumberStyles.None, CultureInfo.InvariantCulture, out int arity))
			{
				return arity;
			}
		}

		return 0;
	}

	private static bool AppendedPrimitiveAlias(Type value, StringBuilder stringBuilder)
	{
		if (Aliases.TryGetValue(value, out string? typeAlias))
		{
			stringBuilder.Append(typeAlias);
			return true;
		}

		Type? underlyingType = Nullable.GetUnderlyingType(value);

		if (underlyingType != null)
		{
			if (Aliases.TryGetValue(underlyingType, out string? underlyingAlias))
			{
				stringBuilder.Append(underlyingAlias).Append('?');
				return true;
			}

			FormatType(underlyingType, stringBuilder);
			stringBuilder.Append('?');
			return true;
		}

		return false;
	}
}
