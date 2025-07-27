using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public sealed class MemberInfoHelpersTests
{
	[Fact]
	public async Task HasAccessModifier_WhenNull_ShouldReturnFalse()
	{
		MemberInfo? fieldInfo = null;

		await That(fieldInfo.HasAccessModifier(AccessModifiers.Internal)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Private)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Protected)).IsFalse();
		await That(fieldInfo.HasAccessModifier(AccessModifiers.Public)).IsFalse();
	}
}
