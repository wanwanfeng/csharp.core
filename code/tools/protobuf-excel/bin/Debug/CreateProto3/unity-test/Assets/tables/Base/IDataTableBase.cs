using System.Collections.Generic;
using System.Linq;

namespace Table {
	public partial interface IDataTableBase {
		void Init();
		string Name { get; }
	}
	public partial interface IDataTableBase<T, TK> : IDataTableBase where T : IDataInfoBase<TK> {
		Dictionary<TK, T> Cache { get; set; }
		T GetInfo (TK key);
		T GetInfoOrNull (TK key);
	}

	public partial class DataTableBase<T, TK> : IDataTableBase<T, TK> where T : IDataInfoBase<TK> {
		public virtual string Name { get; }
		public List<T> List { get; set; }
		public Dictionary<TK, T> Cache { get; set; }

		protected DataTableBase () {
			List = new List<T> ();
			Cache = new Dictionary<TK, T> ();
		}

		public virtual void Init () {
			List.TrimExcess();
			Cache = List.ToDictionary (p => p.getKey ());
			OnInitEnd ();
		}

		partial void OnInitEnd ();

		public virtual T GetInfo (TK key) {
			T t = default (T);
			if (Cache.TryGetValue (key, out t))
				return t;
			return default (T);
		}

		public virtual T GetInfoOrNull (TK key) {
			T t = default (T);
			if (Cache.TryGetValue (key, out t))
				return t;
			return default (T);
		}
	}
}