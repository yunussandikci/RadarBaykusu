using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using SQLitePCL;
using Android.Gms.Ads;
using Android.Content.PM;
using RadarBaykusu.Core.Model;
using RadarBaykusu.Core;
using Android.Speech.Tts;

namespace RadarBaykusu.Droid
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class LocationActivity : Activity
    {
        #region Local Declerations

        private AlertDialog dialog;
        private MapFragment mapFragment;
        private GoogleMap googleMap;
        private LatLng currentLocation;
        private LinearLayout notificationLayout;
        private double avgApproximity = 0.05;
        private double avgExitApproximity = 0.1;
        private Boolean inTollRoad = false;
        private Boolean isExiting = false;
        private double carMaxSpeed;
        private DateTime tollAreaEnterTime;
        private Area tollAreaEnter;
        private List<ExitNotification> NotificationList = new List<ExitNotification>();
        private List<Area> tollAreaList;
        private List<AreaWithDistance> tollAreaExitList;
        private string timeDisplayFormatWithHour = "{0} Sa {1} Dk {2} Sn";
        private string timeDisplayFormat = "{0} Dk {1} Sn";
        private InterstitialAd mInterstitialAd;
        private string dbPath = "";
        private List<Configuration> configurationList = new List<Configuration>();
        private DBOperations dbOperations;
        private TextToSpeech textToSpeech;
        private Java.Util.Locale lang;

        #endregion Local Declerations

        #region Activity Methods

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Location);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            notificationLayout = FindViewById<LinearLayout>(Resource.Id.notificationLayout);

            dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            //connection objesini olustur
            dbOperations = new DBOperations(dbPath);

            tollAreaList = dbOperations.GetAreaList().ReturnObject;
            configurationList = dbOperations.GetConfigurationList().ReturnObject;

            //secilen arac tipine gore hiz limitini getir
            carMaxSpeed = Convert.ToInt32(Intent.Extras.GetString("maxCarSpeed"));

            CreateMapFragment();
            UpdateMapView();

            //şimdi reklamlar
            SetInterstitialAd();
            ShowAd();

            TollWaitingPopUpShow();

            // textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
            // textToSpeech.SetLanguage(Java.Util.Locale.),
        }

        private void TollWaitingPopUpShow()
        {
            Android.App.AlertDialog.Builder firstPopUp = new Android.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_DayNight_Dialog_Alert);

            var bottomContainerLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            bottomContainerLayout.Weight = 5;

            var lastContainerLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            lastContainerLayout.Gravity = GravityFlags.CenterHorizontal;

            var imageLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            imageLayout.Weight = 2;
            imageLayout.Height = 300;
            imageLayout.Width = 300;

            LinearLayout lastContainer = new LinearLayout(this);
            lastContainer.LayoutParameters = lastContainerLayout;
            lastContainer.Orientation = Orientation.Vertical;
            lastContainer.SetPadding(30, 10, 10, 10);
            lastContainer.SetGravity(GravityFlags.Center);

            ImageView image = new ImageView(this);
            image.LayoutParameters = imageLayout;

            TextView Title = new TextView(this);
            Title.LayoutParameters = lastContainerLayout;
            Title.SetTextSize(Android.Util.ComplexUnitType.Sp, 26);
            Title.SetTextColor(Color.Black);
            Title.Gravity = GravityFlags.CenterHorizontal;

            lastContainer.AddView(image);
            lastContainer.AddView(Title);

            image.SetImageResource(Resource.Drawable.logo);
            Title.Text = "Baykuş Otoyola Giriş Yapmanızı Bekliyor.";

            firstPopUp.SetView(lastContainer);
            dialog = firstPopUp.Show();
            dialog.CancelEvent += Dialog_CancelEvent;
            dialog.SetCanceledOnTouchOutside(false);
        }

        private void Dialog_CancelEvent(object sender, EventArgs e)
        {
            BackMainActivity();
        }

        #endregion Activity Methods

        #region AdMob - Reklam

        protected void RequestNewInterstitial()
        {
            var adRequest = new AdRequest.Builder().Build();
            mInterstitialAd.LoadAd(adRequest);
        }

        private void SetInterstitialAd()
        {
            mInterstitialAd = new InterstitialAd(this);
            mInterstitialAd.AdUnitId = GetString(Resource.String.interstitial_ad_unit_id);
            mInterstitialAd.AdListener = new AdListener(this);
        }

        private void ShowInterstitialAd()
        {
            if (mInterstitialAd.IsLoaded)
                mInterstitialAd.Show();
            else
                RequestNewInterstitial();
        }

        private class AdListener : Android.Gms.Ads.AdListener
        {
            private LocationActivity that;

            public AdListener(LocationActivity t)
            {
                that = t;
            }

            public override void OnAdClosed()
            {
                that.RequestNewInterstitial();
            }
        }

        private void ShowAd()
        {
            var id = "ca-app-pub-9037073034438816~5431142084";
            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, id);
            var adView = FindViewById<AdView>(Resource.Id.adView);

            var adRequest = new AdRequest.Builder().Build();
            adView.LoadAd(adRequest);
        }

        #endregion AdMob - Reklam

        #region Movement Control Methods

        private Boolean isFirst = true;

        private void GoogleMap_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            GoogleMap googleMap = sender as GoogleMap;
            currentLocation = new LatLng(e.Location.Latitude, e.Location.Longitude);
            if (!isFirst)
            {
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(currentLocation, googleMap.CameraPosition.Zoom));
            }
            else
            {
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(currentLocation, 16.00f));
                isFirst = false;
            }

            CheckTollRoad();
        }

        private void CheckTollRoad()
        {
            if (!inTollRoad && !isExiting) //Toll Yolda Değilken Ve Çıkmıyor İken
            {                              //Toll Yola Girip Girmediğimizi Burdan Anlıyoruz
                foreach (Area tollAreaItem in tollAreaList.Where(x => x.isBooth))
                {
                    double dist = CommonMethods.GetDistance(tollAreaItem.areaLatitude, tollAreaItem.areaLongitude, currentLocation.Latitude, currentLocation.Longitude);
                    if (dist < avgApproximity && tollAreaItem.isBooth)
                    {
                        tollAreaExitList = dbOperations.GetAreaPassListWithDisctance(tollAreaItem.areaID).ReturnObject.OrderBy(x => x.distance).ToList();

                        if (tollAreaExitList.Count != 0)
                        {
                            NotificationList = new List<ExitNotification>();
                            notificationLayout = FindViewById<LinearLayout>(Resource.Id.notificationLayout);
                            notificationLayout.RemoveAllViews();
                            dialog.Dismiss();
                            foreach (var exitItem in tollAreaExitList)
                            {
                                //bu liste giris sonrasi cikan kutulari yaratiyor
                                if (exitItem.isBooth)
                                {
                                    //eger exitItem gise ise listeye ekle
                                    notificationLayout.AddView(RoadNotification(exitItem.areaID));
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
                    //her exit icin
                    foreach (var tollAreaItem in tollAreaExitList.ToList())
                    {
                        double dist = CommonMethods.GetDistance(tollAreaItem.areaLatitude, tollAreaItem.areaLongitude, currentLocation.Latitude, currentLocation.Longitude);
                        ExitNotification currentNotify = NotificationList.Where(x => x.areaID == tollAreaItem.areaID).FirstOrDefault();
                        notificationLayout = FindViewById<LinearLayout>(Resource.Id.notificationLayout);
                        if (tollAreaItem.isBooth)
                        {
                            LinearLayout Container = (LinearLayout)FindViewById(currentNotify.ContainerID);
                            if (Container != null)
                            {
                                TextView LeftText = (TextView)FindViewById(currentNotify.LeftID);
                                TextView CenterText = (TextView)FindViewById(currentNotify.CenterID);
                                TextView RightText = (TextView)FindViewById(currentNotify.RightID);
                                TextView UpperRight = (TextView)FindViewById(currentNotify.RightUpperID);
                                TimeSpan TimeLeft = CommonMethods.GetMinutesLeft(tollAreaItem.distance, carMaxSpeed, tollAreaEnterTime);
                                Container.Visibility = ViewStates.Visible;

                                if (TimeLeft == TimeSpan.Zero)
                                {
                                    Container.SetBackgroundColor(Color.Argb(200, 39, 174, 96));
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
                                    foreach (ExitNotification item2 in NotificationList)
                                    {
                                        if (item2.areaID != tollAreaItem.areaID)
                                        {
                                            LinearLayout delContainer = (LinearLayout)FindViewById(item2.ContainerID);
                                            notificationLayout.RemoveView(delContainer);
                                        }
                                    }

                                    //cikiliyor olarak kayit yap
                                    isExiting = true;
                                    inTollRoad = false;

                                    //cikis popup mesaji olusturm

                                    #region Popup Mesaji

                                    PopupMessage(this, TimeLeft);
                                    break;

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
                                                ExitNotification delnotify = NotificationList.Where(x => x.areaID == item2.areaID).FirstOrDefault();
                                                LinearLayout delContainer = (LinearLayout)FindViewById(delnotify.ContainerID);
                                                notificationLayout.RemoveView(delContainer);

                                                //AdMob çıkıyor
                                                ShowInterstitialAd();
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
                                                ExitNotification delnotify = NotificationList.Where(x => x.areaID == item2.areaID).FirstOrDefault();
                                                LinearLayout delContainer = (LinearLayout)FindViewById(delnotify.ContainerID);
                                                notificationLayout.RemoveView(delContainer);

                                                //AdMob çıkıyor
                                                ShowInterstitialAd();
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
                        double dist = CommonMethods.GetDistance(item.areaLatitude, item.areaLongitude, currentLocation.Latitude, currentLocation.Longitude);
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
                        TollWaitingPopUpShow();
                    }
                }
            }
        }

        private void PopupMessage(Context context, TimeSpan timeLeft)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context, Resource.Style.Theme_AppCompat_DayNight_Dialog_Alert);
            builder.SetPositiveButton("Tamam", (senderAlert, args) => { TollWaitingPopUpShow(); });

            var bottomContainerLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            bottomContainerLayout.Weight = 5;

            var lastContainerLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            lastContainerLayout.Gravity = GravityFlags.CenterHorizontal;

            var imageLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            imageLayout.Weight = 3;
            imageLayout.Height = 400;
            imageLayout.Width = 400;

            LinearLayout lastContainer = new LinearLayout(this);
            lastContainer.LayoutParameters = lastContainerLayout;
            lastContainer.Orientation = Orientation.Vertical;
            lastContainer.SetPadding(30, 10, 10, 10);
            lastContainer.SetGravity(GravityFlags.Center);

            ImageView image = new ImageView(this);
            image.LayoutParameters = imageLayout;

            TextView Title = new TextView(this);
            Title.LayoutParameters = lastContainerLayout;
            Title.SetTextSize(Android.Util.ComplexUnitType.Sp, 32);
            Title.SetTextColor(Color.Black);
            Title.Gravity = GravityFlags.CenterHorizontal;

            TextView Information = new TextView(this);
            Title.LayoutParameters = lastContainerLayout;
            Title.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
            Title.SetTextColor(Color.Black);

            //Information.Text = "Ortalama Hızınız : " + (item.distance / (tollAreaEnterTime - DateTime.Now).TotalHours).ToString() + " Km/h";

            lastContainer.AddView(image);
            lastContainer.AddView(Title);

            //lastContainer.AddView(Information);
            if (timeLeft == TimeSpan.Zero)
            {
                image.SetImageResource(Resource.Drawable.RadarBaykusYes);
                Title.Text = "Güvenli Bir Şekilde Çıkış Yaptınız.";
            }
            else
            {
                image.SetImageResource(Resource.Drawable.RadarBaykusNo);
                Title.Text = "Otoyol'dan erken çıktınız.\n Ceza almış olabilirsiniz!";
            }

            builder.SetView(lastContainer);
            builder.Show();
        }

        private LinearLayout RoadNotification(double tollExitID)
        {
            var LeftLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            LeftLayout.Weight = 14;

            var CenterLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            CenterLayout.Weight = 13;

            var RightLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            RightLayout.Weight = 14;

            var TextLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            var BracketLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            BracketLayout.Width = 3;

            var ContainerLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ContainerLayout.Height = 150;
            ContainerLayout.SetMargins(20, 20, 20, 0);

            int fontSize = 12;

            LinearLayout Container = new LinearLayout(this);
            Container.LayoutParameters = ContainerLayout;
            Container.Orientation = Orientation.Horizontal;
            Container.Id = GenerateUniqueViewID();
            Container.SetBackgroundColor(Color.Argb(200, 192, 57, 43));
            Container.Visibility = ViewStates.Invisible;

            LinearLayout Left = new LinearLayout(this);
            Left.LayoutParameters = LeftLayout;
            Left.Orientation = Orientation.Vertical;
            Left.SetPadding(30, 10, 10, 10);
            Left.SetGravity(GravityFlags.Center);

            LinearLayout Center = new LinearLayout(this);
            Center.LayoutParameters = CenterLayout;
            Center.Orientation = Orientation.Vertical;
            Center.SetPadding(30, 10, 10, 10);
            Center.SetGravity(GravityFlags.Center);

            LinearLayout Right = new LinearLayout(this);
            Right.LayoutParameters = RightLayout;
            Right.Orientation = Orientation.Vertical;
            Right.SetPadding(30, 10, 10, 10);
            Right.SetGravity(GravityFlags.Center);

            LinearLayout LeftBracket = new LinearLayout(this);
            LeftBracket.Orientation = Orientation.Vertical;
            LeftBracket.LayoutParameters = BracketLayout;
            LeftBracket.SetBackgroundColor(Color.Black);

            LinearLayout RightBracket = new LinearLayout(this);
            RightBracket.Orientation = Orientation.Vertical;
            RightBracket.LayoutParameters = BracketLayout;
            RightBracket.SetBackgroundColor(Color.Black);

            TextView UpperLeft = new TextView(this);
            UpperLeft.Text = "Çıkış Adı";
            UpperLeft.LayoutParameters = TextLayout;
            UpperLeft.SetTextColor(Color.White);
            UpperLeft.SetTextSize(Android.Util.ComplexUnitType.Dip, fontSize);

            TextView BottomLeft = new TextView(this);
            BottomLeft.Text = "";
            BottomLeft.Id = GenerateUniqueViewID();
            BottomLeft.LayoutParameters = TextLayout;
            BottomLeft.SetTextColor(Color.White);
            BottomLeft.SetTextSize(Android.Util.ComplexUnitType.Dip, fontSize);

            TextView UpperCenter = new TextView(this);
            UpperCenter.Text = "Güvenli Çıkış Saati";
            UpperCenter.LayoutParameters = TextLayout;
            UpperCenter.SetTextColor(Color.White);
            UpperCenter.SetTextSize(Android.Util.ComplexUnitType.Dip, fontSize);

            TextView BottomCenter = new TextView(this);
            BottomCenter.Text = "";
            BottomCenter.Id = GenerateUniqueViewID();
            BottomCenter.LayoutParameters = TextLayout;
            BottomCenter.SetTextColor(Color.White);
            BottomCenter.SetTextSize(Android.Util.ComplexUnitType.Dip, fontSize);

            TextView UpperRight = new TextView(this);
            UpperRight.Text = "Kalan Süre";
            UpperRight.LayoutParameters = TextLayout;
            UpperRight.Id = GenerateUniqueViewID();
            UpperRight.SetTextColor(Color.White);
            UpperRight.SetTextSize(Android.Util.ComplexUnitType.Dip, fontSize);

            TextView BottomRight = new TextView(this);
            BottomRight.Text = "";
            BottomRight.Id = View.GenerateViewId() * 100;
            BottomRight.LayoutParameters = TextLayout;
            BottomRight.SetTextColor(Color.White);
            BottomRight.SetTextSize(Android.Util.ComplexUnitType.Dip, fontSize);

            //exit gostergesinin sol kismi
            Left.AddView(UpperLeft);
            Left.AddView(BottomLeft);

            //exit gostergesinin orta kismi
            Center.AddView(UpperCenter);
            Center.AddView(BottomCenter);

            //exit gostergesinin sag kismi
            Right.AddView(UpperRight);
            Right.AddView(BottomRight);

            Container.AddView(Left);

            //birinci cizgi
            Container.AddView(LeftBracket);

            Container.AddView(Center);

            //ikinci cizgi
            Container.AddView(RightBracket);

            Container.AddView(Right);

            NotificationList.Add(new ExitNotification { areaID = tollExitID, RightID = BottomRight.Id, CenterID = BottomCenter.Id, LeftID = BottomLeft.Id, ContainerID = Container.Id, RightUpperID = UpperRight.Id });
            return Container;
        }

        private int GenerateUniqueViewID()
        {
            while (true)
            {
                var tempViewID = View.GenerateViewId();
                var tempView = FindViewById(tempViewID);
                if (tempView == null)
                {
                    return tempViewID;
                }
            }
        }

        #endregion Movement Control Methods

        #region Map Methods

        private void CreateMapFragment()
        {
            //mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;

            mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapFrameLayout);

            if (mapFragment == null)
            {
                var googleMapOptions = new GoogleMapOptions().InvokeMapType(GoogleMap.MapTypeNormal).InvokeZoomControlsEnabled(true).InvokeCompassEnabled(true);
                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                mapFragment = MapFragment.NewInstance(googleMapOptions);
                fragmentTransaction.Add(Resource.Id.mapFrameLayout, mapFragment, "map");
                fragmentTransaction.Commit();
            }
        }

        private void UpdateMapView()
        {
            var mapReadyCallback = new LocalMapReady();
            mapReadyCallback.MapReady += (sender, args) =>
            {
                googleMap = (sender as LocalMapReady).Map;
                googleMap.MyLocationEnabled = true;
                googleMap.MyLocationChange += GoogleMap_MyLocationChange;
            };
            mapFragment.GetMapAsync(mapReadyCallback);
        }

        private class LocalMapReady : Java.Lang.Object, IOnMapReadyCallback
        {
            public GoogleMap Map { get; set; }

            public event EventHandler MapReady;

            public void OnMapReady(GoogleMap googleMAP)
            {
                Map = googleMAP;
                MapReady?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion Map Methods

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            BackMainActivity();
        }

        public void BackMainActivity()
        {
            var intent = new Intent();
            intent.SetClass(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}