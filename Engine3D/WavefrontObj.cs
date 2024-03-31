using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using MathNet.Spatial.Euclidean;
using System;

namespace Engine3D;

public struct Surface
{
  public int[] Vertex { get; set; }
  public int[] VertexTexture { get; set; }
  public int[] VertexNormal { get; set; }
}

class WavefrontObj
{
  public WavefrontObj(string path)
  {
    string line;
    List<double> vertex;

    VertexCoords = Array.Empty<Vector3D>();
    VertexNormalsCoords = Array.Empty<Vector3D>();
    VertexTextureCoords = Array.Empty<Vector2D>();
    Surfaces = Array.Empty<Surface>();

    using (var streamReader = new StreamReader(path))
    {
      while ((line = streamReader.ReadLine()) != null)
      {
        string[] values = line.Split(null);
        switch (values[0])
        {
          case "o":
            Name = values[1];
            break;

          case "v":
          case "vn":
            vertex = new List<double>();

            foreach (var value in values.Skip(1))
            {
              try
              {
                vertex.Add(double.Parse(value, CultureInfo.InvariantCulture) * 100);
              }
              catch
              {
                continue;
              }
            }

            if (values[0] == "v")
            {
              VertexCoords =
                AddToEnd(
                  table: VertexCoords,
                  value:
                    new Vector3D(
                      x: vertex[0],
                      y: vertex[1],
                      z: vertex[2]
                    )
                );
            }
            else if (values[0] == "vn")
            {
              VertexNormalsCoords =
                AddToEnd(
                  table: VertexNormalsCoords,
                  value:
                    new Vector3D(
                      x: vertex[0],
                      y: vertex[1],
                      z: vertex[2]
                    )
                  );
            }
            break;

          case "vt":
            VertexTextureCoords =
              AddToEnd(
                table: VertexTextureCoords,
                value:
                  new Vector2D(
                    x: double.Parse(values[1], CultureInfo.InvariantCulture),
                    y: double.Parse(values[2], CultureInfo.InvariantCulture)
                  )
                );
            break;

          case "f":
            values = values.Skip(1).ToArray();

            var surface = new Surface()
            {
              Vertex = Array.Empty<int>(),
              VertexTexture = Array.Empty<int>(),
              VertexNormal = Array.Empty<int>()
            };

            foreach (var value in values)
            {
              try
              {
                var intValue = int.Parse(value.Split('/')[0]);
                var tmp = surface.Vertex;

                Array.Resize(ref tmp, tmp.Length + 1);

                tmp[^1] = intValue;
                surface.Vertex = tmp;
              }
              catch
              {
                continue;
              }

              if (int.TryParse(value.Split('/')[1], out int vt) == false)
              {
                continue;
              }
              else
              {
                var tmp = surface.VertexTexture;

                Array.Resize(ref tmp, tmp.Length + 1);

                tmp[^1] = vt;
                surface.VertexTexture = tmp;
              }

              if (value.Split('/').ToArray().Length == 3)
              {
                var intValue = int.Parse(value.Split('/')[2]);
                var tmp = surface.VertexNormal;

                Array.Resize(ref tmp, tmp.Length + 1);

                tmp[^1] = intValue;
                surface.VertexNormal = tmp;
              }
            }

            Surfaces = AddToEnd(Surfaces, surface);
            //Surfaces.Add(surface);
            break;
        }
      }
    }

    for (int i = 0; i < Surfaces.Length; ++i)
    {
      var vertices = new int[Surfaces[i].Vertex.Length];
      for (var j = 0; j < Surfaces[i].Vertex.Length; ++j)
      {
        var value = Surfaces[i].Vertex[j];
        vertices[j] = (value > 0) ? (value - 1) : (VertexCoords.Length + value);
      }

      var normalVertices = new int[Surfaces[i].VertexNormal.Length];
      for (var j = 0; j < Surfaces[i].VertexNormal.Length; ++j)
      {
        var value = Surfaces[i].VertexNormal[j];
        normalVertices[j] = (value > 0) ? (value - 1) : (VertexNormalsCoords.Length + value);
      }

      var verticesText = new int[Surfaces[i].VertexTexture.Length];
      for (var j = 0; j < Surfaces[i].VertexTexture.Length; ++j)
      {
        var value = Surfaces[i].VertexTexture[j];
        verticesText[j] = (value > 0) ? (value - 1) : (VertexTextureCoords.Length + value);
      }

      Surfaces[i] =
        new Surface()
        {
          Vertex = vertices,
          VertexNormal = normalVertices,
          VertexTexture = verticesText
        };
    }

    TriangularSurfaces = Array.Empty<Surface>();
    foreach (var surface in Surfaces)
    {
      for (var i = 0; i < surface.Vertex.Length; i += 2)
      {
        var surfaceVertex =
          new int[] {
            surface.Vertex[i],
            surface.Vertex[(i + 1) % surface.Vertex.Length],
            surface.Vertex[(i + 2) % surface.Vertex.Length]
          };
        var vertexTexture =
          new int[] {
            surface.VertexTexture[i],
            surface.VertexTexture[(i + 1) % surface.Vertex.Length],
            surface.VertexTexture[(i + 2) % surface.Vertex.Length]
          };
        var vertexNormal =
          new int[] {
            surface.VertexNormal[i],
            surface.VertexNormal[(i + 1) % surface.Vertex.Length],
            surface.VertexNormal[(i + 2) % surface.Vertex.Length]
          };
        var triangularSurface =
          new Surface()
          {
            Vertex = surfaceVertex,
            VertexTexture = vertexTexture,
            VertexNormal = vertexNormal
          };


        TriangularSurfaces = AddToEnd(TriangularSurfaces, triangularSurface);
      }
    }
  }

  private static T[] AddToEnd<T>(T[] table, T value)
  {
    T[] tmp = table;
    Array.Resize(ref tmp, tmp.Length + 1);
    tmp[^1] = value;

    return tmp;
  }

  public Vector3D Scaling { get; set; }
  public Vector3D Translation { get; set; }
  public Vector3D Rotation { get; set; }
  public Vector3D[] VertexCoords { get; private set; }
  public Vector2D[] VertexTextureCoords { get; }
  public Vector3D[] VertexNormalsCoords { get; private set; }
  public Surface[] Surfaces { get; }
  public Surface[] TriangularSurfaces { get; }
  public Renderer Rendering { get; set; }
  public string Name { get; private set; }

  public void Translate(Vector3D translationVector)
  {
    VertexCoords = VertexCoords.Transform(translationVector);
    VertexNormalsCoords = VertexNormalsCoords.Transform(translationVector);
  }

  public void Rotate(Vector3D rotationVector)
  {
    VertexCoords = VertexCoords.Rotate(rotationVector, VertexCoords.FindCenter());
    VertexNormalsCoords = VertexNormalsCoords.Rotate(rotationVector, VertexNormalsCoords.FindCenter());
  }

  public void Rotate(Vector3D rotationVector, Vector3D center)
  {
    VertexCoords = VertexCoords.Rotate(rotationVector, center);
    VertexNormalsCoords = VertexNormalsCoords.Rotate(rotationVector, center);
  }

  public void RotateAroundAxis(double angle, UnitVector3D axis, Vector3D center)
  {
    VertexCoords = Math3DExtensions.RotateAroundAxis(VertexCoords, axis, angle, center);
    VertexNormalsCoords = Math3DExtensions.RotateAroundAxis(VertexNormalsCoords, axis, angle, center);
  }

  public void Scale(Vector3D scaleVector)
  {
    VertexCoords = VertexCoords.Scaling(scaleVector);
    VertexNormalsCoords = VertexNormalsCoords.Scaling(scaleVector);
  }
}
