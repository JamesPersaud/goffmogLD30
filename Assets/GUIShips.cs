using UnityEngine;
using System.Collections;

namespace Assets
{
    public class GUIShips : MonoBehaviour
    {
        public float lastMoused;

        public GUITexture GuiTexture;
        bool lastover = false;

        private static Texture normaltexture = null;
        public static Texture NormalTexture
        {
            get
            {
                if (normaltexture == null)
                {
                    normaltexture = Resources.Load<Texture>("shiphud");
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
                    overtexture = Resources.Load<Texture>("shiphud_over");
                }

                return overtexture;
            }
        }

        // Use this for initialization
        void Start()
        {
            GuiTexture = GetComponent<GUITexture>();
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
            }
        }

        public void OnMouseDown()
        {
            if (lastover && !Player.Instance.ShowingModal && Player.Instance.Buyingstate == Player.BUYINGSTATE_NONE)
            {
                Player.Instance.Buyingstate = Player.BUYINGSTATE_SHIP;
            }
        }

        public void OnGUI()
        {
            if (Time.time <= lastMoused + Helpers.tooltipdelay)
                GUI.Box(new Rect(Input.mousePosition.x +50, Screen.height - Input.mousePosition.y-20, 200, 25), "View and purchase ships");

            //moused = false;
        }
    }
}
