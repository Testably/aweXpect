namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichInheritDirectlyFrom
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldApplyFilterForBaseType()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichInheritDirectlyFrom>().Types()
							.WhichInheritDirectlyFrom<FooBase>())
						.InheritDirectlyFrom<FooBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldIncludeInheritDirectlyInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichInheritDirectlyFrom>().Types()
							.WhichInheritDirectlyFrom<FooBase>())
						.AreAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which inherit directly from TypeFilters.WhichInheritDirectlyFrom.Tests.FooBase in assembly containing type TypeFilters.WhichInheritDirectlyFrom
					             are all abstract,
					             but it contained non-abstract types

					             Not matching items:
					             [
					               TypeFilters.WhichInheritDirectlyFrom.Tests.FooDerived
					             ]
					             """);
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichInheritDirectlyFrom>().Types()
							.WhichInheritDirectlyFrom<IFoo>())
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface TypeFilters.WhichInheritDirectlyFrom.Tests.IFoo. Use 'Implements' to check for interface implementations.");
			}

			private interface IFoo;

			private class FooBase;

			private class FooDerived : FooBase;

			private class FooGrandChild : FooDerived;
		}
	}

	public sealed class WhichDoNotInheritDirectlyFrom
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldApplyFilterForBaseType()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotInheritDirectlyFrom>().Types()
							.WhichDoNotInheritDirectlyFrom<FooBase>())
						.DoNotInheritDirectlyFrom<FooBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldIncludeDoNotInheritDirectlyInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotInheritDirectlyFrom>().Types()
							.WhichDoNotInheritDirectlyFrom<FooBase>())
						.DoNotInheritFrom<FooBase>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which do not inherit directly from TypeFilters.WhichDoNotInheritDirectlyFrom.Tests.FooBase in assembly containing type TypeFilters.WhichDoNotInheritDirectlyFrom
					             all do not inherit from TypeFilters.WhichDoNotInheritDirectlyFrom.Tests.FooBase,
					             but it contained types that inherit from TypeFilters.WhichDoNotInheritDirectlyFrom.Tests.FooBase

					             Not matching items:
					             [
					               TypeFilters.WhichDoNotInheritDirectlyFrom.Tests.FooGrandChild
					             ]
					             """);
			}

			[Test]
			public async Task WhenBaseTypeIsAnInterface_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotInheritDirectlyFrom>().Types()
							.WhichDoNotInheritDirectlyFrom<IFoo>())
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check inheritance from must be a class, but it was the interface TypeFilters.WhichDoNotInheritDirectlyFrom.Tests.IFoo. Use 'Implements' to check for interface implementations.");
			}

			private interface IFoo;

			private class FooBase;

			private class FooDerived : FooBase;

			private class FooGrandChild : FooDerived;
		}
	}
}
