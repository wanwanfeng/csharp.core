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
        get { return @""; }
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

    public string _Entity = "Info";
    public string _Master = "Table";
    public string _NameSpace = "Table";

    public string Replace(string str)
    {
        return str.Replace("$namespace$", _NameSpace).Replace("$Entity$", _Entity).Replace("$Master$", _Master);
    }

    protected void CreateCS(List<string> relultTableList, List<string> relultInfosList, List<string> fileList)
    {
        relultTableList.Add("}");
        WriteAllLines("tables/Base/DataTableBase.cs", relultTableList.ToArray());
        relultInfosList.Add("}");
        WriteAllLines("tables/Base/DataInfoBase.cs", relultInfosList.ToArray());

        WriteAllText("tables/Base/IDataInfoBase.cs", Replace(str_IDataEntityBase));
        WriteAllText("tables/Base/IDataTableBase.cs", Replace(str_IDataMasterBase));

        string temp = 
            Replace(str_data_master)
            .Replace("$List$",string.Join("\n",fileList.Select(p => string.Format("\t\t\tdataObjects.Add ({0} = new {0}());", p)).ToArray()))
            .Replace("$ListField$",string.Join("\n", fileList.Select(p => string.Format("\t\tpublic {0} {0} {{ get; set; }}", p)).ToArray()));
        WriteAllText("tables/DataTableManager.cs", temp);
    }
}