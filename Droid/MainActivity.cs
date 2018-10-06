using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Compass.Model;

namespace Compass.Droid
{
    [Activity(Label = "Compass", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        CompassModel compass;
        DateTime morning = new DateTime(2018, 1, 1, 6, 0, 0);
        DateTime afternoon = new DateTime(2018, 1, 1, 12, 0, 0);
        DateTime evening = new DateTime(2018, 1, 1, 18, 0, 0);
        TextView timeLabel;
        TextView degreesLabel;
        TextView directionLabel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            compass = new CompassModel();
            compass.DegreesChanged += Compass_DegreesChanged;
            compass.TimeChanged += Compass_TimeChanged;
            SetContentView(Resource.Layout.Main);
            timeLabel = FindViewById<TextView>(Resource.Id.timeLabel);
            degreesLabel = FindViewById<TextView>(Resource.Id.degreesLabel);
            directionLabel = FindViewById<TextView>(Resource.Id.directionLabel);
            timeLabel.Text = DateTime.Now.ToString("hh:mm tt");
            UpdateBackground();

            // Get our button from the layout resource,
            // and attach an event to it
            //Button button = FindViewById<Button>(Resource.Id.myButton);

            //button.Click += delegate { button.Text = $"{count++} clicks!"; };
        }

        void Compass_TimeChanged(object sender, EventArgs e)
        {
            timeLabel.Text = compass.Time;
            UpdateBackground();
        }

        void Compass_DegreesChanged(object sender, EventArgs e)
        {
            //UIView.Animate(0.5, animation: () => magneticNeedle.Transform = CGAffineTransform.MakeRotation((float)(compass.Degrees * Math.PI / 180)));
            degreesLabel.Text = compass.Degrees.ToString() + "°";
            directionLabel.Text = compass.Direction;
        }

        void UpdateBackground()
        {
            var currentTime = DateTime.Now;
            LinearLayout mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            TextView greetings = FindViewById<TextView>(Resource.Id.greetings);

            if (currentTime.Hour >= morning.Hour && currentTime.Hour < afternoon.Hour)
            {

                mainLayout.SetBackgroundResource(Resource.Drawable.morning);
                greetings.Text = "Good morning!";
            }
            else if (currentTime.Hour >= afternoon.Hour && currentTime.Hour < evening.Hour)
            {
                mainLayout.SetBackgroundResource(Resource.Drawable.evening);
                greetings.Text = "Good afternoon!";
            }
            else
            {
                mainLayout.SetBackgroundResource(Resource.Drawable.night);
                greetings.Text = "Good evening!";
            }
        }
    }
}

