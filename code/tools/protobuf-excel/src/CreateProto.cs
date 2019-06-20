using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Excel;
using Library.Extensions;

public class CreateProto : BaseSystemConsole
{
    public static class table
    {
        public static int keysline
        {
            get { return Config.IniReadValue("table", "keysline").AsInt(-1); }
        }

        public static int descline
        {
            get { return Config.IniReadValue("table", "descline").AsInt(-1); }
        }

        public static int typeline
        {
            get { return Config.IniReadValue("table", "typeline").AsInt(-1); }
        }

        public static int nameline
        {
            get { return Config.IniReadValue("table", "nameline").AsInt(-1); }
        }

        public static List<int> lines
        {
            get { return new List<int> {keysline, descline, typeline, nameline}; }
        }
    }

    public string str_IDataEntityBase = @"namespace $namespace$ {
	public partial interface IData$Entity$Base {}
	public partial interface IData$Entity$Base<T> : IData$Entity$Base {
		T getKey ();
	}
	public partial class Data$Entity$Base<T> : IData$Entity$Base<T> {
		public virtual T getKey () {
			return default (T);
		}
	}
}";

    public string str_IDataMasterBase = @"using System.Collections.Generic;
using System.Linq;

namespace $namespace$ {
	public partial interface IData$Master$Base {
		void Init();
		string Name { get; }
	}
	public partial interface IData$Master$Base<T, TK> : IData$Master$Base where T : IData$Entity$Base<TK> {
		Dictionary<TK, T> Cache { get; set; }
		T Get$Entity$ (TK key);
		T Get$Entity$OrNull (TK key);
	}

	public partial class Data$Master$Base<T, TK> : IData$Master$Base<T, TK> where T : IData$Entity$Base<TK> {
		public virtual string Name { get; }
		public List<T> List { get; set; }
		public Dictionary<TK, T> Cache { get; set; }

		protected Data$Master$Base () {
			List = new List<T> ();
			Cache = new Dictionary<TK, T> ();
		}

		public virtual void Init () {
			List.TrimExcess();
			Cache = List.ToDictionary (p => p.getKey ());
			OnInitEnd ();
		}

		partial void OnInitEnd ();

		public virtual T Get$Entity$ (TK key) {
			T t = default (T);
			if (Cache.TryGetValue (key, out t))
				return t;
			return default (T);
		}

		public virtual T Get$Entity$OrNull (TK key) {
			T t = default (T);
			if (Cache.TryGetValue (key, out t))
				return t;
			return default (T);
		}
	}
}";

    public string str_entity_na = @"namespace $namespace$ {
	public partial class $EntityName$ {
		public override $IdType$ getKey () {
			return this.$Id$;
		}
	}
}";

    public string str_master_na = @"namespace $namespace$ {
	public partial class $MasterName$ {
		public override string Name { get { return $MasterName$; } }
	}
}";

    public string str_entity = @"
	public partial class $EntityName$ : $Partent$ {
		public override $IdType$ getKey () {
			return this.$Id$;
		}
	}";

    public string str_master = @"
	public partial class $MasterName$ : $Partent$ {
		public override string Name { get { return $FileName$; } }
		public override void Init () {
			List.AddRange (Data$Master$Manager.tables.$MasterName$);
			base.Init ();
		}
	}";

    public virtual string str_data_master
    {
        get
        {
            return @"using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace $namespace$ {
    public partial class Data$Master$Manager {

        public static tables tables { get; set; }

        public List<IData$Master$Base> dataObjects { get; private set; }

$ListField$

        public Data$Master$Manager () {
            dataObjects = new List<IData$Master$Base> ();
$List$
        }

        public void Init () {
            var stream = new MemoryStream (UnityEngine.Resources.Load<TextAsset> (""masters"").bytes);
            ProtoBuf.Serializer.Serialize<tables> (stream, tables);
            dataObjects.ForEach (p => p.Init ());
        }
    }
}";
        }
    }

    protected void ErrorOut(Exception e, DataTable dt)
    {
        Console.WriteLine("-".PadLeft(Console.WindowWidth - 1, '-'));
        Console.WriteLine(e.Message);
        Console.WriteLine("-".PadLeft(Console.WindowWidth - 1, '-'));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            Console.WriteLine(string.Join("\t|", dt.Rows[i].ItemArray.Select(q => q.ToString()).ToArray()));
        }
        Console.WriteLine("-".PadLeft(Console.WindowWidth - 1, '-'));
    }
}