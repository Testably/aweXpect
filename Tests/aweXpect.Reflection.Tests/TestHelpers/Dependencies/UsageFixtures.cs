using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Store;

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Store
{
	public class TrackStore
	{
		public TrackStore? Next { get; set; }
	}

	public class GenericStore<T>
	{
		public GenericStore<T>? Next { get; set; }
	}

	public class UnusedStore;

	public class CacheStore<T>
	{
		public CacheStore<int>? Root { get; set; }
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Infrastructure
{
	public class StoreAdapter
	{
		public GenericStore<int>? Generic { get; set; }
		public TrackStore? Store { get; set; }
	}

	public class StoreCache<T>
	{
		public CacheStore<T>? Store { get; set; }
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Api
{
	public class TrackController
	{
		public TrackStore[]? Stores { get; set; }
	}
}

namespace aweXpect.Reflection.Tests.TestHelpers.Dependencies.Usage.Jobs
{
	public class SyncJob
	{
		public GenericStore<string>? Generic { get; set; }
		public TrackStore? Store { get; set; }
	}

	public class BodyOnlyJob
	{
		public static object Run() => new UnusedStore();
	}

	public class CacheJob
	{
		public CacheStore<int>? Numbers { get; set; }
		public CacheStore<string>? Texts { get; set; }
	}
}
