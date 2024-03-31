using System;
using Gdk;
using Gtk;
using MathNet.Spatial.Euclidean;
using UI = Gtk.Builder.ObjectAttribute;

namespace Engine3D;

[Flags]
public enum State
{
  None = 0,
  LeftMouseBtn = 1,
  RightMouseBtn = 2,
  Shift = 4
};

public enum Mode
{
  Move,
  Scaling,
  Rotating
};

public partial class MainWindow : Gtk.Window
{
  private static Color _black = new(0, 0, 0);
  private static Color _green = new(0, 255, 0);
  private static Color _gray = new(127, 127, 127);
  private static Color _blue = new(0, 0, 255);
  private static Color _red = new(255, 0, 0);

  private readonly Scene _scene;
  private readonly double _mouseSensitivity;
  private Point _leftMouseBtn, _rightMouseBtn;
  private Mode _mode;
  private State _state;

  [UI] private readonly Label _labelEditMode = null;
  [UI] private readonly Image _imageScreen = null;
  [UI] private readonly CheckButton _checkButtonMesh = null;
  [UI] private readonly CheckButton _checkButtonFloorMesh = null;
  [UI] private readonly ComboBoxText _comboBoxModels = null;

  public MainWindow() : this(new Builder("MainWindow.glade")) { }

  private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
  {
    builder.Autoconnect(this);

    _state = Engine3D.State.None;
    _mouseSensitivity = 0.3;
    _labelEditMode.Text = _mode.ToString();
    _comboBoxModels.Active = 0;

    var backgroundPath = @"background.jpg";
    _scene = new Scene(backgroundPath, GetImageDimensions(backgroundPath), 1000, 100)
    {
      BrushColor = _green,
      BackgroundColor = _black,
    };

    LoadModel(@"Assets/Models/monkey.obj", @"Assets/Textures/sun.jpg");
    LoadModel(@"Assets/Models/monkey.obj", @"Assets/Textures/earth.jpg");

    _scene.World[1].Translate(new Vector3D(500, 0, 0));
    _scene.World[1].Scale(new Vector3D(-50, -50, -50));
    _scene.LightSourceIndex = 0;

    GLib.Timeout.Add(50, new GLib.TimeoutHandler(OnUpdate));

    KeyPressEvent += OnKeyPressEvent;
  }

  private static System.Drawing.Size GetImageDimensions(string imagePath)
  {
    var image = SixLabors.ImageSharp.Image.Load(imagePath);

    return new System.Drawing.Size(image.Width, image.Height);
  }

  protected bool OnUpdate()
  {
    _scene.LightSource = _scene.World[_scene.LightSourceIndex].VertexCoords.FindCenter();

    DrawOnScreen();

    return true;
  }

  protected void OnDeleteEvent(object sender, DeleteEventArgs a)
  {
    Application.Quit();
    a.RetVal = true;
  }

  void LoadModel(string modelPath, string texturePath)
  {
    _scene.World.Add(new WavefrontObj(modelPath));

    if (texturePath != null)
    {
      _scene.World[^1].Rendering = new Renderer(texturePath, _scene);
    }
    _comboBoxModels.AppendText(_scene.World[^1].Name ?? "Model" + (_scene.World.Count - 1));
    _comboBoxModels.Active = _scene.World.Count - 1;
    _scene.World[^1].Rotate(new Vector3D(Math.PI * 100, 0, 0));
  }

  void DrawOnScreen()
  {
    if (_checkButtonMesh.Active == false)
    {
      _scene.Render();
    }
    else
    {
      _scene.DrawMesh();
    }

    if (_checkButtonFloorMesh.Active == true)
    {
      _scene.DrawFloorMesh(2000, 2000, 100, _gray, _blue, _red);
    }

    _imageScreen.Pixbuf =
      new Pixbuf(
        data: _scene.BackBuffer,
        colorspace: Colorspace.Rgb,
        has_alpha: true,
        bits_per_sample: 8,
        width: _scene.Size.Width,
        height: _scene.Size.Height,
        rowstride: 4 * _scene.Size.Width
      );
  }

  private void OnKeyPressEvent(object _sender, KeyPressEventArgs eventArgs)
  {
    switch (eventArgs.Event.Key)
    {
      case Gdk.Key.w:
        _scene.Camera.GoForward(50);
        break;

      case Gdk.Key.s:
        _scene.Camera.GoForward(-50);
        break;

      case Gdk.Key.a:
        _scene.Camera.GoSideways(50);
        break;

      case Gdk.Key.d:
        _scene.Camera.GoSideways(-50);
        break;

      case Gdk.Key.q:
        _scene.Camera.GoUpward(50);
        break;

      case Gdk.Key.z:
        _scene.Camera.GoUpward(-50);
        break;

      case Gdk.Key.Key_1:
        _mode = Mode.Move;
        _labelEditMode.Text = _mode.ToString();
        break;

      case Gdk.Key.Key_2:
        _mode = Mode.Scaling;
        _labelEditMode.Text = _mode.ToString();
        break;

      case Gdk.Key.Key_3:
        _mode = Mode.Rotating;
        _labelEditMode.Text = _mode.ToString();
        break;

      case Gdk.Key.Shift_L:
        _state |= Engine3D.State.Shift;
        break;
    }
  }

  protected override bool OnKeyReleaseEvent(EventKey eventKey)
  {
    if (eventKey.Key == Gdk.Key.Shift_L)
    {
      _state &= ~Engine3D.State.Shift;
    }

    return base.OnKeyReleaseEvent(eventKey);
  }

  protected void OnReplaceModelActionActivated(object _sender, EventArgs e)
  {
    var fileChooser =
      new FileChooserDialog(
        title: "Wybierz model",
        parent: this,
        action: FileChooserAction.Open,
        button_data:
        (
          "Anuluj", ResponseType.Cancel,
          "Otwórz", ResponseType.Ok
        )
      )
      {
        Filter = new FileFilter()
      };

    fileChooser.Filter.AddPattern("*.obj");

    if (fileChooser.Run() == (int)ResponseType.Ok)
    {
      var model = new WavefrontObj(fileChooser.Filename);
      model.Rotate(new Vector3D(Math.PI * 100, 0, 0));
      model.Rendering = _scene.World[_comboBoxModels.Active].Rendering;

      var tmp = _comboBoxModels.Active;
      _scene.World[_comboBoxModels.Active] = model;
      _comboBoxModels.AppendText(model.Name);
      _comboBoxModels.Active = tmp;
    }

    fileChooser.Destroy();
  }

  protected void OnLoadNewModelActionActivated(object _sender, EventArgs eventArgs)
  {
    var fileChooser =
      new FileChooserDialog(
        title: "Wybierz model",
        parent: this,
        action: FileChooserAction.Open,
        button_data:
        (
          "Anuluj", ResponseType.Cancel,
          "Otwórz", ResponseType.Ok
        )
      )
      {
        Filter = new FileFilter()
      };

    fileChooser.Filter.AddPattern("*.obj");

    if (fileChooser.Run() == (int)ResponseType.Ok)
    {
      LoadModel(fileChooser.Filename, null);
    }

    fileChooser.Destroy();
  }

  protected void OnLoadTextureActionActivated(object _sender, EventArgs eventArgs)
  {
    var fileChooser =
      new FileChooserDialog(
        title: "Wybierz model",
        parent: this,
        action: FileChooserAction.Open,
        button_data:
        (
          "Anuluj", ResponseType.Cancel,
          "Otwórz", ResponseType.Ok
        )
      )
      {
        Filter = new FileFilter()
      };

    fileChooser.Filter.AddPattern("*.jpg");
    fileChooser.Filter.AddPattern("*.jpeg");
    fileChooser.Filter.AddPattern("*.jpe");
    fileChooser.Filter.AddPattern("*.png");
    fileChooser.Filter.AddPattern("*.bmp");
    fileChooser.Filter.AddPattern("*.tif");

    if (fileChooser.Run() == (int)ResponseType.Ok)
    {
      _scene.World[_comboBoxModels.Active].Rendering =
        new Renderer(
          path: fileChooser.Filename,
          drawer: _scene
        );
    }

    fileChooser.Destroy();
  }

  protected void OnButtonChangeLightSourceClicked(object _sender, EventArgs _eventArgs)
  {
    _scene.LightSourceIndex = _comboBoxModels.Active;
  }

  protected void OnEventBoxScreenMotionNotifyEvent(object _sender, MotionNotifyEventArgs eventArgs)
  {
    if ((_state & Engine3D.State.LeftMouseBtn) != 0)
    {
      if ((_state & Engine3D.State.Shift) != 0)
      {
        var rotateVector =
          new Vector3D(
            x: 0,
            y: 0,
            z: -(_leftMouseBtn.X - eventArgs.Event.X) / 2
          );

        _scene.Camera.Rotate(rotateVector);
      }
      else
      {
        var rotateVector =
          new Vector3D(
            x: -(_leftMouseBtn.Y - eventArgs.Event.Y) * _mouseSensitivity,
            y: (_leftMouseBtn.X - eventArgs.Event.X) * _mouseSensitivity,
            z: 0
          );

        _scene.Camera.Rotate(rotateVector);
      }

      _leftMouseBtn =
        new Point(
          x: (int)eventArgs.Event.X,
          y: (int)eventArgs.Event.Y
        );
    }

    if ((_state & Engine3D.State.RightMouseBtn) != 0)
    {
      var value =
        new Point(
          x: -(int)(_rightMouseBtn.X - eventArgs.Event.X),
          y: -(int)(_rightMouseBtn.Y - eventArgs.Event.Y)
        );

      switch (_mode)
      {
        case Mode.Move:
          if ((_state & Engine3D.State.Shift) != 0)
          {
            var moveVector =
              new Vector3D(
                x: -value.Y * _scene.Camera.Forward.X * 3,
                y: -value.Y * _scene.Camera.Forward.Y * 3,
                z: -value.Y * _scene.Camera.Forward.Z * 3
              );

            _scene.World[_comboBoxModels.Active].Translate(moveVector);
          }
          else
          {
            var moveVectorX =
              new Vector3D(
                x: value.X * _scene.Camera.Right.X * 3,
                y: value.X * _scene.Camera.Right.Y * 3,
                z: value.X * _scene.Camera.Right.Z * 3
              );
            var moveVectorY =
              new Vector3D(
                x: value.Y * _scene.Camera.Upward.X * 3,
                y: value.Y * _scene.Camera.Upward.Y * 3,
                z: value.Y * _scene.Camera.Upward.Z * 3
              );

            _scene.World[_comboBoxModels.Active].Translate(moveVectorX);
            _scene.World[_comboBoxModels.Active].Translate(moveVectorY);
          }
          break;

        case Mode.Scaling:
          if ((_state & Engine3D.State.Shift) != 0)
          {
            var scaleValue =
              Math.Sqrt(Math.Pow(value.X - value.Y, 2)) *
              Math.Sign(value.X - value.Y) /
              2;
            var scaleVector =
              new Vector3D(
                x: scaleValue,
                y: scaleValue,
                z: scaleValue
              );

            _scene.World[_comboBoxModels.Active].Scale(scaleVector);
          }
          else
          {
            var scaleVectorX =
              new Vector3D(
                x: value.X * _scene.Camera.Right.X,
                y: value.X * _scene.Camera.Right.Y,
                z: value.X * _scene.Camera.Right.Z
              );
            var scaleVectorY =
              new Vector3D(
                x: -value.Y * _scene.Camera.Upward.X,
                y: -value.Y * _scene.Camera.Upward.Y,
                z: -value.Y * _scene.Camera.Upward.Z
              );

            _scene.World[_comboBoxModels.Active].Scale(scaleVectorX);
            _scene.World[_comboBoxModels.Active].Scale(scaleVectorY);
          }
          break;

        case Mode.Rotating:
          if ((_state & Engine3D.State.Shift) != 0)
          {
            _scene.World[_comboBoxModels.Active].RotateAroundAxis(
              angle: value.X,
              axis: _scene.Camera.Forward,
              center: _scene.World[_comboBoxModels.Active].VertexCoords.FindCenter()
            );
          }
          else
          {
            _scene.World[_comboBoxModels.Active].RotateAroundAxis(
              angle: -value.X,
              axis: _scene.Camera.Upward,
              center: _scene.World[_comboBoxModels.Active].VertexCoords.FindCenter()
            );
            _scene.World[_comboBoxModels.Active].RotateAroundAxis(
              angle: value.Y,
              axis: _scene.Camera.Right,
              center: _scene.World[_comboBoxModels.Active].VertexCoords.FindCenter()
            );
          }
          break;
      }
    }

    _rightMouseBtn =
      new Point(
        x: (int)eventArgs.Event.X,
        y: (int)eventArgs.Event.Y
      );
  }

  protected void OnEventBoxScreenBtnPressEvent(object o, ButtonPressEventArgs args)
  {
    if (args.Event.Button == 1)
    {
      _leftMouseBtn =
        new Point(
          x: (int)args.Event.X,
          y: (int)args.Event.Y
        );
      _state |= Engine3D.State.LeftMouseBtn;
    }

    if (args.Event.Button == 3)
    {
      _rightMouseBtn =
        new Point(
          x: (int)args.Event.X,
          y: (int)args.Event.Y
        );
      _state |= Engine3D.State.RightMouseBtn;
    }
  }

  protected void OnEventBoxScreenBtnReleaseEvent(object _sender, ButtonReleaseEventArgs eventArgs)
  {
    if (eventArgs.Event.Button == 1)
    {
      _state &= ~Engine3D.State.LeftMouseBtn;
    }

    if (eventArgs.Event.Button == 3)
    {
      _state &= ~Engine3D.State.RightMouseBtn;
    }
  }

  protected void OnEventBoxScreenScrollEvent(object _sender, ScrollEventArgs eventArgs)
  {
    if (eventArgs.Event.Direction == ScrollDirection.Down)
    {
      _scene.Distance += 100;
    }
    else if (eventArgs.Event.Direction == ScrollDirection.Up)
    {
      _scene.Distance -= 100;
    }
  }
}

