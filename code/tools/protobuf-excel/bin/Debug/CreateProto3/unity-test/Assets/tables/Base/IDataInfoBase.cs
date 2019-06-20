namespace Table {
	public partial interface IDataInfoBase {}
	public partial interface IDataInfoBase<T> : IDataInfoBase {
		T getKey ();
	}
	public partial class DataInfoBase<T> : IDataInfoBase<T> {
		public virtual T getKey () {
			return default (T);
		}
	}
}