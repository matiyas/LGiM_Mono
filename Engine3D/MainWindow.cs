using System;
using Gdk;
using Gtk;
using MathNet.Spatial.Euclidean;
using UI = Gtk.Builder.ObjectAttribute;

namespace Engine3D;

[Flags] public enum State
{
  none = 0,
  lpm = 1,
  ppm = 2,
  shift = 4
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

  private readonly Scena _scene;
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

    _state = Engine3D.State.none;
    _mouseSensitivity = 0.3;
    _labelEditMode.Text = _mode.ToString();
    _comboBoxModels.Active = 0;

    var backgroundPath = @"background.jpg";
    _scene = new Scena(backgroundPath, GetImageDimensions(backgroundPath), 1000, 100)
    {
      KolorPedzla = _green,
      KolorTla = _black,
    };

    LoadModel(@"modele/monkey.obj", @"tekstury/sun.jpg");
    LoadModel(@"modele/monkey.obj", @"tekstury/earth.jpg");

    _scene.Swiat[1].Przesun(new Vector3D(500, 0, 0));
    _scene.Swiat[1].Skaluj(new Vector3D(-50, -50, -50));
    _scene.ZrodloSwiatlaIndeks = 0;

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
    _scene.ZrodloSwiatla = _scene.Swiat[_scene.ZrodloSwiatlaIndeks].VertexCoords.FindCenter();

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
    _scene.Swiat.Add(new WavefrontObj(modelPath));

    if (texturePath != null)
    {
      _scene.Swiat[_scene.Swiat.Count - 1].Renderowanie = new Renderer(texturePath, _scene);
    }
    _comboBoxModels.AppendText(_scene.Swiat[_scene.Swiat.Count - 1].Nazwa ?? "Model" + (_scene.Swiat.Count - 1));
    _comboBoxModels.Active = _scene.Swiat.Count - 1;
    _scene.Swiat[_scene.Swiat.Count - 1].Obroc(new Vector3D(Math.PI * 100, 0, 0));
  }

  void DrawOnScreen()
  {
    if (_checkButtonMesh.Active == false)
    {
      _scene.Renderuj();
    }
    else
    {
      _scene.RysujSiatke();
    }

    if (_checkButtonFloorMesh.Active == true)
    {
      _scene.RysujSiatkePodlogi(2000, 2000, 100, _gray, _blue, _red);
    }

    _imageScreen.Pixbuf =
      new Pixbuf(
        data: _scene.BackBuffer,
        colorspace: Colorspace.Rgb,
        has_alpha: true,
        bits_per_sample: 8,
        width: _scene.Rozmiar.Width,
        height: _scene.Rozmiar.Height,
        rowstride: 4 * _scene.Rozmiar.Width
      );
  }

  private void OnKeyPressEvent(object _sender, KeyPressEventArgs eventArgs)
  {
    switch (eventArgs.Event.Key)
    {
      case Gdk.Key.w:
        _scene.Kamera.GoForward(50);
        break;

      case Gdk.Key.s:
        _scene.Kamera.GoForward(-50);
        break;

      case Gdk.Key.a:
        _scene.Kamera.GoSideways(50);
        break;

      case Gdk.Key.d:
        _scene.Kamera.GoSideways(-50);
        break;

      case Gdk.Key.q:
        _scene.Kamera.GoUpward(50);
        break;

      case Gdk.Key.z:
        _scene.Kamera.GoUpward(-50);
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
        _state |= Engine3D.State.shift;
        break;
    }
  }

  protected override bool OnKeyReleaseEvent(EventKey eventKey)
  {
    if (eventKey.Key == Gdk.Key.Shift_L)
    {
      _state &= ~Engine3D.State.shift;
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
      model.Obroc(new Vector3D(Math.PI * 100, 0, 0));
      model.Renderowanie = _scene.Swiat[_comboBoxModels.Active].Renderowanie;

      var tmp = _comboBoxModels.Active;
      _scene.Swiat[_comboBoxModels.Active] = model;
      _comboBoxModels.AppendText(model.Nazwa);
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
      _scene.Swiat[_comboBoxModels.Active].Renderowanie =
        new Renderer(
          path: fileChooser.Filename,
          drawer: _scene
        );
    }

    fileChooser.Destroy();
  }

  protected void OnButtonChangeLightSourceClicked(object _sender, EventArgs _eventArgs)
  {
    _scene.ZrodloSwiatlaIndeks = _comboBoxModels.Active;
  }

  protected void OnEventBoxScreenMotionNotifyEvent(object _sender, MotionNotifyEventArgs eventArgs)
  {
    if ((_state & Engine3D.State.lpm) != 0)
    {
      if ((_state & Engine3D.State.shift) != 0)
      {
        var rotateVector =
          new Vector3D(
            x: 0,
            y: 0,
            z: -(_leftMouseBtn.X - eventArgs.Event.X) / 2
          );

        _scene.Kamera.Rotate(rotateVector);
      }
      else
      {
        var rotateVector =
          new Vector3D(
            x: -(_leftMouseBtn.Y - eventArgs.Event.Y) * _mouseSensitivity,
            y: (_leftMouseBtn.X - eventArgs.Event.X) * _mouseSensitivity,
            z: 0
          );

        _scene.Kamera.Rotate(rotateVector);
      }

      _leftMouseBtn =
        new Point(
          x: (int)eventArgs.Event.X,
          y: (int)eventArgs.Event.Y
        );
    }

    if ((_state & Engine3D.State.ppm) != 0)
    {
      var value =
        new Point(
          x: -(int)(_rightMouseBtn.X - eventArgs.Event.X),
          y: -(int)(_rightMouseBtn.Y - eventArgs.Event.Y)
        );

      switch (_mode)
      {
        case Mode.Move:
          if ((_state & Engine3D.State.shift) != 0)
          {
            var moveVector =
              new Vector3D(
                x: -value.Y * _scene.Kamera.Forward.X * 3,
                y: -value.Y * _scene.Kamera.Forward.Y * 3,
                z: -value.Y * _scene.Kamera.Forward.Z * 3
              );

            _scene.Swiat[_comboBoxModels.Active].Przesun(moveVector);
          }
          else
          {
            var moveVectorX =
              new Vector3D(
                x: value.X * _scene.Kamera.Right.X * 3,
                y: value.X * _scene.Kamera.Right.Y * 3,
                z: value.X * _scene.Kamera.Right.Z * 3
              );
            var moveVectorY =
              new Vector3D(
                x: value.Y * _scene.Kamera.Upward.X * 3,
                y: value.Y * _scene.Kamera.Upward.Y * 3,
                z: value.Y * _scene.Kamera.Upward.Z * 3
              );

            _scene.Swiat[_comboBoxModels.Active].Przesun(moveVectorX);
            _scene.Swiat[_comboBoxModels.Active].Przesun(moveVectorY);
          }
          break;

        case Mode.Scaling:
          if ((_state & Engine3D.State.shift) != 0)
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

            _scene.Swiat[_comboBoxModels.Active].Skaluj(scaleVector);
          }
          else
          {
            var scaleVectorX =
              new Vector3D(
                x: value.X * _scene.Kamera.Right.X,
                y: value.X * _scene.Kamera.Right.Y,
                z: value.X * _scene.Kamera.Right.Z
              );
            var scaleVectorY =
              new Vector3D(
                x: -value.Y * _scene.Kamera.Upward.X,
                y: -value.Y * _scene.Kamera.Upward.Y,
                z: -value.Y * _scene.Kamera.Upward.Z
              );

            _scene.Swiat[_comboBoxModels.Active].Skaluj(scaleVectorX);
            _scene.Swiat[_comboBoxModels.Active].Skaluj(scaleVectorY);
          }
          break;

        case Mode.Rotating:
          if ((_state & Engine3D.State.shift) != 0)
          {
            _scene.Swiat[_comboBoxModels.Active].ObrocWokolOsi(
              phi: value.X,
              axis: _scene.Kamera.Forward,
              angle: _scene.Swiat[_comboBoxModels.Active].VertexCoords.FindCenter()
            );
          }
          else
          {
            _scene.Swiat[_comboBoxModels.Active].ObrocWokolOsi(
              phi: -value.X,
              axis: _scene.Kamera.Upward,
              angle: _scene.Swiat[_comboBoxModels.Active].VertexCoords.FindCenter()
            );
            _scene.Swiat[_comboBoxModels.Active].ObrocWokolOsi(
              phi: value.Y,
              axis: _scene.Kamera.Right,
              angle: _scene.Swiat[_comboBoxModels.Active].VertexCoords.FindCenter()
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
      _state |= Engine3D.State.lpm;
    }

    if (args.Event.Button == 3)
    {
      _rightMouseBtn =
        new Point(
          x: (int)args.Event.X,
          y: (int)args.Event.Y
        );
      _state |= Engine3D.State.ppm;
    }
  }

  protected void OnEventBoxScreenBtnReleaseEvent(object _sender, ButtonReleaseEventArgs eventArgs)
  {
    if (eventArgs.Event.Button == 1)
    {
      _state &= ~Engine3D.State.lpm;
    }

    if (eventArgs.Event.Button == 3)
    {
      _state &= ~Engine3D.State.ppm;
    }
  }

  protected void OnEventBoxScreenScrollEvent(object _sender, ScrollEventArgs eventArgs)
  {
    if (eventArgs.Event.Direction == ScrollDirection.Down)
    {
      _scene.Odleglosc += 100;
    }
    else if (eventArgs.Event.Direction == ScrollDirection.Up)
    {
      _scene.Odleglosc -= 100;
    }
  }
}

