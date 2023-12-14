using GTA;
using GTA.Native;
using GTA.Math;
using LemonUI;
using LemonUI.TimerBars;
using GTA.UI;
using System;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

public class GhostHunt : Script
{

    Prop GhostHandleObj;
    Ped GhostHandlePed;
    Blip sphere;
    Blip sphere2;
    Blip sphere3;
    Blip GhostBlip;
    int GhostCreated = 0;
    int MissionStarted = 0;
    string[] GhostModel = new string[5];
    Vector3[] coords = new Vector3[17];
    Vector3 mission_start = new Vector3();
    float[] heading = new float[17];
    int global_index = 0;

    private static readonly ObjectPool pool = new ObjectPool();
    private static readonly TimerBarCollection bar_pool = new TimerBarCollection();
    private static readonly TimerBar testTimer = new TimerBar("DAYLIGHT", "00:00");
    private static readonly TimerBar GhostCount = new TimerBar("GHOSTS EXPOSED", "0");
    int GhostCountInt = 0;
    int HoursInt = 0;
    int MinutesInt = 0;


    public GhostHunt()
    {
        Tick += OnTick;

        mission_start = new Vector3(1572.968f, 3589.784f, 34.36174f);
        GhostBlip = World.CreateBlip(mission_start);
        Function.Call(Hash.SET_BLIP_SPRITE, GhostBlip, 484);

        bar_pool.Add(testTimer);
        bar_pool.Add(GhostCount);
        pool.Add(bar_pool);

        GhostModel[0] = "m23_1_prop_m31_ghostrurmeth_01a";
        GhostModel[1] = "m23_1_prop_m31_ghostsalton_01a";
        GhostModel[2] = "m23_1_prop_m31_ghostskidrow_01a";
        GhostModel[3] = "m23_1_prop_m31_ghostzombie_01a";
        GhostModel[4] = "m23_1_prop_m31_ghostjohnny_01a";

        coords[0] = new Vector3(1602.959f, 3566.88f, 38.7752f);
        heading[0] = 25.09695f;

        coords[1] = new Vector3(1609.553f, 3574.596f, 38.77521f);
        heading[1] = 124.495f;

        coords[2] = new Vector3(1596.607f, 3579.314f, 38.77007f);
        heading[2] = 16.78974f;

        coords[3] = new Vector3(1600.898f, 3585.954f, 38.76653f);
        heading[3] = 176.7548f;

        coords[4] = new Vector3(1598.654f, 3590.576f, 38.76653f);
        heading[4] = 112.4711f;

        coords[5] = new Vector3(1592.128f, 3586.007f, 38.76653f);
        heading[5] = 240.6818f;

        coords[6] = new Vector3(1596.517f, 3595.96f, 38.7665f);
        heading[6] = 117.7747f;

        coords[7] = new Vector3(1575.774f, 3613.366f, 38.77521f);
        heading[7] = 286.7294f;

        coords[8] = new Vector3(1583.25f, 3620.584f, 38.77521f);
        heading[8] = 121.6129f;

        coords[9] = new Vector3(1560.482f, 3597.966f, 38.77518f);
        heading[9] = 117.1574f;

        coords[10] = new Vector3(1552.854f, 3603.876f, 38.77518f);
        heading[10] = 210.5382f;

        coords[11] = new Vector3(1543.702f, 3587.101f, 38.76653f);
        heading[11] = 36.04033f;

        coords[12] = new Vector3(1538.743f, 3595.181f, 38.76653f);
        heading[12] = 206.6823f;

        coords[13] = new Vector3(1538.801f, 3583.286f, 38.76653f);
        heading[13] = 29.49369f;

        coords[14] = new Vector3(1531.388f, 3591.508f, 38.76653f);
        heading[14] = 29.49369f;

        coords[15] = new Vector3(1515.001f, 3570.178f, 38.73648f);
        heading[15] = 107.9317f;

        coords[16] = new Vector3(1506.79f, 3577.065f, 38.73648f);
        heading[16] = 210.2054f;

        for (int i = 0; i > 5; i++)
        {
            var model = new Model(GhostModel[i]);
            model.Request(250);

            if (model.IsInCdImage && model.IsValid)
            {
                while (!model.IsLoaded) Script.Wait(50);
            }
        }
    }

    private void OnTick(object sender, EventArgs e)
    {

        FoundGhostsCheck();
        MissionStartSphere();
    }

    void MissionStartSphere()
    {
        if (MissionStarted == 0)
        {
            World.DrawMarker(MarkerType.VerticalCylinder, mission_start, Vector3.Zero, Vector3.Zero, new Vector3(1.0f, 1.0f, 1.0f), Color.LightBlue);
            if (World.GetDistance(Game.Player.Character.Position, mission_start) < 1.5f)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_PICKUP~ to start the Ghosts Exposed mission.");
            }

            if (World.GetDistance(Game.Player.Character.Position, mission_start) < 1.5f && Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, 38))
            {
                int current_hours = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                if (current_hours > 1 && current_hours < 4)
                {
                    sphere = GTA.Native.Function.Call<Blip>(GTA.Native.Hash.ADD_BLIP_FOR_AREA, 1589.493f, 3599.215f, 42.11668f, 50.0f, 100.0f);
                    Function.Call(Hash.SET_BLIP_SPRITE, sphere, 9);
                    Function.Call(Hash.SET_BLIP_COLOUR, sphere, 47);
                    Function.Call(Hash.SET_BLIP_ALPHA, sphere, 50);
                    Function.Call(Hash.SET_BLIP_DISPLAY, sphere, 8);
                    Function.Call(Hash.SET_BLIP_ROTATION, sphere, 0);
                    sphere2 = GTA.Native.Function.Call<Blip>(GTA.Native.Hash.ADD_BLIP_FOR_AREA, 1529.361f, 3587.22f, 35.46088f, 50.0f, 100.0f);
                    Function.Call(Hash.SET_BLIP_SPRITE, sphere2, 9);
                    Function.Call(Hash.SET_BLIP_COLOUR, sphere2, 47);
                    Function.Call(Hash.SET_BLIP_ALPHA, sphere2, 50);
                    Function.Call(Hash.SET_BLIP_DISPLAY, sphere2, 8);
                    Function.Call(Hash.SET_BLIP_ROTATION, sphere2, 120);

                    Random rnd = new Random();
                    int index_model = rnd.Next(0, 4);
                    int index_crds = GetCoords();
                    GenerateGhostInMotel();
                    GTA.UI.Screen.ShowSubtitle("Find the ghosts in ~y~the abandoned motel.", 999999);
                    Function.Call(Hash.SET_MISSION_FLAG, true);
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, "HALLOWEEN_START_MUSIC");
                    GhostBlip.Delete();
                    MissionStarted = 1;
                }
                else
                {
                    GTA.UI.Screen.ShowHelpText("Ghost Expose is available from 2am to 4am.");
                    Wait(3000);
                }
            }
        }
    }

    void GenerateGhostInMotel()
    {
        if (GhostCreated == 0)
        {
            Random rnd = new Random();
            int index_model = rnd.Next(0, 4);
            int index_crds = GetCoords();
            global_index = index_crds;
            CreateGhostModel(index_model, coords[index_crds], heading[index_crds]);
            GhostCreated = 1;
        }
    }

    void FoundGhostsCheck()
    {
        if (MissionStarted == 1)
        {
            Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 19, true);
        }

        if (GhostCreated == 1)
        {
            int current_hours = Function.Call<int>(Hash.GET_CLOCK_HOURS);
            int current_minutes = Function.Call<int>(Hash.GET_CLOCK_MINUTES);
            string final_hours;
            string final_minutes;

            if (sphere != null)
            {
                if (Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 1572.968f, 3589.784f, 35.36174f) > 70.0)
                {
                    if (sphere3 == null)
                    {
                        sphere.Delete();
                        sphere = null;
                        sphere2.Delete();
                        sphere3 = World.CreateBlip(mission_start);
                        Function.Call(Hash.SET_BLIP_SPRITE, sphere3, 1);
                        Function.Call(Hash.SET_BLIP_COLOUR, sphere3, 5);
                        GTA.UI.Screen.ShowSubtitle("Go back to ~y~the motel.", 999999);
                    }
                }
            }
            else
            {
                if (sphere3 != null)
                {
                    if (Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 1572.968f, 3589.784f, 35.36174f) < 40.0)
                    {
                        sphere3.Delete();
                        sphere3 = null;
                        sphere = GTA.Native.Function.Call<Blip>(GTA.Native.Hash.ADD_BLIP_FOR_AREA, 1589.493f, 3599.215f, 42.11668f, 50.0f, 100.0f);
                        Function.Call(Hash.SET_BLIP_SPRITE, sphere, 9);
                        Function.Call(Hash.SET_BLIP_COLOUR, sphere, 47);
                        Function.Call(Hash.SET_BLIP_ALPHA, sphere, 50);
                        Function.Call(Hash.SET_BLIP_DISPLAY, sphere, 8);
                        Function.Call(Hash.SET_BLIP_ROTATION, sphere, 0);
                        sphere2 = GTA.Native.Function.Call<Blip>(GTA.Native.Hash.ADD_BLIP_FOR_AREA, 1529.361f, 3587.22f, 35.46088f, 50.0f, 100.0f);
                        Function.Call(Hash.SET_BLIP_SPRITE, sphere2, 9);
                        Function.Call(Hash.SET_BLIP_COLOUR, sphere2, 47);
                        Function.Call(Hash.SET_BLIP_ALPHA, sphere2, 50);
                        Function.Call(Hash.SET_BLIP_DISPLAY, sphere2, 8);
                        Function.Call(Hash.SET_BLIP_ROTATION, sphere2, 120);
                        GTA.UI.Screen.ShowSubtitle("Find the ghosts in ~y~the abandoned motel.", 999999);
                    }
                }
            }

            if (current_hours < 6)
            {
                HoursInt = 5 - current_hours;

                if (current_minutes == 0)
                {
                    int new_hours = Function.Call<int>(Hash.GET_CLOCK_HOURS);
                    HoursInt = 5 - new_hours + 1;
                    MinutesInt = 0;
                }
                else
                {
                    MinutesInt = 60 - current_minutes;
                }

                final_hours = "0" + HoursInt;

                if (MinutesInt.ToString().Length == 1)
                {
                    final_minutes = "0" + MinutesInt;
                }
                else
                {
                    final_minutes = MinutesInt.ToString();
                }

                string times = "" + final_hours + ":" + final_minutes;
                testTimer.Info = times;

                pool.Process();
                var position = Game.Player.Character.GetOffsetPosition(new Vector3(0, 0, 0));
                if (GhostPhotographed())
                {
                    Wait(2000);
                    Function.Call(Hash.DESTROY_MOBILE_PHONE);
                    ScaleFormMessages.Message.SHOW_SHARD_CREW_RANKUP_MP_MESSAGE("The ghost has been exposed", "Keep looking for the others", 10000);
                    Function.Call(Hash.PLAY_SOUND, -1, "package_delivered_success", "DLC_GR_Generic_Mission_Sounds", 0, 0, 1);
                    GhostCountInt++;
                    string GhostInfo = GhostCountInt.ToString();
                    GhostCount.Info = GhostInfo;
                    GhostDelete();
                    GhostCreated = 0;
                    GenerateGhostInMotel();
                }
                else
                {
                    if (Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, coords[global_index].X, coords[global_index].Y, coords[global_index].Z, position.X, position.Y, position.Z, 0) < 1.5)
                    {
                        GTA.UI.Screen.ShowHelpText("You missed a ghost. Look for another one!");
                        GhostDelete();
                        GhostCreated = 0;
                        GenerateGhostInMotel();
                    }
                }
            }
            else
            {
                if (Game.Player.Character.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, 0))
                {
                    MissionCleanup();
                    Function.Call(Hash.SET_MISSION_FLAG, false);
                    Function.Call(Hash.CLEAR_PRINTS);
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, "HALLOWEEN_FAST_STOP_MUSIC");
                    GhostCreated = 0;
                }
                else
                {
                    Function.Call(Hash.PLAY_MISSION_COMPLETE_AUDIO, "FRANKLIN_BIG_01");
                    ScaleFormMessages.Message.SHOW_SHARD_CREW_RANKUP_MP_MESSAGE("Mission complete", "You were able to catch " + GhostCountInt + " ghosts in one night", 10000);
                    GhostCreated = 0;
                    MissionCleanup();
                    Function.Call(Hash.SET_MISSION_FLAG, false);
                    Function.Call(Hash.CLEAR_PRINTS);
                    Function.Call(Hash.TRIGGER_MUSIC_EVENT, "HALLOWEEN_FAST_STOP_MUSIC");
                }
            }
        }
    }

    void CreateGhostModel(int index, Vector3 coords, float heading)
    {
        var modelObj = new Model(GhostModel[index]);
        var modelPed = new Model("a_f_y_beach_01");
        modelObj.Request(250);
        modelPed.Request(250);
        while (!modelObj.IsLoaded) Script.Wait(0);
        while (!modelPed.IsLoaded) Script.Wait(0);

        GhostHandleObj = World.CreateProp(modelObj, coords, true, true);
        GhostHandleObj.Heading = heading;
        GhostHandlePed = World.CreatePed(modelPed, coords);
        Function.Call(Hash.SET_ENTITY_NO_COLLISION_ENTITY, GhostHandlePed, GhostHandleObj, false);
        Function.Call(Hash.SET_ENTITY_VISIBLE, GhostHandlePed, false, false);
    }

    bool GhostPhotographed()
    {
        if (Function.Call<bool>(Hash.CELL_CAM_IS_CHAR_VISIBLE_NO_FACE_CHECK, GhostHandlePed) && Function.Call<bool>(Hash.PHONEPHOTOEDITOR_IS_ACTIVE) && Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, 176))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void GhostDelete()
    {

        while(!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, "scr_rcbarry1"))
        {
            Script.Wait(0);
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, "scr_rcbarry1");
        }
        Function.Call(Hash.USE_PARTICLE_FX_ASSET, "scr_rcbarry1");
        Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, "scr_alien_teleport", GhostHandlePed, 0, 0, 0, 0, 0, 0, 8.0f, 0, 0, 0);
        Wait(1000);
        GhostHandleObj.Delete();
        GhostHandlePed.Delete();
        GhostHandleObj = null;
        GhostHandlePed = null;
    }

    void MissionCleanup()
    {
        if (GhostHandleObj != null)
        {
            GhostHandleObj.Delete();
        }

        if (GhostHandlePed != null)
        {
            GhostHandlePed.Delete();
        }

        if (sphere != null)
        {
            sphere.Delete();
        }

        if (sphere2 != null)
        {
            sphere2.Delete();
        }

        MissionStarted = 0;
        Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 19, false);
        mission_start = new Vector3(1572.968f, 3589.784f, 34.36174f);
        GhostBlip = World.CreateBlip(mission_start);
        Function.Call(Hash.SET_BLIP_SPRITE, GhostBlip, 484);
    }

    int GetCoords()
    {
        Random rnd = new Random();
        int index = rnd.Next(0, 17);
        var position = Game.Player.Character.GetOffsetPosition(new Vector3(0, 0, 0));

        float dist = 0.0f;
        while (dist < 3.0)
        {
            index = rnd.Next(0, 17);
            dist = Function.Call<float>(Hash.GET_DISTANCE_BETWEEN_COORDS, coords[index].X, coords[index].Y, coords[index].Z, position.X, position.Y, position.Z, 0);
        }
        return index;
    }

    public class ScaleFormMessage
    {
        private Scaleform _sc;
        private int _start;
        private int _timer;

        internal void Load()
        {
            if (_sc != null) return;
            _sc = new Scaleform("MP_BIG_MESSAGE_FREEMODE");
            var timeout = 1000;
            var start = DateTime.Now;
            while (!Function.Call<bool>(Hash.HAS_SCALEFORM_MOVIE_LOADED, _sc.Handle) &&
                    DateTime.Now.Subtract(start).TotalMilliseconds < timeout) Script.Yield();
        }

        internal void Dispose()
        {
            Function.Call(Hash.SET_SCALEFORM_MOVIE_AS_NO_LONGER_NEEDED, new OutputArgument(_sc.Handle));
            _sc = null;
        }

        public void SHOW_MISSION_PASSED_MESSAGE(string msg, int time = 5000)
        {
            Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg, "", 100, true, 0, true);
            _timer = time;
        }

        public void SHOW_SHARD_CREW_RANKUP_MP_MESSAGE(string msgA, string msgB, int time = 5000)
        {
            Load();
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CREW_RANKUP_MP_MESSAGE", msgA, msgB, 100, true, 0, true);
            _timer = time;
        }

        public void CALL_FUNCTION(string funcName, params object[] paremeters)
        {
            Load();
            _sc.CallFunction(funcName, paremeters);
        }

        internal void DoTransition()
        {
            if (_sc == null) return;
            _sc.Render2D();
            if (_start != 0 && Game.GameTime - _start > _timer)
            {
                _sc.CallFunction("TRANSITION_OUT");
                _start = 0;
                Dispose();
            }
        }
    }

    public class ScaleFormMessages : Script
    {
        public ScaleFormMessages()
        {
            Message = new ScaleFormMessage();

            Tick += (sender, args) => { Message.DoTransition(); };
        }

        public static ScaleFormMessage Message { get; set; }
    }
}