using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CoreLocation;

namespace RadarBaykusu.iOS
{

    public class MasterController : UIViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBarHidden = true;
        }
        public void CheckLocationServicesEnabled()
        {
            var asd = CLLocationManager.Status;
            if (!CLLocationManager.LocationServicesEnabled || CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                
                UIAlertView LocationServicesAlert = new UIAlertView() { Title = "Radar Baykuşu", Message = "Lütfen Ayarlardan Konum Servislerini Aktif Hale Getirin Ve Radar Baykuşuna Konum'a Erişim İzni Verin." };
                LocationServicesAlert.AddButton("Tamam");
                LocationServicesAlert.Clicked += (sender, buttonArgs) =>
                {
                    if (buttonArgs.ButtonIndex == 0 && !CLLocationManager.LocationServicesEnabled)
                    {
                        //CheckLocationServicesEnabled();
                    }

                };
                LocationServicesAlert.Show();
            }
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CheckLocationServicesEnabled();

            nfloat WINDOW_HEIGHT = View.Bounds.Height;
            nfloat WINDOW_WIDTH = View.Bounds.Width;

            UIImageView Logo = new UIImageView();
            Logo.Frame = new CGRect(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT * 0.5);
            Logo.Image = UIImage.FromFile("Images/RadarBaykusuLogoMavi.png");
            Logo.ContentMode = UIViewContentMode.ScaleAspectFit;

            UIImageView Ninety = new UIImageView();
            Ninety.Frame = new CGRect(0, (WINDOW_HEIGHT * 0.65), WINDOW_WIDTH * 0.5, (WINDOW_HEIGHT * 0.2) - 10);
            Ninety.Image = UIImage.FromFile("Images/rsz_1rsz_speedlimit2.png");
            Ninety.ContentMode = UIViewContentMode.ScaleAspectFit;

            UIImageView Hundred = new UIImageView();
            Hundred.Frame = new CGRect(WINDOW_WIDTH * 0.5, (WINDOW_HEIGHT * 0.65), WINDOW_WIDTH * 0.5, (WINDOW_HEIGHT * 0.2) - 10);
            Hundred.Image = UIImage.FromFile("Images/rsz_2rsz_1speedlimit1.png");
            Hundred.ContentMode = UIViewContentMode.ScaleAspectFit;

            UIButton BinekButton = new UIButton();
            BinekButton.Frame = new CGRect(0, WINDOW_HEIGHT * 0.5, WINDOW_WIDTH * 0.5, WINDOW_HEIGHT * 0.4);
            BinekButton.BackgroundColor = UIColor.FromRGBA(22, 160, 133, 255); ;
            BinekButton.ContentEdgeInsets = new UIEdgeInsets(0,0,WINDOW_HEIGHT * 0.2f,0);
            BinekButton.SetTitle("Binek Araç", UIControlState.Normal);
            BinekButton.TouchDown += (s, e) =>
            {
                var MapController = new MapController();
                MapController.driveType = 1;
                this.NavigationController.PushViewController(MapController, true);
            };

            UIButton TicariButton = new UIButton();
            TicariButton.Frame = new CGRect(WINDOW_WIDTH * 0.5, WINDOW_HEIGHT * 0.5, WINDOW_WIDTH * 0.5, WINDOW_HEIGHT * 0.4);
            TicariButton.BackgroundColor = UIColor.FromRGBA(41, 128, 185, 255);
            TicariButton.ContentEdgeInsets = new UIEdgeInsets(0, 0, WINDOW_HEIGHT * 0.2f, 0);
            TicariButton.SetTitle("Ticari Araç", UIControlState.Normal);
            TicariButton.TouchDown += (s, e) =>
            {
                var MapController = new MapController();
                MapController.driveType = 2;
                this.NavigationController.PushViewController(MapController, true);
            };

            UILabel Information = new UILabel();
            Information.Frame = new CGRect(10, WINDOW_HEIGHT * 0.90, WINDOW_WIDTH - 20, WINDOW_HEIGHT * 0.10);
            Information.Text = "Radar Baykuşu © 2017 Pergamon Solutions";
            Information.TextAlignment = UITextAlignment.Center;
            Information.AdjustsFontSizeToFitWidth = true;
            Information.TextColor = UIColor.White;

            View.BackgroundColor = UIColor.FromRGBA(52, 73, 94, 255);
            View.AddSubviews(new UIView[] { Logo, BinekButton, Ninety, TicariButton, Hundred, Information }); ;
        }
    }
}