using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grabacr07.KanColleViewer.Composition;
using Application = System.Windows.Application;
using System.Xml;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class Windows7Notifier : INotifier
	{
		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;
        private const string NOTIFIER_SETTING_NAME = "NotifierSettings.xml";

		public void Initialize()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
				return;

			var streamResourceInfo = Application.GetResourceStream(uri);
			if (streamResourceInfo == null)
				return;

			using (var stream = streamResourceInfo.Stream)
			{
				this.notifyIcon = new NotifyIcon
				{
					Text = App.ProductInfo.Title,
					Icon = new Icon(stream),
					Visible = true,
				};
			}
        }

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
            if (!PlaySound())
                ShowBalloon(type, header, body, activated, failed);
		}
        
		public void ShowBalloon(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
        {
            if (this.notifyIcon == null)
                return;

            if (activated != null)
            {
                this.notifyIcon.BalloonTipClicked -= this.activatedAction;

                this.activatedAction = (sender, args) => activated();
                this.notifyIcon.BalloonTipClicked += this.activatedAction;
            }
            
            notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
        }

        public bool PlaySound()
        {
            try
            {
                if (!System.IO.File.Exists(NOTIFIER_SETTING_NAME))
                    return false;

                string soundPath = string.Empty;
                XmlTextReader reader = new XmlTextReader(NOTIFIER_SETTING_NAME);
                while (reader.Read())
                {
                    if (reader.Name == "Sound")
                    {
                        soundPath = reader.ReadString().Trim();
                    }
                } 
                reader.Close();

                if (!string.IsNullOrEmpty(soundPath) && System.IO.Path.GetExtension(soundPath) == ".wav")
                {
                    if (System.IO.File.Exists(soundPath))
                    {
                        System.Media.SoundPlayer sp = new System.Media.SoundPlayer(soundPath);
//                         sp.LoadCompleted += sp_LoadCompleted;
//                         sp.LoadAsync();
                        sp.Load();
                        sp.Play();
                    }

                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        void sp_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                    ((System.Media.SoundPlayer)sender).Play();
            }
            catch (Exception)
            {
            }
        }

		public object GetSettingsView()
		{
			return null;
		}

		public void Dispose()
		{
			if (this.notifyIcon != null)
			{
				this.notifyIcon.Dispose();
			}
		}
	}
}
