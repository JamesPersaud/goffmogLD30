using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Ship : MonoBehaviour
    {
        public const float SPEED = 0.2f;
        public const float TURN_SPEED = 1.4f;

        public const float MAINTENANCE_COST = 20;
        public const float MAINTENANCE_FREQUENCY = 10;
        public float timeUntilMaint = MAINTENANCE_FREQUENCY;

        public const float PASSENGER_CAPACITY = 1000;

        public const int SHIPTYPE_PASSENGER = 1;

        public bool InStorage = true;        

        public bool headingBack = false;
        public bool startedMission = false;

        public Body Source;
        public Body Target;
        public int ShipType;
        public string Name;

        public int Passengers = 0;

        public float currentAngle;
        public float targetAngle;
        public float angleMod;

        public void Start()
        {
            gameObject.name = Name;
        }

        public void Update()
        {
            timeUntilMaint -= Time.deltaTime;
            if(timeUntilMaint < 0)
            {
                Player.Instance.Expenditure += MAINTENANCE_COST;
                Player.Instance.Credits -= MAINTENANCE_COST;

                timeUntilMaint = MAINTENANCE_FREQUENCY;
            }

            Vector3 destination;
            float radius = 0;
            if (headingBack)
            {
                destination = Source.transform.position;
                radius = Source.BodyRadius;
            }
            else
            {
                destination = Target.transform.position;            
                radius = Target.BodyRadius;
            }

            float turnamount = TURN_SPEED * Time.deltaTime;
            

            float dx = destination.x - transform.position.x;
            float dy = destination.z - transform.position.z;
            targetAngle = Mathf.Atan2(dy, dx) * 180 / Mathf.PI;
            currentAngle = transform.rotation.eulerAngles.y;

            targetAngle *= -1;
            targetAngle -= 90;

            if (currentAngle > 360) currentAngle -= 360;
            if (currentAngle < 0) currentAngle += 360;

            if (targetAngle > 360) targetAngle -= 360;
            if (targetAngle < 0) targetAngle += 360;                       

            if (targetAngle > currentAngle)
            {                
                if (Mathf.Abs(targetAngle - currentAngle) > 180)
                {
                    turnamount *= -1;
                }                    
            }

            if (targetAngle < currentAngle)
            {                
                if (Mathf.Abs(targetAngle - currentAngle) <= 180)
                {
                    turnamount *= -1;
                }                    
            }            

            transform.RotateAroundLocal(new Vector3(0, 1, 0), turnamount);            

            transform.Translate(Vector3.up * SPEED);

            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            if(Vector3.Distance(destination,transform.position) <= radius)
            {
                ReachWaypoint();
            }
        }

        public void ReachWaypoint()
        {            
            headingBack = !headingBack;

            Body b;
            Body oldb;

            if (headingBack)
            {
                b = Target;
                oldb = Source;
            }
            else
            {
                b = Source;
                oldb = Target;
            }

            int oldpassengers = Passengers;

            if (b.BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
            {
                Passengers = Helpers.GalacticRandom.Next(100, 1000);
            }
            else if (b.BodyType == Galaxy.BODY_TYPE_MOON)
            {
                Passengers = Helpers.GalacticRandom.Next(1, 100);
            }
            else if (b.BodyType == Galaxy.BODY_TYPE_OUTER_PLANET)
            {
                Passengers = 0; ;
            }

            float income = 0;

            if (b.BodyType != Galaxy.BODY_TYPE_OUTER_PLANET && oldb.BodyType != Galaxy.BODY_TYPE_OUTER_PLANET)
            {
                income = Passengers * 0.02f * (Mathf.Abs(Source.Orbit - Target.Orbit) *5);
                Messaging.Instance.PushMessage(Name + ": \n\nDropped off " + oldpassengers.ToString() + " passengers at " + oldb.Name + "\nPicked up " + Passengers.ToString() + " passengers at " + b.Name + "\n\nIncome : " + income.ToString("N0") + " CR");
            }
            else
            {
                Messaging.Instance.PushMessage(Name + ": \n\nDropped off " + oldpassengers.ToString() + " passengers at " + oldb.Name + "\nPicked up " + Passengers.ToString() + " passengers at " + b.Name + "\n\nIncome : " + income.ToString("N0") + " CR\n\nGas giants aren't too popular this time of year. I hear habitable worlds are all the rage with passengers.");
            }
                        
            Player.Instance.Credits += income;
            Player.Instance.Income += income;
        }

        public void startMission()
        {
            if(!startedMission)
            {
                gameObject.transform.position = Source.gameObject.transform.position;
                headingBack = false;
                InStorage = false;

                if(Source.BodyType == Galaxy.BODY_TYPE_INNER_PLANET)
                {
                    Passengers = Helpers.GalacticRandom.Next(100,1000);
                }
                else if (Source.BodyType == Galaxy.BODY_TYPE_MOON)
                {
                    Passengers = Helpers.GalacticRandom.Next(1, 100);
                }
                else if (Source.BodyType == Galaxy.BODY_TYPE_OUTER_PLANET)
                {
                    Passengers = 0;
                }

                Messaging.Instance.PushMessage(Name + " picked up " + Passengers.ToString() + " at " + Source.Name);
            }
            else
            {
                headingBack = true;
                Passengers = 0;
            }            
        }

        public static Ship newShip(int t, Body source, Body target)
        {
            GameObject o = new GameObject();
            Ship s = o.AddComponent<Ship>();            
            
            ConeMaker.makeCone(o);
            MeshRenderer rend = o.AddComponent<MeshRenderer>();
            BoxCollider collider = o.AddComponent <BoxCollider>();

            Helpers.AddTrails(o, null);

            o.transform.position = Vector3.zero;

            o.transform.Rotate(Vector3.left, 90);

            rend.material = new Material(Shader.Find("Diffuse"));
            rend.material.mainTexture = Resources.Load<Texture>("redship");

            s.ShipType = t;
            s.Source = source;
            s.Target = target;            
            o.SetActive(false);

            return s;
        }
    }
}
