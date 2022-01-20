using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gunfight
{
    class Bullet
    {
        #region Variables
        private struct graphics2DBullet
        {
            public Rectangle _rect;
            public Texture2D _texture;
        };


        private graphics2DBullet bGraphics;
        private BoundingSphere bSphere;
        private bool wasShotFromPlayer2;
        private float _angle;
        private float xTranslation;
        private float yTranslation;
        #endregion

        // Constructor
        public Bullet(Rectangle playerRect, Texture2D texture, float angle, bool isPlayer2)
        {
            // Calculate the translation to be used when moving the object based on the angle from which it was shot
            xTranslation = (float)Math.Cos(angle);
            yTranslation = (float)Math.Sin(angle);

            _angle = angle;

            // Check from player it was shot for the direction of movement
            wasShotFromPlayer2 = isPlayer2;

            bGraphics._texture = texture;
            bGraphics._rect = new Rectangle(playerRect.X + 5, playerRect.Y, bGraphics._texture.Width, bGraphics._texture.Height);
        }

        #region Public Functions
        // Updates the bullet position and bounding sphere
        public void UpdateBullet()
        {
            Vector3 position = new Vector3(bGraphics._rect.X, bGraphics._rect.Y, 0);

            Vector3 direction = new Vector3(xTranslation, yTranslation, 0);
            direction.Normalize();

            direction = wasShotFromPlayer2 ? -direction : direction;

            position += direction * 80;

            bGraphics._rect.X = (int)position.X;
            bGraphics._rect.Y = (int)position.Y;

            bSphere = new BoundingSphere(position, bGraphics._rect.Width / 2);
        }

        #region Getters
        public Texture2D GetTexture()
        {
            return bGraphics._texture;
        }

        public Rectangle GetRectangle()
        {
            return bGraphics._rect;
        }

        public float GetRotation()
        {
            return _angle;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return bSphere;
        }
        #endregion Getters
        #endregion
    }
}
