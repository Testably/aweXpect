using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace aweXpect.Reflection.Tests.TestHelpers;

internal static class TypeExtensions
{
	/// <summary>
	///     Gets a value indicating whether the <see cref="Type" /> is static.
	/// </summary>
	/// <param name="type">The <see cref="Type" />.</param>
	/// <remarks>https://stackoverflow.com/a/1175901</remarks>
	public static bool IsStatic(this Type type)
		=> type is { IsAbstract: true, IsSealed: true, IsInterface: false, };

	public static bool IsRecordClass(this Type? type)
		=> type?.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) is not
			   null &&
		   type.GetProperty("EqualityContract",
				   BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)?
			   .GetMethod?.HasAttribute<CompilerGeneratedAttribute>() == true;

	public static bool IsRecordStruct(this Type? type) =>
		// As noted here: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-10.0/record-structs#open-questions
		// recognizing record structs from metadata is an open point. The following check is based on common sense
		// and heuristic testing, apparently giving good results but not supported by official documentation.
		type?.BaseType == typeof(ValueType) &&
		type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
			[typeof(StringBuilder),], null) is not null &&
		type.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null,
				[type, type,], null)?
			.HasAttribute<CompilerGeneratedAttribute>() == true;
	
	public static bool HasAttribute<TAttribute>(
		this MethodInfo methodInfo,
		Func<TAttribute, bool>? predicate = null,
		bool inherit = true)
		where TAttribute : Attribute
	{
		object? attribute = Attribute.GetCustomAttributes(methodInfo, typeof(TAttribute), inherit)
			.FirstOrDefault();
		if (attribute is TAttribute castedAttribute)
		{
			return predicate?.Invoke(castedAttribute) ?? true;
		}

		return false;
	}
}
