using MathNet.Spatial.Euclidean;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Engine3D;

class Camera
{
  private Vector3D _position;
  private Vector3D _target;
  private UnitVector3D _forward;
  private UnitVector3D _upward;
  private UnitVector3D _right;

  public Camera()
  {
    _position = new Vector3D(0, 0, 10);
    _target = new Vector3D(0, 0, 0);
    _forward = UnitVector3D.Create(0, 0, 1);
    _upward = UnitVector3D.Create(0, 1, 0);
    _right = UnitVector3D.Create(1, 0, 0);
  }

  public Camera(Vector3D position, Vector3D target)
  {
    _position = position;
    _target = target;

    Recalculate();
  }

  void Recalculate()
  {
    _forward = (_position - _target).Normalize();
    _right = _upward.CrossProduct(_forward);
    _upward = _forward.CrossProduct(_right);
  }

  public Vector3D Position
  {
    get => _position;
    set
    {
      _position = value;
      Recalculate();
    }
  }

  public Vector3D Target
  {
    get => _target;
    set
    {
      _target = value;
      Recalculate();
    }
  }

  public UnitVector3D Forward => _forward;

  public UnitVector3D Right => _right;

  public UnitVector3D Upward => _upward;

  public DenseMatrix LookAt
  {
    get
    {
      var translationMatrixStorage = new double[]
      {
        1, 0, 0, -Position.X,
        0, 1, 0, -Position.Y,
        0, 0, 1, -Position.Z,
        0, 0, 0, 1
      };
      var viewMatrixStorage = new double[]
      {
        Right.X, Right.Y, Right.Z, 0,
        Upward.X, Upward.Y, Upward.Z, 0,
        Forward.X, Forward.Y, Forward.Z, 0,
        0, 0, 0, 1
      };

      var translationMatrix = new DenseMatrix(4, 4, translationMatrixStorage);
      var viewMatrix = new DenseMatrix(4, 4, viewMatrixStorage);

      return viewMatrix * translationMatrix;
    }
  }

  public void GoForward(double value)
  {
    UnitVector3D forward = UnitVector3D.Create(0, 0, 1);
    _position -= new Vector3D(forward.X * value, forward.Y * value, forward.Z * -value);
    _target -= new Vector3D(forward.X * value, forward.Y * value, forward.Z * -value);
  }

  public void GoSideways(double value)
  {
    UnitVector3D right = UnitVector3D.Create(1, 0, 0);
    _position -= new Vector3D(right.X * value, right.Y * value, right.Z * -value);
    _target -= new Vector3D(right.X * value, right.Y * value, right.Z * -value);
  }

  public void GoUpward(double value)
  {
    UnitVector3D upward = UnitVector3D.Create(0, 1, 0);
    _position -= new Vector3D(upward.X * value, upward.Y * value, upward.Z * -value);
    _target -= new Vector3D(upward.X * value, upward.Y * value, upward.Z * -value);
  }

  public void Rotate(Vector3D angle)
  {
    _forward = Math3DExtensions.RotateAroundAxis(_forward, _upward, -angle.Y);
    _right = _upward.CrossProduct(_forward);

    _forward = Math3DExtensions.RotateAroundAxis(_forward, _right, -angle.X);
    _upward = _forward.CrossProduct(_right);

    _right = Math3DExtensions.RotateAroundAxis(_right, _forward, -angle.Z);
    _upward = _forward.CrossProduct(_right);

    _target = _position - _forward;
  }
}
