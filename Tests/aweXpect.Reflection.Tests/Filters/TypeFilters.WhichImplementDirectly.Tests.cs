namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichImplementDirectly
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldApplyFilterForInterface()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichImplementDirectly>().Types()
							.WhichImplementDirectly<IFoo>())
						.ImplementDirectly<IFoo>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldIncludeImplementDirectlyInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichImplementDirectly>().Types()
							.WhichImplementDirectly<IFoo>())
						.AreAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which directly implement TypeFilters.WhichImplementDirectly.Tests.IFoo in assembly containing type TypeFilters.WhichImplementDirectly
					             all are abstract,
					             but none of 1 were

					             Not matching items:
					             [
					               TypeFilters.WhichImplementDirectly.Tests.FooImpl
					             ]

					             Collection:
					             [
					               TypeFilters.WhichImplementDirectly.Tests.FooImpl
					             ]
					             """);
			}

			[Test]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichImplementDirectly>().Types()
							.WhichImplementDirectly<FooImpl>())
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was TypeFilters.WhichImplementDirectly.Tests.FooImpl. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			private interface IFoo;

			private class FooImpl : IFoo;

			private class FooImplDerived : FooImpl;
		}
	}

	public sealed class WhichDoNotImplementDirectly
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldApplyFilterForInterface()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotImplementDirectly>().Types()
							.WhichDoNotImplementDirectly<IFoo>())
						.DoNotImplementDirectly<IFoo>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldIncludeDoNotImplementDirectlyInformationInErrorMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotImplementDirectly>().Types()
							.WhichDoNotImplementDirectly<IFoo>())
						.DoNotImplement<IFoo>();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that types which do not directly implement TypeFilters.WhichDoNotImplementDirectly.Tests.IFoo in assembly containing type TypeFilters.WhichDoNotImplementDirectly
					             all do not implement TypeFilters.WhichDoNotImplementDirectly.Tests.IFoo,
					             but it contained types that implement TypeFilters.WhichDoNotImplementDirectly.Tests.IFoo

					             Not matching items:
					             [
					               TypeFilters.WhichDoNotImplementDirectly.Tests.FooImplDerived
					             ]
					             """);
			}

			[Test]
			public async Task WhenInterfaceTypeIsAClass_ShouldThrowArgumentException()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<WhichDoNotImplementDirectly>().Types()
							.WhichDoNotImplementDirectly<FooImpl>())
						.AreNotAbstract();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check implementation of must be an interface, but it was TypeFilters.WhichDoNotImplementDirectly.Tests.FooImpl. Use 'InheritsFrom' to check for base-class inheritance.");
			}

			private interface IFoo;

			private class FooImpl : IFoo;

			private class FooImplDerived : FooImpl;
		}
	}
}
