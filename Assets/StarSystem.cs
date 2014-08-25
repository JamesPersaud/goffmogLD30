using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Assets
{
    public class StarSystem
    {
        public List<Body> Bodies;
        public Vector3 Location;

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                rename(name);
            }
        }
        
        public void rename(string newname)
        {
            name = newname;
            int ip = 0;
            foreach (Body b in Bodies)
            {                
                b.gameObject.name = Name + " " + Helpers.getNumeral(ip);
                b.Name = b.gameObject.name;

                int im = 0;
                foreach (Body m in b.childBodies)
                {
                    m.gameObject.name = b.name + Helpers.getAlpha(im);
                    m.Name = m.gameObject.name;
                    im++;
                }

                ip++;
            }
        }

        public string Description()
        {
            int countInner = 0;
            int countouter = 0;
            int countmoons = 0;

            foreach(Body planet in Bodies)
            {
                foreach (Body moon in planet.childBodies)
                    countmoons++;

                if (planet.BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
                    countInner++;
                else
                    countouter++;
            }

            string d = Name + " is a white dwarf with " + countInner.ToString() + " inner planet" + ((countInner > 1) ? "s" : "") + ", " + countouter.ToString() + " outer planet" + ((countouter > 1) ? "s" : "") + " and " + countmoons.ToString() + " moon" + ((countmoons > 1) ? "s" : "") + ".";
            return d;
        }

        public StarSystem()
        {
            Bodies = new List<Body>();
        }
    }
}
