using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using System;
public class test : MonoBehaviour {

	void Start () {
		var data = new DataTableManager ();
		data.Init();
		Debug.Log(data.AudioTable.List[0]);
		Debug.Log(data.AiTable.List[0]);
		Debug.Log(data.BasePropertyTable.List[0]);
		Debug.Log(data.BattleLevelTable.List[0]);
		Debug.Log(data.TaskTable.List[0]);
		Debug.Log(data.GuideTable.List[0]);
	}
}