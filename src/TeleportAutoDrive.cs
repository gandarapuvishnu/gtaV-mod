using GTA;
using GTA.Math;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using GTA.Native;
using System.Text;
using System.IO;

namespace TeleportAutoDrive
{
    public class TeleportAutoDrive : Script
    {
        public Ped ped = Game.Player.Character;
        public Vehicle vehicle;
        public readonly string ModName = "~g~TeleportAutoDrive~g~";
        public readonly string Developer = "~b~GandarapuVishnu~b~";
        public readonly string Version = " 1.1";
        // public readonly string notfis_path = @"scripts/notifs.txt";
        public readonly string error_path = @"scripts/Error.txt";
        public Vector3 coords;

        public TeleportAutoDrive()
        {
            vehicle = ped.CurrentVehicle;
            UI.Notify(message: ModName + Version + " created by " + Developer);
        }
    }

    public class Teleport : TeleportAutoDrive
    {
        private Vector3 place2 = new Vector3(-587.585f, 264.192f, 82.082f);
        public Teleport()
        {
            UI.Notify(message: "Del - Teleport.");

            KeyDown += OnKeyDown;
            //Tick += OnTick;
            Interval = 1;//1ms
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //TeleportToDefaultPlace2
            if (e.KeyCode == Keys.Delete)
            {
                if (vehicle != null)
                {
                    UI.Notify(message: "Teleporting to Place2");

                    vehicle.Repair();
                    vehicle.Position = place2;
                }
            }
        }

        /*Checking collision for fun.
        private void OnTick(object sender, EventArgs e)
        {
            if (Function.Call<bool>(Hash.HAS_ENTITY_COLLIDED_WITH_ANYTHING, vehicle))
            {
                File.AppendAllText(notfis_path, "Collided, at " + DateTime.Now.ToString("h:mm:ss tt") + "\n");
            }
        }*/
    }

    public class AutoDrive : TeleportAutoDrive
    {
        private readonly Dictionary<string, int> DrivingStyles = new Dictionary<string, int>() {
                    { "AvoidTraffic", 786468 },
                    {"IgnoreLights", 2883621 },
                    {"Normal", 786603 },
                    {"Rushed", 1074528293 },
                    { "AvoidTrafficExtremely",  6},
                    {"SometimesOvertakeTraffic", 5 },
                    { "GoReverseAvoidObstacles", 1076},
                    {"StopBeforePeds", 2 },
                    {"AvoidVehicles", 4 },
                    {"StopBeforeVehicles", 1 },
                    {"AvoidObjects", 32 },
                    { "StopAtTrafficLights", 128},
                    {"AvoidPeds", 16},
                    {"IgnoreRoads", 4194304 },
                    {"AllowGoingWrongWay", 512},
                    {"GoReverse", 1024 },
                    {"TakeShortestPath", 262144 },
                    {"AvoidOffroad", 524288 }
        };

        private int HybridDrivingStyle;
        private readonly float speed = 30f;

        public AutoDrive()
        {            
            HybridDrivingStyle = DrivingStyles["AvoidTraffic"] |
                            DrivingStyles["IgnoreLights"] |
                            DrivingStyles["AvoidVehicles"] |
                            DrivingStyles["Rushed"] |
                            DrivingStyles["AvoidPeds"];

            KeyDown += OnKeyDown;
            Interval = 1;//1ms

            UI.Notify(message: "G - Drive to Waypoint.");
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //Go
            if (e.KeyCode == Keys.G)
            {
                try
                {                    
                    coords = World.GetWaypointPosition();

                    if (coords == null)
                    {
                        UI.Notify(message: "Waypoint not declared!");

                        File.AppendAllText(error_path, "NO WAYPOINT SET" + "\n", Encoding.UTF8);
                    }

                    Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped, vehicle,
                             coords.X, coords.Y, coords.Z, speed, 1f, vehicle.GetHashCode(), HybridDrivingStyle, 1f, true);

                    UI.Notify(message: "Driving to Waypoint");

                }
                catch (NullReferenceException ex)
                {
                    File.AppendAllText(error_path, ex.StackTrace + "\n", Encoding.UTF8);
                }
            }
        }
    }
}