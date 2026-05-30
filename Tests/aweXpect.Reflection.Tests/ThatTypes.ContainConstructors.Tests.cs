using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class ContainConstructors
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesContainMatchingConstructor_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedConstructor, ClassWithTwoMarkedConstructors>();

				async Task Act()
					=> await That(subject).ContainConstructors(constructors => constructors.With<MarkerAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeContainsNoMatchingConstructor_ShouldFail()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedConstructor, ClassWithoutMarkedConstructor>();

				async Task Act()
					=> await That(subject).ContainConstructors(constructors => constructors.With<MarkerAttribute>());

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that in types ThatTypes.ContainConstructors.ClassWithMarkedConstructor and ThatTypes.ContainConstructors.ClassWithoutMarkedConstructor
					             all contain constructors with ThatTypes.ContainConstructors.MarkerAttribute at least once,
					             but it contained not matching types [
					               ThatTypes.ContainConstructors.ClassWithoutMarkedConstructor
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSomeTypeContainsNoMatchingConstructor_ShouldSucceed()
			{
				Filtered.Types subject = In.Types<ClassWithMarkedConstructor, ClassWithoutMarkedConstructor>();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they
						=> they.ContainConstructors(constructors => constructors.With<MarkerAttribute>()));

				await That(Act).DoesNotThrow();
			}
		}

		[AttributeUsage(AttributeTargets.Constructor)]
		private class MarkerAttribute : Attribute
		{
		}

		private class ClassWithMarkedConstructor
		{
			[Marker]
			public ClassWithMarkedConstructor()
			{
			}
		}

		private class ClassWithTwoMarkedConstructors
		{
			[Marker]
			public ClassWithTwoMarkedConstructors()
			{
			}

			[Marker]
			public ClassWithTwoMarkedConstructors(int value)
			{
			}
		}

		private class ClassWithoutMarkedConstructor
		{
			public ClassWithoutMarkedConstructor()
			{
			}
		}
	}
}
