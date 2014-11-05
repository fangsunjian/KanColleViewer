using Grabacr07.KanColleWrapper;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class LogContentBattleResultViewModel : ViewModel
    {
        public LogContentBattleResult BattleResult { get; private set; }

        public LogContentBattleResultViewModel(LogContentBattleResult contentBattleResult)
        {
            this.BattleResult = contentBattleResult;
        }
    }
}
