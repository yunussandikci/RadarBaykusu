using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Net;
using System.Collections.Generic;
using SQLitePCL;
using System;
using Android;
using Android.Gms.Ads;
using Android.Views;
using Android.Content.PM;
using RadarBaykusu.Core.Model;
using RadarBaykusu.Core;

namespace RadarBaykusu.Droid
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class MainActivity : Activity
    {
        #region Local Declerations

        private Button TicariButton;
        private Button BinekButton;
        private string dbPath;
        private DBOperations dbOperations;

        #endregion

        #region Activity Methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            GetPermission();

            //db = new SQLiteConnection(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "radarbaykusu.db"));
            dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            //platform bilgisini al ve connection objesini olustur
            dbOperations = new DBOperations(dbPath);

            SetContentView(Resource.Layout.Main);
            FindViews();
            ClickHandler();
            ShowAd();
        }

        #endregion

        #region Other Methods

        private void FindViews()
        {
            BinekButton = FindViewById<Button>(Resource.Id.BinekButton);
            TicariButton = FindViewById<Button>(Resource.Id.TicariButton);
        }

        private void ClickHandler()
        {
            BinekButton.Click += ModeButton_Click;
            TicariButton.Click += ModeButton_Click;
        }

        //ticari - binek butonuna basma sonucu
        private void ModeButton_Click(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            var intent = new Intent();
            intent.SetClass(this, typeof(LocationActivity));
            Configuration config = new Configuration();
            switch ((VehicleType)Enum.Parse(typeof(VehicleType), btn.Tag.ToString()))
            {
                case VehicleType.Ticari:
                    config = dbOperations.GetConfiguration("hizLimiti_Ticari").ReturnObject;
                    intent.PutExtra("maxCarSpeed", config.ParamValue);
                    break;
                case VehicleType.Binek:
                    config = dbOperations.GetConfiguration("hizLimiti_Binek").ReturnObject;
                    intent.PutExtra("maxCarSpeed", config.ParamValue);
                    break;
                case VehicleType.Motorsiklet:
                    config = dbOperations.GetConfiguration("hizLimiti_Motorsiklet").ReturnObject;
                    intent.PutExtra("maxCarSpeed", config.ParamValue);
                    break;
                default:
                    config = dbOperations.GetConfiguration("hizLimiti_Binek").ReturnObject;
                    intent.PutExtra("maxCarSpeed", config.ParamValue);
                    break;
            }

            StartActivity(intent);
        }

        private void ShowAd()
        {
            var id = "ca-app-pub-9037073034438816~5431142084";
            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, id);
            var adView = FindViewById<AdView>(Resource.Id.adView);

            var adRequest = new AdRequest.Builder().Build();
            adView.LoadAd(adRequest);
        }

        #endregion

        #region izinler

        private void GetPermission()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                LocationPermission();
            }
        }

        private void LocationPermission()
        {
            string[] PermissionLocation =
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation,
            };

            int RequestLocationId = 0;
            string permission = Manifest.Permission.AccessFineLocation;

            if (CheckSelfPermission(permission) == (int)Permission.Granted)
                return;
            else
                RequestPermissions(PermissionLocation, RequestLocationId);

        }

        private void ExternalStoragePermission()
        {
            string[] PermissionExternal =
            {
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage
            };

            int RequestExternalId = 0;
            string permission = Manifest.Permission.WriteExternalStorage;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
                return;
            else
                RequestPermissions(PermissionExternal, RequestExternalId);

        }

        Boolean check = true; //TODO ???
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (grantResults != null && grantResults[0] == Permission.Denied)
            {
                this.FinishAffinity();
            }

            if (check)
            {
                ExternalStoragePermission();
                check = false;
            }
        }

        #endregion
    }
}

