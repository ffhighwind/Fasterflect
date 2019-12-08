#region License

// Copyright © 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/

#endregion

#if (NET45 || NETSTANDARD || NETCOREAPP)
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fasterflect
{
	[DebuggerStepThrough]
	internal sealed class Cache<TKey, TValue>
		where TValue : class
	{
		private readonly ConcurrentDictionary<TKey, WeakReference<TValue>> entries;

		public Cache()
		{
			entries = new ConcurrentDictionary<TKey, WeakReference<TValue>>();
		}

		public Cache(IEqualityComparer<TKey> equalityComparer)
		{
			entries = new ConcurrentDictionary<TKey, WeakReference<TValue>>(equalityComparer);
		}

		#region Properties
		/// <summary>
		/// Returns the number of entries currently stored in the cache. Accessing this property
		/// causes a check of all entries in the cache to ensure collected entries are not counted.
		/// </summary>
		public int Count => ClearCollected();
		#endregion

		#region Indexers
		/// <summary>
		/// Indexer for accessing or adding cache entries.
		/// </summary>
		public TValue this[TKey key] {
			get => Get(key);
			set => Insert(key, value);
		}
		#endregion

		#region Insert Methods
		/// <summary>
		/// Insert a collectible object into the cache.
		/// </summary>
		/// <param name="key">The cache key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		public void Insert(TKey key, TValue value)
		{
			entries[key] = new WeakReference<TValue>(value);
		}
		#endregion

		#region Get Methods
		/// <summary>
		/// Retrieves an entry from the cache using the given key.
		/// </summary>
		/// <param name="key">The cache key of the item to retrieve.</param>
		/// <returns>The retrieved cache item or null if not found.</returns>
		public TValue Get(TKey key)
		{
			if (entries.TryGetValue(key, out WeakReference<TValue> entry)) {
				entry.TryGetTarget(out TValue target);
				return target;
			}
			return null;
		}
		#endregion

		#region Remove Methods
		/// <summary>
		/// Removes the object associated with the given key from the cache.
		/// </summary>
		/// <param name="key">The cache key of the item to remove.</param>
		/// <returns>True if an item removed from the cache and false otherwise.</returns>
		public bool Remove(TKey key)
		{
			return entries.TryRemove(key, out WeakReference<TValue> _);
		}
		#endregion

		#region Clear Methods
		/// <summary>
		/// Removes all entries from the cache.
		/// </summary>
		public void Clear()
		{
			entries.Clear();
		}

		/// <summary>
		/// Process all entries in the cache and remove entries that refer to collected entries.
		/// </summary>
		/// <returns>The number of live cache entries still in the cache.</returns>
		private int ClearCollected()
		{
			List<TKey> keys = new List<TKey>();
			foreach (var kv in entries) {
				if (!kv.Value.TryGetTarget(out TValue target)) {
					keys.Add(kv.Key);
				}
			}
			foreach (TKey key in keys) {
				entries.TryRemove(key, out WeakReference<TValue> _);
			}
			return entries.Count;
		}
		#endregion

		#region ToString
		/// <summary>
		/// This method returns a string with information on the cache contents (number of contained objects).
		/// </summary>
		public override string ToString()
		{
			int count = ClearCollected();
			return count > 0 ? string.Format("Cache contains {0} live objects.", count) : "Cache is empty.";
		}
		#endregion
	}
}

#elif NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace Fasterflect
{
	[DebuggerStepThrough]
	internal sealed class Cache<TKey, TValue>
	{
		private readonly Dictionary<TKey, WeakReference> entries;
		private int owner;

#region Constructors
		public Cache()
		{
			entries = new Dictionary<TKey, WeakReference>();
		}

		public Cache(IEqualityComparer<TKey> equalityComparer)
		{
			entries = new Dictionary<TKey, WeakReference>(equalityComparer);
		}
#endregion

#region Properties
		/// <summary>
		/// Returns the number of entries currently stored in the cache. Accessing this property
		/// causes a check of all entries in the cache to ensure collected entries are not counted.
		/// </summary>
		public int Count {
			get { return ClearCollected(); }
		}
#endregion

#region Indexers
		/// <summary>
		/// Indexer for accessing or adding cache entries.
		/// </summary>
		public TValue this[TKey key] {
			get => Get(key);
			set => Insert(key, value);
		}
#endregion

#region Insert Methods
		/// <summary>
		/// Insert a collectible object into the cache.
		/// </summary>
		/// <param name="key">The cache key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		public void Insert(TKey key, TValue value)
		{
			WeakReference entry = new WeakReference(value);
			int current = Thread.CurrentThread.ManagedThreadId;
			while (Interlocked.CompareExchange(ref owner, current, 0) != current) { }
			entries[key] = entry;
			if (current != Interlocked.Exchange(ref owner, 0))
				throw new UnauthorizedAccessException("Thread had access to cache even though it shouldn't have.");
		}
#endregion

#region GetValue Methods
		/// <summary>
		/// Retrieves an entry from the cache using the given key.
		/// </summary>
		/// <param name="key">The cache key of the item to retrieve.</param>
		/// <returns>The retrieved cache item or null if not found.</returns>
		public TValue Get(TKey key)
		{
			int current = Thread.CurrentThread.ManagedThreadId;
			while (Interlocked.CompareExchange(ref owner, current, 0) != current) { }
			entries.TryGetValue(key, out WeakReference entry);
			if (current != Interlocked.Exchange(ref owner, 0))
				throw new UnauthorizedAccessException("Thread had access to cache even though it shouldn't have.");
			return (TValue)(entry is WeakReference wr ? wr.Target : entry);
		}
#endregion

#region Remove Methods
		/// <summary>
		/// Removes the object associated with the given key from the cache.
		/// </summary>
		/// <param name="key">The cache key of the item to remove.</param>
		/// <returns>True if an item removed from the cache and false otherwise.</returns>
		public bool Remove(TKey key)
		{
			int current = Thread.CurrentThread.ManagedThreadId;
			while (Interlocked.CompareExchange(ref owner, current, 0) != current) { }
			bool found = entries.Remove(key);
			if (current != Interlocked.Exchange(ref owner, 0))
				throw new UnauthorizedAccessException("Thread had access to cache even though it shouldn't have.");
			return found;
		}
#endregion

#region Clear Methods
		/// <summary>
		/// Removes all entries from the cache.
		/// </summary>
		public void Clear()
		{
			int current = Thread.CurrentThread.ManagedThreadId;
			while (Interlocked.CompareExchange(ref owner, current, 0) != current) { }
			entries.Clear();
			if (current != Interlocked.Exchange(ref owner, 0))
				throw new UnauthorizedAccessException("Thread had access to cache even though it shouldn't have.");
		}

		/// <summary>
		/// Process all entries in the cache and remove entries that refer to collected entries.
		/// </summary>
		/// <returns>The number of live cache entries still in the cache.</returns>
		private int ClearCollected()
		{
			int current = Thread.CurrentThread.ManagedThreadId;
			while (Interlocked.CompareExchange(ref owner, current, 0) != current) { }
			List<TKey> keys = entries.Where(kvp => kvp.Value is WeakReference && !(kvp.Value as WeakReference).IsAlive).Select(kvp => kvp.Key).ToList();
			foreach(var key in keys) {
				entries.Remove(key);
			}
			int count = entries.Count;
			if (current != Interlocked.Exchange(ref owner, 0))
				throw new UnauthorizedAccessException("Thread had access to cache even though it shouldn't have.");
			return count;
		}
#endregion

#region ToString
		/// <summary>
		/// This method returns a string with information on the cache contents (number of contained objects).
		/// </summary>
		public override string ToString()
		{
			int count = ClearCollected();
			return count > 0 ? String.Format("Cache contains {0} live objects.", count) : "Cache is empty.";
		}
#endregion
	}
}
#else
#error At least one of the compilation symbols NET45, NET35, or NETSTANDARD2_0 must be defined. 
#endif
