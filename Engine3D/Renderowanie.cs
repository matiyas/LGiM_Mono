﻿using System;
using Drawing = System.Drawing;
using System.Linq;
using MathNet.Spatial.Euclidean;
using Gdk;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Engine3D;

class Renderowanie
{
  Scena rysownik;
  Drawing.Size rozmiarTekstury;
  SixLabors.ImageSharp.Color[,] teksturaKolory;

  Image<Rgb24> bmp;

  public Renderowanie(string sciezka, Scena rysownik)
  {
    bmp = Image.Load(sciezka).CloneAs<Rgb24>();
    this.rysownik = rysownik;
    teksturaKolory = new SixLabors.ImageSharp.Color[bmp.Width, bmp.Height];
    for (int y = 0; y < bmp.Height; ++y)
    {
      for (int x = 0; x < bmp.Width; ++x)
      {
        teksturaKolory[x, y] = new SixLabors.ImageSharp.Color(bmp[x, y]);
      }
    }
  }

  public Renderowanie(Scena rysownik)
  {
    this.rysownik = rysownik;

    teksturaKolory = null;
  }

  public void RenderujTrojkat(Vector3D[] wektor, double[] wektorNormalny, Vector2D[] wektorTekstura, double[,] zBufor)
  {
    for (int i = 0; i < wektorTekstura.Length; ++i)
    {
      wektorTekstura[i] = new Vector2D(wektorTekstura[i].X * bmp.Width, wektorTekstura[i].Y * bmp.Height);
    }

    IOrderedEnumerable<Vector3D> tmp = wektor.OrderBy(e => e.Y);
    Vector3D y0 = tmp.First();
    Vector3D y1 = tmp.Last();

    Vector3D[] graniczneX;
    double[] gradient;

    // Przechodź po obszarze figury od góry
    for (int y = (int)y0.Y; y <= y1.Y; ++y)
    {
      graniczneX = new Vector3D[3];
      gradient = new double[3];

      // Przechodź po wszystkich krawędziach trójkąta
      int k = 0;
      for (int i = 0; i < wektor.Length; ++i)
      {
        int j = (i + 1) % wektor.Length;
        double maxX = Math.Max(wektor[i].X, wektor[j].X);
        double minX = Math.Min(wektor[i].X, wektor[j].X);
        double maxY = Math.Max(wektor[i].Y, wektor[j].Y);
        double minY = Math.Min(wektor[i].Y, wektor[j].Y);

        double dxdy = (wektor[i].X - wektor[j].X) / (wektor[i].Y - wektor[j].Y);
        double x = dxdy * (y - wektor[i].Y) + wektor[i].X;

        if (x < minX || x > maxX || y < minY || y > maxY) { continue; }

        double m = (wektor[i].Y - y) / (wektor[i].Y - wektor[j].Y);
        double z = wektor[i].Z + (wektor[j].Z - wektor[i].Z) * m;
        double cos = wektorNormalny[i] + (wektorNormalny[j] - wektorNormalny[i]) * m;
        graniczneX[k] = new Vector3D(x, y, z);
        gradient[k] = cos;
        ++k;
      }

      if (k <= 1) { continue; }

      Vector3D x0 = graniczneX[0];
      Vector3D x1 = graniczneX[0];
      double vn0 = gradient[0];
      double vn1 = gradient[0];

      for (int i = 1; i < k; ++i)
      {
        if (x0.X > graniczneX[i].X)
        {
          x0 = graniczneX[i];
          vn0 = gradient[i];
        }

        if (x1.X < graniczneX[i].X)
        {
          x1 = graniczneX[i];
          vn1 = gradient[i];
        }
      }

      // Dla obliczonych par punktów przechodź w poziomie
      for (int x = (int)x0.X + 1; x <= x1.X; ++x)
      {
        double m = (x1.X - x) / (x1.X - x0.X);
        double z = x1.Z + (x0.Z - x1.Z) * m;
        double jasnosc = vn1 + (vn0 - vn1) * m;

        if (x < 0 || x >= zBufor.GetLength(0) || y < 0 || y >= zBufor.GetLength(1) || zBufor[x, y] < z || z <= 300) { continue; }

        double d10x = wektor[1].X - wektor[0].X;
        double d20y = wektor[2].Y - wektor[0].Y;
        double d10y = wektor[1].Y - wektor[0].Y;
        double d20x = wektor[2].X - wektor[0].X;
        double d0x = x - wektor[0].X;
        double d0y = y - wektor[0].Y;

        double mianownik = d10x * d20y - (d10y * d20x);
        double v = (d0x * d20y - d0y * d20x) / mianownik;
        double w = (d10x * d0y - d10y * d0x) / mianownik;
        double u = 1 - v - w;

        if (u < 0 || u > 1 || v < 0 || v > 1 || w < 0 || w > 1) { continue; }

        double tx = u * wektorTekstura[0].X + v * wektorTekstura[1].X + w * wektorTekstura[2].X;
        double ty = u * wektorTekstura[0].Y + v * wektorTekstura[1].Y + w * wektorTekstura[2].Y;

        double a = tx - Math.Floor(tx);
        double b = ty - Math.Floor(ty);

        int txx = (int)(tx + 1 < bmp.Width ? tx + 1 : tx);
        int tyy = (int)(ty + 1 < bmp.Height ? ty + 1 : ty);

        if (teksturaKolory == null)
        {
          var color = new Gdk.Color(
            (byte)(127 * jasnosc),
            (byte)(127 * jasnosc),
            (byte)(127 * jasnosc)
          );
          rysownik.RysujPiksel(new Vector2D(x, y), color);

          zBufor[x, y] = z;
          continue;
        }

        if (tx >= bmp.Width || ty >= bmp.Height) { continue; }

        var kolorP1 = teksturaKolory[(int)tx, (int)ty].ToPixel<Rgb24>();
        var kolorP2 = teksturaKolory[(int)tx, tyy].ToPixel<Rgb24>();
        var kolorP3 = teksturaKolory[txx, (int)ty].ToPixel<Rgb24>();
        var kolorP4 = teksturaKolory[txx, tyy].ToPixel<Rgb24>();

        double db = 1 - b;
        double da = 1 - a;

        var c = new Gdk.Color(
          (byte)((db * (da * kolorP1.R + a * kolorP3.R) + b * (da * kolorP2.R + a * kolorP4.R)) * jasnosc),
          (byte)((db * (da * kolorP1.G + a * kolorP3.G) + b * (da * kolorP2.G + a * kolorP4.G)) * jasnosc),
          (byte)((db * (da * kolorP1.B + a * kolorP3.B) + b * (da * kolorP2.B + a * kolorP4.B)) * jasnosc)
        );

        rysownik.RysujPiksel(new Vector2D(x, y), c);
        zBufor[x, y] = z;
      }
    }
  }

  public static double Jasnosc(Vector3D zrodlo, Vector3D wierzcholek, Vector3D srodek)
  {
    zrodlo -= srodek;
    wierzcholek -= srodek;

    return Math.Max(0, Math.Cos(zrodlo.AngleTo(wierzcholek).Radians));
  }
}
