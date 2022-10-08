﻿using System;
using Microsoft.Xna.Framework;

namespace BCSL.Graphics
{
    public sealed class Camera
    {
        #region Fields

        private Screen screen;
        private Vector2 position;
        private Matrix view;
        private Matrix proj;

        private float aspectRatio;
        private float fieldOfView;
        private double baseZ;
        private double z;

        private int zoom;
        private bool updateRequired;

        private float angle;
        private Vector2 up;

        private const float MinZ = 1f;
        private const float MaxZ = 2000f;

        private const int MinZoom = 1;
        private const int MaxZoom = 64;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return position; }
        }

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return proj; }
        }

        internal float AspectRatio
        {
            get { return aspectRatio; }
        }

        internal float FieldOfView
        {
            get { return fieldOfView; }
        }

        public double BaseZ
        {
            get { return baseZ; }
        }

        public double Z
        {
            get { return z; }
        }

        public int Zoom
        {
            get { return zoom; }
            set
            {
                zoom = BasicMath.Clamp(value, MinZoom, MaxZoom);
                z = baseZ * (1d / zoom);

                updateRequired = true;
            }
        }

        public Vector2 Up
        {
            get { return up; }
        }

        public float Angle
        {
            get { return angle; }
        }

        #endregion

        public Camera(Screen screen)
        {
            this.screen = screen ?? throw new ArgumentNullException("screen");
            position = Vector2.Zero;
            view = Matrix.Identity;
            proj = Matrix.Identity;

            aspectRatio = screen.Width / (float)screen.Height;
            fieldOfView = MathHelper.PiOver2;
            baseZ = GetZFromHeight(screen.Height);
            z = baseZ;

            angle = 0f;
            up = new Vector2(MathF.Sin(angle), MathF.Cos(angle));

            zoom = 1;
            updateRequired = true;

            Update();

        }

        private double GetZFromHeight(double height)
        {
            double result = height * 0.5d / Math.Tan(fieldOfView * 0.5d);
            return result;
        }

        private double GetVisibleHeightFromZ(double z)
        {
            double result = z * Math.Tan(fieldOfView * 0.5f) * 2f;
            return result;
        }

        private double GetVisibleHeight()
        {
            double result = 2d * Math.Tan(fieldOfView * 0.5d) * z;
            return result;
        }

        public void Update()
        {
            // Only update the camera view and projection when changes have occured.
            if (!updateRequired)
            {
                return;
            }

            view = Matrix.CreateLookAt(new Vector3(0, 0, (float)z), Vector3.Zero, new Vector3(up, 0f));
            proj = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, MinZ, MaxZ);

            updateRequired = false;
        }

        public void ResetZ()
        {
            z = baseZ;
            Update();
        }

        public void Move(Vector2 amount)
        {
            position += amount;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }

        public void MoveZ(float amount)
        {
            double new_z = z + amount;

            if (new_z < MinZ ||
                new_z > MaxZ)
            {
                return;
            }

            z = new_z;
            updateRequired = true;
        }

        public void IncZoom()
        {
            int new_zoom = zoom + 1;

            if (new_zoom < MinZoom || new_zoom > MaxZoom)
            {
                return;
            }

            zoom = new_zoom;
            z = baseZ * (1d / zoom);

            updateRequired = true;
        }

        public void DecZoom()
        {
            int new_zoom = zoom - 1;

            if (new_zoom < MinZoom || new_zoom > MaxZoom)
            {
                return;
            }

            zoom = new_zoom;
            z = baseZ * (1d / zoom);

            updateRequired = true;
        }

        public void Rotate(float amount)
        {
            angle += amount;
            up = new Vector2(MathF.Sin(angle), MathF.Cos(angle));
            updateRequired = true;
        }

        public void GetExtents(out float width, out float height)
        {
            height = (float)GetVisibleHeight();
            width = height * aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            GetExtents(out float width, out float height);

            left = position.X - width * 0.5f;
            right = left + width;
            bottom = position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            GetExtents(out float left, out float right, out float bottom, out float top);

            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

    }
}

