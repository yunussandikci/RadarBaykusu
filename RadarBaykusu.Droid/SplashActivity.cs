using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLitePCL;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Android.Support.V7.App;
using Android;
using Android.Content.PM;
using RadarBaykusu.Core;
using RadarBaykusu.Core.Model;


namespace RadarBaykusu.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private string dbPath;
        private DBOperations dbOperations;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            SetContentView(Resource.Layout.splash_screen);
        }

        protected override void OnResume()
        {
            base.OnResume();
            //sqlite connection icin baglanti cumlersini olustur
            dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            //platform bilgisini al ve connection objesini olustur
            dbOperations = new DBOperations(dbPath);

            SimulateStartup();
        }

        // Simulates background work that happens behind the splash screen
        private void SimulateStartup()
        {
            var startTime = DateTime.Now;
            //veritabaninin olup olmadigini kontrol eder ve yoksa yenisini yaratir. 
            CommonMethods.DBCheck(dbPath);
            var endTimeResult = DateTime.Now;
            var timeResult = endTimeResult - startTime;

            //splash screen'in 5 saniye calismasi gerekiyor. 
            //O yuzden dbcheck icin harcanan sureyi 5 saniyeden cikartip aradaki farki splash'in goruntulenmesi icin sleep de kullaniliyor
            if (timeResult.Seconds < 5)
            {
                Thread.Sleep((5 - timeResult.Seconds) * 1000);
            }

            var result = dbOperations.GetConfiguration("isTermsAccepted");

            if (result != null && result.Result)
            {
                if (result.ReturnObject.ParamValue == "true")
                { //daha onceden agreement kabul edilmisse
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                }
                else //daha onceden agreement kabul edilmemisse
                {
                    StartActivity(new Intent(Application.Context, typeof(AgreementActivity)));
                }
            }
        }
    }
}