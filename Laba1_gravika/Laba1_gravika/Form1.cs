using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Laba1_gravika
{

    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "image files|*.png;*.jpg;*.bmp|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlueFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрГаусаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GausFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayS();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sepia();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void повыситьЯркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Yarkost();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sobel();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void повыситьРезкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Rezkost();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void выделениеГраницToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Shara();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void выделениеГраницПрюиттаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Priut();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void переносToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Perenos();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void поворотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Povorot();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волны1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Volna1();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void волны2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Volna2();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void стеклоToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Filters filter = new Steklo();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void суженияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new antiDilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter();
            filter =new antiDilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new antiDilationFilter();
            filter = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void зеркалоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Zerkalo();
            backgroundWorker1.RunWorkerAsync(filter);
        }
    }
    public abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    worker.ReportProgress((int)((float)i/resultImage.Width*100));
                    if (worker.CancellationPending)
                        return null;
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            return resultImage;
        }
        public int Clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }
    }
    public class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    }

    public class GrayS : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            double red = sourceColor.R;
            double blue = sourceColor.B;
            double green = sourceColor.G;
            int res =(int) Math.Round(red * 0.299+0.587*green+0.114*blue);
            Color resultColor = Color.FromArgb(res, res, res);
            return resultColor;
        }
    }

    public class Sepia : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            double red = sourceColor.R;
            double blue = sourceColor.B;
            double green = sourceColor.G;
            int res = (int) Math.Round(red * 0.299 + 0.587 * green + 0.114 * blue);
            Color resultColor = Color.FromArgb(Clamp(res+2*50,0,255), Clamp(res + 25 , 0, 255), Clamp(res - 1 * 50, 0, 255));
            return resultColor;
        }
    }

    public class Yarkost : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(Clamp(sourceColor.R+10,0,255), Clamp(sourceColor.G + 10, 0, 255), Clamp(sourceColor.B + 10, 0, 255));
            return resultColor;
        }
    }

    public class Perenos : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            x = x + 50;
            int k = sourceImage.Width - 1;
            x=Clamp(x, 0, sourceImage.Width-1);
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb((sourceColor.R), (sourceColor.G ), (sourceColor.B));
            return resultColor;
        }
    }

    public class Povorot : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int xc = sourceImage.Width / 2;
            int yc = sourceImage.Height / 2;
            double xv = (x-xc)*0.7-(y-yc)*0.7+xc;
            double yv= (x - xc) * 0.7 + (y - yc) * 0.7 + yc;
            x = (int)Math.Round(xv, 0);
            y = (int)Math.Round(yv, 0);
            x = Clamp(x, 0, sourceImage.Width - 1);
            y = Clamp(y, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb((sourceColor.R), (sourceColor.G), (sourceColor.B));
            return resultColor;
        }
    }

    public class Volna1 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            double xv = x + 20 * Math.Sin(2 * Math.PI * y / 60);
            x = (int)Math.Round(xv, 0);
            x = Clamp(x, 0, sourceImage.Width - 1);
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb((sourceColor.R), (sourceColor.G), (sourceColor.B));
            return resultColor;
        }
    }

    public class Volna2 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            double xv = x + 20 * Math.Sin(2 * Math.PI * x / 60);
            x = (int)Math.Round(xv, 0);
            x = Clamp(x, 0, sourceImage.Width - 1);
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb((sourceColor.R), (sourceColor.G), (sourceColor.B));
            return resultColor;
        }
    }

    public class Zerkalo : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            if(x>sourceImage.Width/2)
                return sourceImage.GetPixel(sourceImage.Width-x, y);
            return sourceImage.GetPixel(x, y); 
        }
    }

    public class Steklo : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
             x = x+ rand.Next(-1,2);
             y = y + rand.Next(-1, 2);
            //x = (int)Math.Round(xv, 0);
           // y = (int)Math.Round(yv, 0);
            x = Clamp(x, 0, sourceImage.Width - 1);
            y = Clamp(y, 0, sourceImage.Height - 1);
            //Color sourceColor = sourceImage.GetPixel(x, y);
            //Color resultColor = Color.FromArgb((sourceColor.R), (sourceColor.G), (sourceColor.B));
            return sourceImage.GetPixel(x,y);
        }
        Random rand = new Random();
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int Xradius = kernel.GetLength(0) / 2;
            int Yradius = kernel.GetLength(1) / 2;
            float Rres = 0, Bres = 0, Gres = 0;
            for (int l = -Yradius; l <= Yradius; l++)
                for (int k = -Xradius; k <= Xradius; k++)
                {
                    int idx = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idy = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idx, idy);
                    Rres += neighborColor.R * kernel[k + Xradius, l + Yradius];
                    Gres += neighborColor.G * kernel[k + Xradius, l + Yradius];
                    Bres += neighborColor.B * kernel[k + Xradius, l + Yradius];
                }
            return Color.FromArgb(Clamp((int)Rres, 0, 255), Clamp((int)Gres, 0, 255), Clamp((int)Bres, 0, 255));

        }
    }
    class BlueFilter : MatrixFilter
    {
        public BlueFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    }
    class GausFilter : MatrixFilter
    {
        public void createGausKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)Math.Exp(-(i * i + j * j) / (2 * sigma * sigma));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
        public GausFilter()
        { createGausKernel(3, 2); }
    }

    class Sobel : MatrixFilter
    {
        public Sobel()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0 ,0]= -1; kernel[0, 1] = -2; kernel[0, 2] = -1;
            kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0;
            kernel[2, 0] = 1; kernel[2, 1] = 2; kernel[2, 2] = 1;
        }

    }

    class Rezkost : MatrixFilter
    {
        public Rezkost()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = 0; kernel[0, 1] = -1; kernel[0, 2] = 0;
            kernel[1, 0] = -1; kernel[1, 1] = 5; kernel[1, 2] = -1;
            kernel[2, 0] = 0; kernel[2, 1] = -1; kernel[2, 2] = 0;
        }

    }

    class Shara : MatrixFilter
    {
        public Shara()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = 3; kernel[0, 1] = 10; kernel[0, 2] = 3;
            kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0;
            kernel[2, 0] = -3; kernel[2, 1] = -10; kernel[2, 2] = -3;
        }

    }

    class Priut : MatrixFilter
    {
        public Priut()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = -1; kernel[0, 1] = -1; kernel[0, 2] = -1;
            kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0;
            kernel[2, 0] = 1; kernel[2, 1] = 1; kernel[2, 2] = 1;
        }

    }

    public class DilationFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int clampX = Clamp(x, 0, sourceImage.Width - 1);
            int clampY = Clamp(y, 0, sourceImage.Height - 1);

            int max = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int sampleX = Clamp(clampX + i, 0, sourceImage.Width - 1);
                    int sampleY = Clamp(clampY + j, 0, sourceImage.Height - 1);

                    int pixel = sourceImage.GetPixel(sampleX, sampleY).R;

                    max = Math.Max(max, pixel);
                }
            }

            return Color.FromArgb(max, max, max);
        }
    }

    public class antiDilationFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int clampX = Clamp(x, 0, sourceImage.Width - 1);
            int clampY = Clamp(y, 0, sourceImage.Height - 1);

            int max = 255;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int sampleX = Clamp(clampX + i, 0, sourceImage.Width - 1);
                    int sampleY = Clamp(clampY + j, 0, sourceImage.Height - 1);

                    int pixel = sourceImage.GetPixel(sampleX, sampleY).R;

                    max = Math.Min(max, pixel);
                }
            }

            return Color.FromArgb(max, max, max);
        }
    }

    public class Open : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int clampX = Clamp(x, 0, sourceImage.Width - 1);
            int clampY = Clamp(y, 0, sourceImage.Height - 1);

            int max = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int sampleX = Clamp(clampX + i, 0, sourceImage.Width - 1);
                    int sampleY = Clamp(clampY + j, 0, sourceImage.Height - 1);

                    int pixel = sourceImage.GetPixel(sampleX, sampleY).R;

                    max = Math.Max(max, pixel);
                }
            }

            max = 255;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int sampleX = Clamp(clampX + i, 0, sourceImage.Width - 1);
                    int sampleY = Clamp(clampY + j, 0, sourceImage.Height - 1);

                    int pixel = sourceImage.GetPixel(sampleX, sampleY).R;

                    max = Math.Min(max, pixel);
                }
            }

            return Color.FromArgb(max, max, max);
        }
    }



}

