using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Dev
{
	public class DebugTabViewModel : TabItemViewModel
	{
		public override string Name
		{
			//get { return Properties.Resources.Debug; }
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

		public void Notify()
		{
			PluginHost.Instance.GetNotifier()
				.Show(NotifyType.Other, Properties.Resources.Debug_NotificationMessage_Title, Properties.Resources.Debug_NotificationMessage, () => App.ViewModelRoot.Activate());
            KanColleClient.Current.Homeport.Logger.Test();
        }

        public DebugTabViewModel()
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
		}
	}
}
