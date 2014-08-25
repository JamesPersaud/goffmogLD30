using UnityEngine;
using System.Collections.Generic;

namespace Assets
{    
    public class Body : MonoBehaviour
    {
        public static float tooltipdelay = 0.5f;

        public Texture2D BodyTexture = null;
        public Texture2D HUDTexture = null;
        public Texture2D HUDTexture_over = null;

        public Body ParentBody = null;
        public List<Body> childBodies = new List<Body>();
        public string Name = "Planet X";
        public float Orbit = 10;
        public float Period = 60;
        public float Spin = 0.5f;
        public float BodyRadius = 2;

        public long Population;

        public Color SkyColor;
        public Color CloudColor;

        public float angle = 0;

        public float ClearedOrbit = 0;
        public int BodyType;

        public GameObject OrbitalRing = null;        
        public float lastMoused;

        // Use this for initialization
        void Start()
        {            
            angle = (float)(Helpers.GalacticRandom.NextDouble() * Mathf.PI * 2);            

            SkyColor = new Color(
                Mathf.Clamp((float)Helpers.GalacticRandom.NextDouble(),0.2f,0.6f),
                Mathf.Clamp((float)Helpers.GalacticRandom.NextDouble(), 0.2f, 0.6f),
                Mathf.Clamp((float)Helpers.GalacticRandom.NextDouble(), 0.2f, 0.6f),1.0f);

            CloudColor = new Color(
                Mathf.Clamp((float)Helpers.GalacticRandom.NextDouble(), 0.2f, 0.6f),
                Mathf.Clamp((float)Helpers.GalacticRandom.NextDouble(), 0.2f, 0.6f),         
                Mathf.Clamp((float)Helpers.GalacticRandom.NextDouble(), 0.2f, 0.6f), 1.0f);

            SphereMaker.Spherify(this.gameObject, this);
            getHudTexture();
            //if(BodyType != Galaxy.BODY_TYPE_MOON)
              //  Helpers.AddTrails(gameObject, this);

            renderer.castShadows = false;
            renderer.receiveShadows = false;

            OrbitalRing = TorusMaker.MakeTorus(this);

            switch(BodyType)
            {
                case Galaxy.BODY_TYPE_INNER_PLANET: Population = Helpers.GetFibonacci(0, 12) * 10 * Helpers.GalacticRandom.Next(0, 11); break;
                case Galaxy.BODY_TYPE_OUTER_PLANET: Population = 0; break;
                case Galaxy.BODY_TYPE_MOON: Population = Helpers.GetFibonacci(0,7) * Helpers.GalacticRandom.Next(10,101); break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if parent is null then it orbits the centre of the system.            
            angle += (Mathf.PI / Period) * Time.deltaTime;                        

            this.transform.position = Helpers.PoleToCart(Orbit, angle, getParentPosition());

            transform.RotateAroundLocal(new Vector3(0, 1, 0), Spin * Time.deltaTime);

            if (ParentBody != null)
                OrbitalRing.transform.position = ParentBody.transform.position;
        }

        public string GetPopulationString()
        {
            string i = "Population : " + Population.ToString("N0");
            if (BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
                i += " Million";
            return i;
        }

        public string GetInfo()
        {
            string i = string.Empty;

            i += Name + "\n\n";

            if (BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
                i += "Habitable rocky planet\n\n";
            if (BodyType == Galaxy.BODY_TYPE_OUTER_PLANET)
                i += "Inhospitable gas giant\n\n";
            if (BodyType == Galaxy.BODY_TYPE_MOON)
                i += "Habitable rocky natural satellite\n\n";

            i += "Orbit : " + Orbit.ToString() + " Standard Galactic Units\n";
            i += "Size (radius) : " + BodyRadius.ToString() + " Standard Galactic Units\n";
            i += "Year : " + (Period / MainScript.SECONDS_PER_DAY).ToString() + " Standard Galactic Days\n";
            i += "Day : " + ((Mathf.PI * 2 / Spin) / MainScript.SECONDS_PER_DAY).ToString() + " Standard Galactic Days\n\n";

            i += "Population : " + Population.ToString("N0");
            if (BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
                i += " Million";

            return i;
        }

        public Texture2D getHudTexture()
        {            
            if(HUDTexture == null)
            {                
                Texture2D hudtex = new Texture2D(BodyTexture.width,BodyTexture.height);
                Texture2D overtex = new Texture2D(BodyTexture.width, BodyTexture.height);
                Color[] pix = BodyTexture.GetPixels();
                Color[] pix2 = BodyTexture.GetPixels();
                
                for(int i =0; i < pix.Length ; i++)
                {
                    int x = BodyTexture.width / 2 - (i % BodyTexture.width);
                    int y = BodyTexture.height / 2 - (i / BodyTexture.height);

                    float dist = Mathf.Sqrt( ( (float)x*(float)x + (float)y*(float)y ) );

                    if(Mathf.Abs(dist) > BodyTexture.width/2)
                    {
                        pix[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        pix[i] = Color.Lerp(pix[i], Color.white, 0.5f);
                    }
                }
                
                hudtex.SetPixels(pix);
                hudtex.Apply();

                pix2 = hudtex.GetPixels();
                for (int i = 0; i < pix.Length; i++)
                {
                    if(pix2[i] != new Color(0.0f, 0.0f, 0.0f, 0.0f))
                    {
                        pix2[i] = Color.Lerp(pix[i], Color.white, 0.5f);
                    }
                }

                overtex.SetPixels(pix2);
                overtex.Apply();

                HUDTexture_over = overtex;
                HUDTexture = hudtex;
            }

            return HUDTexture;
        }

        private Vector3 getParentPosition()
        {
            if (ParentBody == null)
                return Vector3.zero;

            return ParentBody.transform.position;
        }

        public void OnMouseOver()
        {
            //Debug.Log("Moused Over " + Name);            
            lastMoused = Time.time;
        }

        public void OnGUI()
        {
            if(Time.time <= lastMoused + tooltipdelay)
                GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y+20, 150, 25), Name);

            //moused = false;
        }
    }
}
