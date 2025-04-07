using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Reflection.Collections;

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
}
