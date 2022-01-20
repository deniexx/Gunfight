using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Gunfight
{
    // Gunfight game
    // Coded by Denis Dimitrov
    // 20.01.2022
    public class Game1 : Game
    {
        #region Variables
        // Player variables and extra textures
        private Player player1;
        private Player player2;
        private Texture2D playerText;
        private Texture2D gunText;
        private Texture2D bulletText;
        private Texture2D background;
        private Texture2D mainMenu;
        private Texture2D controls;

        // Lists
        private List<Bullet> p1Bullets = new List<Bullet> { }; // Here we store the bullets of player1
        private List<Bullet> p2Bullets = new List<Bullet> { }; // Here we store the bullets of player2
        private List<string> mainMenuOptions = new List<string> { "Play", "Controls" }; // Here are the options to choose from the main menu

        // The game state, used in the state machine
        private GameState GAME_STATE;

        // Extra variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private SpriteFont _bigFont;
        private SoundEffect gunSound;
        private Song backgroundMusic;

        private float _keyTimer = 0.3f;
        private float _remainingTime = 5.0f;
        private float _defaultTime = 60.0f;
        private int _selectedMenuIndex = 0;
        #endregion

        #region MonoGame Methods
        // Game Constructor where we initialize the variables and set things like the resolution, capped fps and MediaPlayer
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / 30d);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GAME_STATE = GameState.MainMenu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loading content and initializing the 2 players
            background = Content.Load<Texture2D>("sand");
            mainMenu = Content.Load<Texture2D>("mainmenu");
            controls = Content.Load<Texture2D>("controls");
            playerText = Content.Load<Texture2D>("Cowboy");
            gunText = Content.Load<Texture2D>("Revolver");
            bulletText = Content.Load<Texture2D>("Bullet");
            _font = Content.Load<SpriteFont>("Font");
            _bigFont = Content.Load<SpriteFont>("BigFont");
            gunSound = Content.Load<SoundEffect>("gunShot");
            backgroundMusic = Content.Load<Song>("papada");

            MediaPlayer.Play(backgroundMusic);

            player1 = new Player(_graphics, false, gunSound);
            player1.SetPlayerTextureAndRectangle(playerText, 0, 0);
            player1.SetGunTextureAndRectangle(gunText);

            player2 = new Player(_graphics, true, gunSound);
            player2.SetPlayerTextureAndRectangle(playerText, GraphicsDevice.Viewport.Width - 64, 0);
            player2.SetGunTextureAndRectangle(gunText);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();

            // State Machine for the Update Method
            switch(GAME_STATE)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(keyboard, gameTime);
                    break;
                case GameState.Controls:
                    UpdateControls(keyboard);
                    break;
                case GameState.GameLoop:
                    UpdateGameLoop(gameTime, keyboard);
                    break;
                case GameState.GameEndScreen:
                    UpdateGameEndScreen(keyboard);
                    break;

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // State Machine for the Draw Method
            switch (GAME_STATE)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.Controls:
                    DrawControls();
                    break;
                case GameState.GameLoop:
                    DrawGameLoop();
                    break;
                case GameState.GameEndScreen:
                    DrawGameEndScreen();
                    break;

            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion MonoGame Methods

        #region Custom Methods
        #region State Machine Methods
        protected void UpdateMainMenu(KeyboardState keyboard, GameTime gameTime)
        {
            _keyTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Down) && _keyTimer < 0.05f)
            {
                _selectedMenuIndex = _selectedMenuIndex + 1 > mainMenuOptions.Count - 1 ? 0 : _selectedMenuIndex + 1;
                _keyTimer = 0.3f;
            }

            else if (keyboard.IsKeyDown(Keys.Up) && _keyTimer < 0.05f)
            {
                _selectedMenuIndex = _selectedMenuIndex - 1 < 0 ? 1 : _selectedMenuIndex - 1;
                _keyTimer = 0.3f;
            }

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                switch(_selectedMenuIndex)
                {
                    case 0:
                        ResetGame();
                        ClearBullets();
                        GAME_STATE = GameState.GameLoop;
                        break;
                    case 1:
                        GAME_STATE = GameState.Controls;
                        break;
                }
            }
        }

        protected void DrawMainMenu()
        {
            _spriteBatch.Draw(mainMenu, Vector2.Zero, Color.White);

            float textPositionY = ((GraphicsDevice.Viewport.Height / 2) * 1.5f) - 150;
            for (int i = 0; i < mainMenuOptions.Count; i++)
            {
                if (i == _selectedMenuIndex)
                    _spriteBatch.DrawString(_bigFont, mainMenuOptions[i], new Vector2(50, textPositionY), Color.Red);
                else
                    _spriteBatch.DrawString(_font, mainMenuOptions[i], new Vector2(50, textPositionY), Color.Black);

                textPositionY += 50;
            }

            _spriteBatch.DrawString(_font, "You can press Escape at any time to QUIT", new Vector2(50, textPositionY + 50), Color.Black);
        }

        protected void UpdateControls(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Back))
            {
                GAME_STATE = GameState.MainMenu;
            }
        }

        protected void DrawControls()
        {
            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.Draw(controls, Vector2.Zero, Color.White);
            _spriteBatch.DrawString(_font, "Press Backspace to go back", new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString("Press Backspace to go back").X / 2, GraphicsDevice.Viewport.Height - 50), Color.Black);
        }

        protected void UpdateGameLoop(GameTime gameTime, KeyboardState keyboard)
        {
            if (_remainingTime <= 0.5f)
            {
                ClearBullets();
                GAME_STATE = GameState.GameEndScreen;
            }

            CleanUpBullets();

            CheckForCollision();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _remainingTime -= delta;

            player1.UpdateShootTimer(delta);
            player2.UpdateShootTimer(delta);

            ProcessP1Input(keyboard);
            ProcessP2Input(keyboard);

            foreach (Bullet bullet in p1Bullets)
            {
                if (bullet != null)
                    bullet.UpdateBullet();
            }

            foreach (Bullet bullet in p2Bullets)
            {
                if (bullet != null)
                    bullet.UpdateBullet();
            }
        }

        protected void DrawGameLoop()
        {
            CleanUpBullets();
            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.Draw(player1.GetPlayerTexture(), player1.GetPlayerRectangle(), null, player1.GetColor(), 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            _spriteBatch.Draw(player1.GetGunTexture(), player1.GetGunRectangle(), null, player1.GetColor(), player1.GetGunRotation(), Vector2.Zero, SpriteEffects.None, 0);
            _spriteBatch.Draw(player2.GetPlayerTexture(), player2.GetPlayerRectangle(), null, player2.GetColor(), 0.0f, Vector2.Zero, SpriteEffects.None, 0);
            _spriteBatch.Draw(player2.GetGunTexture(), player2.GetGunRectangle(), null, player2.GetColor(), player2.GetGunRotation(), Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            _spriteBatch.DrawString(_font, ((int)_remainingTime).ToString(), new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString(((int)_remainingTime).ToString()).X, 10), Color.Black);
            _spriteBatch.DrawString(_font, "Score: " + player1.GetScore(), new Vector2(((GraphicsDevice.Viewport.Width / 2) / 2) - _font.MeasureString("Score: " + player1.GetScore()).X, GraphicsDevice.Viewport.Height - 50), Color.Black);
            _spriteBatch.DrawString(_font, "Score: " + player2.GetScore(), new Vector2(((GraphicsDevice.Viewport.Width / 2) * 1.5f), GraphicsDevice.Viewport.Height - 50), Color.Black);

            foreach (Bullet bullet in p1Bullets)
            {
                if (bullet != null)
                    _spriteBatch.Draw(bullet.GetTexture(), bullet.GetRectangle(), null, Color.DarkRed, bullet.GetRotation(), Vector2.Zero, SpriteEffects.None, 0);
            }

            foreach (Bullet bullet in p2Bullets)
            {
                if (bullet != null)
                    _spriteBatch.Draw(bullet.GetTexture(), bullet.GetRectangle(), null, Color.DarkRed, bullet.GetRotation(), Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
        }

        protected void UpdateGameEndScreen(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Enter))
            {
                ResetGame();
                ClearBullets();
                GAME_STATE = GameState.GameLoop;
            }
        }

        protected void DrawGameEndScreen()
        {
            string winner = player1.GetScore() > player2.GetScore() ? "Player 1 WINS!" : "Player 2 WINS!";
            if (player1.GetScore() == player2.GetScore()) winner = "Draw!!!";
            _spriteBatch.DrawString(_font, winner, new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString(winner).X / 2, GraphicsDevice.Viewport.Height / 2 - 50), Color.Black);
            _spriteBatch.DrawString(_font, "Press Enter to play again!", new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString("Press Enter to play again!").X / 2, GraphicsDevice.Viewport.Height / 2), Color.Black);
            _spriteBatch.DrawString(_font, "Press Escape to quit!", new Vector2(GraphicsDevice.Viewport.Width / 2 - _font.MeasureString("Press Escape to quit!").X / 2, GraphicsDevice.Viewport.Height / 2 + 50), Color.Black);
        }
        #endregion State Machine Methods

        protected void ProcessP1Input(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.W))
                player1.UpdatePosition(Direction.Up);
            else if (keyboard.IsKeyDown(Keys.S))
                player1.UpdatePosition(Direction.Down);

            if (keyboard.IsKeyDown(Keys.A))
                player1.UpdateGun(Direction.GunUp);
            else if (keyboard.IsKeyDown(Keys.D))
                player1.UpdateGun(Direction.GunDown);

            if (keyboard.IsKeyDown(Keys.LeftShift))
                p1Bullets.Add(player1.Shoot(player1.GetGunRotation(), bulletText));
        }

        protected void ProcessP2Input(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Up))
                player2.UpdatePosition(Direction.Up);
            else if (keyboard.IsKeyDown(Keys.Down))
                player2.UpdatePosition(Direction.Down);

            if (keyboard.IsKeyDown(Keys.Right))
                player2.UpdateGun(Direction.GunUp);
            else if (keyboard.IsKeyDown(Keys.Left))
                player2.UpdateGun(Direction.GunDown);

            if (keyboard.IsKeyDown(Keys.RightShift))
                p2Bullets.Add(player2.Shoot(player2.GetGunRotation(), bulletText));
        }

        protected void CleanUpBullets()
        {
            for (int i = 0; i < p1Bullets.Count; i++)
            {
                if (p1Bullets[i] == null)
                {
                    p1Bullets.Remove(p1Bullets[i]);
                    return;
                }
                    
                if (p1Bullets[i].GetRectangle().X < 0 || p1Bullets[i].GetRectangle().X > GraphicsDevice.Viewport.Width + 40)
                    p1Bullets.Remove(p1Bullets[i]);
            }

            for (int i = 0; i < p2Bullets.Count; i++)
            {
                if (p2Bullets[i] == null)
                {
                    p2Bullets.Remove(p2Bullets[i]);
                    return;
                }

                if (p2Bullets[i].GetRectangle().X < 0 || p2Bullets[i].GetRectangle().X > GraphicsDevice.Viewport.Width + 40)
                    p2Bullets.Remove(p2Bullets[i]);
            }
        }

        protected void CheckForCollision()
        {
            for (int i = 0; i < p1Bullets.Count; i++)
            {
                if (p1Bullets[i] != null)
                {
                    if (p1Bullets[i].GetBoundingSphere().Intersects(player2.GetBoundingSphere()))
                    {
                        player2.ResetPlayer();
                        player1.IncreaseScore();
                        p1Bullets.Remove(p1Bullets[i]);
                    }
                }
            }

            for (int i = 0; i < p2Bullets.Count; i++)
            {
                if (p2Bullets[i] != null)
                {
                    if (p2Bullets[i].GetBoundingSphere().Intersects(player1.GetBoundingSphere()))
                    {
                        player1.ResetPlayer();
                        player2.IncreaseScore();
                        p2Bullets.Remove(p2Bullets[i]);
                    }
                }
            }
        }

        protected void ResetGame()
        {
            player1.ResetPlayer();
            player2.ResetPlayer();
            _remainingTime = _defaultTime;
            ClearBullets();
        }

        protected void ClearBullets()
        {
            p1Bullets.Clear();
            p2Bullets.Clear();
        }
        #endregion
    }
}
