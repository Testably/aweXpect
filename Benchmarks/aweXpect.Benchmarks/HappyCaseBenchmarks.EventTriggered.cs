using aweXpect.Recording;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Events;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that an event was raised.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _eventName = nameof(Player.Started);

	[Benchmark]
	public async Task EventTriggered_aweXpect()
	{
		Player player = new();
		IEventRecording<Player> recording = player.Record().Events();
		player.Play();
		await Expect.That(recording).Triggered(_eventName);
	}

	[Benchmark]
	public IEventRecording EventTriggered_FluentAssertions()
	{
		Player player = new();
		using IMonitor<Player> monitor = player.Monitor();
		player.Play();
		return monitor.Should().Raise(_eventName);
	}

	public sealed class Player
	{
		public event EventHandler? Started;

		public void Play() => Started?.Invoke(this, EventArgs.Empty);
	}
}
