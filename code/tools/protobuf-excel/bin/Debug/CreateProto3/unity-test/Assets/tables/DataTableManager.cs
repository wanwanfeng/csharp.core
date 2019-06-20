using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Table {
	public partial class DataTableManager {

		public static tables tables { get; set; }

		public List<IDataTableBase> dataObjects { get; private set; }

		public AiTable AiTable { get; set; }
		public AudioTable AudioTable { get; set; }
		public BasePropertyTable BasePropertyTable { get; set; }
		public BattleActionTable BattleActionTable { get; set; }
		public BattleLevelTable BattleLevelTable { get; set; }
		public DialogueTable DialogueTable { get; set; }
		public DToDTable DToDTable { get; set; }
		public EquipTable EquipTable { get; set; }
		public FxTable FxTable { get; set; }
		public GuideTable GuideTable { get; set; }
		public HeroTable HeroTable { get; set; }
		public HostTable HostTable { get; set; }
		public ItemTable ItemTable { get; set; }
		public LanguageTable LanguageTable { get; set; }
		public LocalLanguageTable LocalLanguageTable { get; set; }
		public MiscTable MiscTable { get; set; }
		public ModelTable ModelTable { get; set; }
		public MovieTable MovieTable { get; set; }
		public OpenFunctionTable OpenFunctionTable { get; set; }
		public ShopGoodsTable ShopGoodsTable { get; set; }
		public ShopTable ShopTable { get; set; }
		public SkillTable SkillTable { get; set; }
		public TaskTable TaskTable { get; set; }

		public DataTableManager () {
			dataObjects = new List<IDataTableBase> ();
			dataObjects.Add (AiTable = new AiTable ());
			dataObjects.Add (AudioTable = new AudioTable ());
			dataObjects.Add (BasePropertyTable = new BasePropertyTable ());
			dataObjects.Add (BattleActionTable = new BattleActionTable ());
			dataObjects.Add (BattleLevelTable = new BattleLevelTable ());
			dataObjects.Add (DialogueTable = new DialogueTable ());
			dataObjects.Add (DToDTable = new DToDTable ());
			dataObjects.Add (EquipTable = new EquipTable ());
			dataObjects.Add (FxTable = new FxTable ());
			dataObjects.Add (GuideTable = new GuideTable ());
			dataObjects.Add (HeroTable = new HeroTable ());
			dataObjects.Add (HostTable = new HostTable ());
			dataObjects.Add (ItemTable = new ItemTable ());
			dataObjects.Add (LanguageTable = new LanguageTable ());
			dataObjects.Add (LocalLanguageTable = new LocalLanguageTable ());
			dataObjects.Add (MiscTable = new MiscTable ());
			dataObjects.Add (ModelTable = new ModelTable ());
			dataObjects.Add (MovieTable = new MovieTable ());
			dataObjects.Add (OpenFunctionTable = new OpenFunctionTable ());
			dataObjects.Add (ShopGoodsTable = new ShopGoodsTable ());
			dataObjects.Add (ShopTable = new ShopTable ());
			dataObjects.Add (SkillTable = new SkillTable ());
			dataObjects.Add (TaskTable = new TaskTable ());
		}

		public void Init () {
			tables = Table.tables.Parser.ParseFrom (System.Convert.FromBase64String (System.IO.File.ReadAllText ("masters.data")));
			dataObjects.ForEach (p => p.Init ());
		}
	}
}