//
// CompassModel.cs
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
using System.Threading.Tasks;
using Xamarin.Essentials;
namespace Compass.Model
{
    public class CompassModel
    {
        public int Degrees { get; private set; }
        public string Direction { get; private set; }
        public string Time { get; private set; }

        readonly string[] CompassDirections = {"North", "N-NE", "NE", "E-NE",
                                               "East", "E-SE", "SE", "S-SE",
                                               "South", "S-SW", "SW", "W-SW",
                                               "West", "W-NW", "NW", "N-NW"};
        public event EventHandler DegreesChanged;
        public event EventHandler TimeChanged;
        SensorSpeed speed = SensorSpeed.Game;

        public CompassModel()
        {
            Xamarin.Essentials.Compass.ReadingChanged += Compass_ReadingChanged;
            if (Xamarin.Essentials.Compass.IsMonitoring)
                return;
            else
                Xamarin.Essentials.Compass.Start(speed);
            UpdateTime();
        }

        async void UpdateTime()
        {
            var delayToNextMinute = 60 - DateTime.Now.Second;
            await Task.Delay(delayToNextMinute * 1000);
            while (true)
            {
                Time = DateTime.Now.ToString("hh:mm tt");
                TimeChanged?.Invoke(this, new EventArgs());
                await Task.Delay(60000);
            }
        }

        void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;
            Degrees = (int)data.HeadingMagneticNorth;
            UpdateDirection();
            DegreesChanged?.Invoke(this, e);
        }

        void UpdateDirection()
        {
            int index = (int)(Degrees / 11.25);
            index = (index % 2 != 0 ? (index + 1) / 2 : index / 2) % 16;
            Direction = CompassDirections[index];
        }
    }
}
