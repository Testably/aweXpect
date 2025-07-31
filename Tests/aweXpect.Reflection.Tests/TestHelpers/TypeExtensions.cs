using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
