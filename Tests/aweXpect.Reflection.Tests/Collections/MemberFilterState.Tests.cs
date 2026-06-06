using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Collections;

public sealed class MemberFilterStateTests
{
	public sealed class AbstractTests
	{
		[Fact]
		public async Task Events_ShouldIncludeAbstractAndExcludeNonAbstract()
		{
			Reflection.Collections.Filtered.Events events =
				In.Type<AbstractClassWithMembers>().Abstract.Events();

			await That(events).Contains(e => e.Name == nameof(AbstractClassWithMembers.AbstractEvent));
			await That(events).None().Satisfy(e => e.Name == nameof(AbstractClassWithMembers.VirtualEvent));
			await That(events).All().Satisfy(e => e.AddMethod?.IsAbstract == true).And.IsNotEmpty();
		}

		[Fact]
		public async Task Methods_ShouldIncludeAbstractAndExcludeNonAbstract()
		{
			Reflection.Collections.Filtered.Methods methods =
				In.Type<AbstractClassWithMembers>().Abstract.Methods();

			await That(methods).Contains(m => m.Name == nameof(AbstractClassWithMembers.AbstractMethod));
			await That(methods).None().Satisfy(m => m.Name == nameof(AbstractClassWithMembers.VirtualMethod));
			await That(methods).None().Satisfy(m => m.Name == nameof(AbstractClassWithMembers.RegularMethod));
			await That(methods).All().Satisfy(m => m.IsAbstract).And.IsNotEmpty();
		}

		[Fact]
		public async Task Properties_ShouldIncludeAbstractAndExcludeNonAbstract()
		{
			Reflection.Collections.Filtered.Properties properties =
				In.Type<AbstractClassWithMembers>().Abstract.Properties();

			await That(properties).Contains(p => p.Name == nameof(AbstractClassWithMembers.AbstractProperty));
			await That(properties).None().Satisfy(p => p.Name == nameof(AbstractClassWithMembers.VirtualProperty));
			await That(properties).All()
				.Satisfy(p => p.GetMethod?.IsAbstract == true || p.SetMethod?.IsAbstract == true).And.IsNotEmpty();
		}
	}

	public sealed class SealedTests
	{
		[Fact]
		public async Task Events_ShouldIncludeSealedAndExcludeNonSealed()
		{
			Reflection.Collections.Filtered.Events events =
				In.Type<ClassWithSealedMembers>().Sealed.Events();

			await That(events).Contains(e => e.Name == nameof(ClassWithSealedMembers.VirtualEvent));
			await That(events).None().Satisfy(e => e.Name == nameof(ClassWithSealedMembers.AbstractEvent));
			await That(events).All().Satisfy(e => e.AddMethod?.IsFinal == true).And.IsNotEmpty();
		}

		[Fact]
		public async Task Methods_ShouldIncludeSealedAndExcludeNonSealed()
		{
			Reflection.Collections.Filtered.Methods methods =
				In.Type<ClassWithSealedMembers>().Sealed.Methods();

			await That(methods).Contains(m => m.Name == nameof(ClassWithSealedMembers.VirtualMethod));
			await That(methods).None().Satisfy(m => m.Name == nameof(ClassWithSealedMembers.AbstractMethod));
			await That(methods).All().Satisfy(m => m is { IsVirtual: true, IsFinal: true, }).And.IsNotEmpty();
		}

		[Fact]
		public async Task Properties_ShouldIncludeSealedAndExcludeNonSealed()
		{
			Reflection.Collections.Filtered.Properties properties =
				In.Type<ClassWithSealedMembers>().Sealed.Properties();

			await That(properties).Contains(p => p.Name == nameof(ClassWithSealedMembers.VirtualProperty));
			await That(properties).None().Satisfy(p => p.Name == nameof(ClassWithSealedMembers.AbstractProperty));
			await That(properties).All()
				.Satisfy(p => p.GetMethod?.IsFinal == true || p.SetMethod?.IsFinal == true).And.IsNotEmpty();
		}
	}

	public sealed class StaticTests
	{
		[Fact]
		public async Task Constructors_ShouldIncludeStaticAndExcludeNonStatic()
		{
			Reflection.Collections.Filtered.Constructors constructors =
				In.Type<TestClassWithStaticMembers>().Static.Constructors();

			await That(constructors).All().Satisfy(c => c.IsStatic).And.IsNotEmpty();
		}

		[Fact]
		public async Task Fields_ShouldIncludeStaticAndExcludeNonStatic()
		{
			Reflection.Collections.Filtered.Fields fields =
				In.Type<TestClassWithStaticMembers>().Static.Fields();

			await That(fields).Contains(f => f.Name == nameof(TestClassWithStaticMembers.StaticField));
			await That(fields).None().Satisfy(f => f.Name == nameof(TestClassWithStaticMembers.NonStaticField));
			await That(fields).All().Satisfy(f => f.IsStatic).And.IsNotEmpty();
		}

		[Fact]
		public async Task Methods_ShouldIncludeStaticAndExcludeNonStatic()
		{
			Reflection.Collections.Filtered.Methods methods =
				In.Type<TestClassWithStaticMembers>().Static.Methods();

			await That(methods).Contains(m => m.Name == nameof(TestClassWithStaticMembers.StaticMethod));
			await That(methods).None().Satisfy(m => m.Name == nameof(TestClassWithStaticMembers.NonStaticMethod));
			await That(methods).All().Satisfy(m => m.IsStatic).And.IsNotEmpty();
		}

		[Fact]
		public async Task Properties_ShouldIncludeStaticAndExcludeNonStatic()
		{
			Reflection.Collections.Filtered.Properties properties =
				In.Type<TestClassWithStaticMembers>().Static.Properties();

			await That(properties).Contains(p => p.Name == nameof(TestClassWithStaticMembers.StaticProperty));
			await That(properties).None()
				.Satisfy(p => p.Name == nameof(TestClassWithStaticMembers.NonStaticProperty));
			await That(properties).All().Satisfy(p => p.GetMethod?.IsStatic == true).And.IsNotEmpty();
		}
	}

	public sealed class AccessTests
	{
		[Fact]
		public async Task Constructors_ShouldIncludePublicAndExcludeNonPublic()
		{
			Reflection.Collections.Filtered.Constructors constructors =
				In.Type<TestClassWithStaticMembers>().Public.Constructors();

			await That(constructors).All().Satisfy(c => c.IsPublic).And.IsNotEmpty();
		}

		[Fact]
		public async Task Events_ShouldIncludePublicAndExcludeNonPublic()
		{
			Reflection.Collections.Filtered.Events events =
				In.Type<AbstractClassWithMembers>().Public.Events();

			await That(events).Contains(e => e.Name == nameof(AbstractClassWithMembers.AbstractEvent));
			await That(events).All().Satisfy(e => e.AddMethod?.IsPublic == true).And.IsNotEmpty();
		}

		[Fact]
		public async Task Fields_ShouldIncludePublicAndExcludeNonPublic()
		{
			Reflection.Collections.Filtered.Fields fields =
				In.Type<TestClassWithStaticMembers>().Public.Fields();

			await That(fields).Contains(f => f.Name == nameof(TestClassWithStaticMembers.StaticField));
			await That(fields).All().Satisfy(f => f.IsPublic).And.IsNotEmpty();
		}

		[Fact]
		public async Task Fields_WithoutModifier_ShouldReturnNullFilterAndIncludeAll()
		{
			Reflection.Collections.Filtered.Fields fields =
				In.Type<TestClassWithStaticMembers>().Fields();

			await That(fields).Contains(f => f.Name == nameof(TestClassWithStaticMembers.StaticField));
			await That(fields).Contains(f => f.Name == nameof(TestClassWithStaticMembers.NonStaticField));
		}

		[Fact]
		public async Task Methods_ShouldIncludePublicAndExcludeNonPublic()
		{
			Reflection.Collections.Filtered.Methods methods =
				In.Type<TestClassWithStaticMembers>().Public.Methods();

			await That(methods).Contains(m => m.Name == nameof(TestClassWithStaticMembers.StaticMethod));
			await That(methods).All().Satisfy(m => m.IsPublic).And.IsNotEmpty();
		}

		[Fact]
		public async Task Methods_WithoutModifier_ShouldReturnNullFilterAndIncludeAll()
		{
			Reflection.Collections.Filtered.Methods methods =
				In.Type<TestClassWithStaticMembers>().Methods();

			await That(methods).Contains(m => m.Name == nameof(TestClassWithStaticMembers.StaticMethod));
			await That(methods).Contains(m => m.Name == nameof(TestClassWithStaticMembers.NonStaticMethod));
		}

		[Fact]
		public async Task Properties_ShouldIncludePublicAndExcludeNonPublic()
		{
			Reflection.Collections.Filtered.Properties properties =
				In.Type<TestClassWithStaticMembers>().Public.Properties();

			await That(properties).Contains(p => p.Name == nameof(TestClassWithStaticMembers.StaticProperty));
			await That(properties).All()
				.Satisfy(p => p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true).And.IsNotEmpty();
		}
	}
}
