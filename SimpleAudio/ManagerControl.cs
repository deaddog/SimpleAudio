using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonifyControls;

using DeadDog.Audio;
using DeadDog.Audio.Libraries;
using DeadDog.Audio.Scan;

using DeadDog.GUI;

namespace SimpleAudio
{
    public class ManagerControl : XNAControls.Forms.GraphicsDeviceControl
    {
        private ControlManager manager;

        private TextBox search;
        private ListBox<Track> list;

        public ManagerControl()
            : base("Content")
        {
            manager = new MoonifyControls.ControlManager(this.Handle, Services, GraphicsDevice);

            search = new MoonifyControls.TextBox()
            {
                IconType = MoonifyControls.TextBox.IconTypes.Search,
                BackgroundText = "Search...",
                Location = new Vector2(10, 19),
                Width = 280
            };
            search.TextChanged += (s, e) => search.BackgroundText = search.Text.Length > 0 ? "" : "Search...";
            list = new ListBox<Track>()
            {
                Location = new Vector2(10, 52),
                Width = 280,
                Height = 438
            };

            manager.Controls.Add(search);
            manager.Controls.Add(list);

            manager.KeyboardControl = search;
        }

        protected override void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            spritebatch.Begin();
            spritebatch.Draw(manager.Background, Vector2.Zero, Color.White);
            spritebatch.End();

            manager.Draw(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            manager.Update(gameTime);
        }
    }
}
