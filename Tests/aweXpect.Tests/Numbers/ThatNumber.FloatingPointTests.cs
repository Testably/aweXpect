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
		[Arguments(nameof(global::aweXpect.ThatNumber.IsFinite))]
		[Arguments(nameof(global::aweXpect.ThatNumber.IsInfinite))]
		[Arguments(nameof(global::aweXpect.ThatNumber.IsNaN))]
		[Arguments(nameof(global::aweXpect.ThatNumber.IsNotFinite))]
		[Arguments(nameof(global::aweXpect.ThatNumber.IsNotInfinite))]
		[Arguments(nameof(global::aweXpect.ThatNumber.IsNotNaN))]
		public async Task ShouldOnlyAcceptIeee754FloatingPointNumbers(string methodName)
		{
			MethodInfo[] methods = typeof(global::aweXpect.ThatNumber).GetMethods()
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
