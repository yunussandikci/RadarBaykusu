using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using RadarBaykusu.Core;
using RadarBaykusu.Core.Model;

namespace RadarBaykusu.iOS
{

    public class AgreementController : UIViewController
    {
        public UIWindow Window { get; set; }
        public AgreementController(UIWindow window)
        {
            Window = window;
        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            string dbLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library"); // Documents folder
            string dbPath = System.IO.Path.Combine(dbLocation, "radarbaykusu.db");
            DBOperations dbOperations = new DBOperations(dbLocation);
            nfloat WINDOW_HEIGHT = View.Bounds.Height;
            nfloat WINDOW_WIDTH = View.Bounds.Width;
            View.BackgroundColor = UIColor.FromRGBA(52,73,94,255);

            var agreementText = new UILabel();
            agreementText.Text = "Radar Baykusu'nun verdigi bilgiler GPS, Hava Durumu, Kullanılan Cihaz Özellikleri, vb... gibi birçok nedenle farklılıklar gösterebilir. Radar Baykusu arayüzünde gösterilen bilgiler fikir vermek amacı taşır ve kesin bilgi olarak kabul edilmemelidir. Yazılımın kullanımı sonucunda oluşabilecek trafik cezaları için Pergamon Solutions hiç bir sorumluluk kabul etmez.";
            agreementText.Frame = new CGRect(10, WINDOW_HEIGHT / 4, WINDOW_WIDTH - 20, WINDOW_HEIGHT / 2);
            agreementText.TextColor = UIColor.Black;
            agreementText.Lines = 10;
            agreementText.TextAlignment = UITextAlignment.Center;
            agreementText.AdjustsFontSizeToFitWidth = true;
            agreementText.TextColor = UIColor.White;

            var agreeButton = new UIButton();
            agreeButton.Frame = new CGRect(10, WINDOW_HEIGHT - 200, WINDOW_WIDTH - 20, 45);
            agreeButton.SetTitle("Kabul Ediyorum", UIControlState.Normal);
            agreeButton.BackgroundColor = UIColor.White;
            agreeButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            agreeButton.Layer.CornerRadius = 5f;
            agreeButton.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            //agreeButton.Enabled = false;

            agreeButton.TouchUpInside += delegate
            {
                var agreement = dbOperations.GetConfiguration("isTermsAccepted").ReturnObject;
                agreement.ParamValue = "true";
                var updateResult = dbOperations.Update(agreement, typeof(Configuration));
                Window.RootViewController = new UINavigationController(new MasterController());
            };
            View.AddSubviews(new UIView[] { agreementText, agreeButton });
        }
    }
}