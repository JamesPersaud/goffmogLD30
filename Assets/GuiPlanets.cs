using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets
{
    public class GuiPlanets : MonoBehaviour
    {
        public float lastMoused;

        public GUITexture GuiTexture;
        public List<GuiPlanet> GuiPlanetList;
        bool lastover = false;

        private static Texture normaltexture = null;
        public static Texture NormalTexture
        {
            get
            {
                if (normaltexture == null)
                {
                    normaltexture = Resources.Load<Texture>("planethud");
                }

                return normaltexture;
            }
        }

        private static Texture overtexture = null;
        public static Texture OverTexture
        {
            get
            {
                if (overtexture == null)
                {
                    overtexture = Resources.Load<Texture>("planethud_over");
                }

                return overtexture;
            }
        }

        // Use this for initialization
        void Start()
        {
            GuiTexture = GetComponent<GUITexture>();
            GuiPlanetList = new List<GuiPlanet>();
        }

        // Update is called once per frame
        void Update()
        {            
            if (!Player.Instance.ShowingModal)
            {
                bool mouseover = false;
                if (Input.mousePosition.x >= guiTexture.pixelInset.xMin
                    && Input.mousePosition.x <= guiTexture.pixelInset.xMax
                    && Input.mousePosition.y >= guiTexture.pixelInset.yMin
                    && Input.mousePosition.y <= guiTexture.pixelInset.yMax)
                {
                    mouseover = true;
                    lastMoused = Time.time;
                }

                if (mouseover && !lastover)
                {
                    GuiTexture.texture = OverTexture;
                }
                else if (!mouseover && lastover)
                {
                    GuiTexture.texture = NormalTexture;
                }

                lastover = mouseover;

                if(HudMasterBehaviour.ShowingPlanetHUD)
                {
                    // show all the planets (inner and outer) as icons to the right
                    if (GuiPlanetList.Count == 0)
                    {
                        float padding = 16f;
                        float left = this.GuiTexture.pixelInset.xMax + padding;
                        float bottom = 0;

                        //create planet gui icons
                        foreach(Body planet in Galaxy.Instance.CurrentStarSystem.Bodies)
                        {
                            GameObject o = new GameObject();
                            GUITexture gt = o.AddComponent<GUITexture>();
                            GuiPlanet gp = o.AddComponent<GuiPlanet>();

                            gt.gameObject.name = "GuiPlanet_" + planet.Name;                            
                            gp.GuiTexture = gt;
                            gp.AssociatedBody = planet;          

                            gt.texture = planet.HUDTexture;
                            gt.transform.position = Vector3.zero;
                            gt.transform.localScale = Vector3.zero;                            

                            float size = 64;
                            if (planet.BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
                                size = 32;                            
                            
                            gp.targetx = left;
                            gp.targety = 0;
                            //gt.pixelInset = new Rect(left, 0, size, size);
                            gt.pixelInset = new Rect(64, 0, size, size);

                            GuiPlanetList.Add(gp);                            

                            bottom = size + padding/2;

                            foreach(Body moon in planet.childBodies)
                            {
                                GameObject mo = new GameObject();
                                GUITexture mgt = mo.AddComponent<GUITexture>();
                                GuiPlanet mgp = mo.AddComponent<GuiPlanet>();

                                mgt.gameObject.name = "GuiPlanet_" + moon.Name;
                                mgp.GuiTexture = mgt;
                                mgp.AssociatedBody = moon;

                                mgt.texture = moon.HUDTexture;
                                mgt.transform.position = Vector3.zero;
                                mgt.transform.localScale = Vector3.zero;

                                size = 16;      
                                
                                mgp.targetx = left;
                                mgp.targety = bottom;
                                mgt.pixelInset = new Rect(64, 0, size, size);

                                GuiPlanetList.Add(mgp);
                                bottom += 16 + padding/2;
                            }

                            left += gt.pixelInset.width + padding;
                            bottom = 0;
                        }
                    }

                    foreach (GuiPlanet p in GuiPlanetList)
                    {
                        p.gameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (GuiPlanet p in GuiPlanetList)
                    {
                        p.gameObject.SetActive(false);
                        p.guiTexture.pixelInset = new Rect(64, 0, p.guiTexture.pixelInset.width, p.guiTexture.pixelInset.height);
                    }
                }
            }
        }


        public void OnMouseDown()
        {
            if (lastover && !Player.Instance.ShowingModal)
            {                
                HudMasterBehaviour.ShowingPlanetHUD = !HudMasterBehaviour.ShowingPlanetHUD;
            }
        }

        public void OnGUI()
        {
            if (Time.time <= lastMoused + Helpers.tooltipdelay)
                GUI.Box(new Rect(Input.mousePosition.x + 50, Screen.height - Input.mousePosition.y-20, 200, 25), "View and select planets.");

            //moused = false;
        }
    }
}
