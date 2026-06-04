using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class MemberAccess
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldChainAbstractBeforeMethods()
			{
				Filtered.Methods methods = In.Type<AbstractMemberClass>().Abstract.Methods();

				await That(methods).All().Satisfy(m => m.IsAbstract).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("abstract methods in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainAbstractBeforeProperties()
			{
				Filtered.Properties properties = In.Type<AbstractMemberClass>().Abstract.Properties();

				await That(properties).All()
					.Satisfy(p => p.GetMethod?.IsAbstract == true || p.SetMethod?.IsAbstract == true)
					.And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("abstract properties in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainPrivateProtectedBeforeFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Classes().Private.Protected.Fields();

				await That(fields).All().Satisfy(f => f.IsFamilyAndAssembly);
				await That(fields.GetDescription())
					.IsEqualTo("private protected fields in classes in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainProtectedInternalBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Classes().Protected.Internal.Methods();

				await That(methods).All().Satisfy(m => m.IsFamilyOrAssembly);
				await That(methods.GetDescription())
					.IsEqualTo("protected internal methods in classes in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainPublicBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Classes().Public.Methods();

				await That(methods).All().Satisfy(m => m.IsPublic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("public methods in classes in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainSealedBeforeMethods()
			{
				Filtered.Methods methods = In.Type<SealedMemberClass>().Sealed.Methods();

				await That(methods).All().Satisfy(m => m is { IsVirtual: true, IsFinal: true, })
					.And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("sealed methods in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainStaticBeforeConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Classes().Static.Constructors();

				await That(constructors).All().Satisfy(c => c.IsStatic).And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("static constructors in classes in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldChainStaticBeforeMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Classes().Static.Methods();

				await That(methods).All().Satisfy(m => m.IsStatic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("static methods in classes in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldCombinePublicAndStatic()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Classes().Public.Static.Methods();

				await That(methods).All().Satisfy(m => m.IsStatic && m.IsPublic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("public static methods in classes in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldResetModifierStateAfterSelector()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>().Classes();

				Filtered.Methods publicMethods = types.Public.Methods();
				Filtered.Methods anyMethods = types.Methods();

				await That(publicMethods).All().Satisfy(m => m.IsPublic).And.IsNotEmpty();
				await That(anyMethods).Any().Satisfy(m => !m.IsPublic);
				await That(publicMethods.GetDescription())
					.IsEqualTo("public methods in classes in assembly").AsPrefix();
				await That(anyMethods.GetDescription())
					.IsEqualTo("methods in classes in assembly").AsPrefix();
			}

			private abstract class AbstractMemberClass
			{
				public abstract int AbstractProperty { get; }
				public abstract void AbstractMethod();

#pragma warning disable CA1822
				// ReSharper disable once UnusedMember.Local
				public void ConcreteMethod() { }
#pragma warning restore CA1822
			}

			private class ConcreteForSealed
			{
				// ReSharper disable once UnusedMember.Global
				public virtual void VirtualMethod() { }
			}

			// ReSharper disable once ClassNeverInstantiated.Local
			private sealed class SealedMemberClass : ConcreteForSealed
			{
				// 'sealed' is required so MethodInfo.IsFinal is true, which the .Sealed
				// filter relies on (see ShouldChainSealedBeforeMethods); do not remove.
				public sealed override string ToString() => "Sealed";
				public sealed override void VirtualMethod() { }
			}
		}
	}
}
