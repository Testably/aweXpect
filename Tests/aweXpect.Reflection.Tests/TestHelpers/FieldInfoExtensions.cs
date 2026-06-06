using System.Linq;
using System.Reflection;

namespace aweXpect.Reflection.Tests.TestHelpers;

public static class FieldInfoExtensions
{
	/// <summary>
	///     Checks if the <paramref name="fieldInfo" /> is required (marked with the <c>required</c> modifier).
	/// </summary>
	/// <param name="fieldInfo">The <see cref="FieldInfo" /> to check.</param>
	public static bool IsRequired(this FieldInfo? fieldInfo)
		=> fieldInfo != null && fieldInfo.GetCustomAttributes(true)
			.Any(attribute => attribute.GetType().FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute");
}
