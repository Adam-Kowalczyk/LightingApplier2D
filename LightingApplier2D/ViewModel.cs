using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LightingApplier2D
{
    public class ViewModel : Observable
    {
        public bool ShowMesh
        {
            get
            {
                return showMesh;
            }
            set
            {
                if (value == showMesh) return;
                showMesh = value;
                //UpdateBitmap();
                OnPropertyChanged(nameof(ShowMesh));
            }
        }
        bool showMesh = true;

        public bool UseSelectedColor
        {
            get
            {
                return useSelectedColor;
            }
            set
            {
                if (useSelectedColor == value) return;
                useSelectedColor = value;
                //UpdateBitmap();
            }
        }
        bool useSelectedColor = true;

        public bool UsePredefinedNormal
        {
            get
            {
                return usePredefinedNormal;
            }
            set
            {
                if (usePredefinedNormal == value) return;
                usePredefinedNormal = value;
                //UpdateBitmap();
            }
        }
        bool usePredefinedNormal = true;

        public WriteableBitmap Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
            }
        }
        WriteableBitmap bitmap;

        public WriteableBitmap Bitmap1 { get; set; }

        public WriteableBitmap NormalMap { get; set; } = null;

        public WriteableBitmap Texture { get; set; } = null;


        (byte, byte, byte)[,] pixels;

        (byte, byte, byte)[,] texturePixels;

        (byte, byte, byte)[,] normalPixels;

        public int Width { get; set; } = 800;
        public int Height { get; set; } = 800;

        public int N
        {
            get
            {
                return _n;
            }
            set
            {
                if (_n == value) return;
                _n = value;
                OnPropertyChanged(nameof(N));
                Initialize();
            }
        }
        int _n = 9;

        public int M
        {
            get
            {
                return _m;
            }
            set
            {
                if (_m == value) return;
                _m = value;
                OnPropertyChanged(nameof(M));
                Initialize();
            }
        }
        int _m = 12;

        public List<FillingTriangle> Triangles { get; set; } = new List<FillingTriangle>();

        public DragablePoint[,] Points { get; set; }

        public double Kd
        {
            get
            {
                return kd;
            }
            set
            {
                kd = value;
                OnPropertyChanged(nameof(Kd));
                //UpdateBitmap();
            }
        }
        public double kd = 0.5;

        public double Ks
        {
            get
            {
                return ks;
            }
            set
            {
                ks = value;
                OnPropertyChanged(nameof(Ks));
                //UpdateBitmap();
            }
        }
        public double ks = 0.5;

        public bool UseSetValues
        {
            get
            {
                return useSetValues;
            }
            set
            {
                if (useSetValues == value) return;
                useSetValues = value;
                OnPropertyChanged(nameof(UseSetValues));
            }
        }
        bool useSetValues = true;

        public Color IL
        {
            get
            {
                return iL;
            }
            set
            {
                iL = value;
                OnPropertyChanged(nameof(IL));
                //UpdateBitmap();
            }
        }
        Color iL = Colors.White;

        public Color IO
        {
            get
            {
                return iO;
            }
            set
            {
                iO = value;
                OnPropertyChanged(nameof(IO));
                //UpdateBitmap();
            }
        }
        Color iO = Colors.Crimson;

        public Vector3 L
        {
            get
            {
                return lV;
            }
            set
            {
                lV = value;
                OnPropertyChanged(nameof(L));
                //UpdateBitmap();
            }
        }
        Vector3 lV = new Vector3(0, 0, 1);

        public Vector3 DefaultNormalVector
        {
            get
            {
                return defaultNormalVector;
            }
            set
            {
                defaultNormalVector = value;
            }
        }
        Vector3 defaultNormalVector = new Vector3(0, 0, 1);

        public int MValue
        {
            get
            {
                return mValue;
            }
            set
            {
                if (mValue == value) return;
                mValue = value;
                OnPropertyChanged(nameof(MValue));
                //UpdateBitmap();
            }
        }
        int mValue = 4;

        public LightSource Light { get; set; } = new LightSource(100, 100, 10);

        public bool UseConstantLight
        {
            get
            {
                return useConstantLight;
            }
            set
            {
                if (useConstantLight == value) return;
                useConstantLight = value;
                OnPropertyChanged(nameof(UseConstantLight));

            }
        }
        bool useConstantLight = true;

        Random randomG = new Random(3456);

        public bool InterpolColor
        {
            get
            {
                return interpolColor;
            }
            set
            {
                if (interpolColor == value) return;
                interpolColor = value;
                OnPropertyChanged(nameof(InterpolColor));

            }
        }
        bool interpolColor = false;

        public bool InterpolColorNormal
        {
            get
            {
                return interpolColorNormal;
            }
            set
            {
                if (interpolColorNormal == value) return;
                interpolColorNormal = value;
                OnPropertyChanged(nameof(InterpolColorNormal));

            }
        }
        bool interpolColorNormal = false;
        public void CreateTexture()
        {
            if (Texture == null) return;

            textureW = (int)Texture.Width;
            textureH = (int)Texture.Height;

            texturePixels = new (byte, byte, byte)[textureW, textureH];
            unsafe
            {
                using (var ctx = Texture.GetBitmapContext(ReadWriteMode.ReadOnly))
                {
                    for (int i = 0; i < ctx.Width; i++)
                    {
                        for (int j = 0; j < ctx.Height; j++)
                        {
                            var p = ctx.Pixels[j * ctx.Width + i];
                            texturePixels[i, j] = ((byte)(p >> 16 & 0x00FF), (byte)(p >> 8 & 0x00FF), (byte)(p & 0x00FF));
                        }
                    }
                }
            }
        }
        int textureW;
        int textureH;

        public void CreateNormal()
        {
            if (NormalMap == null) return;


            normalPixels = new (byte, byte, byte)[(int)NormalMap.Width, (int)NormalMap.Height];
            unsafe
            {
                using (var ctx = NormalMap.GetBitmapContext(ReadWriteMode.ReadOnly))
                {
                    for (int i = 0; i < ctx.Width; i++)
                    {
                        for (int j = 0; j < ctx.Height; j++)
                        {
                            var p = ctx.Pixels[j * ctx.Width + i];
                            normalPixels[i, j] = ((byte)(p >> 16 & 0x00FF), (byte)(p >> 8 & 0x00FF), (byte)(p & 0x00FF));
                        }
                    }
                }
            }
        }

        

        public void Initialize()
        {
            if (NormalMap != null)
            {
                Width = (int)NormalMap.Width;
                Height = (int)NormalMap.Height;
            }
            //Bitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr24, null);
            pixels = new (byte, byte, byte)[Width, Height];
            Bitmap1 = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);
            Points = new DragablePoint[M + 1, N + 1];
            var verticalEdge = (int)(Height / N);
            var horizontalEdge = (int)(Width / M);

            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < M + 1; j++)
                {
                    Points[j, i] = new DragablePoint(j != M ? j * horizontalEdge : j * horizontalEdge - 1,
                        i != N ? i * verticalEdge : i * verticalEdge - 1);
                }
            }
            Triangles.Clear();

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Triangles.Add(new FillingTriangle(Points[j, i], Points[j + 1, i], Points[j, i + 1], randomG));
                    Triangles.Add(new FillingTriangle(Points[j + 1, i], Points[j, i + 1], Points[j + 1, i + 1], randomG));

                }
            }
            if (Texture != null)
                CreateTexture();
             
            if (NormalMap != null)
                CreateNormal();

            if(BackgroundTask == null)
            {
                
                BackgroundTask = Task.Run(BackgroundUpdate);
            }
            
        }
        public Task BackgroundTask { get; set; } = null;

        public Dispatcher Dispatcher { get; set; }

        public void BackgroundUpdate()
        {
            double angle = 0.0;
            while (true)
            {
                angle+=0.2;
                angle = angle % 360;
                var rads = angle / 180 * Math.PI;
                var radius = Width / 4;
                var center = (Width / 2, Height / 2);
                Light.Position = new Vector3((float)Math.Cos(angle) * radius + center.Item1,
                    (float)Math.Sin(angle) * radius + center.Item2,
                    (float)Math.Sin(angle) * 50 + 60);
                UpdateBitmap();
            }
        }

        public void UpdateBitmap()
        {
            pixels = new (byte, byte, byte)[Width, Height];

            bool useTexture = (Texture != null && !UseSelectedColor);

            bool useMap = (NormalMap != null && !UsePredefinedNormal);
            //FillPolygon(Triangles[1]);

            bool useAnimated = !UseConstantLight;

            bool useRandom = !UseSetValues;

            bool interCol = InterpolColor;

            bool interNorm = InterpolColorNormal;
            foreach (var tri in Triangles.ToList())
                FillPolygon(tri, useTexture, useMap, useAnimated, useRandom, interCol, interNorm);


            //Parallel.ForEach(Triangles, (x) => FillPolygon(x, useTexture, useMap));
            try
            {
                Dispatcher.Invoke(() => FillPixels());
                //FillPixels();

                if (ShowMesh)
                {
                    Dispatcher.Invoke(() => DrawTriangles());
                }
            }
            catch(Exception)
            {

            }

            Bitmap = Bitmap1;
            OnPropertyChanged(nameof(Bitmap));
        }

        public void DrawTriangles()
        {
            foreach (var tri in Triangles)
            {
                if (tri.Points.Count == 3)
                    Bitmap1.DrawTriangle(tri.Points[0].X, tri.Points[0].Y,
                        tri.Points[1].X, tri.Points[1].Y,
                        tri.Points[2].X, tri.Points[2].Y, Colors.Black);
            }
        }

        public void FillPixels()
        {
            //Bitmap1.Lock();
            unsafe
            {
                using (var ctx = Bitmap1.GetBitmapContext())
                {
                    for (int i = 0; i < ctx.Width; i++)
                    {
                        for (int j = 0; j < ctx.Height; j++)
                        {
                            var col = pixels[i, j];
                            ctx.Pixels[j * ctx.Width + i] = (col.Item1 << 16) | (col.Item2 << 8) | (col.Item3);
                        }
                    }
                }
            }
            //Bitmap1.Unlock();
        }

        public void FillPolygon(FillingTriangle triangle, bool useTexture = false,
            bool useMap = false, bool useAnimated = true, bool useRandom = false,
            bool interCol = false, bool interNorm = false)
        {
            var maxY = triangle.Points.Max(x => x.Y);
            var minY = triangle.Points.Min(x => x.Y);
            var etTable = new List<EdgeStruct>[maxY - minY + 1];
            for (int i = 0; i < triangle.Points.Count; i++)
            {
                var p1 = triangle.Points[i];
                var p2 = triangle.Points[i + 1 == triangle.Points.Count ? 0 : i + 1];
                if (p1.Y == p2.Y) continue;
                if (p1.Y > p2.Y)
                {
                    var tmp = p2;
                    p2 = p1;
                    p1 = tmp;
                }
                //double slp;
                var str = new EdgeStruct { YMax = p2.Y, X = p1.Y < p2.Y ? p1.X : p2.X, Slope = (double)(p1.X - p2.X) / (p1.Y - p2.Y) };
                if (etTable[p1.Y - minY] == null)
                {
                    etTable[p1.Y - minY] = new List<EdgeStruct>();
                }
                etTable[p1.Y - minY].Add(str);
            }

            var aetTable = new List<EdgeStruct>();
            int y = minY;
            (byte, byte, byte) col = (IO.R, IO.G, IO.B);
            var lightCol = new Vector3(IL.R / 255, IL.G / 255, IL.B / 255);
            var lightVector = DefaultNormalVector;
            var kdparam = Kd;
            var ksparam = Ks;
            var mparam = MValue;
            if(useRandom)
            {
                kdparam = triangle.Kd;
                ksparam = triangle.Ks;
                mparam = triangle.M;
            }

            var cp0 = col;
            var cp1 = col;
            var cp2 = col;
            if (useTexture)
            {
                cp0 = texturePixels[triangle.Points[0].X, triangle.Points[0].Y];
                cp1 = texturePixels[triangle.Points[1].X, triangle.Points[1].Y];
                cp2 = texturePixels[triangle.Points[2].X, triangle.Points[2].Y];
            }



            (byte, byte, byte)[] vertix = new (byte, byte, byte)[3] {cp0, cp1, cp2 };
            if (interCol)
            {

                for (int i = 0; i < 3; i++)
                {
                    var nvi = DefaultNormalVector;
                    if (useMap)
                    {
                        nvi = new Vector3(normalPixels[triangle.Points[i].X, triangle.Points[i].Y].Item1 - 127,
                            normalPixels[triangle.Points[i].X, triangle.Points[i].Y].Item2 - 127,
                            normalPixels[triangle.Points[i].X, triangle.Points[i].Y].Item3);
                        nvi = Vector3.Normalize(nvi);
                    }

                    if (useAnimated)
                    {
                        lightVector = Light.UnitToLight(triangle.Points[i].X, triangle.Points[i].Y);
                    }

                    vertix[i] = CalculateColors(kdparam, ksparam, lightCol, vertix[i], lightVector, nvi, mparam);
                }
            }

            Vector3[] normals = new Vector3[3];
            for(int i =0;i< 3; i++)
            {
                if (useMap)
                {
                    normals[i] = new Vector3(normalPixels[triangle.Points[i].X, triangle.Points[i].Y].Item1 - 127, normalPixels[triangle.Points[i].X, triangle.Points[i].Y].Item2 - 127, normalPixels[triangle.Points[i].X, triangle.Points[i].Y].Item3);
                    normals[i] = Vector3.Normalize(normals[i]);
                }
                else
                    normals[i] = DefaultNormalVector;
            }
            (byte, byte, byte)[] colors = new (byte, byte, byte)[3] { cp0, cp1, cp2 };

            while (y <= maxY)
            {
                if (etTable[y - minY] != null)
                {
                    var tempList = etTable[y - minY];
                    //foreach (var edge in tempList)
                    //{
                    //    //if(edge.Slope < 0)
                    //        edge.X = edge.X - (edge.YMax - y) * edge.Slope;
                    //}

                    aetTable.AddRange(tempList);
                }

                aetTable = aetTable.OrderBy(x => x.X).ToList();
                for (int i = 0; i < aetTable.Count; i += 2)
                {
                    if (aetTable.Count - i == 1)
                    {
                        continue;
                    }
                    var first = aetTable[i];
                    var second = aetTable[i + 1];
                    
                    for (int j = (int)(first.X); j < second.X; j++)
                    {
                        if (interCol)
                        {
                            var dist0 = triangle.Points[0].Distance(j, y);
                            var dist1 = triangle.Points[1].Distance(j, y);
                            var dist2 = triangle.Points[2].Distance(j, y);

                            dist0 = dist0 == 0 ? 1 : 1 / dist0;
                            dist1 = dist1 == 0 ? 1 : 1 / dist1;
                            dist2 = dist2 == 0 ? 1 : 1 / dist2;

                            var sum = dist0 + dist1 + dist2;

                            var cr = (byte)((vertix[0].Item1 * dist0 + vertix[1].Item1 * dist1 + vertix[2].Item1 * dist2) / sum);
                            var cg = (byte)((vertix[0].Item2 * dist0 + vertix[1].Item2 * dist1 + vertix[2].Item2 * dist2) / sum);
                            var cb = (byte)((vertix[0].Item3 * dist0 + vertix[1].Item3 * dist1 + vertix[2].Item3 * dist2) / sum);

                            pixels[j, y] = (cr, cg, cb);
                        }
                        else if(interNorm)
                        {
                            var dist0 = triangle.Points[0].Distance(j, y);
                            var dist1 = triangle.Points[1].Distance(j, y);
                            var dist2 = triangle.Points[2].Distance(j, y);

                            dist0 = dist0 == 0 ? 1 : 1 / dist0;
                            dist1 = dist1 == 0 ? 1 : 1 / dist1;
                            dist2 = dist2 == 0 ? 1 : 1 / dist2;

                            var sum = dist0 + dist1 + dist2;

                            var cr = (byte)((vertix[0].Item1 * dist0 + vertix[1].Item1 * dist1 + vertix[2].Item1 * dist2) / sum);
                            var cg = (byte)((vertix[0].Item2 * dist0 + vertix[1].Item2 * dist1 + vertix[2].Item2 * dist2) / sum);
                            var cb = (byte)((vertix[0].Item3 * dist0 + vertix[1].Item3 * dist1 + vertix[2].Item3 * dist2) / sum);

                            var cl = (cr, cg, cb);

                            var nx = (float)((normals[0].X * dist0 + normals[1].X * dist1 + normals[2].X * dist2) / sum);
                            var ny = (float)((normals[0].Y * dist0 + normals[1].Y * dist1 + normals[2].Y * dist2) / sum);
                            var nz = (float)((normals[0].Z * dist0 + normals[1].Z * dist1 + normals[2].Z * dist2) / sum);

                            var ni = new Vector3(nx, ny, nz);

                            if (useAnimated)
                            {
                                lightVector = Light.UnitToLight(j, y);
                            }

                            pixels[j, y] = CalculateColors(kdparam, ksparam, lightCol, cl, lightVector, ni, mparam);
                        }
                        else
                        {
                            var cl = col;
                            if (useTexture)
                                cl = texturePixels[j % textureW, y % textureH];
                            var nv = DefaultNormalVector;
                            if (useMap)
                            {
                                nv = new Vector3(normalPixels[j, y].Item1 - 127, normalPixels[j, y].Item2 - 127, normalPixels[j, y].Item3);
                                nv = Vector3.Normalize(nv);
                            }

                            if (useAnimated)
                            {
                                lightVector = Light.UnitToLight(j, y);
                            }

                            pixels[j, y] = CalculateColors(kdparam, ksparam, lightCol, cl, lightVector, nv, mparam);
                        }
                    }
                }
                foreach (var edge in aetTable.ToList())
                {
                    if (edge.YMax == y + 1)
                    {
                        aetTable.Remove(edge);
                    }
                    else
                    {
                        edge.X += edge.Slope;
                    }
                }
                y++;
            }
        }

        public (byte, byte, byte) CalculateColors(double _kd, double _ks, Vector3 _il, (byte, byte, byte) _io, Vector3 _l, Vector3 _n, int m)
        {
            
            var vvector = new Vector3(0, 0, 1);
            var rvector = Vector3.Reflect(_l * -1, _n);
            var p1 = (Vector3.Dot(_n, _l)) / (Vector3.Dot(_n, _n) * Vector3.Dot(_l, _l));
            var p2 = Math.Pow((Vector3.Dot(vvector, rvector)) / (Vector3.Dot(vvector, vvector) * Vector3.Dot(rvector, rvector)), m);
            var f1 = (int)(_kd * _il.X * _io.Item1 * p1 + _ks * _il.X * _io.Item1 * p2);
            var f2 = (int)(_kd * _il.Y * _io.Item2 * p1 + _ks * _il.Y * _io.Item2 * p2);
            var f3 = (int)(_kd * _il.Z * _io.Item3 * p1 + _ks * _il.Z * _io.Item3 * p2);

            //f1 *= 255;
            if (f1 > 255)
                f1 = 255;
            if (f1 < 0)
                f1 = 0;
            //f2 *= 255;
            if (f2 > 255)
                f2 = 255;
            if (f2 < 0)
                f2 = 0;
            //f3 *= 255;
            if (f3 > 255)
                f3 = 255;
            if (f3 < 0)
                f3 = 0;
            return (Convert.ToByte(f1), Convert.ToByte(f2), Convert.ToByte(f3));
        }
    }


    public class EdgeStruct
    {
        public int YMax;
        public double X;
        public double Slope;
    }
}
