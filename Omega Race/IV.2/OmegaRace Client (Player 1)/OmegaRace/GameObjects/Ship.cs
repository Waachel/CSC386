using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

namespace OmegaRace
{
    public class Ship : GameObject
    {
        float maxSpeed;
        float maxForce;
        float rotateSpeed;

        Vec2 localFwd;
        Vec2 respawnPos;
        bool respawning;

        PlayerData owner;
        PlayerManager PlMgr;

        int missileCount = 0;

        public Ship(PlayerData own, PlayerManager pMgr, Azul.Rect screenRect, Azul.Color color)
            : base (GAMEOBJECT_TYPE.SHIP, new Azul.Rect(0, 0, 32, 32), screenRect, TextureCollection.shipTexture, color)
        {
            PhysicBody_Data data = new PhysicBody_Data();
            data.position = new Vec2(screenRect.x, screenRect.y);
            data.size = new Vec2(screenRect.width, screenRect.height);
            data.active = true;
            data.angle = 0;
            data.shape_type = PHYSICBODY_SHAPE_TYPE.SHIP_MANIFOLD;
            CreatePhysicBody(data);

            owner = own;
            PlMgr = pMgr;
            localFwd = new Vec2(1, 0);

            // maxSpeed is m/s
            maxSpeed = 3;
            maxForce = .3f;
            rotateSpeed = 5.0f;

            respawnPos = data.position;
        }

        public PlayerData GetOwner()
        {
            return owner;
        }

        public override void Update()
        {
            if (respawning == false)
            {
                base.Update();
                LimitSpeed();
            }
            else // needed because we can't change the physical properties during collision processing
            {
                pBody.SetPhysicalPosition(respawnPos);
                pBody.SetPixelVelocity(new Vec2(0, 0));
                respawning = false;
            }
            
        }


        public override void Draw()
        {
            base.Draw();
        }

        public void Move(int vertInput)
        {
            if(vertInput < 0)
            {
                vertInput = 0;
            }
            Vec2 heading = pBody.GetBody().GetWorldVector(localFwd);
            pBody.ApplyForce(heading * vertInput * maxForce, GetPixelPosition());
        }

        public void Rotate(int horzInput)
        {
            pBody.SetAngle(pBody.GetAngleDegs() + (horzInput * -rotateSpeed));
        }

        public void LimitSpeed()
        {
            Vec2 shipVel = pBody.GetPhysicalVelocity();
            float magnitude = shipVel.Length();

            if(magnitude > maxSpeed)
            {
                shipVel.Normalize();
                shipVel *= maxSpeed;
                pBody.GetBody().SetLinearVelocity(shipVel);
            }
        }

        public void Respawn(Vec2 v)
        {
            respawning = true;
            respawnPos = v;
        }

        public Vec2 GetHeading()
        {
            return pBody.GetBody().GetWorldVector(localFwd);
        }

        public int getMissileCount()
        {
            return missileCount;
        }
        public void OnHit()
        {
            PlMgr.PlayerKilled(this.owner);
        }

        public override void Accept(GameObject obj)
        {
            obj.VisitShip(this);
        }

        public override void VisitMissile(Missile m)
        {
            CollisionEvent.Action(m, this);
        }
        public override void VisitMine(Mine m)
        {
            CollisionEvent.Action(this, m);
        }

        public override void VisitFence(Fence f)
        {
            CollisionEvent.Action(f, this);
        }
    }
}
