using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Gunfight
{
    class Player
    {
        #region Variables
        // Struct with variables for the graphics of the player
        private struct graphics2D
        {
            public Rectangle _rect;
            public Texture2D _texture;
            public Texture2D _gunTexture;
            public Rectangle _gunRect;
            public float _gunRotation;
            public Color _color;
        };

        // Const variables
        private const int _speed = 10;

        // Extra variables
        private graphics2D pGraphics;
        private Vector3 position = new Vector3(0, 0, 0);
        private float _gunAngle;
        private int _score = 0;
        private BoundingSphere bSphere;
        private float shootTimer = 0.0f;
        private bool _isPlayer2 = false;
        private Vector2 initialPosition;
        private SoundEffect _gunSound;


        private int SCREEN_BOUNDS_Y;
        #endregion

        #region Constructor
        // Main Construction
        public Player(GraphicsDeviceManager graphics, bool isPlayer2, SoundEffect gunSound)
        {
            pGraphics._color = Color.FloralWhite;
            _isPlayer2 = isPlayer2;
            _gunSound = gunSound;

            // Using 64 since we already know the size of the player sprite
            SCREEN_BOUNDS_Y = graphics.GraphicsDevice.Viewport.Height - 64;
        }
        #endregion

        #region Public Functions
        // Setting up for first use, should be called in Load after we load in the Textures
        public void SetPlayerTextureAndRectangle(Texture2D texture, int x, int y)
        {
            pGraphics._texture = texture;
            pGraphics._rect = new Rectangle(x, y, pGraphics._texture.Width, pGraphics._texture.Height);
            position = new Vector3(pGraphics._rect.X, pGraphics._rect.Y, 0);
            bSphere = new BoundingSphere(position, pGraphics._rect.Width / 2);

            initialPosition = new Vector2(x, y);
        }

        // Should be called right after setting the Player texture, this is separated in 2 different functions for flexibility in case we want to change gun textures midway through duel, for example: with power-ups
        public void SetGunTextureAndRectangle(Texture2D texture)
        {
            pGraphics._gunTexture = texture;
            pGraphics._gunRect = new Rectangle(pGraphics._rect.X + 5, pGraphics._rect.Y, pGraphics._texture.Width, pGraphics._texture.Height);
        }

        // Updates the position of the player based on the direction which is decided by the key presses of the player
        public void UpdatePosition(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    position.Y = MathHelper.Clamp(position.Y - _speed, 0, SCREEN_BOUNDS_Y);
                    break;
                case Direction.Down:
                    position.Y = MathHelper.Clamp(position.Y + _speed, 0, SCREEN_BOUNDS_Y);
                    break;
                default:
                    break;
            }

            pGraphics._rect.Y = (int)position.Y;
            pGraphics._gunRect.Y = (int)position.Y;
            bSphere = new BoundingSphere(position, pGraphics._rect.Width / 2);
        }

        // Update Gun angle
        public void UpdateGun(Direction direction)
        {
            switch (direction)
            {
                case Direction.GunUp:
                    _gunAngle = MathHelper.Clamp(_gunAngle - 3, -30, 30);
                    break;
                case Direction.GunDown:
                    _gunAngle = MathHelper.Clamp(_gunAngle + 3, -30, 30);
                    break;
                default:
                    break;
            }

            pGraphics._gunRotation = MathHelper.ToRadians(_gunAngle);
        }

        // This function spawns the bullets
        public Bullet Shoot(float angle, Texture2D texture)
        {
            if (shootTimer < 0.05)
            {
                Bullet bullet = new Bullet(pGraphics._rect, texture, angle, _isPlayer2);
                shootTimer = 0.5f;
                _gunSound.Play(0.5f, 0, 0);
                return bullet;
            }

            return null;
        }

        // Lower the value of the shooting timer so that we can shoot again
        public void UpdateShootTimer(float gameTime)
        {
            shootTimer = MathHelper.Clamp(shootTimer - gameTime, 0.0f, 0.5f);
        }

        #region Getters
        public Texture2D GetPlayerTexture()
        {
            return pGraphics._texture;
        }

        public Rectangle GetPlayerRectangle()
        {
            return pGraphics._rect;
        }

        public Color GetColor()
        {
            return pGraphics._color;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return bSphere;
        }

        public Texture2D GetGunTexture()
        {
            return pGraphics._gunTexture;
        }

        public Rectangle GetGunRectangle()
        {
            return pGraphics._gunRect;
        }

        public float GetGunRotation()
        {
            return pGraphics._gunRotation;
        }

        public int GetScore()
        {
            return _score;
        }
        #endregion Getters

        // Increases the player score with 1 point
        public void IncreaseScore()
        {
            _score++;
        }

        // Resets the variables within the player
        public void ResetPlayer()
        {
            pGraphics._rect.X = (int)initialPosition.X;
            pGraphics._rect.Y = (int)initialPosition.Y;
            pGraphics._gunRect.X = (int)initialPosition.X;
            pGraphics._gunRect.Y = (int)initialPosition.Y;
            pGraphics._gunRotation = MathHelper.ToRadians(0);
            position = new Vector3(initialPosition.X, initialPosition.Y, 0);
            shootTimer = 0.0f;
        }
        #endregion
    }
}
