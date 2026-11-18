using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed class TypeKinds
		{
			public sealed class Tests
			{
				[Test]
				public async Task Attributes_ShouldApplyFilterForAttributes()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Attributes();

					await That(types).All().Satisfy(type => typeof(Attribute).IsAssignableFrom(type)).And
						.Contains(typeof(TypeKindsAttribute));
				}

				[Test]
				public async Task Attributes_ShouldHaveExpectedDescription()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Public.Attributes();

					string description = types.GetDescription();

					await That(description)
						.IsEqualTo("public attributes in assembly containing type Filtered.Assemblies.TypeKinds.Tests");
				}

				[Test]
				public async Task Delegates_ShouldApplyFilterForDelegates()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Delegates();

					await That(types).All().Satisfy(type => typeof(MulticastDelegate).IsAssignableFrom(type)).And
						.Contains(typeof(TypeKindsDelegate));
				}

				[Test]
				public async Task Delegates_ShouldHaveExpectedDescription()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Delegates();

					string description = types.GetDescription();

					await That(description)
						.IsEqualTo("delegates in assembly containing type Filtered.Assemblies.TypeKinds.Tests");
				}

				[Test]
				public async Task Exceptions_ShouldApplyFilterForExceptions()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Exceptions();

					await That(types).All().Satisfy(type => typeof(Exception).IsAssignableFrom(type)).And
						.Contains(typeof(TypeKindsException));
				}

				[Test]
				public async Task Exceptions_ShouldHaveExpectedDescription()
				{
					Reflection.Collections.Filtered.Types types = In.AssemblyContaining<Tests>().Nested.Exceptions();

					string description = types.GetDescription();

					await That(description)
						.IsEqualTo("nested exceptions in assembly containing type Filtered.Assemblies.TypeKinds.Tests");
				}

				[Test]
				public async Task Exceptions_ShouldIncludeErrorMessage()
				{
					async Task Act()
						=> await That(In.AssemblyContaining<Tests>().Exceptions()).AreNotSealed();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that exceptions in assembly containing type Filtered.Assemblies.TypeKinds.Tests
						             none are sealed,
						             but 1 of * were

						             Matching items:
						             [
						               Filtered.Assemblies.TypeKinds.TypeKindsException
						             ]

						             Collection:
						             [
						               *
						             ]
						             """).AsWildcard();
				}
			}

			private delegate void TypeKindsDelegate();

			private sealed class TypeKindsException : Exception;

			[AttributeUsage(AttributeTargets.Class)]
			private sealed class TypeKindsAttribute : Attribute;
		}
	}
}
