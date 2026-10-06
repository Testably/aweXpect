#if NET8_0_OR_GREATER
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace aweXpect.Tests;

public sealed partial class ThatNumber
{
	public sealed class FloatingPointTests
	{
		[Test]
		[Arguments(nameof(aweXpect.ThatNumber.IsFinite))]
		[Arguments(nameof(aweXpect.ThatNumber.IsInfinite))]
		[Arguments(nameof(aweXpect.ThatNumber.IsNaN))]
		[Arguments(nameof(aweXpect.ThatNumber.IsNotFinite))]
		[Arguments(nameof(aweXpect.ThatNumber.IsNotInfinite))]
		[Arguments(nameof(aweXpect.ThatNumber.IsNotNaN))]
		public async Task ShouldOnlyAcceptIeee754FloatingPointNumbers(string methodName)
		{
			MethodInfo[] methods = typeof(aweXpect.ThatNumber).GetMethods()
				.Where(x => x.Name == methodName)
				.ToArray();

			await That(methods).HasCount(2);
			await That(methods).All().Satisfy(x => x.GetGenericArguments()[0].GetGenericParameterConstraints()
					.Any(c => c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IFloatingPointIeee754<>)))
				.Because("a decimal is never NaN or infinite, so the expectation could never verify anything");
		}
	}
}
#endif
