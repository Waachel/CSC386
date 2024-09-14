using OmegaRace.Data_Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace.Managers.MessageManager
{
    public class MessageManager
    {
        private readonly MessagePool<PlayerMovementMessage> playerMovementMessagePool;
        private readonly MessagePool<MissileEvent> missileEventMessagePool;
        private readonly MessagePool<MineEvent> mineEventMessagePool;
        private readonly MessagePool<MissileMessage> missileMessagePool;
        private readonly MessagePool<MineMessage> mineMessagePool;
        private readonly MessagePool<UpdatePlayerMovementMessage> updatePlayerMovementMessagePool;
        private readonly MessagePool<UpdateMissileMessage> updateMissileMessagePool;
        private readonly MessagePool<UpdateMineMessage> updateMineMessagePool;
        private readonly MessagePool<CollisionsList> collisionsListMessagePool;
        private readonly MessagePool<CollisionsFenceMissile> collisionsFenceMissileMessagePool;
        private readonly MessagePool<CollisionsFenceShip> collisionsFenceShipMessagePool;
        private readonly MessagePool<CollisionsMissileShip> collisionsMissileShipMessagePool;
        private readonly MessagePool<CollisionsMissileFencePost> collisionsMissileFencePostMessagePool;
        private readonly MessagePool<CollisionsShipMine> collisionsShipMineMessagePool;
        private readonly MessagePool<TimeReplyMessage> timeReplyMessagePool;
        private readonly MessagePool<TimeRequestMessage> timeRequestMessagePool;

        public MessageManager(int maxPoolSize)
        {
            playerMovementMessagePool = new MessagePool<PlayerMovementMessage>(maxPoolSize);
            missileEventMessagePool = new MessagePool<MissileEvent>(maxPoolSize);
            mineEventMessagePool = new MessagePool<MineEvent>(maxPoolSize);
            missileMessagePool = new MessagePool<MissileMessage>(maxPoolSize);
            mineMessagePool = new MessagePool<MineMessage>(maxPoolSize);
            updatePlayerMovementMessagePool = new MessagePool<UpdatePlayerMovementMessage>(maxPoolSize);
            updateMissileMessagePool = new MessagePool<UpdateMissileMessage>(maxPoolSize);
            updateMineMessagePool = new MessagePool<UpdateMineMessage>(maxPoolSize);
            collisionsListMessagePool = new MessagePool<CollisionsList>(maxPoolSize);
            collisionsFenceMissileMessagePool = new MessagePool<CollisionsFenceMissile>(maxPoolSize);
            collisionsFenceShipMessagePool = new MessagePool<CollisionsFenceShip>(maxPoolSize);
            collisionsMissileShipMessagePool = new MessagePool<CollisionsMissileShip>(maxPoolSize);
            collisionsMissileFencePostMessagePool = new MessagePool<CollisionsMissileFencePost>(maxPoolSize);
            collisionsShipMineMessagePool = new MessagePool<CollisionsShipMine>(maxPoolSize);
            timeReplyMessagePool = new MessagePool<TimeReplyMessage>(maxPoolSize);
            timeRequestMessagePool = new MessagePool<TimeRequestMessage>(maxPoolSize);
        }

        public PlayerMovementMessage GetPlayerMovementMessage()
        {
            return playerMovementMessagePool.GetMessage();
        }

        public void ReleasePlayerMovementMessage(PlayerMovementMessage message)
        {
            playerMovementMessagePool.ReleaseMessage(message);
        }

        public MissileEvent GetMissileEvent()
        {
            return missileEventMessagePool.GetMessage();
        }

        public void ReleaseMissileEvent(MissileEvent message)
        {
            missileEventMessagePool.ReleaseMessage(message);
        }

        public MineEvent GetMineEvent()
        {
            return mineEventMessagePool.GetMessage();
        }

        public void ReleaseMineEvent(MineEvent message)
        {
            mineEventMessagePool.ReleaseMessage(message);
        }

        public MissileMessage GetMissileMessage()
        {
            return missileMessagePool.GetMessage();
        }

        public void ReleaseMissileMessage(MissileMessage message)
        {
            missileMessagePool.ReleaseMessage(message);
        }

        public MineMessage GetMineMessage()
        {
            return mineMessagePool.GetMessage();
        }

        public void ReleaseMineMessage(MineMessage message)
        {
            mineMessagePool.ReleaseMessage(message);
        }

        public UpdatePlayerMovementMessage GetUpdatePlayerMovementMessage()
        {
            return updatePlayerMovementMessagePool.GetMessage();
        }

        public void ReleaseUpdatePlayerMovementMessage(UpdatePlayerMovementMessage message)
        {
            updatePlayerMovementMessagePool.ReleaseMessage(message);
        }

        public UpdateMissileMessage GetUpdateMissileMessage()
        {
            return updateMissileMessagePool.GetMessage();
        }

        public void ReleaseUpdateMissileMessage(UpdateMissileMessage message)
        {
            updateMissileMessagePool.ReleaseMessage(message);
        }

        public UpdateMineMessage GetUpdateMineMessage()
        {
            return updateMineMessagePool.GetMessage();
        }

        public void ReleaseUpdateMineMessage(UpdateMineMessage message)
        {
            updateMineMessagePool.ReleaseMessage(message);
        }

        public CollisionsList GetCollisionsList()
        {
            return collisionsListMessagePool.GetMessage();
        }

        public void ReleaseCollisionsList(CollisionsList message)
        {
            collisionsListMessagePool.ReleaseMessage(message);
        }

        public CollisionsFenceMissile GetCollisionsFenceMissile()
        {
            return collisionsFenceMissileMessagePool.GetMessage();
        }

        public void ReleaseCollisionsFenceMissile(CollisionsFenceMissile message)
        {
            collisionsFenceMissileMessagePool.ReleaseMessage(message);
        }

        public CollisionsFenceShip GetCollisionsFenceShip()
        {
            return collisionsFenceShipMessagePool.GetMessage();
        }

        public void ReleaseCollisionsFenceShip(CollisionsFenceShip message)
        {
            collisionsFenceShipMessagePool.ReleaseMessage(message);
        }

        public CollisionsMissileShip GetCollisionsMissileShip()
        {
            return collisionsMissileShipMessagePool.GetMessage();
        }

        public void ReleaseCollisionsMissileShip(CollisionsMissileShip message)
        {
            collisionsMissileShipMessagePool.ReleaseMessage(message);
        }

        public CollisionsMissileFencePost GetCollisionsMissileFencePost()
        {
            return collisionsMissileFencePostMessagePool.GetMessage();
        }

        public void ReleaseCollisionsMissileFencePost(CollisionsMissileFencePost message)
        {
            collisionsMissileFencePostMessagePool.ReleaseMessage(message);
        }

        public CollisionsShipMine GetCollisionsShipMine()
        {
            return collisionsShipMineMessagePool.GetMessage();
        }

        public void ReleaseCollisionsShipMine(CollisionsShipMine message)
        {
            collisionsShipMineMessagePool.ReleaseMessage(message);
        }

        public TimeReplyMessage GetTimeReplyMessage()
        {
            return timeReplyMessagePool.GetMessage();
        }

        public void ReleaseTimeReplyMessage(TimeReplyMessage message)
        {
            timeReplyMessagePool.ReleaseMessage(message);
        }

        public TimeRequestMessage GetTimeRequestMessage()
        {
            return timeRequestMessagePool.GetMessage();
        }

        public void ReleaseTimeRequestMessage(TimeRequestMessage message)
        {
            timeRequestMessagePool.ReleaseMessage(message);
        }
    }
}
