using System;
using System.Linq;
using MathNet.Spatial.Euclidean;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Engine3D;

class Renderer
{
  private readonly Scene _drawer;
  private readonly Color[,] _textureColors;
  private readonly Image<Rgb24> _bitmap;

  public Renderer(string path, Scene drawer)
  {
    _bitmap = Image.Load(path).CloneAs<Rgb24>();
    _drawer = drawer;
    _textureColors = new Color[_bitmap.Width, _bitmap.Height];

    for (var y = 0; y < _bitmap.Height; ++y)
    {
      for (var x = 0; x < _bitmap.Width; ++x)
      {
        _textureColors[x, y] = new Color(_bitmap[x, y]);
      }
    }
  }

  public Renderer(Scene drawer)
  {
    _drawer = drawer;
    _textureColors = null;
  }

  public void RenderTriangle(Vector3D[] vertices, double[] normalVertex, Vector2D[] textureVertex, double[,] zBuffer)
  {
    var tmp = vertices.OrderBy(vertex => vertex.Y);
    var y0 = tmp.First();
    var y1 = tmp.Last();
    Vector3D[] borderXs;
    double[] gradient;

    for (var i = 0; i < textureVertex.Length; ++i)
    {
      textureVertex[i] =
        new Vector2D(
          x: textureVertex[i].X * _bitmap.Width,
          y: textureVertex[i].Y * _bitmap.Height
        );
    }

    for (var y = (int)y0.Y; y <= y1.Y; ++y)
    {
      borderXs = new Vector3D[3];
      gradient = new double[3];

      var k = 0;
      for (var i = 0; i < vertices.Length; ++i)
      {
        var j = (i + 1) % vertices.Length;
        var maxX = Math.Max(vertices[i].X, vertices[j].X);
        var minX = Math.Min(vertices[i].X, vertices[j].X);
        var maxY = Math.Max(vertices[i].Y, vertices[j].Y);
        var minY = Math.Min(vertices[i].Y, vertices[j].Y);
        var dXdY = (vertices[i].X - vertices[j].X) / (vertices[i].Y - vertices[j].Y);
        var x = dXdY * (y - vertices[i].Y) + vertices[i].X;

        if (
          x < minX ||
          x > maxX ||
          y < minY ||
          y > maxY
        )
        {
          continue;
        }

        var m = (vertices[i].Y - y) / (vertices[i].Y - vertices[j].Y);
        var z = vertices[i].Z + (vertices[j].Z - vertices[i].Z) * m;
        var cos = normalVertex[i] + (normalVertex[j] - normalVertex[i]) * m;
        borderXs[k] = new Vector3D(x, y, z);
        gradient[k] = cos;
        ++k;
      }

      if (k <= 1)
      {
        continue;
      }

      var x0 = borderXs[0];
      var x1 = borderXs[0];
      var vn0 = gradient[0];
      var vn1 = gradient[0];

      for (var i = 1; i < k; ++i)
      {
        if (x0.X > borderXs[i].X)
        {
          x0 = borderXs[i];
          vn0 = gradient[i];
        }

        if (x1.X < borderXs[i].X)
        {
          x1 = borderXs[i];
          vn1 = gradient[i];
        }
      }

      for (var x = (int)x0.X + 1; x <= x1.X; ++x)
      {
        var m = (x1.X - x) / (x1.X - x0.X);
        var z = x1.Z + (x0.Z - x1.Z) * m;
        var brightness = vn1 + (vn0 - vn1) * m;

        if (
          x < 0 ||
          x >= zBuffer.GetLength(0) ||
          y < 0 ||
          y >= zBuffer.GetLength(1) ||
          zBuffer[x, y] < z ||
          z <= 300
        )
        {
          continue;
        }

        var d10X = vertices[1].X - vertices[0].X;
        var d20Y = vertices[2].Y - vertices[0].Y;
        var d10Y = vertices[1].Y - vertices[0].Y;
        var d20X = vertices[2].X - vertices[0].X;
        var d0X = x - vertices[0].X;
        var d0Y = y - vertices[0].Y;

        var denominator = d10X * d20Y - (d10Y * d20X);

        var v = (d0X * d20Y - d0Y * d20X) / denominator;
        var w = (d10X * d0Y - d10Y * d0X) / denominator;
        var u = 1 - v - w;

        if (u < 0 || u > 1 || v < 0 || v > 1 || w < 0 || w > 1) { continue; }

        var tx = u * textureVertex[0].X + v * textureVertex[1].X + w * textureVertex[2].X;
        var ty = u * textureVertex[0].Y + v * textureVertex[1].Y + w * textureVertex[2].Y;

        var a = tx - Math.Floor(tx);
        var b = ty - Math.Floor(ty);

        var txx = (int)((tx + 1 < _bitmap.Width) ? (tx + 1) : tx);
        var tyy = (int)((ty + 1 < _bitmap.Height) ? (ty + 1) : ty);

        if (_textureColors == null)
        {
          var color = new Gdk.Color(
            r: (byte)(127 * brightness),
            g: (byte)(127 * brightness),
            b: (byte)(127 * brightness)
          );
          _drawer.DrawPixel(new Vector2D(x, y), color);

          zBuffer[x, y] = z;
          continue;
        }

        if (
          tx >= _bitmap.Width ||
          ty >= _bitmap.Height
        )
        {
          continue;
        }

        var colorP1 = _textureColors[(int)tx, (int)ty].ToPixel<Rgb24>();
        var colorP2 = _textureColors[(int)tx, tyy].ToPixel<Rgb24>();
        var colorP3 = _textureColors[txx, (int)ty].ToPixel<Rgb24>();
        var colorP4 = _textureColors[txx, tyy].ToPixel<Rgb24>();

        var db = 1 - b;
        var da = 1 - a;

        var pixelColor =
          new Gdk.Color(
            r: (byte)((db * (da * colorP1.R + a * colorP3.R) + b * (da * colorP2.R + a * colorP4.R)) * brightness),
            g: (byte)((db * (da * colorP1.G + a * colorP3.G) + b * (da * colorP2.G + a * colorP4.G)) * brightness),
            b: (byte)((db * (da * colorP1.B + a * colorP3.B) + b * (da * colorP2.B + a * colorP4.B)) * brightness)
          );

        _drawer.DrawPixel(new Vector2D(x, y), pixelColor);
        zBuffer[x, y] = z;
      }
    }
  }

  public static double Brightness(Vector3D srcVertex, Vector3D dstVertex, Vector3D center)
  {
    srcVertex -= center;
    dstVertex -= center;

    return Math.Max(0, Math.Cos(srcVertex.AngleTo(dstVertex).Radians));
  }
}
