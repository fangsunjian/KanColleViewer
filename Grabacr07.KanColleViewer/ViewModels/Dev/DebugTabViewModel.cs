﻿using System;
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
            get { return Properties.Resources.Debug; }
            protected set { throw new NotImplementedException(); }
        }

        public void Notify()
        {
            PluginHost.Instance.GetNotifier()
                .Show(NotifyType.Other, Properties.Resources.Debug_NotificationMessage_Title, Properties.Resources.Debug_NotificationMessage, () => App.ViewModelRoot.Activate());
            KanColleClient.Current.Homeport.Logger.Test();
        }

        public DebugTabViewModel()
        {
        }
        
        private void Update()
        {
        }
    }
}
