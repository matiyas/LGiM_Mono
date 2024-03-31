using Drawing = System.Drawing;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Engine3D;

class Scene
{
  private static Gdk.Color _blackColor = new(0, 0, 0);
  private static Gdk.Color _greenColor = new(0, 255, 0);
  private static Gdk.Color _whiteColor = new(255, 255, 255);

  private readonly byte[] _background;
  private readonly double[,] _zBuffer;
  private readonly Vector2D _center;

  public Scene(string backgroundPath, Drawing.Size size, double distance, double minDistance)
  {
    Size = size;
    _center =
      new Vector2D(
        x: size.Width / 2,
        y: size.Height / 2
      );
    BackBuffer = new byte[4 * size.Width * size.Height];

    _background = ToByteArray(backgroundPath);
    _background.CopyTo(BackBuffer, 0);

    World = new List<WavefrontObj>();
    Camera = new Camera();
    Distance = distance;
    _zBuffer = new double[size.Width, size.Height];

    ClearZBuffer();

    MinDistance = minDistance;
  }

  public List<WavefrontObj> World { get; set; }
  public Camera Camera { get; set; }
  public int LightSourceIndex { get; set; } = -1;
  public Vector3D LightSource { get; set; }
  public byte[] BackBuffer { get; private set; }
  public Drawing.Size Size { get; private set; }
  public Gdk.Color BrushColor { get; set; } = _blackColor;
  public Gdk.Color BackgroundColor { get; set; } = _whiteColor;
  public double Distance { get; set; }
  public double MinDistance { get; set; }

  public void DrawPixel(Vector2D vertex, Gdk.Color color)
  {
    if (
      vertex.X < 0 ||
      vertex.X >= Size.Width ||
      vertex.Y < 0 ||
      vertex.Y >= Size.Height
    )
    {
      return;
    }

    var position = 4 * ((int)vertex.Y * Size.Width + (int)vertex.X);
    BackBuffer[position] = (byte)color.Blue;
    BackBuffer[position + 1] = (byte)color.Green;
    BackBuffer[position + 2] = (byte)color.Red;
    //BackBuffer[position + 3] = (byte)color.A;
  }

  public void DrawLine(Vector3D lineStart, Vector3D lineEnd, Gdk.Color color)
  {
    var startX = lineStart.X < lineEnd.X ? lineStart : lineEnd;
    var endX = lineStart.X > lineEnd.X ? lineStart : lineEnd;
    var startY = lineStart.Y < lineEnd.Y ? lineStart : lineEnd;
    var endY = lineStart.Y > lineEnd.Y ? lineStart : lineEnd;

    var dx = (int)(endX.X - startX.X);
    var dy = (int)(endY.Y - startY.Y);

    if (dx > dy)
    {
      var step = (startX.Z - endX.Z) / dx;
      var z = startX.Z;

      for (var x = (int)startX.X; x <= endX.X; ++x)
      {
        var y = dy / (double)dx * (x - lineStart.X) + lineStart.Y;

        if (
          (lineEnd.X > lineStart.X && lineEnd.Y > lineStart.Y) ||
          (lineEnd.X < lineStart.X && lineEnd.Y < lineStart.Y)
        )
        {
          if (
            x >= 0 &&
            x < _zBuffer.GetLength(0) &&
            y >= 0 &&
            y < _zBuffer.GetLength(1) &&
            _zBuffer[x, (int)y] > z && z > MinDistance
          )
          {
            DrawPixel(new Vector2D(x, y), color);
          }
        }
        else if (
          x >= 0 &&
          x < _zBuffer.GetLength(0) &&
          2 * lineStart.Y - y >= 0 &&
          2 * lineStart.Y - y < _zBuffer.GetLength(1) &&
          _zBuffer[x, (int)(2 * lineStart.Y - y)] > z &&
          z > MinDistance
        )
        {
          DrawPixel(new Vector2D(x, 2 * lineStart.Y - y), color);
        }

        y += step;
      }
    }
    else
    {
      var step = (startY.Z - endY.Z) / dy;
      var z = startY.Z;

      for (int y = (int)startY.Y; y <= endY.Y; ++y)
      {
        var x = dx / (double)dy * (y - lineStart.Y) + lineStart.X;

        if (
          (lineEnd.X > lineStart.X && lineEnd.Y > lineStart.Y) ||
          (lineEnd.X < lineStart.X && lineEnd.Y < lineStart.Y)
        )
        {
          if (
            x >= 0 &&
            x < _zBuffer.GetLength(0) &&
            y >= 0 &&
            y < _zBuffer.GetLength(1) &&
            _zBuffer[(int)x, y] > z &&
            z > MinDistance
          )
          {
            DrawPixel(new Vector2D(x, y), color);
          }
        }
        else if (
          2 * lineStart.X - x >= 0 &&
          2 * lineStart.X - x < _zBuffer.GetLength(0) &&
          y >= 0 &&
          y < _zBuffer.GetLength(1) &&
          _zBuffer[(int)(2 * lineStart.X - x), y] > z &&
          z > MinDistance
        )
        {
          DrawPixel(new Vector2D(2 * lineStart.X - x, y), color);
        }

        z += step;
      }
    }
  }

  public void ClearZBuffer()
  {
    for (var i = 0; i < _zBuffer.GetLength(0); ++i)
    {
      for (var j = 0; j < _zBuffer.GetLength(1); ++j)
      {
        _zBuffer[i, j] = double.PositiveInfinity;
      }
    }
  }

  public void ClearScreen()
  {
    for (int i = 0; i < BackBuffer.Length; i += 4)
    {
      BackBuffer[i] = (byte)BackgroundColor.Blue;
      BackBuffer[i + 1] = (byte)BackgroundColor.Green;
      BackBuffer[i + 2] = (byte)BackgroundColor.Red;
      //BackBuffer[i + 3] = (byte)BackgroundColor.A;
    }
  }

  public void DrawFloorMesh(
    int width,
    int height,
    int step,
    Gdk.Color meshColor,
    Gdk.Color axisXColor,
    Gdk.Color axisZColor
  )
  {
    for (var z = -height / 2; z < height / 2; z += step)
    {
      for (var x = -width / 2; x < width / 2; x += step)
      {
        var vertices =
          new Vector3D[]
          {
            new Vector3D(x, 0, z).PerspectiveView(Distance, _center, Camera),
            new Vector3D(x + step, 0, z).PerspectiveView(Distance, _center, Camera),
            new Vector3D(x + step, 0, z + step).PerspectiveView(Distance, _center, Camera),
            new Vector3D(x, 0, z + step).PerspectiveView(Distance, _center, Camera)
          };

        for (int i = 0; i < vertices.Length; ++i)
        {
          if (
            vertices[i].Z <= MinDistance ||
            vertices[(i + 1) % vertices.Length].Z <= MinDistance
          )
          {
            continue;
          }

          Gdk.Color color;

          if (x == 0 && i == 3)
          {
            color = axisZColor;
          }
          else if (z == 0 && i == 0)
          {
            color = axisXColor;
          }
          else
          {
            color = meshColor;
          }

          DrawLine(
            lineStart: vertices[i],
            lineEnd: vertices[(i + 1) % vertices.Length],
            color: color
          );
        }
      }
    }
  }

  public void DrawMesh()
  {
    ClearScreen();
    ClearZBuffer();

    foreach (var model in World)
    {
      var modelPerspectiveView = model.VertexCoords.PerspectiveView(Distance, _center, Camera);

      foreach (var surface in model.Sciany)
      {
        for (var i = 0; i < surface.Vertex.Length; ++i)
        {
          DrawLine(
            lineStart: modelPerspectiveView[surface.Vertex[i]],
            lineEnd: modelPerspectiveView[surface.Vertex[(i + 1) % surface.Vertex.Length]],
            color: _greenColor
          );
        }
      }
    }
  }

  public void Render()
  {
    _background.CopyTo(BackBuffer, 0);
    ClearZBuffer();

    foreach (var model in World)
    {
      var modelPerspectiveView = Math3DExtensions.PerspectiveView(model.VertexCoords, Distance, _center, Camera);
      var modelCenter = model.VertexNormalsCoords.FindCenter();

      if (
        model.Sciany == null ||
        modelPerspectiveView == null ||
        model.Renderowanie == null
      )
      {
        continue;
      }

      foreach (var surface in model.ScianyTrojkatne)
      {
        if (
          modelPerspectiveView[surface.Vertex[0]].Z <= MinDistance &&
          modelPerspectiveView[surface.Vertex[1]].Z <= MinDistance &&
          modelPerspectiveView[surface.Vertex[2]].Z <= MinDistance
        )
        {
          continue;
        }

        double[] gradient;

        if (World.IndexOf(model) != LightSourceIndex)
        {
          gradient =
            new double[]
            {
              Renderer.Brightness(LightSource, model.VertexNormalsCoords[surface.VertexNormal[0]], modelCenter),
              Renderer.Brightness(LightSource, model.VertexNormalsCoords[surface.VertexNormal[1]], modelCenter),
              Renderer.Brightness(LightSource, model.VertexNormalsCoords[surface.VertexNormal[2]], modelCenter),
            };
        }
        else
        {
          gradient = new double[] { 1, 1, 1 };
        }

        var surfaceVertices =
          new Vector3D[]
          {
            modelPerspectiveView[surface.Vertex[0]],
            modelPerspectiveView[surface.Vertex[1]],
            modelPerspectiveView[surface.Vertex[2]]
          };
        Vector2D[] texture;

        if (
          surface.VertexTexture[0] >= 0 &&
          surface.VertexTexture[1] >= 0 &&
          surface.VertexTexture[2] >= 0
        )
        {
          texture =
            new Vector2D[]
            {
              model.VertexTextureCoords[surface.VertexTexture[0]],
              model.VertexTextureCoords[surface.VertexTexture[1]],
              model.VertexTextureCoords[surface.VertexTexture[2]]
            };
        }
        else
        {
          texture =
            new Vector2D[] {
              new(0, 0),
              new(0, 0),
              new(0, 0)
            };
        }

        model.Renderowanie.RenderTriangle(
          vertices: surfaceVertices,
          normalVertex: gradient,
          textureVertex: texture,
          zBuffer: _zBuffer
        );
      }
    }
  }

  public static byte[] ToByteArray(string path)
  {
    var bitmap = Image.Load(path).CloneAs<Rgba32>();
    var pixels = new byte[bitmap.Size.Width * bitmap.Height * 4];

    for (var x = 0; x < bitmap.Width; ++x)
    {
      for (var y = 0; y < bitmap.Height; ++y)
      {
        var pos = 4 * (y * bitmap.Width + x);
        var color = bitmap[x, y];

        pixels[pos++] = color.B;
        pixels[pos++] = color.G;
        pixels[pos++] = color.R;
        pixels[pos] = color.A;
      }
    }

    return pixels;
  }
}
