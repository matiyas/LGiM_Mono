using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using static System.Math;

namespace Engine3D;

static class Math3D
{
  public static Vector3D FindCenter(this Vector3D[] vertices)
  {
    return new Vector3D(
      x: (vertices.Max(v => v.X) + vertices.Min(v => v.X)) / 2,
      y: (vertices.Max(v => v.Y) + vertices.Min(v => v.Y)) / 2,
      z: (vertices.Max(v => v.Z) + vertices.Min(v => v.Z)) / 2
    );
  }

  public static Vector3D[] Transform(this Vector3D[] vertices, Vector3D translation)
  {
    var transformedVertices = new Vector3D[vertices.Length];
    var translationMatrix =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, translation.X,
          0, 1, 0, translation.Y,
          0, 0, 1, translation.Z,
          0, 0, 0, 1
        }
      );

    for (int i = 0; i < vertices.Length; ++i)
    {
      var vectorWithHomogeneousCoordsStorage =
        new double[]
        {
          vertices[i].X,
          vertices[i].Y,
          vertices[i].Z,
          1
        };
      var vectorWithHomogeneousCoords = new DenseVector(vectorWithHomogeneousCoordsStorage);
      var transformedVector = vectorWithHomogeneousCoords * translationMatrix;

      transformedVertices[i] =
        new Vector3D(
          x: transformedVector[0],
          y: transformedVector[1],
          z: transformedVector[2]
        );
    }

    return transformedVertices;
  }

  public static Vector3D[] Rotate(this Vector3D[] vertices, Vector3D angle, Vector3D center)
  {
    var transformedAngle =
      new Vector3D(
        x: +angle.X / 100,
        y: angle.Y / 100,
        z: angle.Z / 100
      );
    var transformedVertices = new Vector3D[vertices.Length];
    var transformationMatrix1 =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, -center.X,
          0, 1, 0, -center.Y,
          0, 0, 1, -center.Z,
          0, 0, 0, 1
        }
      );
    var transformationMatrix2 =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, center.X,
          0, 1, 0, center.Y,
          0, 0, 1, center.Z,
          0, 0, 0, 1
        }
      );
    var rotationMatrixX =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, 0,
          0, Cos(transformedAngle.X), -Sin(transformedAngle.X), 0,
          0, Sin(transformedAngle.X), Cos(transformedAngle.X), 0,
          0, 0, 0, 1
        }
      );
    var rotationMatrixY =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          Cos(transformedAngle.Y), 0, Sin(transformedAngle.Y), 0,
          0, 1, 0, 0,
          -Sin(transformedAngle.Y), 0, Cos(transformedAngle.Y), 0,
          0, 0, 0, 1
        }
      );
    var rotationMatrixZ =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          Cos(transformedAngle.Z), -Sin(transformedAngle.Z), 0, 0,
          Sin(transformedAngle.Z), Cos(transformedAngle.Z), 0, 0,
          0, 0, 1, 0,
          0, 0, 0, 1
        }
      );

    for (int i = 0; i < vertices.Length; ++i)
    {
      var vertexWithHomogeneousCoordsStorage =
        new double[]
        {
          vertices[i].X,
          vertices[i].Y,
          vertices[i].Z,
          1
        };
      var transformedVertex = new DenseVector(vertexWithHomogeneousCoordsStorage) * transformationMatrix1;

      if (angle.X.CompareTo(0) != 0) transformedVertex *= rotationMatrixX;
      if (angle.Y.CompareTo(0) != 0) transformedVertex *= rotationMatrixY;
      if (angle.Z.CompareTo(0) != 0) transformedVertex *= rotationMatrixZ;

      transformedVertex *= transformationMatrix2;
      transformedVertices[i] =
        new Vector3D(
          x: transformedVertex[0],
          y: transformedVertex[1],
          z: transformedVertex[2]
        );
    }

    return transformedVertices;
  }

  public static Vector3D[] Scaling(this Vector3D[] vertices, Vector3D angle)
  {
    var tmpX = angle.X;
    var tmpY = angle.Y;
    var tmpZ = angle.Z;
    angle =
      new Vector3D(
        x: angle.X / 100.0,
        y: angle.Y / 100.0,
        z: angle.Z / 100
      );
    angle =
      new Vector3D(
        x: (tmpX >= 0 || angle.X < 1.0) ? (angle.X + 1) : (1.0 / angle.X),
        y: (tmpY >= 0 || angle.Y < 1.0) ? (angle.Y + 1) : (1.0 / angle.Y),
        z: (tmpZ >= 0 || angle.Z < 1.0) ? (angle.Z + 1) : (1.0 / angle.Z)
      );
    var transformedVertices = new Vector3D[vertices.Length];
    var centerVertex = vertices.FindCenter();
    var transformationMatrix1 =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, -centerVertex.X,
          0, 1, 0, -centerVertex.Y,
          0, 0, 1, -centerVertex.Z,
          0, 0, 0, 1
        }
      );
    var transformationMatrix2 =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, centerVertex.X,
          0, 1, 0, centerVertex.Y,
          0, 0, 1, centerVertex.Z,
          0, 0, 0, 1
        }
      );
    var scaleMatrix =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          angle.X, 0, 0, 0,
          0, angle.Y, 0, 0,
          0, 0, angle.Z, 0,
          0, 0, 0, 1
        }
      );

    for (int i = 0; i < vertices.Length; ++i)
    {
      var vectorWithHomogeneousCoordsStorage =
        new double[]
        {
          vertices[i].X,
          vertices[i].Y,
          vertices[i].Z,
          1
        };
      var vectorWithHomogeneousCoords = new DenseVector(vectorWithHomogeneousCoordsStorage);
      var transformedVertex = vectorWithHomogeneousCoords * transformationMatrix1 * scaleMatrix * transformationMatrix2;

      transformedVertices[i] =
        new Vector3D(
          x: transformedVertex[0],
          y: transformedVertex[1],
          z: transformedVertex[2]
        );
    }

    return transformedVertices;
  }

  public static Vector3D PerspectiveView(this Vector3D vertex, double distance, Vector2D center, Camera camera)
  {
    var projectionMatrix =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, 0,
          0, 1, 0, 0,
          0, 0, 1, 0,
          0, 0, 1 / distance, 1
        }
      );
    var vectorWithHomogenousCoords = new DenseVector(new double[] { vertex.X, vertex.Y, vertex.Z, 1 });
    var transformedPoint = vectorWithHomogenousCoords * camera.LookAt * projectionMatrix;

    return new Vector3D(
      x: transformedPoint[0] / transformedPoint[3] + center.X,
      y: transformedPoint[1] / transformedPoint[3] + center.Y,
      z: transformedPoint[2] + distance
    );
  }

  public static Vector3D[] PerspectiveView(this Vector3D[] vertices, double distance, Vector2D center, Camera camera)
  {
    var transformedVertices = new Vector3D[vertices.Length];

    for (int i = 0; i < vertices.Length; ++i)
    {
      transformedVertices[i] = PerspectiveView(vertices[i], distance, center, camera);
    }

    return transformedVertices;
  }

  public static Vector3D RotateAroundAxis(Vector3D vertex, UnitVector3D axis, double angle, Vector3D center)
  {
    angle /= 100;

    var transformationMatrix1 =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, -center.X,
          0, 1, 0, -center.Y,
          0, 0, 1, -center.Z,
          0, 0, 0, 1
        }
      );
    var transformationMatrix2 =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new double[]
        {
          1, 0, 0, center.X,
          0, 1, 0, center.Y,
          0, 0, 1, center.Z,
          0, 0, 0, 1
        }
      );
    var rotationMatrixRow1 =
      new double[]
      {
        axis.X * axis.X * (1 - Cos(angle)) + Cos(angle),
        axis.X * axis.Y * (1 - Cos(angle)) - axis.Z * Sin(angle),
        axis.X * axis.Z * (1 - Cos(angle)) + axis.Y * Sin(angle),
        0
      };
    var rotationMatrixRow2 =
      new double[]
      {
        axis.X * axis.Y * (1 - Cos(angle)) + axis.Z * Sin(angle),
        axis.Y * axis.Y * (1 - Cos(angle)) + Cos(angle),
        axis.Y * axis.Z * (1 - Cos(angle)) - axis.X * Sin(angle),
        0
      };
    var rotationMatrixRow3 =
      new double[]
      {
        axis.X * axis.Z * (1 - Cos(angle)) - axis.Y * Sin(angle),
        axis.Y * axis.Z * (1 - Cos(angle)) + axis.X * Sin(angle),
        axis.Z * axis.Z * (1 - Cos(angle)) + Cos(angle),
        0
      };
    var rotationMatrixRow4 = new double[] { 0, 0, 0, 1 };
    var rotationMatrix =
      new DenseMatrix(
        rows: 4,
        columns: 4,
        storage: new[]
        {
          rotationMatrixRow1,
          rotationMatrixRow2,
          rotationMatrixRow3,
          rotationMatrixRow4
        }.SelectMany(row => row).ToArray()
      );
    var vectorWithHomogeneousCoords = new DenseVector(new double[] { vertex.X, vertex.Y, vertex.Z, 1 });
    var transformedVertex =
      vectorWithHomogeneousCoords *
      transformationMatrix1 *
      rotationMatrix *
      transformationMatrix2;

    return new Vector3D(
      x: transformedVertex[0],
      y: transformedVertex[1],
      z: transformedVertex[2]
    );
  }

  public static UnitVector3D RotateAroundAxis(UnitVector3D vertex, UnitVector3D axis, double angle)
  {
    var transformedVertex =
      RotateAroundAxis(
        vertex: new Vector3D(vertex.X, vertex.Y, vertex.Z),
        axis: axis,
        angle: angle,
        center: new Vector3D(0, 0, 0)
      );

    return new Vector3D(
      x: transformedVertex.X,
      y: transformedVertex.Y,
      z: transformedVertex.Z
    ).Normalize();
  }

  public static UnitVector3D RotateAroundAxis(UnitVector3D vertex, UnitVector3D axis, double angle, Vector3D center)
  {
    var transformedVertex =
      RotateAroundAxis(
        vertex: new Vector3D(vertex.X, vertex.Y, vertex.Z),
        axis: axis,
        angle: angle,
        center: center
      );

    return new Vector3D(
      x: transformedVertex.X,
      y: transformedVertex.Y,
      z: transformedVertex.Z
    ).Normalize();
  }

  public static Vector3D[] RotateAroundAxis(Vector3D[] vertices, UnitVector3D axis, double angle, Vector3D center)
  {
    var transformedVertices = new Vector3D[vertices.Length];

    for (int i = 0; i < vertices.Length; ++i)
    {
      transformedVertices[i] = RotateAroundAxis(vertices[i], axis, angle, center);
    }

    return transformedVertices;
  }
}
