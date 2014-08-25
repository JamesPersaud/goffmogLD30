using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets
{
    public class Player
    {        
        public const int PLAYSTATE_MAIN_MENU = 1;
        public const int PLAYSTATE_MAIN_GAME = 2;

        public const int BUYINGSTATE_NONE = 0;
        
        public const int BUYINGSTATE_ROUTE_CHOOSESOURCE = 1;
        public const int BUYINGSTATE_ROUTE_CHOOSETARGET = 2;
        public const int BUYINGSTATE_SHIP = 3;
        public const int BUYINGSTATE_ROUTE_CHOOSESHIP = 4;
        public const int BUYINGSTATE_ROUTE_CONFIRM = 5;

        public int Buyingstate = BUYINGSTATE_NONE;

        public Body RoutePlanet1;
        public Body RoutePlanet2;
        public Ship RouteShip;

        public bool ShowingModal = false;
        public string ModalMessage = "";
        public string ModalTitle = "";

        public int Playstate = 1;

        public string Name = "Goffmog";
        public string Company = "Goffmog & co";
        public string Motto = "Connecting Worlds";

        public float Credits = 10000f;
        public float Loan = 10000f;
        public float Expenditure;

        public float lastrepayment = 0;

        public float repayment = 100;

        public List<Ship> Ships;

        public float Income;
        

        public void EndMonth()
        {            
            if (Loan > 0)
            {                
                Credits -= Mathf.Min(new float[] { Loan, repayment });                            
                Loan -= Mathf.Min(new float[] { Loan, repayment });
                lastrepayment = Mathf.Min(new float[] { Loan, repayment });
            }
            else
            {
                lastrepayment = 0;
            }                        

            //send end of month message
            string summary = Galaxy.Instance.getStardateString() + " End of Galactic Standard Month\n\n";
            summary += "Income      : " + Income.ToString("N0") + " CR\n";
            summary += "Repayments      : " + lastrepayment.ToString("N0") + " CR\n";
            summary += "Expenditure : " + Expenditure.ToString("N0") + " CR\n\n";
            summary += "Monthly Balance : " + (Income - Expenditure - lastrepayment).ToString("N0") + " CR";

            Messaging.Instance.PushMessage(summary);

            Expenditure = 0;
            Income = 0;
        }

        private Player()
        {
            Ships = new List<Ship>();
        }

#region singleton
        private static Player instance;
        public static Player Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Player();
                }
                return instance;
            }            
        }
#endregion
    }
}
