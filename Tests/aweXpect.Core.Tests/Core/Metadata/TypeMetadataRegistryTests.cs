using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core.Metadata;

namespace aweXpect.Core.Tests.Core.Metadata;

public sealed class TypeMetadataRegistryTests
{
	[Fact]
	public async Task RegisterBatch_ShouldNotDelayARegistrationOnAnotherThread()
	{
		TypeMetadataRegistry.Registration registration = new();
		bool isVisibleWhilePending = false;

		registration.Batch(() =>
		{
			OnAnotherThread(() => registration.AddProperty(typeof(Other), "Value", typeof(int), _ => 1));
			isVisibleWhilePending = registration.TryGet(typeof(Other), out _);
		});

		await That(isVisibleWhilePending).IsTrue()
			.Because("only the registrations of the thread that runs the batch are published together");
	}

	[Fact]
	public async Task RegisterBatch_ShouldPublishTheEventsTogetherWithTheMembers()
	{
		TypeMetadataRegistry.Registration registration = new();
		bool isVisibleWhilePending = true;

		registration.Batch(() =>
		{
			registration.AddEvent(typeof(Dummy), "Changed", _ => new Action(() => { }), (_, _) => { },
				(_, _) => { });
			isVisibleWhilePending = IsRegisteredOnAnotherThread(registration, typeof(Dummy));
			registration.AddProperty(typeof(Dummy), "Value", typeof(int), _ => 1);
		});

		registration.TryGet(typeof(Dummy), out TypeMetadataRegistry.TypeMetadata? metadata);
		await That(isVisibleWhilePending).IsFalse()
			.Because("a recording must not see the events of a type whose members are still being registered");
		await That(metadata!.Events.Keys).IsEqualTo(["Changed",]);
		await That(metadata.Properties.Keys).IsEqualTo(["Value",]);
	}

	[Fact]
	public async Task RegisterBatch_ShouldPublishTheTypeOnlyOnceTheCallbackReturns()
	{
		TypeMetadataRegistry.Registration registration = new();
		bool isVisibleWhilePending = true;

		registration.Batch(() =>
		{
			registration.AddProperty(typeof(Dummy), "First", typeof(int), _ => 1);
			isVisibleWhilePending = IsRegisteredOnAnotherThread(registration, typeof(Dummy));
			registration.AddField(typeof(Dummy), "Second", typeof(int), _ => 2);
		});

		registration.TryGet(typeof(Dummy), out TypeMetadataRegistry.TypeMetadata? metadata);
		await That(isVisibleWhilePending).IsFalse()
			.Because("a lookup must not see a type with only some of its members");
		await That(metadata!.Properties.Keys).IsEqualTo(["First",]);
		await That(metadata.Fields.Keys).IsEqualTo(["Second",]);
		await That(metadata.Properties["First"].Order).IsLessThan(metadata.Fields["Second"].Order)
			.Because("the members keep the order in which they were registered");
	}

	[Fact]
	public async Task RegisterBatch_WhenNested_ShouldPublishOnceTheOutermostCallbackReturns()
	{
		TypeMetadataRegistry.Registration registration = new();
		bool isVisibleAfterInnerBatch = true;

		registration.Batch(() =>
		{
			registration.Batch(() => registration.AddProperty(typeof(Dummy), "First", typeof(int), _ => 1));
			isVisibleAfterInnerBatch = IsRegisteredOnAnotherThread(registration, typeof(Dummy));
			registration.AddProperty(typeof(Dummy), "Second", typeof(int), _ => 2);
		});

		registration.TryGet(typeof(Dummy), out TypeMetadataRegistry.TypeMetadata? metadata);
		await That(isVisibleAfterInnerBatch).IsFalse()
			.Because("a nested batch joins the outer one");
		await That(metadata!.Properties.Keys).IsEqualTo(["First", "Second",]).InAnyOrder();
	}

	[Fact]
	public async Task RegisterBatch_WhenTheCallbackThrows_ShouldPublishNothing()
	{
		TypeMetadataRegistry.Registration registration = new();

		void Act()
			=> registration.Batch(() =>
			{
				registration.AddProperty(typeof(Dummy), "Value", typeof(int), _ => 1);
				throw new InvalidOperationException("broken registration");
			});

		await That(Act).Throws<InvalidOperationException>().WithMessage("broken registration");
		await That(registration.TryGet(typeof(Dummy), out _)).IsFalse()
			.Because("an incomplete batch must not publish a partial type");
		registration.AddProperty(typeof(Other), "Value", typeof(int), _ => 1);
		await That(registration.TryGet(typeof(Other), out _)).IsTrue()
			.Because("a registration after the failed batch is no longer part of it");
	}

	[Fact]
	public async Task RegisterBatch_WhenTheTypeIsAlreadyRegistered_ShouldKeepThePublishedMembersUntilItReturns()
	{
		TypeMetadataRegistry.Registration registration = new();
		registration.AddProperty(typeof(Dummy), "Value", typeof(int), _ => 1);
		registration.TryGet(typeof(Dummy), out TypeMetadataRegistry.TypeMetadata? before);
		string[] namesWhilePending = [];

		registration.Batch(() =>
		{
			registration.AddProperty(typeof(Dummy), "Value", typeof(int), _ => 2);
			registration.AddProperty(typeof(Dummy), "Other", typeof(int), _ => 3);
			OnAnotherThread(() =>
			{
				registration.TryGet(typeof(Dummy), out TypeMetadataRegistry.TypeMetadata? pending);
				namesWhilePending = pending!.Properties.Keys.ToArray();
			});
		});

		registration.TryGet(typeof(Dummy), out TypeMetadataRegistry.TypeMetadata? after);
		await That(namesWhilePending).IsEqualTo(["Value",])
			.Because("the published members stay as they were until the batch is complete");
		await That(before!.Properties.Keys).IsEqualTo(["Value",])
			.Because("a published entry is replaced, not changed in place");
		await That(after!.Properties.Keys).IsEqualTo(["Value", "Other",]).InAnyOrder();
		await That(after.Properties["Value"].GetValue(new Dummy())).IsEqualTo(2)
			.Because("the last registration of a member wins");
	}

	[Fact]
	public async Task RegisterBatch_WithTheLiveRegistry_ShouldPublishTheRegistrations()
	{
		TypeMetadataRegistry.RegisterBatch(() =>
			TypeMetadataRegistry.RegisterProperty<Batched, int>(nameof(Batched.Value), x => x.Value));

		bool isRegistered = TypeMetadataRegistry.Instance.TryGet(typeof(Batched),
			out TypeMetadataRegistry.TypeMetadata? metadata);

		await That(isRegistered).IsTrue();
		await That(metadata!.Properties.Keys).IsEqualTo([nameof(Batched.Value),]);
	}

	[Theory]
	[InlineData(typeof(IDictionary<string, Batched>))]
	[InlineData(typeof(IReadOnlyDictionary<string, Batched>))]
	public async Task RegisterDictionary_ShouldRegisterAReaderOfTheKeyComparer(Type dictionaryInterface)
	{
		Dictionary<string, Batched> dictionary = new(StringComparer.OrdinalIgnoreCase);

		TypeMetadataRegistry.RegisterDictionary<string, Batched>();

		TypeMetadataRegistry.Instance.TryGet(dictionaryInterface, out TypeMetadataRegistry.TypeMetadata? metadata);
		IEqualityComparer<object>? keyComparer = metadata?.KeyComparer?.Read(dictionary) as IEqualityComparer<object>;
		await That(keyComparer?.Equals("a", "A")).IsTrue()
			.Because("the comparison finds the reader through the dictionary interface the runtime type implements");
	}

	[Fact]
	public async Task RegisterSet_ShouldRegisterAReaderOfTheItemComparer()
	{
		HashSet<Batched> set = new(new SameValueComparer());

		TypeMetadataRegistry.RegisterSet<Batched>();

		TypeMetadataRegistry.Instance.TryGet(typeof(ISet<Batched>), out TypeMetadataRegistry.TypeMetadata? metadata);
		Func<object?, object?, bool>? itemComparer = metadata?.ItemComparer?.Read(set);
		await That(itemComparer?.Invoke(new Batched { Value = 1, }, new Batched { Value = 1, })).IsTrue()
			.Because("the comparison finds the reader through the set interface the runtime type implements");
	}

	private static bool IsRegisteredOnAnotherThread(TypeMetadataRegistry.Registration registration, Type type)
	{
		bool isRegistered = false;
		OnAnotherThread(() => isRegistered = registration.TryGet(type, out _));
		return isRegistered;
	}

	/// <remarks>
	///     A dedicated thread, because waiting for a task can run it inline on the waiting thread.
	/// </remarks>
	private static void OnAnotherThread(Action action)
	{
		Thread thread = new(() => action());
		thread.Start();
		thread.Join();
	}

	private sealed class Batched
	{
		public int Value { get; set; }
	}

	private sealed class Dummy;

	private sealed class SameValueComparer : IEqualityComparer<Batched>
	{
		public bool Equals(Batched? x, Batched? y) => x?.Value == y?.Value;

		public int GetHashCode(Batched obj) => obj.Value;
	}

	private sealed class Other;
}
