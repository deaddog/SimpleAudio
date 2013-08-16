using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonifyControls;

namespace SimpleAudio
{
    public class ManagerControl : XNAControls.Forms.GraphicsDeviceControl
    {
        private ControlManager manager;

        private TextBox search;
        private ListBox<string> list;

        public ManagerControl()
            : base("Content")
        {
            manager = new MoonifyControls.ControlManager(this.Handle, Services, GraphicsDevice);
        
            search = new MoonifyControls.TextBox()
            {
                IconType = MoonifyControls.TextBox.IconTypes.Search,
                BackgroundText = "Search...",
                Location = new Vector2(10, 19),
                Width = this.Width - 20
            };
            search.TextChanged += (s, e) => search.BackgroundText = search.Text.Length > 0 ? "" : "Search...";
            list = new ListBox<string>("Hej", "med", "dig", "Metallica")
            {
                Location = new Vector2(10, 52),
                Width = this.Width - 20,
                Height = this.Height - 62
            };

            manager.Controls.Add(search);
            manager.Controls.Add(list);

            manager.KeyboardControl = search;
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);

            if (search != null)
                search.Width = this.Width - 20;
            if (list != null)
            {
                list.Width = this.Width - 20;
                list.Height = this.Height - 62;
            }
        }

        protected override void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            spritebatch.Begin();
            spritebatch.Draw(manager.Background,Vector2.Zero, Color.White);
            spritebatch.End();

            manager.Draw(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            manager.Update(gameTime);
        }
    }
}
