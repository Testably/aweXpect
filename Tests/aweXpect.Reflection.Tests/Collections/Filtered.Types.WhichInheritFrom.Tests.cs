using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed class Filtered
{
	public sealed class Types
	{
		public sealed class WhichInheritFrom
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForBaseType()
				{
					async Task Act()
						=> await That(In.AssemblyContaining<WhichInheritFrom>().Types().WhichInheritFrom<FooBase>())
							.AreNotAbstract();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldIncludeInheritInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AssemblyContaining<WhichInheritFrom>().Types().WhichInheritFrom<FooBase>())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that types which inherit from Filtered.Types.WhichInheritFrom.Tests.FooBase in assembly containing type Filtered.Types.WhichInheritFrom
						             are all abstract,
						             but it contained non-abstract types [
						               Filtered.Types.WhichInheritFrom.Tests.FooDerived
						             ]
						             """);
				}

				private class FooBase;

				// ReSharper disable once UnusedType.Local
				private class FooDerived : FooBase;
			}
		}
	}
}
