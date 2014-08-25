using UnityEngine;
using System.Collections;

namespace Assets
{
    public class HudMasterBehaviour : MonoBehaviour
    {
        public GameObject shiphud;
        public GameObject routehud;
        public GameObject planethud;

        public static bool ShowingPlanetHUD = false;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {            
            if(Player.Instance.Playstate == Player.PLAYSTATE_MAIN_GAME)
            {
                enableMainHUDs(true);
            }
            else
            {
                enableMainHUDs(false);
            }
        }

        private void enableMainHUDs(bool toggle)
        {
            shiphud.SetActive(toggle);
            routehud.SetActive(toggle);
            planethud.SetActive(toggle);
        }
    }
}
