using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonifyControls;

using DeadDog.Audio.Libraries;

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
            search.KeyDown += new XNAControls.KeyEventHandler(search_KeyDown);
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

        private void search_KeyDown(object sender, XNAControls.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Microsoft.Xna.Framework.Input.Keys.Up:
                    if (list.SelectedIndex > 0)
                        list.SelectedIndex--;
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Down:
                    if (list.SelectedIndex < list.Items.Count - 1)
                        list.SelectedIndex++;
                    break;
            }
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
