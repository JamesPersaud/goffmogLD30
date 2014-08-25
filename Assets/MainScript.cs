using UnityEngine;
using System.Collections;

namespace Assets
{
    public class MainScript : MonoBehaviour
    {
        public static float SECONDS_PER_DAY = 1f;
        public float elapsed = 0;

        // Use this for initialization
        void Start()
        {
            //Init the game!
            Galaxy.Instance.LetThereBeLight();

        }

        // Update is called once per frame
        void Update()
        {
            if (Player.Instance.Playstate == Player.PLAYSTATE_MAIN_GAME)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= SECONDS_PER_DAY)
                {
                    int dayspassed = (int)Mathf.Floor(elapsed / SECONDS_PER_DAY);
                    Galaxy.Instance.AddTime(dayspassed);
                    elapsed = elapsed - SECONDS_PER_DAY;
                }

                Messaging.Instance.DoGravity(Time.deltaTime);
            }
        }

        void OnGUI()
        {
            float menuMinWidth = 600;
            float menuMinHeight = 400;

            if(Player.Instance.Playstate == Player.PLAYSTATE_MAIN_MENU)
            {
                GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight), "Astro Tycoon : Main Menu");

                float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                float top = ((float)Screen.height - menuMinHeight) / 2f + 40f;

                GUI.Label(new Rect(left, top, 400,20), "Your name is");
                Player.Instance.Name = GUI.TextField(new Rect(left + 400, top, 150, 20), Player.Instance.Name);

                top += 40;

                GUI.Label(new Rect(left, top, 400, 20), "You are the proud owner of the fledgling transport company");
                Player.Instance.Company = GUI.TextField(new Rect(left + 400, top, 150, 20), Player.Instance.Company);

                top += 20;
                GUI.Label(new Rect(left,top,400,20), "(and an enormous loan from the galactic development bank)");

                top += 40;

                GUI.Label(new Rect(left, top, 400, 20), "Your company motto is");
                Player.Instance.Motto = GUI.TextField(new Rect(left + 400, top, 150, 20), Player.Instance.Motto);

                top += 40;
                GUI.Label(new Rect(left, top, 400, 20), "Your home star system is called");
                Galaxy.Instance.CurrentStarSystem.Name = GUI.TextField(new Rect(left + 400, top, 150, 20), Galaxy.Instance.CurrentStarSystem.Name);

                top += 40;
                GUI.Label(new Rect(left, top, 600, 20), Galaxy.Instance.CurrentStarSystem.Description());

                top += 60;

                if (GUI.Button(new Rect(left +50, top, 400, 40), "Re-Gen System"))
                {
                    Galaxy.Instance.TheBigGnab();
                    Galaxy.Instance.LetThereBeLight();
                }

                top += 60;

                if (GUI.Button(new Rect(left + 50, top, 400, 40), "Play"))
                {
                    Player.Instance.Playstate = Player.PLAYSTATE_MAIN_GAME;
                    Messaging.Instance.PushMessage("Welcome to Astro Tycoon! - A Ludum Dare compo entry (LD30)\nBuilt from scratch in under 48 hours by James Persaud (@goffmog).\n\nUse WASD to pan around and the mousewheel to zoom in and out.");
                }
            }
            else if(Player.Instance.Playstate == Player.PLAYSTATE_MAIN_GAME)
            {
                GUI.Box(new Rect(0,0,Screen.width,80), "");

                GUI.Label(new Rect(20, 10, 200, 20), Player.Instance.Name);
                GUI.Label(new Rect(20, 30, 200, 40), Player.Instance.Company);
                GUI.Label(new Rect(20, 50, 200, 60), Player.Instance.Motto);

                GUI.Label(new Rect(200, 10, 80, 20), "Credits");                

                GUI.Label(new Rect(280, 10, 200, 20), " : " + Player.Instance.Credits.ToString("N0") + " CR");                

                GUI.Label(new Rect(400, 10, 150, 60), Galaxy.Instance.getStardateString());

                if(Player.Instance.Buyingstate == Player.BUYINGSTATE_SHIP)
                {
                    GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight), "Previously owned starshps, every one a bargain");

                    float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                    float top = ((float)Screen.height - menuMinHeight) / 2f + 80f;

                    string message = "You have " + Player.Instance.Ships.Count.ToString() + " ship" + ((Player.Instance.Ships.Count ==1) ? "" : "s") + "\n\n";
                    GUI.Label(new Rect(left, top, 500, 25), message);

                    float shiptop = top;
                    foreach(Ship ship in Player.Instance.Ships)
                    {
                        shiptop += 30;
                        string shipstring = ship.Name + " a Passenger craft : ";

                        if(ship.InStorage)
                        {
                            shipstring += "In Storage";
                        }
                        else
                        {
                            shipstring += "Ferrying passengers between " + ship.Source.Name + " and " + ship.Target.Name;
                        }

                        GUI.Label(new Rect(left, shiptop, 500, 30), shipstring);
                    }

                    if (Player.Instance.Credits >= 2000)
                    {
                        if (GUI.Button(new Rect(left + 50, top + 200, 400, 40), "Purchase a passenger craft : 2000 CR"))
                        {
                            Player.Instance.Expenditure += 2000;
                            Player.Instance.Credits -= 2000;
                            Ship newship = Ship.newShip(Ship.SHIPTYPE_PASSENGER, null, null);
                            Player.Instance.Ships.Add(newship);
                            newship.Name = "Ship " + Player.Instance.Ships.Count.ToString();
                        }
                    }
                    else
                    {
                        GUI.Label(new Rect(400, top + 200, 400, 40), "There is nothing here that you can afford :(");
                    }

                    if (GUI.Button(new Rect(left + 50, top + 250, 400, 40), "Exit"))
                    {
                        Player.Instance.Buyingstate = Player.BUYINGSTATE_NONE;
                    }
                }
                else if (Player.Instance.Buyingstate == Player.BUYINGSTATE_ROUTE_CHOOSESHIP)
                {
                    GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight), "Fleet Standing By : Choose a ship to reassign.");

                    float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                    float top = ((float)Screen.height - menuMinHeight) / 2f + 80f;

                    string message = "You have " + Player.Instance.Ships.Count.ToString() + " ship" + ((Player.Instance.Ships.Count == 1) ? "" : "s") + "\n\n";
                    GUI.Label(new Rect(left, top, 500, 25), message);

                    float shiptop = top;
                    foreach (Ship ship in Player.Instance.Ships)
                    {
                        shiptop += 30;
                        string shipstring = ship.Name + " a Passenger craft : ";

                        if (ship.InStorage)
                        {
                            shipstring += "In Storage";
                        }
                        else
                        {
                            shipstring += "Ferrying passengers between " + ship.Source.Name + " and " + ship.Target.Name;
                        }

                        if (GUI.Button(new Rect(left + 50, shiptop, 500, 30), shipstring))
                        {
                            Player.Instance.Buyingstate = Player.BUYINGSTATE_ROUTE_CHOOSESOURCE;
                            Player.Instance.RouteShip = ship;
                            HudMasterBehaviour.ShowingPlanetHUD = !HudMasterBehaviour.ShowingPlanetHUD;
                        }
                    }

                    if (Player.Instance.Ships.Count < 1)
                    {                    
                        GUI.Label(new Rect(400, top + 200, 400, 40), "There are some lovely bargains at the second-hand stardock.");
                    }

                    if (GUI.Button(new Rect(left + 50, top + 250, 400, 40), "Exit"))
                    {
                        Player.Instance.Buyingstate = Player.BUYINGSTATE_NONE;
                    }
                }
                else if (Player.Instance.Buyingstate == Player.BUYINGSTATE_ROUTE_CHOOSESOURCE)
                {
                    GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight - 100), "Route Planning : Choose the origin planet");

                    float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                    float top = ((float)Screen.height - menuMinHeight) / 2f + 80f;

                    string message = Player.Instance.RouteShip.Name + " will transport passengers between two worlds, the amount of money that can be made from this route depends on the population of the worlds and their distance apart. Long-haul flights make more money but cost more to operate.\n\n";
                    GUI.Label(new Rect(left, top, 500, 75), message);

                    
                    GUI.Label(new Rect(left + 50, top + 100, 400, 40), "Choose the planet of origin for this route ");

                    if (GUI.Button(new Rect(left + 50, top + 150, 400, 40), "Exit"))
                    {
                        Player.Instance.Buyingstate = Player.BUYINGSTATE_NONE;
                    }
                }
                else if (Player.Instance.Buyingstate == Player.BUYINGSTATE_ROUTE_CHOOSETARGET)
                {
                    GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight - 100), "Route Planning : Choose the destination planet");

                    float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                    float top = ((float)Screen.height - menuMinHeight) / 2f + 80f;
                    
                    string message = Player.Instance.RouteShip.Name + " will transport passengers from " + Player.Instance.RoutePlanet1.Name + " (pop. " + Player.Instance.RoutePlanet1.GetPopulationString()+")\n\n";                    
                    GUI.Label(new Rect(left, top, 500, 75), message);


                    GUI.Label(new Rect(left + 50, top + 100, 400, 40), "Choose the destination planet for this route ");

                    if (GUI.Button(new Rect(left + 50, top + 150, 400, 40), "Exit"))
                    {
                        Player.Instance.Buyingstate = Player.BUYINGSTATE_NONE;
                    }
                }
                else if (Player.Instance.Buyingstate == Player.BUYINGSTATE_ROUTE_CONFIRM)
                {
                    GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight), "Route Planning : requesting Confirmation");

                    float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                    float top = ((float)Screen.height - menuMinHeight) / 2f + 80f;

                    string message = Player.Instance.RouteShip.Name + " will transport passengers between \n" 
                        + Player.Instance.RoutePlanet1.Name + " (pop. " + Player.Instance.RoutePlanet1.GetPopulationString()+")\nand " 
                        + Player.Instance.RoutePlanet2.Name + " (pop. " + Player.Instance.RoutePlanet2.GetPopulationString()+")";

                    GUI.Label(new Rect(left, top, 500, 75), message);

                    GUI.Label(new Rect(left, top + 100, 500, 25), "Distance at perihelic opposition " + (Mathf.Abs(Player.Instance.RoutePlanet2.Orbit - Player.Instance.RoutePlanet1.Orbit)).ToString("N0") + " Standard Units");


                    if (GUI.Button(new Rect(left + 50, top + 200, 400, 40), "Engage!"))
                    {
                        Player.Instance.Buyingstate = Player.BUYINGSTATE_NONE;
                        Player.Instance.RouteShip.Source = Player.Instance.RoutePlanet1;
                        Player.Instance.RouteShip.Target = Player.Instance.RoutePlanet2;
                        Player.Instance.RouteShip.InStorage = false;
                        Player.Instance.RouteShip.gameObject.SetActive(true);

                        Player.Instance.RouteShip.startMission();
                    }

                    if (GUI.Button(new Rect(left + 50, top + 250, 400, 40), "Delay that order"))
                    {
                        Player.Instance.Buyingstate = Player.BUYINGSTATE_NONE;
                    }
                }
                else if(Player.Instance.ShowingModal)
                {
                    GUI.Box(new Rect(((float)Screen.width - menuMinWidth) / 2f, ((float)Screen.height - menuMinHeight) / 2f, menuMinWidth, menuMinHeight), Player.Instance.ModalTitle);

                    float left = ((float)Screen.width - menuMinWidth) / 2f + 20f;
                    float top = ((float)Screen.height - menuMinHeight) / 2f + 80f;

                    GUI.Label(new Rect(left, top, 500, 200), Player.Instance.ModalMessage);

                    if (GUI.Button(new Rect(left + 50, top + 250, 400, 40), "Continue"))
                    {
                        Player.Instance.ShowingModal = false;
                    }
                }
            }
        }
    }
}

