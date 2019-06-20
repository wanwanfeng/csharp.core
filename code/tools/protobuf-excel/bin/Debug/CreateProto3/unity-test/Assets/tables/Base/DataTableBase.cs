namespace Table
{

	public partial class AiTable :  DataTableBase<AiInfo, int> {
		public override string Name { get { return"AiTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.AiTable);
			base.Init ();
		}
	}

	public partial class AudioTable :  DataTableBase<AudioInfo, int> {
		public override string Name { get { return"AudioTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.AudioTable);
			base.Init ();
		}
	}

	public partial class BasePropertyTable :  DataTableBase<BasePropertyInfo, string> {
		public override string Name { get { return"BasePropertyTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.BasePropertyTable);
			base.Init ();
		}
	}

	public partial class BattleActionTable :  DataTableBase<BattleActionInfo, string> {
		public override string Name { get { return"BattleActionTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.BattleActionTable);
			base.Init ();
		}
	}

	public partial class BattleLevelTable :  DataTableBase<BattleLevelInfo, string> {
		public override string Name { get { return"BattleLevelTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.BattleLevelTable);
			base.Init ();
		}
	}

	public partial class DialogueTable :  DataTableBase<DialogueInfo, string> {
		public override string Name { get { return"DialogueTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.DialogueTable);
			base.Init ();
		}
	}

	public partial class DToDTable :  DataTableBase<DToDInfo, int> {
		public override string Name { get { return"DToDTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.DToDTable);
			base.Init ();
		}
	}

	public partial class EquipTable :  DataTableBase<EquipInfo, string> {
		public override string Name { get { return"EquipTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.EquipTable);
			base.Init ();
		}
	}

	public partial class FxTable :  DataTableBase<FxInfo, int> {
		public override string Name { get { return"FxTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.FxTable);
			base.Init ();
		}
	}

	public partial class GuideTable :  DataTableBase<GuideInfo, string> {
		public override string Name { get { return"GuideTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.GuideTable);
			base.Init ();
		}
	}

	public partial class HeroTable :  DataTableBase<HeroInfo, string> {
		public override string Name { get { return"HeroTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.HeroTable);
			base.Init ();
		}
	}

	public partial class HostTable :  DataTableBase<HostInfo, int> {
		public override string Name { get { return"HostTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.HostTable);
			base.Init ();
		}
	}

	public partial class ItemTable :  DataTableBase<ItemInfo, string> {
		public override string Name { get { return"ItemTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.ItemTable);
			base.Init ();
		}
	}

	public partial class LanguageTable :  DataTableBase<LanguageInfo, int> {
		public override string Name { get { return"LanguageTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.LanguageTable);
			base.Init ();
		}
	}

	public partial class LocalLanguageTable :  DataTableBase<LocalLanguageInfo, int> {
		public override string Name { get { return"LocalLanguageTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.LocalLanguageTable);
			base.Init ();
		}
	}

	public partial class MiscTable :  DataTableBase<MiscInfo, int> {
		public override string Name { get { return"MiscTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.MiscTable);
			base.Init ();
		}
	}

	public partial class ModelTable :  DataTableBase<ModelInfo, int> {
		public override string Name { get { return"ModelTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.ModelTable);
			base.Init ();
		}
	}

	public partial class MovieTable :  DataTableBase<MovieInfo, int> {
		public override string Name { get { return"MovieTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.MovieTable);
			base.Init ();
		}
	}

	public partial class OpenFunctionTable :  DataTableBase<OpenFunctionInfo, string> {
		public override string Name { get { return"OpenFunctionTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.OpenFunctionTable);
			base.Init ();
		}
	}

	public partial class ShopGoodsTable :  DataTableBase<ShopGoodsInfo, string> {
		public override string Name { get { return"ShopGoodsTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.ShopGoodsTable);
			base.Init ();
		}
	}

	public partial class ShopTable :  DataTableBase<ShopInfo, string> {
		public override string Name { get { return"ShopTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.ShopTable);
			base.Init ();
		}
	}

	public partial class SkillTable :  DataTableBase<SkillInfo, string> {
		public override string Name { get { return"SkillTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.SkillTable);
			base.Init ();
		}
	}

	public partial class TaskTable :  DataTableBase<TaskInfo, string> {
		public override string Name { get { return"TaskTable"; } }
		public override void Init () {
			List.AddRange (DataTableManager.tables.TaskTable);
			base.Init ();
		}
	}
}
