namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichInheritFrom
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldApplyFilterForBaseType()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichInheritFrom>().Types().WhichInheritFrom<FooBase>())
						.AreNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldIncludeInheritInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichInheritFrom>().Types().WhichInheritFrom<FooBase>())
						.AreAbstract();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types which inherit from TypeFilters.WhichInheritFrom.Tests.FooBase in assembly containing type TypeFilters.WhichInheritFrom
					             are all abstract,
					             but it contained non-abstract types [
					               TypeFilters.WhichInheritFrom.Tests.FooDerived
					             ]
					             """);
			}

			[Fact]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichInheritFrom>().Types().WhichInheritFrom<IFoo>())
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface TypeFilters.WhichInheritFrom.Tests.IFoo. Use 'Implements' to check for interface implementations.");
			}

			private interface IFoo;

			private class FooBase;

			// ReSharper disable once UnusedType.Local
			private class FooDerived : FooBase;
		}
	}

	public sealed class WhichDoNotInheritFrom
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldApplyFilterForBaseType()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotInheritFrom>().Types().WhichDoNotInheritFrom<FooBase>())
						.DoNotInheritFrom<FooBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldIncludeDoNotInheritInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotInheritFrom>().Types().WhichDoNotInheritFrom<FooBase>())
						.AreAbstract();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that types which do not inherit from TypeFilters.WhichDoNotInheritFrom.Tests.FooBase in assembly containing type TypeFilters.WhichDoNotInheritFrom
					             are all abstract,
					             but it contained non-abstract types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotInheritFrom>().Types().WhichDoNotInheritFrom<IFoo>())
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface TypeFilters.WhichDoNotInheritFrom.Tests.IFoo. Use 'Implements' to check for interface implementations.");
			}

			private interface IFoo;

			private class FooBase;

			// ReSharper disable once UnusedType.Local
			private class FooDerived : FooBase;
		}
	}
}
