using System.Diagnostics;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Signaling;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Metrology;

namespace aweXpect.Benchmarks;

/// <summary>
///     In these benchmarks we measure the time and the allocations of expectations with options, so that a change of the
///     options model can be compared with the state before it.
/// </summary>
[Config(typeof(Config))]
[MarkdownExporterAttribute.GitHub]
[MemoryDiagnoser]
public class OptionsAllocationBenchmarks
{
	private static bool _isFirstUseMeasured;
	private readonly IEqualityComparer<string> _comparer = StringComparer.OrdinalIgnoreCase;
	private readonly DateOnly _dateOnly = new(2026, 10, 25);
	private readonly DateTime _dateTime = new(2026, 10, 25, 12, 0, 0);
	private readonly double[] _doubles = [1.0, 2.0, 3.0,];
	private readonly double[] _doublesNearby = [1.05, 2.0, 2.95,];
	private readonly string _exact = "foo";
	private readonly TimeSpan _executionLimit = TimeSpan.FromSeconds(1);
	private readonly double _expectedNumber = 1.0;
	private readonly double _expectedPercentNumber = 100.0;
	private readonly Item _item = new(1);
	private readonly IEqualityComparer<object> _itemComparer = EqualityComparer<object>.Default;
	private readonly List<string> _list = Enumerable.Repeat("FOO", 10).ToList();
	private readonly Action _noOp = () => { };
	private readonly double _number = 1.05;
	private readonly string[] _ordered = ["a", "b", "c", "d",];
	private readonly string _padded = "  FOO";
	private readonly string _pattern = "f*";
	private readonly double _percentNumber = 100.5;
	private readonly int _percentTolerance = 1;
	private readonly string _repeated = "xax";
	private readonly string[] _reversed = ["d", "c", "b", "a",];
	private readonly Signaler _signaler = new();
	private readonly string[] _sparse = ["a", "c",];
	private readonly double _tolerance = 0.1;
	private readonly string _trailing = "FOO  ";
	private readonly string _upper = "FOO";

	/// <remarks>
	///     Runs before the first benchmark of the class, so that the first use of <see cref="StringEqualityOptions" />
	///     in the process is measured, followed by the steady state.
	/// </remarks>
	[GlobalSetup(Target = nameof(String_IsEqualTo_NoOption))]
	public void MeasureFirstUse()
	{
		_signaler.Signal();
		if (_isFirstUseMeasured)
		{
			return;
		}

		_isFirstUseMeasured = true;
		long bytes = GC.GetAllocatedBytesForCurrentThread();
		Stopwatch sw = Stopwatch.StartNew();
		_ = new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default).IgnoringCase().IgnoringTrailingWhiteSpace();
		sw.Stop();
		long firstBytes = GC.GetAllocatedBytesForCurrentThread() - bytes;
		double firstNs = sw.Elapsed.TotalMilliseconds * 1_000_000;

		const int count = 20_000;
		bytes = GC.GetAllocatedBytesForCurrentThread();
		sw.Restart();
		for (int i = 0; i < count; i++)
		{
			_ = new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default).IgnoringCase().IgnoringTrailingWhiteSpace();
		}

		sw.Stop();
		long steadyBytes = GC.GetAllocatedBytesForCurrentThread() - bytes;
		double steadyNs = sw.Elapsed.TotalMilliseconds * 1_000_000 / count;
		Console.WriteLine(
			$"// First use of StringEqualityOptions with two options: {firstNs:F0} ns, {firstBytes} B; steady state: {steadyNs:F1} ns, {steadyBytes / count} B per operation");

		bytes = GC.GetAllocatedBytesForCurrentThread();
		int values = UseEverySharedValue();
		Console.WriteLine(
			$"// Using all {values} shared values of StringEqualityOptions afterwards: {GC.GetAllocatedBytesForCurrentThread() - bytes} B");
	}

	/// <summary>
	///     Combines every option without a reference with every built-in match type that takes no arguments, so that
	///     every shared value of <see cref="StringEqualityOptions" /> is created.
	/// </summary>
	/// <remarks>
	///     A block compares the lines on its own, so it is only combined with the casing.
	/// </remarks>
	private static int UseEverySharedValue()
	{
		Func<StringEqualityOptions, StringEqualityOptions>[] matchTypes =
		[
			o => o, o => o.AsPrefix(), o => o.AsSuffix(), o => o.Containing(), o => o.AsBlock(), o => o.AsWildcard(),
			o => o.AsRegex(),
		];
		int count = 0;
		for (int index = 0; index < matchTypes.Length; index++)
		{
			int combinations = index == 4 ? 2 : 64;
			for (int flags = 0; flags < combinations; flags++)
			{
				_ = matchTypes[index](StringEqualityOptions.Default)
					.IgnoringCase((flags & 1) != 0)
					.IgnoringIndentation((flags & 2) != 0)
					.IgnoringNewlineStyle((flags & 4) != 0)
					.IgnoringLeadingWhiteSpace((flags & 8) != 0)
					.IgnoringTrailingWhiteSpace((flags & 16) != 0)
					.NormalizingWhiteSpace((flags & 32) != 0);
				count++;
			}
		}

		return count;
	}

	[GlobalSetup]
	public void Setup() => _signaler.Signal();

	[Benchmark]
	public async Task<string?> String_IsEqualTo_NoOption()
		=> await Expect.That(_exact).IsEqualTo("foo");

	[Benchmark]
	public async Task<string?> String_IsEqualTo_IgnoringCase()
		=> await Expect.That(_upper).IsEqualTo("foo").IgnoringCase();

	[Benchmark]
	public async Task<string?> String_IsEqualTo_IgnoringCaseAndLeadingWhiteSpace()
		=> await Expect.That(_padded).IsEqualTo("foo").IgnoringCase().IgnoringLeadingWhiteSpace();

	[Benchmark]
	public async Task<string?> String_IsEqualTo_Using()
		=> await Expect.That(_upper).IsEqualTo("foo").Using(_comparer);

	[Benchmark]
	public async Task<string?> String_IsEqualTo_UsingAndTrailingWhiteSpace()
		=> await Expect.That(_trailing).IsEqualTo("foo").Using(_comparer).IgnoringTrailingWhiteSpace();

	[Benchmark]
	public async Task<string?> String_IsEqualTo_AsWildcard()
		=> await Expect.That(_exact).IsEqualTo(_pattern).AsWildcard();

	[Benchmark]
	public async Task<string?> String_IsEqualTo_AsWildcardAndIgnoringCase()
		=> await Expect.That(_upper).IsEqualTo(_pattern).AsWildcard().IgnoringCase();

	[Benchmark]
	public async Task<string?> String_IsEqualTo_AsRegexMultilineAndIgnoringCase()
		=> await Expect.That(_upper).IsEqualTo("^f.*$").AsRegex(RegexOptions.Multiline).IgnoringCase();

	[Benchmark]
	public async Task<string?> String_StartsWith_NoOption()
		=> await Expect.That(_exact).StartsWith("fo");

	[Benchmark]
	public async Task String_AllComplyWith_IgnoringCase()
		=> await Expect.That(_list).All().ComplyWith(x => x.IsEqualTo("foo").IgnoringCase());

	[Benchmark]
	public async Task<string?> String_Contains_AtLeast()
		=> await Expect.That(_repeated).Contains("x").AtLeast(2);

	[Benchmark]
	public async Task Collection_Contains_AtLeast()
		=> await Expect.That(_list).Contains("FOO").AtLeast(2);

	[Benchmark]
	public async Task Collection_HasItem_AtIndex()
		=> await Expect.That(_ordered).HasItem("c").AtIndex(2);

	[Benchmark]
	public async Task Collection_IsEqualTo_InAnyOrder()
		=> await Expect.That(_ordered).IsEqualTo(_reversed).InAnyOrder();

	[Benchmark]
	public async Task Collection_Contains_IgnoringInterspersedItems()
		=> await Expect.That(_ordered).Contains(_sparse).IgnoringInterspersedItems();

	[Benchmark]
	public async Task Collection_IsEqualTo_Within()
		=> await Expect.That(_doubles).IsEqualTo(_doublesNearby).Within(0.1);

	[Benchmark]
	public async Task<double> Number_IsEqualTo_Within()
		=> await Expect.That(_number).IsEqualTo(_expectedNumber).Within(_tolerance);

	[Benchmark]
	public async Task<double> Number_IsEqualTo_WithinPercent()
		=> await Expect.That(_percentNumber).IsEqualTo(_expectedPercentNumber).Within(_percentTolerance).Percent();

	[Benchmark]
	public async Task<DateTime> DateTime_IsEqualTo_Within()
		=> await Expect.That(_dateTime).IsEqualTo(_dateTime.AddSeconds(1)).Within(TimeSpan.FromSeconds(2));

	[Benchmark]
	public async Task<DateOnly> DateOnly_IsEqualTo_Within()
		=> await Expect.That(_dateOnly).IsEqualTo(_dateOnly.AddDays(1)).Within(TimeSpan.FromDays(1));

	[Benchmark]
	public async Task Object_IsEqualTo_NoOption()
		=> await Expect.That(_item).IsEqualTo(_item);

	[Benchmark]
	public async Task Object_IsEqualTo_Using()
		=> await Expect.That(_item).IsEqualTo(_item).Using(_itemComparer);

	[Benchmark]
	public async Task Signaler_Signaled_Within()
		=> await Expect.That(_signaler).Signaled().Within(TimeSpan.FromSeconds(1));

	[Benchmark]
	public async Task<string?> Generic_Satisfies_Within()
		=> await Expect.That(_exact).Satisfies(x => x.Length == 3).Within(TimeSpan.FromSeconds(1));

	[Benchmark]
	public async Task Delegate_ExecutesIn_AtMost()
		=> await Expect.That(_noOp).ExecutesIn().AtMost(_executionLimit);

	[Benchmark]
	public OptionsSlot<StringEqualityOptions> StringOptions_NoOption()
		=> new(StringEqualityOptions.Default);

	[Benchmark]
	public OptionsSlot<StringEqualityOptions> StringOptions_IgnoringCase()
		=> new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default).IgnoringCase();

	[Benchmark]
	public OptionsSlot<StringEqualityOptions> StringOptions_Using()
		=> new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default).Using(_comparer);

	public sealed class Item(int value)
	{
		public int Value { get; } = value;
	}

	private sealed class Config : ManualConfig
	{
		public Config()
		{
			AddJob(Job.MediumRun
				.WithLaunchCount(1)
				.WithToolchain(InProcessEmitToolchain.Instance)
				.WithId("InProcess"));
			SummaryStyle = SummaryStyle.Default.WithSizeUnit(SizeUnit.B);
		}
	}
}
