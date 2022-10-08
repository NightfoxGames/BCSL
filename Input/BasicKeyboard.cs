﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BCSL.Input
{
    public sealed class BasicKeyboard
    {
        private static Lazy<BasicKeyboard> LazyInstance = new Lazy<BasicKeyboard>(() => new BasicKeyboard());

        public static BasicKeyboard Instance
        {
            get { return LazyInstance.Value; }
        }

        private KeyboardState currKeyboardState;
        private KeyboardState prevKeyboardState;
        
        public bool IsKeyAvailable
        {
            get { return this.currKeyboardState.GetPressedKeyCount() > 0; }
        }
        
        private BasicKeyboard()
        {
            this.currKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            this.prevKeyboardState = this.currKeyboardState;
        }

        public void Update()
        {
            this.prevKeyboardState = this.currKeyboardState;
            this.currKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key)
        {
            return this.currKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
        }
    }
}
