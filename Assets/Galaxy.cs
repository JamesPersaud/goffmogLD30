using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Galaxy
    {
        public const int BODY_TYPE_INNER_PLANET =1;
        public const int BODY_TYPE_OUTER_PLANET =2;
        public const int BODY_TYPE_MOON =3;

        public StarSystem CurrentStarSystem;
        public List<StarSystem> StarSystems;        

        private DateTime time = new DateTime(2072,1,1);
        float partialTime = 0;

        public DateTime Time
        {            
            get
            {                
                return time;
            }
        }

        public string getStardateString()
        {
            return "Stardate : " + Galaxy.Instance.Time.Year.ToString() + "." + Galaxy.Instance.Time.Month.ToString() + "." + Galaxy.Instance.Time.Day.ToString();
        }

        public void AddTime(int days)
        {
            //Debug.Log("Addtime " + days.ToString());

            int mon = time.Month;
            time = time.AddDays(days);
            if(time.Month != mon)
            {
                Player.Instance.EndMonth();
            }
        }

        public void TheBigGnab()
        {
            foreach(Body b in CurrentStarSystem.Bodies)
            {
                GameObject.Destroy(b.OrbitalRing);
                foreach(Body m in b.childBodies)
                {
                    GameObject.Destroy(m.OrbitalRing);
                    GameObject.Destroy(m.gameObject);
                }
                GameObject.Destroy(b.gameObject);
            }

            StarSystems.Clear();
        }

        public void LetThereBeLight()
        {
            StarSystem newSystem = new StarSystem();

            newSystem.Location = Vector3.zero;
            newSystem.Name = "Goffmogia"; 

            int numberOfInnerPlanets = Helpers.GetSmallFibonacci() * 2;
            int numberOfOuterPlanets = Helpers.GetSmallFibonacci() * 2;

            float lastorbit = 15;
            float lastradius = 0;

            for (int i = 0; i < numberOfInnerPlanets; i++ )
            {
                Body planet = createInnerPlanet(ref lastorbit, ref lastradius);
                planet.BodyType = BODY_TYPE_INNER_PLANET;
                createMoonsForPlanet(planet, false);
                newSystem.Bodies.Add(planet);
                planet.Orbit += planet.ClearedOrbit;
                lastradius += planet.ClearedOrbit *2f;
            }

            lastorbit += Helpers.GetFibonacci(5, 9);

            for (int i = 0; i < numberOfOuterPlanets; i++)
            {
                Body planet = createOuterPlanet(ref lastorbit, ref lastradius);
                planet.BodyType = BODY_TYPE_OUTER_PLANET;
                createMoonsForPlanet(planet, true);
                newSystem.Bodies.Add(planet);
                lastorbit += Helpers.GetFibonacci(5, 9);
                planet.Orbit += planet.ClearedOrbit;
                lastradius += planet.ClearedOrbit *2f;
            }                       

            CurrentStarSystem = newSystem;

            //name bodies
            CurrentStarSystem.rename(CurrentStarSystem.Name);
        }

        public static void createMoonsForPlanet(Body planet, bool outer)
        {
            int chance = outer ? 3 : 5;

            if ((Helpers.GalacticRandom.Next(6) + 1) < chance)
                return;

            int numberOfMoons = outer ? Helpers.GetFibonacci(1,6) : Helpers.GetSmallFibonacci();

            float lastorbit = Helpers.GetFibonacci(0,7);
            float lastradius = 0;

            for(int i = 0; i < numberOfMoons; i++)
            {
                Body moon = createMoon(ref lastorbit, ref lastradius, planet.BodyRadius, outer);
                moon.BodyType = BODY_TYPE_MOON;
                planet.childBodies.Add(moon);
                moon.ParentBody = planet;
            }

            planet.ClearedOrbit = lastorbit + lastradius;
        }

        public static Body createMoon(ref float lastorbit, ref float lastradius, float planetsize, bool outer)
        {
            GameObject o = new GameObject();
            Body body = o.AddComponent<Body>();            

            body.BodyRadius = (outer ? planetsize/2f : planetsize) / 8 * (float)Helpers.GetSmallFibonacci();
            body.Orbit = (float)Helpers.GetSmallFibonacci() + lastorbit + lastradius + (body.BodyRadius*2) + planetsize;
            if (outer)
                body.Period = (float)Helpers.GetFibonacci(7, 12) / 4f;
            else
                body.Period = (float)Helpers.GetFibonacci(4, 9) /4f;
            body.Spin = (float)Helpers.GetSmallFibonacci() / 8f;

            lastorbit = body.Orbit;
            lastradius = body.BodyRadius;
            
            return body;
        }

        public static Body createInnerPlanet(ref float lastorbit, ref float lastradius)
        {
            GameObject o = new GameObject();
            Body body = o.AddComponent<Body>();

            body.BodyRadius = (float)Helpers.GetSmallFibonacci();
            body.Orbit = (float)Helpers.GetFibonacci(2,7) + lastorbit + lastradius + body.BodyRadius;
            body.Period = (float)Helpers.GetFibonacci(7, 12);
            body.Spin = (float)Helpers.GetSmallFibonacci();

            lastorbit = body.Orbit;
            lastradius = body.BodyRadius;

            return body;
        }

        public static Body createOuterPlanet(ref float lastorbit, ref float lastradius)
        {            
            GameObject o = new GameObject();
            Body body = o.AddComponent<Body>();

            body.BodyRadius = (float)Helpers.GetFibonacci(5, 8);
            body.Orbit = (float)Helpers.GetFibonacci(2, 7) + lastorbit + lastradius + body.BodyRadius;
            body.Period = (float)Helpers.GetFibonacci(12, 16);
            body.Spin = (float)Helpers.GetSmallFibonacci() /8f;

            lastorbit = body.Orbit;
            lastradius = body.BodyRadius;

            return body;
        }

        public static Body createMoon()
        {
            GameObject o = new GameObject();
            Body body = o.AddComponent<Body>();



            return body;
        }

        private Galaxy()
        {
            StarSystems = new List<StarSystem>();
        }

        #region singleton
        private static Galaxy instance;
        public static Galaxy Instance
        {
            get
            {
                if (instance == null)
                    instance = new Galaxy();

                return instance;
            }
        }
        #endregion
    }
}
