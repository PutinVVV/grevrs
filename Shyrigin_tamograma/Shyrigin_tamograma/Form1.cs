
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace Shyrigin_tamograma
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
        class Bin
        {
            public static int X, Y, Z;
            public static short[] array;
            public Bin() { }
            public Int32 Zer() { return Z; }
            public void readBIN(string path)
            {
                if (File.Exists(path))
                {
                    BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                    X = reader.ReadInt32();
                    Y = reader.ReadInt32();
                    Z = reader.ReadInt32();
                    int arraySize = X * Y * Z;
                    array = new short[arraySize + 1];
                    for (int i = 0; i < arraySize; i++)
                        array[i] = reader.ReadInt16();
                    
                }
            }
        }
        class View
        {
            Bitmap textureImage;
            int VBOtexture;
            public void Load2DTexture() 
            {
                GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
                BitmapData data = textureImage.LockBits(new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, data.Scan0);
                !!!!!!!!!!!!!
            }
            public void SetupView(int wid, int hei)
            {
                GL.ShadeModel(ShadingModel.Smooth);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, Bin.X, Bin.Y, 0, -1, 1);
                GL.Viewport(0, 0, wid, hei);
            }
            public int Clamp(int val, int min, int max)
            {
                if (val < min)
                    return min;
                if (val > max)
                    return max;
                return val;
            }
            public Color TransferFunction(short val)
            {
                int min = 0;
                int max = 2000;
                int newVal = Clamp((val - min) * 255 / (max - min), 0, 255);
                return Color.FromArgb(255, newVal, newVal, newVal);
            }
            public void DrawQuads(int layerNumber)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.Begin(BeginMode.Quads);
                for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
                    for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                    {
                        short value;
                        //1vershina
                        value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord, y_coord);
                        //2vershina
                        value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord, y_coord + 1);
                        //33vershina
                        value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord + 1, y_coord + 1);
                        //4vershina
                        value = Bin.array[x_coord + 1 + (y_coord) * Bin.X + layerNumber * Bin.X * Bin.Y];
                        GL.Color3(TransferFunction(value));
                        GL.Vertex2(x_coord + 1, y_coord);
                    }
                GL.End();
            }
        }

        private void glControl1_Load(object sender, EventArgs e)
        {

        }

        bool loaded = false;
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                Bin bin = new Bin();
                bin.readBIN(str);
                View view = new View();
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                trackBar1.Maximum = bin.Zer()-1;
            }
        }
        int currentLayer = 0;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                View view = new View();
                view.DrawQuads((int)currentLayer);
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
    }

}

