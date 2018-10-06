//
// ViewController.cs
//
// Author:
//       Mihail <mihaillgolban@gmail.com>
//
// Copyright (c) 2018 (c) Golban Mihail
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

using UIKit;
using CoreLocation;
using CoreGraphics;
using Compass.Model;
using System.Runtime.CompilerServices;

namespace Compass.iOS
{
    public partial class ViewController : UIViewController
    {
        CompassModel compass;
        DateTime morning = new DateTime(2018, 1, 1, 6, 0, 0);
        DateTime afternoon = new DateTime(2018, 1, 1, 12, 0, 0);
        DateTime evening = new DateTime(2018, 1, 1, 18, 0, 0);
        UIView[] marks = new UIView[72];

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            // Perform any additional setup after loading the view, typically from a nib.
            compass = new CompassModel();
            compass.DegreesChanged += Compass_DegreesChanged;
            compass.TimeChanged += Compass_TimeChanged;
            timeLabel.Text = DateTime.Now.ToString("hh:mm tt");
            UpdateBackgroundColor();
            stackviewDegrees.Alignment = UIStackViewAlignment.Center;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            DrawCompass();
        }

        void Compass_TimeChanged(object sender, EventArgs e)
        {
            timeLabel.Text = compass.Time;
            UpdateBackgroundColor();
        }


        void Compass_DegreesChanged(object sender, EventArgs e)
        {
            UIView.Animate(0.5, animation: () => magneticNeedle.Transform = CGAffineTransform.MakeRotation((float)(compass.Degrees * Math.PI / 180)));
            degreesLabel.Text = compass.Degrees.ToString() +"°";
            directionLabel.Text = compass.Direction;
        }

        void UpdateBackgroundColor()
        {
            var currentTime = DateTime.Now;
            if (currentTime.Hour >= morning.Hour && currentTime.Hour < afternoon.Hour)
            {
                backgroundImage.Image = UIImage.FromBundle("morning-background");
                greetings.Text = "Good morning!";
            }
            else if (currentTime.Hour >= afternoon.Hour && currentTime.Hour < evening.Hour)
            {
                backgroundImage.Image = UIImage.FromBundle("afternoon-background");
                greetings.Text = "Good afternoon!";
            }
            else
            {
                backgroundImage.Image = UIImage.FromBundle("night-background");
                greetings.Text = "Good evening!";
            }
        }

        void DrawCompass()
        {
            var compassRadius = compassView.Bounds.GetMidX();
            var height = 4;
            for (int i = 0; i < 72; i++)
            {
                var width = 0.08 * compassRadius;
                var xPosition = compassRadius - width / 2 + compassRadius * Math.Cos((i * 5 - 90) * Math.PI / 180);
                var yPosition = compassRadius - height / 2 + compassRadius * Math.Sin((i * 5 - 90) * Math.PI / 180);
                marks[i] = new UIView(new CGRect(xPosition, yPosition, width, height));
                marks[i].BackgroundColor = UIColor.White;
                if (i % 9 != 0)
                    marks[i].Alpha = (nfloat)0.3;
                else if (i % 18 == 0)
                {
                    marks[i].Bounds = new CGRect(xPosition, yPosition, width * 2, height);
                    var DirectionTextLabel = "N";
                    var margin = 5;
                    switch (i)
                    {
                        case 0:
                            DirectionTextLabel = "E";
                            xPosition = 2 * compassRadius - width - margin - 20;
                            yPosition = compassRadius - 10;
                            break;
                        case 18:
                            DirectionTextLabel = "S";
                            xPosition = compassRadius - 10;
                            yPosition = 2 * compassRadius - width - margin - 20;
                            break;
                        case 36:
                            DirectionTextLabel = "W";
                            xPosition = width + margin;
                            yPosition = compassRadius - 10;
                            break;
                        case 54:
                            DirectionTextLabel = "N";
                            xPosition = compassRadius - 10;
                            yPosition = width + margin;
                            break;
                        default:
                            break;
                    }

                    var markLabel = new UILabel(new CGRect(xPosition, yPosition , 20, 20));
                    markLabel.Text = DirectionTextLabel;
                    markLabel.Font = UIFont.FromName("Courier", 20);
                    markLabel.TextColor = UIColor.White;
                    markLabel.TextAlignment = UITextAlignment.Center;
                    compassView.AddSubview(markLabel);
                }
                    
                else
                    marks[i].Bounds = new CGRect(xPosition, yPosition, width * 1.5, height);
                marks[i].Layer.CornerRadius = 2;
                marks[i].Transform = CGAffineTransform.MakeRotation((float)((i * 5 - 90) * Math.PI / 180));
                compassView.AddSubview(marks[i]);
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}