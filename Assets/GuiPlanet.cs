using UnityEngine;
using System.Collections;

namespace Assets
{
    public class GuiPlanet : MonoBehaviour
    {
        public float movespeed = 8.0f;

        public float lastMoused;        

        public float targetx;
        public float targety;

        public GUITexture GuiTexture;
        bool lastover = false;
        
        public bool InPosition
        {
            get
            {
                return (guiTexture.pixelInset.x == targetx
                    && guiTexture.pixelInset.y == targety);
            }
        }

        public Texture NormalTexture
        {
            get
            {
                return AssociatedBody.HUDTexture;
            }
        }
        public Texture OverTexture
        {
            get
            {
                return AssociatedBody.HUDTexture_over;
            }
        }

        public Body AssociatedBody = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!InPosition)
            {
                float moveby = movespeed;

                if(guiTexture.pixelInset.x < targetx)
                {                    
                    if (targetx - guiTexture.pixelInset.x < movespeed)
                        moveby = targetx - guiTexture.pixelInset.x;

                    guiTexture.pixelInset = new Rect(guiTexture.pixelInset.x + moveby, guiTexture.pixelInset.y,guiTexture.pixelInset.width, guiTexture.pixelInset.height);
                }
                else if (guiTexture.pixelInset.y < targety)
                {
                    if (targety - guiTexture.pixelInset.y < movespeed)
                        moveby = targety - guiTexture.pixelInset.y;

                    guiTexture.pixelInset = new Rect(guiTexture.pixelInset.x, guiTexture.pixelInset.y + moveby,guiTexture.pixelInset.width, guiTexture.pixelInset.height);
                }
            }
            else
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
        }        

        public void OnMouseDown()
        {
            if (lastover && !Player.Instance.ShowingModal && Player.Instance.Buyingstate == Player.BUYINGSTATE_NONE)
            {
                CameraBehaviour cb = Camera.mainCamera.GetComponent<CameraBehaviour>();
                cb.StartPanningToBody(this.AssociatedBody);

                Player.Instance.ShowingModal = true;
                Player.Instance.ModalTitle = "Analyzing Object";
                Player.Instance.ModalMessage = AssociatedBody.GetInfo();
            }        
            else if(Player.Instance.Buyingstate == Player.BUYINGSTATE_ROUTE_CHOOSESOURCE)
            {
                Player.Instance.RoutePlanet1 = AssociatedBody;
                Player.Instance.Buyingstate = Player.BUYINGSTATE_ROUTE_CHOOSETARGET;
            }
            else if (Player.Instance.Buyingstate == Player.BUYINGSTATE_ROUTE_CHOOSETARGET)
            {
                Player.Instance.RoutePlanet2 = AssociatedBody;
                Player.Instance.Buyingstate = Player.BUYINGSTATE_ROUTE_CONFIRM;
            }
        }

        public void OnGUI()
        {
            if (Time.time <= lastMoused + Helpers.tooltipdelay)
                GUI.Box(new Rect(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y - 20, 200, 25), AssociatedBody.Name);            
        }
    }
}
