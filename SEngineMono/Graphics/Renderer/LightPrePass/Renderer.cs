using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEngineMono.Graphics.Renderer.LightPrePass
{
    public class Renderer
    {
        private Game1 game;
        private GraphicsDevice graphicsDevice;

        public RenderTarget2D depthRT;
        public RenderTarget2D normalsRT;
        public RenderTarget2D lightRT;
        public RenderTarget2D specularRT;

        public RenderTarget2D finalRT;

        private RenderTargetBinding[] gBuffer;
        private RenderTargetBinding[] lightBuffer;

        private Effect clearDepthNormalEffect;
        private Effect renderDepthNormalEffect;

        public Renderer(Game1 game)
        {
            this.game = game;
            this.graphicsDevice = game.GraphicsDevice;

            depthRT = new RenderTarget2D(graphicsDevice, 1024, 768, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            normalsRT = new RenderTarget2D(graphicsDevice, 1024, 768, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            lightRT = new RenderTarget2D(graphicsDevice, 1024, 768, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            finalRT = new RenderTarget2D(graphicsDevice, 1024, 768, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

            gBuffer = new[] { new RenderTargetBinding(depthRT), new RenderTargetBinding(normalsRT) };
            //lightBuffer = new[] { new RenderTargetBinding(lightRT), new RenderTargetBinding(specularRT) };

            clearDepthNormalEffect = game.Content.Load<Effect>("Shaders/ClearDepthNormal");
            renderDepthNormalEffect = game.Content.Load<Effect>("Shaders/RenderDepthNormal");
        }

        public void Render(List<Tuple<Model, Matrix>> models)
        {
            graphicsDevice.SetRenderTargets(gBuffer);

            ClearGBuffer();

            RenderGBuffer(models);
        }

        private void ClearGBuffer()
        {
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            clearDepthNormalEffect.CurrentTechnique.Passes[0].Apply();
            QuadRenderer.RenderFullScreenQuad(graphicsDevice);
        }

        private void RenderGBuffer(List<Tuple<Model, Matrix>> models)
        {
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            renderDepthNormalEffect.CurrentTechnique.Passes[0].Apply();

            foreach (Tuple<Model, Matrix> model in models)
            {
                model.Item1.Draw(model.Item2, game.camera.viewMatrix, game.camera.projectionMatrix);
            }
        }
    }
}
