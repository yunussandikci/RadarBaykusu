using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Google.Maps;
using RadarBaykusu.Core;
using CoreLocation;
using RadarBaykusu.Core.Model;
using CoreGraphics;

namespace RadarBaykusu.iOS
{
    internal class MapController : UIViewController
    {
        #region Local Declerations

        //Drive Type FirstController tarafından bu controller view'i açılmadan değiştirilir araba tipini belirtir.
        private MapView mapView;

        private UIAlertView waitingAlert;
        private UIAlertView EndOfTheRoadAlert;

        public int driveType { get; set; }

        private double SCREEN_WIDTH;
        private double SCREEN_HEIGHT;
        private double carMaxSpeed;
        private DBOperations dbOperations;
        private CLLocation currentLocation;
        private double avgApproximity = 0.05;
        private double avgExitApproximity = 0.1;
        private Boolean inTollRoad = false;
        private Boolean isExiting = false;
        private DateTime tollAreaEnterTime;
        private Area tollAreaEnter;
        private List<Area> tollAreaList;
        private string timeDisplayFormatWithHour = "{0} Sa {1} Dk {2} Sn";
        private string timeDisplayFormat = "{0} Dk {1} Sn";
        private List<AreaWithDistance> tollAreaExitList;
        private List<Configuration> configurationList = new List<Configuration>();
        private List<ExitNotificationIOS> NotificationList = new List<ExitNotificationIOS>();

        #endregion Local Declerations

        public override void LoadView()
        {
            base.LoadView();
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            NavigationController.NavigationBarHidden = false;
            string dbLocation = System.IO.Path.Combine((Environment.GetFolderPath(Environment.SpecialFolder.Personal)), "Library"); // Documents folder
            dbOperations = new DBOperations(dbLocation);
            tollAreaList = dbOperations.GetAreaList().ReturnObject;
            configurationList = dbOperations.GetConfigurationList().ReturnObject;
            if (driveType == 1)
            {
                carMaxSpeed = Convert.ToDouble(configurationList.Where(x => x.ParamName == "hizLimiti_Binek").FirstOrDefault().ParamValue);
            }
            else if (driveType == 2)
            {
                carMaxSpeed = Convert.ToDouble(configurationList.Where(x => x.ParamName == "hizLimiti_Ticari").FirstOrDefault().ParamValue);
            }
            UIJobs();
            CreateWaitingAlert();
            initMap();
            CreateNotifyLayout();
        }

        public void CreateNotifyLayout()
        {
            UIStackView stackView = new UIStackView();
            stackView.UserInteractionEnabled = false;
            stackView.Axis = UILayoutConstraintAxis.Vertical;
            stackView.Distribution = UIStackViewDistribution.EqualSpacing;
            stackView.TranslatesAutoresizingMaskIntoConstraints = false;
            stackView.Alignment = UIStackViewAlignment.Top;
            stackView.Spacing = 12;
            stackView.Tag = 1;

            View.AddSubview(stackView);
        }

        public void AddNotify(int areaID)
        {
            UIStackView stackView = View.ViewWithTag(1) as UIStackView;

            UIStackView Container = new UIStackView();
            Container.Axis = UILayoutConstraintAxis.Horizontal;
            Container.BackgroundColor = UIColor.Red;
            Container.Distribution = UIStackViewDistribution.FillEqually;
            Container.HeightAnchor.ConstraintEqualTo((float)(SCREEN_HEIGHT / 9)).Active = true;
            Container.WidthAnchor.ConstraintEqualTo((float)(SCREEN_WIDTH - 20)).Active = true;
            Container.Tag = (areaID * 10) + 0;
            Container.Hidden = true;

            UIView BG = new UIView();
            BG.BackgroundColor = UIColor.FromRGBA(255, 0, 0, 150);
            BG.Frame = Container.Bounds;
            BG.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            BG.Tag = (areaID * 10) + 5;
            UIStackView Left = new UIStackView();
            Left.Axis = UILayoutConstraintAxis.Vertical;
            Left.Distribution = UIStackViewDistribution.FillEqually;

            UIStackView Center = new UIStackView();
            Center.Axis = UILayoutConstraintAxis.Vertical;
            Center.Distribution = UIStackViewDistribution.FillEqually;

            UIStackView Right = new UIStackView();
            Right.Axis = UILayoutConstraintAxis.Vertical;
            Right.Distribution = UIStackViewDistribution.FillEqually;

            UILabel TopLeft = new UILabel();
            TopLeft.Text = "Çıkış Adı";
            TopLeft.TextAlignment = UITextAlignment.Center;
            TopLeft.TextColor = UIColor.White;

            UILabel BottomLeft = new UILabel();
            BottomLeft.Text = "BottomLeft";
            BottomLeft.TextAlignment = UITextAlignment.Center;
            BottomLeft.TextColor = UIColor.White;
            BottomLeft.Tag = (areaID * 10) + 1;

            UILabel TopCenter = new UILabel();
            TopCenter.Text = "Güvenli Çıkış";
            TopCenter.TextAlignment = UITextAlignment.Center;
            TopCenter.TextColor = UIColor.White;

            UIWebView Page = new UIWebView();

            UILabel BottomCenter = new UILabel();
            BottomCenter.Text = "TopRight";
            BottomCenter.TextAlignment = UITextAlignment.Center;
            BottomCenter.TextColor = UIColor.White;
            BottomCenter.Tag = (areaID * 10) + 2;

            UILabel TopRight = new UILabel();
            TopRight.Text = "Kalan Süre";
            TopRight.TextAlignment = UITextAlignment.Center;
            TopRight.TextColor = UIColor.White;
            TopRight.Tag = (areaID * 10) + 4;

            UILabel BottomRight = new UILabel();
            BottomRight.Text = "BottomRight";
            BottomRight.TextAlignment = UITextAlignment.Center;
            BottomRight.TextColor = UIColor.White;
            BottomRight.Tag = (areaID * 10) + 3;

            Left.AddArrangedSubview(TopLeft);
            Left.AddArrangedSubview(BottomLeft);

            Center.AddArrangedSubview(TopCenter);
            Center.AddArrangedSubview(BottomCenter);

            Right.AddArrangedSubview(TopRight);
            Right.AddArrangedSubview(BottomRight);

            Container.AddSubview(BG);
            Container.AddArrangedSubview(Left);
            Container.AddArrangedSubview(Center);
            Container.AddArrangedSubview(Right);

            stackView.AddArrangedSubview(Container);

            stackView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            stackView.TopAnchor.ConstraintEqualTo(View.TopAnchor, 20).Active = true;

            NotificationList.Add(new ExitNotificationIOS { areaID = areaID, RightID = Convert.ToInt32(BottomRight.Tag), CenterID = Convert.ToInt32(BottomCenter.Tag), LeftID = Convert.ToInt32(BottomLeft.Tag), ContainerID = Convert.ToInt32(Container.Tag), RightUpperID = Convert.ToInt32(TopRight.Tag), BGID = Convert.ToInt32(BG.Tag) });
        }

        public void UIJobs()
        {
            //Ekran boyutlarını alır
            SCREEN_WIDTH = UIScreen.MainScreen.Bounds.Width;
            SCREEN_HEIGHT = UIScreen.MainScreen.Bounds.Height;

            //soldan sağa kaydırmayla geçişi engeller
            this.NavigationController.InteractivePopGestureRecognizer.Enabled = false;

            //View'ın navigation bar'ın altında kalmasını engeller
            this.EdgesForExtendedLayout = UIRectEdge.None;
        }

        public void initMap()
        {
            mapView = MapView.FromCamera(new CGRect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), CameraPosition.FromCamera(latitude: 37.79, longitude: -122.40, zoom: 6));
            InvokeOnMainThread(() =>
            {
                mapView.MyLocationEnabled = true;
                mapView.AddObserver(this, new NSString("myLocation"), NSKeyValueObservingOptions.New, IntPtr.Zero);
            });
            View.AddSubview(mapView);
        }

        public void CreateWaitingAlert()
        {
            waitingAlert = new UIAlertView()
            {
                Title = "Radar Baykuşu",
                Message = "Radar  Baykuşu Otoyola Giriş Yapmanızı Bekliyor",
            };
            waitingAlert.AddButton("Ana Ekrana Dön");
            waitingAlert.Clicked += (sender, buttonArgs) =>
            {
                if (buttonArgs.ButtonIndex == 0)
                {
                    NavigationController.PopToRootViewController(true);
                }
            };
            waitingAlert.Show();
        }

        public void CreateEndOfTheRoadAlert(TimeSpan timeLeft)
        {
            EndOfTheRoadAlert = new UIAlertView()
            {
                Title = "Radar Baykuşu",
            };
            if (timeLeft == TimeSpan.Zero)
            {
                EndOfTheRoadAlert.Message = "Otoyoldan Güvenli Bir Şekilde Çıkış Yaptınız.";
            }
            else
            {
                EndOfTheRoadAlert.Message = "Otoyoldan Erken Çıkış Yaptınız. Ceza Yemiş Olabilirsiniz.";
            }
            EndOfTheRoadAlert.AddButton("Tamam");
            EndOfTheRoadAlert.Dismissed += (s, e) => { CreateWaitingAlert(); };
            EndOfTheRoadAlert.Show();
        }

        public Boolean isFirst = true;

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            try
            {
                currentLocation = change.ObjectForKey(NSValue.ChangeNewKey) as CLLocation;
                if (!isFirst)
                {
                    mapView.Animate(new CameraPosition(new CLLocationCoordinate2D(currentLocation.Coordinate.Latitude, currentLocation.Coordinate.Longitude), mapView.Camera.Zoom, mapView.Camera.Bearing, mapView.Camera.ViewingAngle));
                }
                else
                {
                    mapView.Camera = CameraPosition.FromCamera(currentLocation.Coordinate, 16.00f);
                    isFirst = false;
                }

                CheckTollRoad();
            }
            catch (Exception)
            {
                //TODO: Burası Sisteme Loglanıcak.
            }
        }

        private void CheckTollRoad()
        {
            if (!inTollRoad && !isExiting) //Toll Yolda Değilken Ve Çıkmıyor İken
            {                             //Toll Yola Girip Girmediğimizi Burdan Anlıyoruz
                foreach (Area tollAreaItem in tollAreaList.Where(x => x.isBooth))
                {
                    double dist = CommonMethods.GetDistance(tollAreaItem.areaLatitude, tollAreaItem.areaLongitude, currentLocation.Coordinate.Latitude, currentLocation.Coordinate.Longitude);
                    if (dist < avgApproximity && tollAreaItem.isBooth)
                    {
                        tollAreaExitList = dbOperations.GetAreaPassListWithDisctance(tollAreaItem.areaID).ReturnObject.OrderBy(x => x.distance).ToList();

                        if (tollAreaExitList.Count != 0)
                        {
                            foreach (ExitNotificationIOS item in NotificationList)
                            {
                                var delview = View.ViewWithTag(item.ContainerID);
                                delview.RemoveFromSuperview();
                            }

                            NotificationList = new List<ExitNotificationIOS>();
                            foreach (var exitItem in tollAreaExitList)
                            {
                                //bu liste giris sonrasi cikan kutulari yaratiyor
                                if (exitItem.isBooth)
                                {
                                    //eger exitItem gise ise listeye ekle
                                    AddNotify(Convert.ToInt32(exitItem.areaID));
                                }
                            }

                            //yola girildi olarak kayit atiliyor
                            inTollRoad = true;
                            tollAreaEnter = tollAreaItem;
                            tollAreaEnterTime = DateTime.Now;
                            break;
                        }
                    }
                }
            }
            else //Toll yola girdikten sonra
            {
                //Yolun İçindeyken ve cikis halinde degilken
                if (!isExiting)
                {
                    waitingAlert.DismissWithClickedButtonIndex(0, true);

                    //her exit icin
                    foreach (var tollAreaItem in tollAreaExitList.ToList())
                    {
                        double dist = CommonMethods.GetDistance(tollAreaItem.areaLatitude, tollAreaItem.areaLongitude, currentLocation.Coordinate.Latitude, currentLocation.Coordinate.Longitude);
                        ExitNotificationIOS currentNotify = NotificationList.Where(x => x.areaID == tollAreaItem.areaID).FirstOrDefault();
                        if (tollAreaItem.isBooth)
                        {
                            UIStackView Container = View.ViewWithTag(currentNotify.ContainerID) as UIStackView;
                            if (Container != null)
                            {
                                UILabel LeftText = View.ViewWithTag(currentNotify.LeftID) as UILabel;
                                UILabel CenterText = View.ViewWithTag(currentNotify.CenterID) as UILabel;
                                UILabel RightText = View.ViewWithTag(currentNotify.RightID) as UILabel;
                                UILabel UpperRight = View.ViewWithTag(currentNotify.RightUpperID) as UILabel;
                                TimeSpan TimeLeft = CommonMethods.GetMinutesLeft(tollAreaItem.distance, carMaxSpeed, tollAreaEnterTime);
                                UIView BG = View.ViewWithTag(currentNotify.BGID) as UIView;
                                Container.Hidden = false;

                                if (TimeLeft == TimeSpan.Zero)
                                {
                                    BG.BackgroundColor = UIColor.FromRGBA(46, 204, 113, 150);
                                }

                                //kutularin icindeki verileri guncelle
                                LeftText.Text = tollAreaItem.areaName;
                                CenterText.Text = CommonMethods.GetExitTime(tollAreaItem.distance, carMaxSpeed, tollAreaEnterTime).ToString("HH:mm");
                                RightText.Text = TimeLeft != null && TimeLeft.Hours > 0 ? string.Format(timeDisplayFormatWithHour, TimeLeft.Hours, TimeLeft.Minutes, TimeLeft.Seconds) :
                                    string.Format(timeDisplayFormat, TimeLeft.Minutes, TimeLeft.Seconds);

                                //eger cikisa gelinmis ise
                                if (dist < avgApproximity)
                                {
                                    //yaziyi degistir
                                    UpperRight.Text = "Çıkış Saati";

                                    //cikis saatini goster
                                    RightText.Text = DateTime.Now.ToString("HH:mm");

                                    //diger tum cikislari listeden cikar
                                    foreach (ExitNotificationIOS item2 in NotificationList)
                                    {
                                        if (item2.areaID != tollAreaItem.areaID)
                                        {
                                            UIStackView delContainer = View.ViewWithTag(item2.ContainerID) as UIStackView;
                                            if (delContainer != null)
                                            {
                                                delContainer.RemoveFromSuperview();
                                            }
                                        }
                                    }

                                    //cikiliyor olarak kayit yap
                                    isExiting = true;
                                    inTollRoad = false;

                                    //cikis popup mesaji olusturm

                                    #region Popup Mesaji

                                    CreateEndOfTheRoadAlert(TimeLeft);

                                    #endregion Popup Mesaji
                                }
                            }
                        }
                        else //Yon tayini ile ilgili bolumler
                        {
                            //gatelerden birine girmisse
                            if (dist < avgApproximity)
                            {
                                //giris noktasindan yolun bitis istikametine dogru gidiyor
                                if (tollAreaItem.areaID > tollAreaEnter.areaID)
                                {
                                    foreach (AreaWithDistance item2 in tollAreaExitList.ToList())
                                    {
                                        if (item2.areaID < tollAreaItem.areaID)
                                        {
                                            if (item2.isBooth)
                                            {
                                                ExitNotificationIOS delnotify = NotificationList.Where(x => x.areaID == item2.areaID).FirstOrDefault();
                                                UIStackView delContainer = View.ViewWithTag(delnotify.ContainerID) as UIStackView;
                                                delContainer.RemoveFromSuperview();
                                            }
                                            tollAreaExitList.Remove(item2);
                                        }
                                    }
                                }
                                else //giris noktasindan yolun baslangic istikametine dogru gidiyor
                                {
                                    foreach (AreaWithDistance item2 in tollAreaExitList.ToList())
                                    {
                                        if (tollAreaItem.areaID < item2.areaID)
                                        {
                                            if (item2.isBooth)
                                            {
                                                ExitNotificationIOS delnotify = NotificationList.Where(x => x.areaID == item2.areaID).FirstOrDefault();
                                                UIStackView delContainer = View.ViewWithTag(delnotify.ContainerID) as UIStackView;
                                                delContainer.RemoveFromSuperview();
                                            }
                                            tollAreaExitList.Remove(item2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else //Yoldan Çıkıyorken
                {
                    Boolean stillExiting = false;
                    foreach (AreaWithDistance item in tollAreaExitList)
                    {
                        double dist = CommonMethods.GetDistance(item.areaLatitude, item.areaLongitude, currentLocation.Coordinate.Latitude, currentLocation.Coordinate.Longitude);
                        if (dist < avgExitApproximity)
                            stillExiting = true;
                    }

                    if (!stillExiting)
                    {
                        isExiting = stillExiting;
                        inTollRoad = false;
                        tollAreaEnter = null;
                        tollAreaEnterTime = DateTime.MinValue;
                        tollAreaExitList = null;
                    }
                }
            }
        }
    }
}