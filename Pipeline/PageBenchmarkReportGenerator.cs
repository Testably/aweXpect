using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Build;

public class PageBenchmarkReportGenerator
{
	private static readonly JsonSerializerOptions BenchmarkSerializerOptions = new()
	{
		WriteIndented = true,
	};

	public static (string, string) Append(CommitInfo commitInfo, string currentFileContent,
		List<string> benchmarkReportsContents, int limit)
	{
		string prefix = "window.BENCHMARK_DATA = ";
		if (!currentFileContent.StartsWith(prefix))
		{
			throw new NotSupportedException($"The benchmark data file is incorrect (does not start with {prefix})");
		}

		PageReportData pageReport =
			JsonSerializer.Deserialize<PageReportData>(currentFileContent.Substring(prefix.Length));

		if (pageReport.Values.Any(r => r.Commits.Any(c => c.Sha == commitInfo.Sha)))
		{
			Log.Warning(
				$"The benchmark already has data for {commitInfo.Sha}: {commitInfo.Message} by {commitInfo.Author} on {commitInfo.Date}");
			return (null, null);
		}

		Log.Debug(
			$"Updating benchmark report for {commitInfo.Sha}: {commitInfo.Message} by {commitInfo.Author} on {commitInfo.Date}");

		foreach (string benchmarkReportContent in benchmarkReportsContents)
		{
			BenchmarkReport benchmarkReport = JsonSerializer.Deserialize<BenchmarkReport>(benchmarkReportContent);
			if (!pageReport.Append(commitInfo, benchmarkReport))
			{
				throw new NotSupportedException("The new benchmark data is incorrect");
			}
		}

		string newFileContent =
			$"{prefix}{JsonSerializer.Serialize(pageReport, BenchmarkSerializerOptions)}";
		string limitedFileContent =
			$"{prefix}{JsonSerializer.Serialize(pageReport.Limit(limit), BenchmarkSerializerOptions)}";
		return (newFileContent, limitedFileContent);
	}

	private sealed class PageReportData : Dictionary<string, PageReport>
	{
		public bool Append(CommitInfo commitInfo, BenchmarkReport benchmarkReport)
		{
			foreach (BenchmarkReport.Benchmark benchmark in benchmarkReport.Benchmarks)
			{
				if (!Append(commitInfo, benchmark))
				{
					return false;
				}
			}

			return true;
		}

		private bool Append(CommitInfo commitInfo, BenchmarkReport.Benchmark benchmark)
		{
			if (!ParseMethod(benchmark.Method, out string name, out string type))
			{
				return false;
			}

			if (!IsIncluded(type))
			{
				return true;
			}
			
			if (!TryGetValue(name, out PageReport pageReport))
			{
				pageReport = new PageReport();
				this[name] = pageReport;
			}

			if (type == "aweXpect")
			{
				pageReport.Commits.Add(commitInfo);
				pageReport.Labels.Add(commitInfo.Sha.Substring(0, 8));
			}

			AppendTimeDataset(benchmark, pageReport, type);
			AppendMemoryDataset(benchmark, pageReport, type);

			return true;
		}

		void AppendMemoryDataset(BenchmarkReport.Benchmark benchmark, PageReport pageReport, string type)
		{
			PageReport.Dataset memoryDataset = pageReport.Datasets.FirstOrDefault(x
				=> x.Label.StartsWith(type, StringComparison.OrdinalIgnoreCase) && x.YAxisId == "y1");
			if (memoryDataset == null)
			{
				memoryDataset = new PageReport.Dataset
				{
					Label = $"{type} memory",
					Unit = "b",
					PointStyle = "triangle",
					BorderDash = [5, 5,],
					YAxisId = "y1",
					BackgroundColor = GetColor(type),
					BorderColor = GetColor(type),
					Data = new List<double>(),
				};
				pageReport.Datasets.Add(memoryDataset);
			}

			memoryDataset.Data.Add(benchmark.Metrics
				.Where(x => x.Descriptor.Id == "Allocated Memory")
				.Select(x => x.Value)
				.FirstOrDefault(double.NaN));
		}

		void AppendTimeDataset(BenchmarkReport.Benchmark benchmark, PageReport pageReport, string type)
		{
			PageReport.Dataset timeDataset = pageReport.Datasets.FirstOrDefault(x
				=> x.Label.StartsWith(type, StringComparison.OrdinalIgnoreCase) && x.YAxisId == "y");
			if (timeDataset == null)
			{
				timeDataset = new PageReport.Dataset
				{
					Label = $"{type} time",
					Unit = "ns",
					PointStyle = "circle",
					YAxisId = "y",
					BackgroundColor = GetColor(type),
					BorderColor = GetColor(type),
					Data = new List<double>(),
				};
				pageReport.Datasets.Add(timeDataset);
			}

			timeDataset.Data.Add(benchmark.Statistics.Mean);
		}

		bool IsIncluded(string type)
			=> type is "aweXpect" or "FluentAssertions" or "TUnit";

		string GetColor(string type)
			=> type switch
			{
				"aweXpect" => "#63A2AC",
				"FluentAssertions" => "#FF671B",
				"TUnit" => "#1A6029",
				_ => "#e84393",
			};

		private bool ParseMethod(string method, out string name, out string type)
		{
			int index = method.LastIndexOf('_');
			if (index <= 0)
			{
				name = null;
				type = null;
				return false;
			}

			name = method.Substring(0, index);
			type = method.Substring(index + 1);
			return true;
		}

		public PageReportData Limit(int limit)
		{
			PageReportData pageReportData = new();
			foreach (var (key, pageReport) in this)
			{
				pageReportData[key] = pageReport.Limit(limit);
			}

			return pageReportData;
		}
	}

	public class CommitInfo(string sha, string author, string date, string message)
	{
		[JsonPropertyName("sha")] public string Sha { get; } = sha;
		[JsonPropertyName("author")] public string Author { get; } = author;
		[JsonPropertyName("date")] public string Date { get; } = date;
		[JsonPropertyName("message")] public string Message { get; } = message;
	}

	private sealed class PageReport
	{
		[JsonPropertyName("commits")] public List<CommitInfo> Commits { get; init; } = new();
		[JsonPropertyName("labels")] public List<string> Labels { get; init; } = new();

		[JsonPropertyName("datasets")] public List<Dataset> Datasets { get; init; } = new();

		public PageReport Limit(int limit)
			=> new()
			{
				Commits = Commits.TakeLast(limit).ToList(),
				Labels = Labels.TakeLast(limit).ToList(),
				Datasets = Datasets.Select(dataset => dataset.Limit(limit)).ToList(),
			};

		public class Dataset
		{
			[JsonPropertyName("label")] public string Label { get; init; }
			[JsonPropertyName("unit")] public string Unit { get; set; }

			[JsonPropertyName("data")] public List<double> Data { get; init; }

			[JsonPropertyName("borderColor")] public string BorderColor { get; set; }

			[JsonPropertyName("backgroundColor")] public string BackgroundColor { get; set; }

			[JsonPropertyName("yAxisID")] public string YAxisId { get; init; }

			[JsonPropertyName("borderDash")] public int[] BorderDash { get; set; } = [];

			[JsonPropertyName("pointStyle")] public string PointStyle { get; set; }

			public Dataset Limit(int limit)
				=> new()
				{
					Label = Label,
					Unit = Unit,
					Data = Data.TakeLast(limit).ToList(),
					BorderColor = BorderColor,
					BackgroundColor = BackgroundColor,
					YAxisId = YAxisId,
					BorderDash = BorderDash,
					PointStyle = PointStyle,
				};
		}
	}

	private sealed class BenchmarkReport
	{
		public Benchmark[] Benchmarks { get; init; }

		public class Benchmark
		{
			public string Type { get; init; }
			public string Method { get; init; }
			public BenchmarkStatistics Statistics { get; init; }
			public BenchmarkMetrics[] Metrics { get; init; }
		}

		public class BenchmarkStatistics
		{
			public double Mean { get; init; }
		}

		public class BenchmarkMetrics
		{
			public double Value { get; init; }
			public BenchmarkMetricDescriptor Descriptor { get; init; }
		}

		public class BenchmarkMetricDescriptor
		{
			public string Id { get; init; }
			public string DisplayName { get; init; }
			public string Unit { get; init; }
		}
	}
}
