using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class LogTabViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return "日誌"; }
            protected set { throw new NotImplementedException(); }
        }

        #region Count 変更通知プロパティ

        private string _LogContent;

        public string LogContent
        {
            get { return this._LogContent; }
            set
            {
                if (this._LogContent != value)
                {
                    this._LogContent = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        public LogContentBattleResultViewModel[] _ContentBattleResult;
        public LogContentBattleResultViewModel[] ContentBattleResult
        {
            get { return _ContentBattleResult; }
            set {
                this._ContentBattleResult = value;
                this.RaisePropertyChanged();
            }
        }

        public string Title1 { get { return "title__"; } set { throw new InvalidOperationException(); } }
//         #region Count 変更通知プロパティ
// 
//         private List<LogContentBattleResult> _ContentBattleResult;
// 
//         public List<LogContentBattleResult> ContentBattleResult
//         {
//             get { return this._ContentBattleResult; }
//             set
//             {
//                 this._ContentBattleResult = value;
//                 this.RaisePropertyChanged();
//             }
//         }
// 
//         #endregion
        public LogTabViewModel()
        {
            this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Logger)
            {
                { "Log", (sender, args) => this.Update() }
            });
            this.Update();
        }

        private void Update()
        {
            LogContent = KanColleClient.Current.Homeport.Logger.Content;
            ContentBattleResult = KanColleClient.Current.Homeport.Logger.ContentBattleResult.Select(x => new LogContentBattleResultViewModel(x)).ToArray(); ;
        }
    }
}
