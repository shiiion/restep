using System;
using System.Collections.Generic;
using System.Text;

namespace restep.Graphics.Renderables
{
    public class Background
    {
        private TexturedQuad backgroundImage;

        private string imagePath;
        public string ImagePath
        {
            get
            {
                return imagePath;
            }

            set
            {
                imagePath = value;
                backgroundImage?.Dispose();
                backgroundImage = new TexturedQuad(imagePath);
            }
        }

        public Background()
        {
            ImagePath = "";
        }

        public void Render(float depth)
        {
            if (backgroundImage.Loaded)
            {
                backgroundImage.Transformation.BaseScale = new OpenTK.Vector2(RestepWindow.Instance.Width, RestepWindow.Instance.Height);
                backgroundImage.Depth = depth;
                backgroundImage.Render();
            }
        }
    }
}
