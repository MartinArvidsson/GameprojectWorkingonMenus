﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gameproject.View;
using Gameproject.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
namespace Gameproject.Controller
{
    class Gamecontroller
    {
        private GraphicsDeviceManager graphics;
        private ContentManager Content;
        private SpriteBatch spriteBatch;
        private Camera camera = new Camera();
        private GameTime gameTime;
        private BallSimulation ballsim;
        private Playersimulation playersim;
        private Startview startview;
        private Drawmap drawmap;
        private LevelOne lvlone;
        private int[,] map;
        private List<Rectangle> Ballcollisions = new List<Rectangle>();
        private List<Vector4> convertedballcollison;
        private List<Rectangle> Playercollision = new List<Rectangle>();
        private List<Rectangle> newTiles = new List<Rectangle>();
        private List<Vector4> convertednewTiles;
        private List<Vector2> playercreatedtiles = new List<Vector2>();
        private List<Texture2D> textures = new List<Texture2D>();

        bool hasplayerclicked = false;
        bool hasplayerclickedplay = false;

        public Gamecontroller(GraphicsDeviceManager _graphics)
        {
            graphics = _graphics;
        }

        public void LoadContent(SpriteBatch _spritebatch,ContentManager _content,Camera _camera)
        {
            spriteBatch = _spritebatch;
            Content = _content;
            camera = _camera;

            ballsim = new BallSimulation();
            playersim = new Playersimulation();

            drawmap = new Drawmap();
            lvlone = new LevelOne();

            startview = new Startview(Content, camera, spriteBatch, ballsim, playersim, drawmap, graphics);

            //Loads the map once when the application starts. Will use update function to call a function in drawmap that allows me to place new tiles..
            map = lvlone.getmap();
            textures = startview.ReturnedTextures();
            drawmap.Drawlevel(map, textures, spriteBatch, camera);
        }

        public void Update(GameTime gameTime)
        {
            var buttonclicked = Keyboard.GetState();

            if (playersim.isgameover == true)
            {
                System.Console.WriteLine("ded");
                //Startmenyn startas för att man dog.
            }

            if (drawmap.playerdidwin == true)
            {
                //Vinn spelet NYI
            }

            //if (buttonclicked.IsKeyDown(Keys.Escape))
            //{
            //    Exit();
            //}

            //if (buttonclicked.IsKeyDown(Keys.P))
            //{
            //    //Pausa, NYI
            //}

            //Ballupdating
            Ballcollisions = drawmap.Returnballcollisions();
            newTiles = drawmap.Returnplayercreatedtiles();
            convertedballcollison = new List<Vector4>();
            convertednewTiles = new List<Vector4>();

            foreach (Rectangle rect in Ballcollisions)
            {
                Vector2 convertedcoords = new Vector2(rect.X, rect.Y);
                Vector2 convertedsize = new Vector2(rect.Width, rect.Height);
                convertedcoords = camera.convertologicalcoords(convertedcoords);
                convertedsize = camera.convertologicalcoords(convertedsize);

                convertedballcollison.Add(new Vector4(convertedcoords.X, convertedcoords.Y, convertedsize.X, convertedsize.Y));
            }

            foreach (Rectangle rect in newTiles)
            {
                Vector2 convertedcoords = new Vector2(rect.X, rect.Y);
                Vector2 convertedsize = new Vector2(rect.Width, rect.Height);
                convertedcoords = camera.convertologicalcoords(convertedcoords);
                convertedsize = camera.convertologicalcoords(convertedsize);

                convertednewTiles.Add(new Vector4(convertedcoords.X, convertedcoords.Y, convertedsize.X, convertedsize.Y));
            }

            ballsim.UpdateBall((float)gameTime.ElapsedGameTime.TotalSeconds, convertedballcollison, convertednewTiles);

            //Playerupdating
            Playercollision = drawmap.Returnplayercollisions();
            playersim.SetBool(drawmap.Returnfinishedcreating());
            playersim.setInt(ballsim.playerbeenhit);
            playersim.setcollisions(Playercollision, Ballcollisions);
            playersim.hitwall(camera);

            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Down) ||
                Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                //Playermovements
                playersim.UpdatePlayer(buttonclicked);
            }

            //Mapupdating
            drawmap.updatedtilestoadd(playersim.getplayercreatedtiles());

        }

        public void Draw()
        {
            startview.Draw();
        }
    }
}
