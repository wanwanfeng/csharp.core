using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Excel;
using Library.Extensions;
using Library.Helper;
using LitJson;

namespace checkcard.Scripts
{
    public class ExportScenario : BaseSystemConsole
    {
        public ExportScenario()
        {
            var faied = new ConcurrentBag<string>();
            faied.Add(DateTime.Now.ToString("yy-MM-dd hh:mm:ss"));
            CheckPath(".xlsx").AsParallel().SelectMany(file =>
            {
                Console.WriteLine(" from : " + file);
                return ExcelByBase.Data.ImportToDataTable(file);
            }).ForAll(dt =>
            {
                Console.WriteLine(" is now : " + dt.FullName);
                if (!dt.IsArray) return;

                try
                {
                    var result = GetJsonValue(dt);
                    if (result == null)
                    {
                        faied.Add(dt.FullName);
                        return;
                    }
                    File.WriteAllText(Path.ChangeExtension(dt.FullName, "json"), JsonHelper.ToJson(result));
                }
                catch (Exception e)
                {
                    faied.Add(dt.FullName);
                    faied.Add("#" + e.StackTrace);
                }
            });
            faied.Add(DateTime.Now.ToString("yy-MM-dd hh:mm:ss"));
            faied.Add(InputPath + " " + string.Join("|", faied.Select(Path.GetFileName).ToList()));
            WriteError("error", faied);
        }

        class ColumIndex
        {
            /// <summary>
            /// 0
            /// </summary>
            public static int group = 0;

            /// <summary>
            /// 1
            /// </summary>
            public static int index = 1;

            /// <summary>
            /// 2
            /// </summary>
            public static int desc = 2;

            /// <summary>
            /// 3
            /// </summary>
            public static int value = 3;

            /// <summary>
            /// 4
            /// </summary>
            public static int key = 4;

            /// <summary>
            /// 5
            /// </summary>
            public static int second = 5;
        }


        List<string> turn_param_float = new List<string>()
        {
            "autoTurn",
            "autoTurnFirst",
            "autoTurnLast",
            "turnChangeInTime",
            "turnChangeOutTime",
            "narrationCoverOpacity",
            "narrationLineSpace",
            "narrationLineMargin",
            "narrationFontSize",
        };

        List<string> turn_param_int = new List<string>()
        {
            "enableSkipAutoTurn",
        };

        List<string> turn_param_list = new List<string>()
        {
        };

        List<string> turn_param_int_list = new List<string>()
        {
            "deleteArmatureList",
        };

        List<string> object_param_int = new List<string>()
        {
            "id",
            "motion",
            "tear",
            "mouthOpen",
            "eyeClose",
            "cheek",
            "pos",
            "posX",
            "posY",
            "flippedX",
            "questPosY",
            "lipSynch",
            "soulGem",
            "alternativeId",
            "armatureId",
        };

        List<string> object_param_float = new List<string>()
        {
            "scale",
        };

        List<string> object_param_list = new List<string>()
        {
            "animation",
        };

        bool can_create_skip_transition_list(DataTable sheet, int row)
        {
            var value = sheet.Rows[row][ColumIndex.group].ToString();
            return value == "skipTransitionList";
        }

        bool can_create_skip_transition(DataTable sheet, int row)
        {
            var value = sheet.Rows[row][ColumIndex.index].ToString();
            return value == "skipTransition";
        }

        bool can_create_group(DataTable sheet, int row)
        {
            var value = sheet.Rows[row][ColumIndex.group].ToString();
            return value != "";
        }

        bool can_create_skip_transition_param(DataTable sheet, int row)
        {
            var value = sheet.Rows[row][ColumIndex.desc].ToString();
            return value != "";
        }

        void add_param_skip_transition(string key, object value, JsonData skip_transition)
        {
            if (key == "canSkip")
                value = (bool) value.ToString().AsBool(true);
            skip_transition[key] = new JsonData(value);
        }

        bool can_create_turn(DataTable sheet, int row)
        {
            var value = sheet.Rows[row].ItemArray[ColumIndex.index].ToString();
            return value.AsInt() > 0;
        }

        bool can_create_turn_param(DataTable sheet, int row)
        {
            var value = sheet.Rows[row][ColumIndex.key].ToString();
            return value != ""; //右移
        }

        private bool is_single_param(DataTable sheet, int row)
        {
            if (row + 1 == sheet.Rows.Count || sheet.Columns.Count <= ColumIndex.second)
                return true;
            return sheet.Rows[row + 1][ColumIndex.second].ToString() == ""; //右移
        }

        void add_single_param(object k, object v, JsonData turn)
        {
            string key = k.ToString();
            string value = v.ToString();
            if (turn_param_float.Contains(key))
            {
                turn[key] = new JsonData(value.AsFloat());
            }
            else if (turn_param_int.Contains(key))
            {
                turn[key] = new JsonData(value.AsInt());
            }
            else if (turn_param_list.Contains(key))
            {
                var list = value.Split(',').ToList();
                JsonData temp = new JsonData();
                list.ForEach(p => temp.Add(p));
                turn[key] = temp;
            }
            else if (turn_param_int_list.Contains(key))
            {
                var list = value.Split(',').AsIntArray().ToList();
                JsonData temp = new JsonData();
                list.ForEach(p => temp.Add(p));
                turn[key] = temp;
            }
            else
            {
                turn[key] = new JsonData(value);
            }
        }

        bool can_create_object_param(DataTable sheet, int row)
        {
            return row < sheet.Rows.Count && sheet.Rows[row][ColumIndex.second].ToString() != ""; //右移
        }

        void add_object_param(object k, object v, JsonData turn)
        {
            string key = k.ToString();
            string value = v.ToString();

            if (object_param_float.Contains(key))
            {
                turn[key] = new JsonData(value.AsFloat());
            }
            else if (object_param_int.Contains(key))
            {
                turn[key] = new JsonData(value.AsInt());
            }
            else if (object_param_list.Contains(key))
            {
                var list = value.Split(',').ToList();
                JsonData temp = new JsonData();
                list.ForEach(p => temp.Add(p));
                turn[key] = temp;
            }
            else
            {
                turn[key] = new JsonData(value);
            }
        }

        private JsonData GetJsonValue(DataTable dt)
        {
            JsonData m_story = new JsonData();
            m_story["version"] = dt.Rows[0][ColumIndex.index].ToString().AsInt();
            m_story["story"] = new JsonData();

            JsonData skip_transition = new JsonData();
            JsonData group = new JsonData();
            JsonData turn = new JsonData();
            JsonData chara_list = new JsonData();
            JsonData item_list = new JsonData();
            JsonData select_list = new JsonData();
            JsonData armature_list = new JsonData();
            JsonData witch = new JsonData();

            var row = 2;
            var sheet_1 = dt;

            bool isFailed = false;

            while (row < dt.Rows.Count)
            {
                if (can_create_skip_transition_list(sheet_1, row))
                {
                    JsonData skip_transition_list = new JsonData();
                    skip_transition_list.SetJsonType(JsonType.Array);
                    m_story["skipTransitionList"] = skip_transition_list;

                    row++;
                    while (true)
                    {
                        if (can_create_group(sheet_1, row))
                        {
                            break;
                        }
                        if (can_create_skip_transition(sheet_1, row))
                        {
                            skip_transition = new JsonData();
                            skip_transition.SetJsonType(JsonType.Object);
                            skip_transition_list.Add(skip_transition);
                            row++;
                        }
                        if (can_create_skip_transition_param(sheet_1, row))
                        {
                            var key = sheet_1.Rows[row][ColumIndex.desc].ToString();
                            var value = sheet_1.Rows[row][ColumIndex.key - 1];
                            add_param_skip_transition(key, value, skip_transition);
                            row++;
                        }
                    }
                }

                if (can_create_group(sheet_1, row))
                {
                    var group_id = sheet_1.Rows[row][ColumIndex.group].ToString();
                    group = new JsonData();
                    group.SetJsonType(JsonType.Array);
                    m_story["story"][group_id] = group;
                    row++;
                    continue;
                }

                if (can_create_turn(sheet_1, row))
                {
                    turn = new JsonData();
                    turn.SetJsonType(JsonType.Object);
                    group.Add(turn);
                    row++;
                    continue;
                }

                if (!can_create_turn_param(sheet_1, row))
                {
                    row++;
                    continue;
                }

                if (is_single_param(sheet_1, row))
                {
                    var key = sheet_1.Rows[row][ColumIndex.key]; //右移
                    var value = sheet_1.Rows[row][ColumIndex.value]; //右移
                    add_single_param(key, value, turn);
                    row++;
                    continue;
                }

                {
                    var kIndex = ColumIndex.second;
                    var vIndex = ColumIndex.key;

                    var key = sheet_1.Rows[row][ColumIndex.key].ToString();

                    switch (key)
                    {
                        case "chara":
                        {
                            if (turn.ContainsKey(key) == false)
                            {
                                chara_list = new JsonData();
                                chara_list.SetJsonType(JsonType.Array);
                                turn[key] = chara_list;
                            }

                            var chara = new JsonData();
                            chara.SetJsonType(JsonType.Object);
                            chara_list.Add(chara);
                            row++;

                            while (true)
                            {
                                if (!can_create_object_param(sheet_1, row))
                                    break;
                                var k = sheet_1.Rows[row][kIndex];
                                var v = sheet_1.Rows[row][vIndex];
                                add_object_param(k, v, chara);
                                row++;
                            }
                            break;
                        }
                        case "item":
                        {
                            if (turn.ContainsKey(key) == false)
                            {
                                item_list = new JsonData();
                                item_list.SetJsonType(JsonType.Array);
                                turn[key] = item_list;
                            }
                            var item = new JsonData();
                            item.SetJsonType(JsonType.Object);
                            item_list.Add(item);
                            row++;

                            while (true)
                            {
                                if (!can_create_object_param(sheet_1, row))
                                    break;
                                var k = sheet_1.Rows[row][kIndex];
                                var v = sheet_1.Rows[row][vIndex];
                                add_object_param(k, v, item);
                                row++;
                            }
                            break;
                        }
                        case "select":
                        {
                            if (turn.ContainsKey(key) == false)
                            {
                                select_list = new JsonData();
                                select_list.SetJsonType(JsonType.Array);
                                turn[key] = select_list;
                            }
                            var select = new JsonData();
                            select.SetJsonType(JsonType.Object);
                            select_list.Add(select);
                            row++;

                            while (true)
                            {
                                if (!can_create_object_param(sheet_1, row))
                                    break;
                                var k = sheet_1.Rows[row][kIndex];
                                var v = sheet_1.Rows[row][vIndex];
                                add_object_param(k, v, select);
                                row++;
                            }
                            break;
                        }
                        case "armatureList":
                        {
                            if (turn.ContainsKey(key) == false)
                            {
                                armature_list = new JsonData();
                                armature_list.SetJsonType(JsonType.Array);
                                turn[key] = armature_list;
                            }
                            var armature = new JsonData();
                            armature.SetJsonType(JsonType.Object);
                            armature_list.Add(armature);
                            row++;

                            while (true)
                            {
                                if (!can_create_object_param(sheet_1, row))
                                    break;
                                var k = sheet_1.Rows[row][kIndex];
                                var v = sheet_1.Rows[row][vIndex];
                                add_object_param(k, v, armature);
                                row++;
                            }
                            break;
                        }
                        case "witchAppear":
                        {
                            if (turn.ContainsKey(key) == false)
                            {
                                witch = new JsonData();
                                witch.SetJsonType(JsonType.Object);
                                turn[key] = witch;
                            }
                            row++;

                            while (true)
                            {
                                if (!can_create_object_param(sheet_1, row))
                                    break;
                                var k = sheet_1.Rows[row][kIndex].ToString();
                                var v = sheet_1.Rows[row][vIndex];
                                witch[k] = new JsonData(k == "witchId" ? (int) v : v);
                                row++;
                            }
                            break;
                        }
                        default:
                            isFailed = true;
                            goto end;
                            break;
                    }
                }
            }
            end:
            return isFailed ? null : m_story;
        }
    }
}