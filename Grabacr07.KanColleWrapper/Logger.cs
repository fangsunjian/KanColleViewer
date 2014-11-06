using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using System.Text;

namespace Grabacr07.KanColleWrapper
{
	public class LogContentBattleResult
	{
		public string QuestName { get; set; }
		public string EnemyTeam { get; set; }
		public string Rank { get; set; }
		public string DropShip { get; set; }
		public string Time { get; set; }
        public override string ToString()
        {
            return QuestName;
        }
	}

	public class Logger : NotificationObject
	{
		public bool EnableLogging { get { return KanColleClient.Current.Settings.EnableLogging; } }

		private bool waitingForShip;
		private int dockid;
		private readonly int[] shipmats;

		StringBuilder sb;

        public string Content { get { return sb.ToString(); } }

        private string _LatestDropShip;
        public string LatestDropShip 
        {
            get { return _LatestDropShip; }
            set {
                if (value != _LatestDropShip)
                {
                    _LatestDropShip = value;
                    this.RaisePropertyChanged("DropShip");
                }
            
            }
        }

		public List<LogContentBattleResult> ContentBattleResult = new List<LogContentBattleResult>();

		private enum LogType
		{
			BuildItem,
			BuildShip,
			ShipDrop
		};

		public void Test()
		{
            LogContentBattleResult result = new LogContentBattleResult()
            {
                QuestName = "Test Quest",
                EnemyTeam = "Enemy",
                Rank = "S",
                DropShip = "Lol",
                Time = DateTime.Now.ToString("M/d/yyyy H:mm")

            };
            ContentBattleResult.Add(result);
			this.RaisePropertyChanged("Log");
		}

		internal Logger(KanColleProxy proxy)
		{
			this.shipmats = new int[5];
			sb = new StringBuilder();
//             LogContentBattleResult result = new LogContentBattleResult()
//             {
//                 QuestName = "Test Quest",
//                 EnemyTeam = "Enemy",
//                 Rank = "S",
//                 DropShip = "Lol",
//                 Time = DateTime.Now.ToString("M/d/yyyy H:mm")
// 
//             };
//             ContentBattleResult.Add(result);
			// ちょっと考えなおす
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data, x.Request));
			proxy.api_req_kousyou_createship.TryParse<kcsapi_createship>().Subscribe(x =>  this.CreateShip(x.Request));
			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x =>this.KDock(x.Data));
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x =>this.BattleResult(x.Data));
//             proxy.api_req_map_start.TryParse<kcsapi_map_start>().Subscribe(x => this.ShipStart(x.Data));
//             proxy.api_req_map_next.TryParse<kcsapi_map_next>().Subscribe(x => this.ShipNext(x.Data));
		}

		private void ShipStart(kcsapi_map_start item)
		{
			sb.AppendLine("GOTO:" + item.api_next);
			this.RaisePropertyChanged("Log");
		}

		private void ShipNext(kcsapi_map_next item)
		{
			sb.AppendLine("GOTO:" + item.api_next);
			this.RaisePropertyChanged("Log");
		}

		private void CreateItem(kcsapi_createitem item, NameValueCollection req)
		{
            this.Log(LogType.BuildItem, "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
				KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name,
                KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Level,
                KanColleClient.Current.Homeport.Admiral.Level,
				req["api_item1"], req["api_item2"], req["api_item3"], req["api_item4"],
                item.api_create_flag == 1 ? KanColleClient.Current.Master.SlotItems[item.api_slot_item.api_slotitem_id].Name : "N/A",
                DateTime.Now.ToString("M/d/yyyy H:mm"));
		}

		private void CreateShip(NameValueCollection req)
		{
			this.waitingForShip = true;
			this.dockid = Int32.Parse(req["api_kdock_id"]);
			this.shipmats[0] = Int32.Parse(req["api_item1"]);
			this.shipmats[1] = Int32.Parse(req["api_item2"]);
			this.shipmats[2] = Int32.Parse(req["api_item3"]);
			this.shipmats[3] = Int32.Parse(req["api_item4"]);
			this.shipmats[4] = Int32.Parse(req["api_item5"]);
		}

		private void KDock(kcsapi_kdock[] docks)
		{
			foreach (var dock in docks.Where(dock => this.waitingForShip && dock.api_id == this.dockid))
			{
                this.Log(LogType.BuildShip, "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Info.Name,
                    KanColleClient.Current.Homeport.Organization.Fleets[1].Ships[0].Level,
                    KanColleClient.Current.Homeport.Admiral.Level,
                    this.shipmats[0], this.shipmats[1], this.shipmats[2], this.shipmats[3], this.shipmats[4],
                    KanColleClient.Current.Master.Ships[dock.api_created_ship_id].Name,
                    DateTime.Now.ToString("M/d/yyyy H:mm"));
				this.waitingForShip = false;
			}
		}

		private void BattleResult(kcsapi_battleresult br)
		{
// 			if (br.api_get_ship == null)
// 				return;

			LogContentBattleResult result = new LogContentBattleResult()
			{
				QuestName = br.api_quest_name,
				EnemyTeam = br.api_enemy_info.api_deck_name,
				Rank = br.api_win_rank,
				DropShip = br.api_get_ship == null ? "" : br.api_get_ship.api_ship_name,
				Time = DateTime.Now.ToString("M/d/yyyy H:mm")

			};

			ContentBattleResult.Add(result);

            LatestDropShip = br.api_get_ship == null ? "N/A" : br.api_get_ship.api_ship_name;

			this.Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", br.api_quest_name, br.api_enemy_info.api_deck_name, br.api_win_rank, br.api_get_ship == null ? "" : br.api_get_ship.api_ship_name, DateTime.Now.ToString("M/d/yyyy H:mm"));
		}

		private void Log(LogType type, string format, params object[] args)
		{
			if (!this.EnableLogging) return;

			var mainFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			switch (type)
			{
				case LogType.BuildItem:
					if (!File.Exists(mainFolder + "\\ItemBuildLog.csv"))
					{
						using (StreamWriter w = new StreamWriter(mainFolder + "\\ItemBuildLog.csv", true, Encoding.UTF8))
						{
                            w.WriteLine("Secretary,SecretaryLv,CommandLv,Fuel,Ammo,Steel,Bauxite,Result,Date", args);
						}
					}
					using (StreamWriter w = new StreamWriter(mainFolder + "\\ItemBuildLog.csv", true, Encoding.UTF8))
					{
						w.WriteLine(format, args);
					}
					break;

				case LogType.BuildShip:
					if (!File.Exists(mainFolder + "\\ShipBuildLog.csv"))
					{
						using (StreamWriter w = new StreamWriter(mainFolder + "\\ShipBuildLog.csv", true, Encoding.UTF8))
						{
                            w.WriteLine("Secretary,SecretaryLv,CommandLv,Fuel,Ammo,Steel,Bauxite,# of Build Materials,Result,Date", args);
						}
					}

					using (StreamWriter w = new StreamWriter(mainFolder + "\\ShipBuildLog.csv", true, Encoding.UTF8))
					{
						w.WriteLine(format, args);
					}
					break;

				case LogType.ShipDrop:
					if (!File.Exists(mainFolder + "\\DropLog.csv"))
					{
						using (StreamWriter w = new StreamWriter(mainFolder + "\\DropLog.csv", true, Encoding.UTF8))
						{
							w.WriteLine("Operation,Enemy Fleet,Rank,Result,Date", args);
						}
					}
					using (StreamWriter w = new StreamWriter(mainFolder + "\\DropLog.csv", true, Encoding.UTF8))
					{
						w.WriteLine(format, args);
					}
					break;
			}

			sb.AppendLine(string.Format(format, args));

			this.RaisePropertyChanged("Log");
		}
	}
}
