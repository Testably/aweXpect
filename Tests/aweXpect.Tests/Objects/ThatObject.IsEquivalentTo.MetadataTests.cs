using System.Collections.Generic;
using aweXpect.Core.Metadata;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class MetadataTests
		{
			[Test]
			public async Task IgnoringFields_ShouldNotIgnoreProperties()
			{
				MixedMembers subject = new()
				{
					Number = 1,
					Text = "foo",
				};
				MixedMembers expected = new()
				{
					Number = 2,
					Text = "bar",
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringFields((_, _) => true));

				await That(Act).Throws<FailException>()
					.WithMessage("*Property Text differed:*").AsWildcard()
					.Because("ignoring every field must leave the properties compared");
			}

			[Test]
			public async Task IgnoringProperties_ShouldNotApplyToCollectionElements()
			{
				List<string> subject = ["foo",];
				List<string> expected = ["bar",];

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringProperties((_, _) => true));

				await That(Act).Throws<FailException>()
					.WithMessage("*Element [0] differed:*").AsWildcard()
					.Because("a collection element is neither a field nor a property");
			}

			[Test]
			public async Task IgnoringProperties_ShouldNotIgnoreFields()
			{
				MixedMembers subject = new()
				{
					Number = 1,
					Text = "foo",
				};
				MixedMembers expected = new()
				{
					Number = 2,
					Text = "bar",
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringProperties((_, _) => true));

				await That(Act).Throws<FailException>()
					.WithMessage("*Field Number differed:*").AsWildcard()
					.Because("ignoring every property must leave the fields compared");
			}

			[Test]
			public async Task WhenOnlyEventsAreRegistered_ShouldCompareByReflection()
			{
				TypeMetadataRegistry.RegisterEvent<WithEvent>(nameof(WithEvent.Changed),
					record => new EventHandler((_, _) => record([])),
					(instance, handler) => instance.Changed += (EventHandler)handler,
					(instance, handler) => instance.Changed -= (EventHandler)handler);
				WithEvent subject = new()
				{
					Number = 1,
				};
				WithEvent expected = new()
				{
					Number = 2,
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("*Property Number differed:*").AsWildcard()
					.Because("an event registration says nothing about the members, which still have to be reflected over");
			}

			[Test]
			public async Task WhenRegistered_ShouldCompareOnlyTheRegisteredMembers()
			{
				RegisterOnlyTheRegisteredProperty();
				RegisteredType subject = new()
				{
					Registered = 1,
					NotRegistered = 1,
				};
				RegisteredType expected = new()
				{
					Registered = 1,
					NotRegistered = 2,
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a registered type is compared through its registration instead of by reflection");
			}

			[Test]
			public async Task WhenRegistered_ShouldCompareTheRegisteredFields()
			{
				TypeMetadataRegistry.RegisterField<RegisteredField, int>(
					nameof(RegisteredField.Number), x => x.Number);
				RegisteredField subject = new()
				{
					Number = 1,
				};
				RegisteredField expected = new()
				{
					Number = 2,
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("*Field Number differed:*").AsWildcard()
					.Because("a registered field is compared through its registration");
			}

			[Test]
			public async Task WhenRegistered_WithNonPublicMembers_ShouldReflectOverThem()
			{
				TypeMetadataRegistry.RegisterProperty<RegisteredWithSecret, int>(
					nameof(RegisteredWithSecret.Registered), x => x.Registered);
				RegisteredWithSecret subject = new(1);
				RegisteredWithSecret expected = new(2);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.IncludingProperties(IncludeMembers.Internal));

				await That(Act).Throws<FailException>()
					.WithMessage("*Property Secret differed:*").AsWildcard()
					.Because("the registry only holds public members, so the non-public ones are still reflected over");
			}

			[Test]
			public async Task WhenRegisteredWithProbe_ShouldCompareAnonymousTypes()
			{
				var probe = new
				{
					MetadataProbeTitle = default(string),
				};
				TypeMetadataRegistry.RegisterProperty(probe, "MetadataProbeTitle", x => x.MetadataProbeTitle);
				var subject = new
				{
					MetadataProbeTitle = "foo",
				};
				var expected = new
				{
					MetadataProbeTitle = "bar",
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("*Property MetadataProbeTitle differed:*").AsWildcard()
					.Because(
						"the probe overload registers a type whose name cannot be written in source, and the registration is shared by every anonymous type of this shape in the assembly");
			}

			private static void RegisterOnlyTheRegisteredProperty()
				=> TypeMetadataRegistry.RegisterProperty<RegisteredType, int>(
					nameof(RegisteredType.Registered), x => x.Registered);

			private sealed class MixedMembers
			{
				public int Number;
				public string Text { get; set; } = "";
			}

			private sealed class RegisteredField
			{
				public int Number;
			}

			private sealed class RegisteredType
			{
				public int NotRegistered { get; set; }
				public int Registered { get; set; }
			}

			private sealed class RegisteredWithSecret(int secret)
			{
				public int Registered { get; set; }
				internal int Secret { get; } = secret;
			}

			private sealed class WithEvent
			{
				public int Number { get; set; }
				public event EventHandler? Changed;
				public void Raise() => Changed?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
