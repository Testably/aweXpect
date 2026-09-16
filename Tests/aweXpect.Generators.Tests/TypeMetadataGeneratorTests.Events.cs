using Microsoft.CodeAnalysis;

namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	public sealed class EventTests
	{
		private const string Publishers = """
		                                  using System;

		                                  namespace Models;

		                                  public class Publisher
		                                  {
		                                  	public delegate void CountedHandler(int count, string name, bool flag, DateTime at, int? optional);

		                                  	public event EventHandler? Changed;
		                                  	public event CountedHandler? Counted;
		                                  	public static event EventHandler? StaticChanged;
		                                  	public int Id { get; set; }
		                                  }

		                                  public class Hiding : Publisher
		                                  {
		                                  	public new int Changed { get; set; }
		                                  }

		                                  public class Generic<T>
		                                  {
		                                  	public event EventHandler<T>? Received;
		                                  }

		                                  public abstract class Abstract
		                                  {
		                                  	public event EventHandler? Changed;
		                                  }

		                                  public interface IPublisher
		                                  {
		                                  	event EventHandler Changed;
		                                  }

		                                  public struct Value
		                                  {
		                                  	public event EventHandler? Changed;
		                                  }

		                                  public class ReturningHandler
		                                  {
		                                  	public event Func<int>? Changed;
		                                  }

		                                  public class ByReference
		                                  {
		                                  	public delegate void RefHandler(ref int value);

		                                  	public event RefHandler? Changed;
		                                  }

		                                  public class Silent
		                                  {
		                                  	public int Id { get; set; }
		                                  }
		                                  """;

		[Fact]
		public async Task WhenBaseEventIsHiddenNonPubliclyInAReferencedAssembly_ShouldNotRegisterIt()
		{
			MetadataReference library = GeneratorRunner.CompileToReference("Lib", """
				using System;

				namespace Lib;

				public class Base
				{
					public event EventHandler? Changed;
					public event EventHandler? Removed;
				}

				public class HidingPrivately : Base
				{
					private new event Action? Changed;
					public event Action? Own;
					public void Raise() => Changed?.Invoke();
				}

				public class HidingInternally : Base
				{
					internal new event Action? Removed;
					public void Raise() => Removed?.Invoke();
				}

				public class Leaf : HidingInternally
				{
					public event Action? More;
				}
				""");

			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Record("new Lib.HidingPrivately().Watch(); new Lib.Leaf().Watch();"),],
				additionalReferences: [library,]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Lib.HidingPrivately>(\"Own\",");
			await That(result.Generated).Contains("RegisterEvent<global::Lib.HidingPrivately>(\"Removed\",");
			await That(result.Generated).DoesNotContain("RegisterEvent<global::Lib.HidingPrivately>(\"Changed\",")
				.Because(
					"reflection hides the base event behind the private declaration, which the default metadata import does not even show");
			await That(result.Generated).Contains("RegisterEvent<global::Lib.Leaf>(\"More\",");
			await That(result.Generated).Contains("RegisterEvent<global::Lib.Leaf>(\"Changed\",");
			await That(result.Generated).DoesNotContain("RegisterEvent<global::Lib.Leaf>(\"Removed\",")
				.Because("an internal declaration on an intermediate base hides the base event as well");
		}

		[Fact]
		public async Task WhenBaseTypeIsFromAnUnreferencedAssembly_ShouldNotRegisterTheType()
		{
			MetadataReference libraryA = GeneratorRunner.CompileToReference("LibA",
				"namespace LibA { public class Publisher { public event System.EventHandler? FromA; } }");
			MetadataReference libraryB = GeneratorRunner.CompileToReference("LibB",
				"namespace LibB { public class Derived : LibA.Publisher { public event System.EventHandler? Own; public int Id { get; set; } } }",
				libraryA);

			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				["[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(LibB.Derived))]",],
				additionalReferences: [libraryB,]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("LibB.Derived")
				.Because("the events and members of the unreferenced base are invisible, so a registration would fall short of reflection");
			await That(result.GeneratorDiagnostics).HasSingle().Which
				.Satisfies(x => x.Id == "aweXpect2001");
		}

		[Fact]
		public async Task WhenBaseTypeNameIsAlsoDeclaredInternallyElsewhere_ShouldRegisterTheType()
		{
			MetadataReference library = GeneratorRunner.CompileToReference("Lib", """
				namespace Lib;

				public class Args : System.EventArgs { }
				public delegate void Handler(Args args);

				public class Publisher
				{
					public event Handler? Changed;
				}
				""");
			MetadataReference polyfill = GeneratorRunner.CompileToReference("Polyfill",
				"namespace Lib { internal class Args { } internal delegate void Handler(int x); }");

			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Record("new Lib.Publisher().Watch();"),],
				additionalReferences: [library, polyfill,]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Lib.Publisher>(\"Changed\",")
				.Because("an inaccessible declaration of the same name does not take part in the lookup");
		}

		[Fact]
		public async Task WhenEventHandlerHasARefParameter_ShouldNotRegisterTheType()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.ByReference().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("a parameter passed by reference cannot be boxed by the generated handler");
		}

		[Fact]
		public async Task WhenEventHandlerParameterIsFromAnUnreferencedAssembly_ShouldNotRegisterTheType()
		{
			MetadataReference libraryA = GeneratorRunner.CompileToReference("LibA",
				"namespace LibA { public class Args : System.EventArgs { } }");
			MetadataReference libraryB = GeneratorRunner.CompileToReference("LibB", """
				namespace LibB;

				public delegate void Handler(LibA.Args args);
				public delegate void ListHandler(System.Collections.Generic.List<LibA.Args> args);

				public class Publisher
				{
					public event Handler? Changed;
					public event System.EventHandler? Fine;
				}

				public class ListPublisher
				{
					public event ListHandler? Changed;
					public event System.EventHandler? Fine;
				}
				""", libraryA);

			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(LibB.Publisher))]",
				"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(LibB.ListPublisher))]",
			], additionalReferences: [libraryB,]);

			await That(result.Errors).IsEmpty()
				.Because("a handler lambda that boxes a parameter of an unknown type would not compile");
			await That(result.Generated).DoesNotContain("RegisterEvent");
		}

		[Fact]
		public async Task WhenEventHandlerReturnsAValue_ShouldNotRegisterTheType()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.ReturningHandler().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("the reflective fallback cannot bind a returning handler either, so the type stays there");
		}

		[Fact]
		public async Task WhenEventIsHiddenByAProperty_ShouldRegisterItThroughTheDeclaringType()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Hiding().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Hiding>(\"Changed\",")
				.Because("reflection hides an event by another event only, so the base event is still returned");
			await That(result.Generated)
				.Contains("(o, h) => ((global::Models.Publisher)o).Changed += (global::System.EventHandler)h,")
				.Because("the property hides the event on the derived type, so it is only reachable through the base");
		}

		[Fact]
		public async Task WhenEventIsObsoleteWithDiagnosticId_ShouldSuppressIt()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				"""
				using System;

				namespace Models;

				public class Publisher
				{
					[Obsolete("gone", DiagnosticId = "LIB0042")]
					public event EventHandler? Old;
				}
				""",
				Record("new Models.Publisher().Watch();"),
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Warnings).IsEmpty();
			await That(result.Generated).Contains("#pragma warning disable CS0612, CS0618, LIB0042");
		}

		[Fact]
		public async Task WhenEventIsStatic_ShouldRegisterItThroughTheType()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Publisher().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated)
				.Contains("(o, h) => global::Models.Publisher.StaticChanged += (global::System.EventHandler)h,")
				.Because("reflection returns the static events of the recorded type, so they have to be recorded too");
		}

		[Fact]
		public async Task WhenExtensionParameterIsNotMarked_ShouldNotRegisterTheArgumentType()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Publishers,
				"""
				using aweXpect.Recording;

				public static class Extensions
				{
					public static IEventRecording<T> Watch<T>(this T subject)
						where T : notnull
						=> subject.Record().Events();
				}

				public class Tests
				{
					public void Test() => new Models.Publisher().Watch();
				}
				""",
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("the call inside the extension only reveals the open type parameter");
		}

		[Fact]
		public async Task WhenExtensionTypeParameterIsMarked_ShouldRegisterTheTypeArgument()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Publishers,
				"""
				using aweXpect.Core.Metadata;
				using aweXpect.Recording;

				public static class Extensions
				{
					public static IEventRecording<T> Watch<[RequiresEventMetadata] T>(object subject)
						where T : notnull
						=> ((T)subject).Record().Events();
				}

				public class Tests
				{
					public void Test() => Extensions.Watch<Models.Publisher>(new Models.Publisher());
				}
				""",
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Publisher>(\"Changed\",")
				.Because("the type is only visible as a type argument at the call site");
		}

		[Fact]
		public async Task WhenBaseTypeNameExistsInTwoReferencedAssemblies_ShouldNotRegisterTheType()
		{
			MetadataReference first = GeneratorRunner.CompileToReference("Dup1", """
				namespace Dup;

				public class Same
				{
					public event System.Action? Changed;
					public int Id { get; set; }
				}
				""");
			MetadataReference second = GeneratorRunner.CompileToReference("Dup2",
				"namespace Dup { public class Same { public event System.Action? Other; } }");
			MetadataReference third = GeneratorRunner.CompileToReference("Dup3",
				"namespace Dup3 { public class Leaf : Dup.Same { public event System.Action? Own; public int Count { get; set; } } }",
				first);

			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Record("new Dup3.Leaf().Watch();"),
				"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Dup3.Leaf))]",
			], additionalReferences: [first, second, third,]);

			await That(result.Errors).IsEmpty()
				.Because("the cast to the base type would be reported as ambiguous in the generated file");
			await That(result.Generated).DoesNotContain("Dup3.Leaf");
		}

		[Fact]
		public async Task WhenSubjectHasNoEvents_ShouldNotRegisterIt()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Silent().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("an empty registration would only claim what reflection finds anyway");
		}

		[Fact]
		public async Task WhenSubjectIsAbstract_ShouldNotRegisterIt()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("Models.Abstract subject = null!; subject.Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("the recording looks up the runtime type, which is never the abstract class");
		}

		[Fact]
		public async Task WhenSubjectIsAnInterface_ShouldNotRegisterIt()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("Models.IPublisher subject = null!; subject.Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("the recording looks up the runtime type, which is never the interface");
		}

		[Fact]
		public async Task WhenSubjectIsAStruct_ShouldNotRegisterIt()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Value().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("a handler added to a boxed copy never sees the events raised on the caller's value");
		}

		[Fact]
		public async Task WhenSubjectIsCompared_ShouldNotRegisterItsEvents()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Call("Expect.That(new Models.Publisher()).IsEquivalentTo(new Models.Publisher());"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterProperty<global::Models.Publisher, int>(\"Id\", o => o.Id);");
			await That(result.Generated).DoesNotContain("RegisterEvent")
				.Because("a comparison never touches the events");
		}

		[Fact]
		public async Task WhenSubjectIsGeneric_ShouldRegisterTheSubstitutedHandler()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Generic<int>().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Generic<int>>(\"Received\",");
			await That(result.Generated).Contains(
				"callback => new global::System.EventHandler<int>((arg1, arg2) => callback(new object[] { arg1, arg2, })),");
		}

		[Fact]
		public async Task WhenSubjectIsNamedInGenerateMetadata_ShouldRegisterItsEvents()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, "[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Publisher))]",]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Publisher>(\"Changed\",")
				.Because("the assembly attribute is the escape hatch for a subject no call site reveals");
			await That(result.Generated).Contains("RegisterProperty<global::Models.Publisher, int>(\"Id\", o => o.Id);")
				.Because("the attribute does not know whether the type is compared or recorded, so it registers both");
			await That(result.GeneratorDiagnostics).IsEmpty();
		}

		[Fact]
		public async Task WhenSubjectIsNamedInGenerateMetadataAndHasNothing_ShouldWarn()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				"namespace Models { public class Empty { } }",
				"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Empty))]",
			]);

			await That(result.Errors).IsEmpty();
			await That(result.GeneratorDiagnostics).HasSingle().Which
				.Satisfies(x => x.Id == "aweXpect2001" &&
				                x.GetMessage().Contains("neither public instance members nor public events"))
				.Because("the message has to name what was looked for, so that a type with events only is not surprising");
		}

		[Fact]
		public async Task WhenSubjectIsNamedInGenerateMetadataAndHasOnlyEvents_ShouldNotWarn()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				"""
				using System;

				namespace Models;

				public class Publisher
				{
					public event EventHandler? Changed;
				}
				""",
				"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof(Models.Publisher))]",
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Publisher>(\"Changed\",");
			await That(result.GeneratorDiagnostics).IsEmpty()
				.Because("an event registration is a registration, even without any member");
		}

		[Fact]
		public async Task WhenSubjectIsRecorded_ShouldBoxTheParametersOfTheHandler()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Publisher().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains(
					"callback => new global::Models.Publisher.CountedHandler((arg1, arg2, arg3, arg4, arg5) => callback(new object[] { arg1, arg2, arg3, arg4, arg5, })),")
				.Because(
					"a lambda boxes value-type arguments at its own call site, which a reflectively bound handler cannot, and it takes any number of parameters");
		}

		[Fact]
		public async Task WhenSubjectIsRecorded_ShouldNotRegisterItsMembers()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Publisher().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).DoesNotContain("RegisterProperty")
				.Because("a recording never touches the members");
		}

		[Fact]
		public async Task WhenSubjectIsRecorded_ShouldRegisterItsEvents()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Publishers, Record("new Models.Publisher().Watch();"),]);

			await That(result.Errors).IsEmpty();
			await That(result.Warnings).IsEmpty();
			await That(result.Generated).Contains("// events of global::Models.Publisher");
			await That(result.Generated).Contains("RegisterEvent<global::Models.Publisher>(\"Changed\",");
			await That(result.Generated).Contains(
				"callback => new global::System.EventHandler((arg1, arg2) => callback(new object[] { arg1, arg2, })),");
			await That(result.Generated).Contains("(o, h) => o.Changed += (global::System.EventHandler)h,");
			await That(result.Generated).Contains("(o, h) => o.Changed -= (global::System.EventHandler)h);");
		}

		[Fact]
		public async Task WhenSubjectIsRecordedAndCompared_ShouldRegisterEventsAndMembers()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Publishers,
				Record(
					"new Models.Publisher().Watch(); Expect.That(new Models.Publisher()).IsEquivalentTo(new Models.Publisher());"),
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Publisher>(\"Changed\",");
			await That(result.Generated).Contains("RegisterProperty<global::Models.Publisher, int>(\"Id\", o => o.Id);")
				.Because("the two registrations of one type are kept apart, so neither drops the other");
		}

#if DEBUG
		[Fact]
		public async Task WhenSubjectIsRecordedDirectly_ShouldRegisterItsEvents()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Publishers,
				"""
				using aweXpect.Recording;

				public class Tests
				{
					public void Test() => new Models.Publisher().Record().Events();
				}
				""",
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).Contains("RegisterEvent<global::Models.Publisher>(\"Changed\",")
				.Because("the marker on Record itself is what makes a plain call site register its subject");
		}
#endif

		/// <remarks>
		///     The marker on <c>Record</c> itself only reaches this project once the branch's <c>aweXpect.Core</c> is
		///     released, so the tests mark an extension of their own and leave the direct call to a Debug-only test.
		/// </remarks>
		private static string Record(string statement)
			=> $$"""
			     using aweXpect;
			     using aweXpect.Core.Metadata;
			     using aweXpect.Recording;

			     public static class Watcher
			     {
			     	public static IEventRecording<T> Watch<T>([RequiresEventMetadata] this T subject)
			     		where T : notnull
			     		=> subject.Record().Events();
			     }

			     public class Tests
			     {
			     	public void Test()
			     	{
			     		{{statement}}
			     	}
			     }
			     """;
	}
}
