using System;
using Gtk;
using Projekt_LGiM;
using Gdk;
using MathNet.Spatial.Euclidean;
using UI = Gtk.Builder.ObjectAttribute;


[Flags] public enum State { none = 0, lpm = 1, ppm = 2, shift = 4 };
public enum Tryb { Przesuwanie, Skalowanie, Obracanie };

public partial class MainWindow : Gtk.Window
{
  static Color Black = new Color(0, 0, 0);
  static Color Green = new Color(0, 255, 0);
  static Color White = new Color(255, 255, 255);
  static Color Gray = new Color(127, 127, 127);
  static Color Blue = new Color(0, 0, 255);
  static Color Red = new Color(255, 0, 0);

  Scena scena;
  Point lpm0, ppm0;
  double czuloscMyszy;
  Tryb tryb;
  State stan;

  [UI] private Label labelTrybEdycji = null;
  [UI] private EventBox eventboxEkran = null;
  [UI] private Image imageEkran = null;
  [UI] private CheckButton checkbuttonSiatka = null;
  [UI] private CheckButton checkbuttonSiatkaPodlogi = null;
  [UI] private ComboBoxText comboboxModele = null;

  public MainWindow() : this(new Builder("MainWindow.glade")) { }

  private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
  {
    builder.Autoconnect(this);

    stan = global::State.none;
    czuloscMyszy = 0.3;
    labelTrybEdycji.Text = tryb.ToString();
    comboboxModele.Active = 0;
    string sciezkaTlo = @"background.jpg";
    scena = new Scena(sciezkaTlo, GetImageDimensions(sciezkaTlo), 1000, 100)
    {
      KolorPedzla = Green,
      KolorTla = Black,
    };

    WczytajModel(@"modele/monkey.obj", @"tekstury/sun.jpg");
    WczytajModel(@"modele/monkey.obj", @"tekstury/earth.jpg");
    scena.Swiat[1].Przesun(new Vector3D(500, 0, 0));
    scena.Swiat[1].Skaluj(new Vector3D(-50, -50, -50));
    scena.ZrodloSwiatlaIndeks = 0;

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
    scena.ZrodloSwiatla = scena.Swiat[scena.ZrodloSwiatlaIndeks].VertexCoords.ZnajdzSrodek();
    RysujNaEkranie();
    return true;
  }

  protected void OnDeleteEvent(object sender, DeleteEventArgs a)
  {
    Application.Quit();
    a.RetVal = true;
  }

  void WczytajModel(string sciezkaModel, string sciezkaTekstura)
  {
    scena.Swiat.Add(new WavefrontObj(sciezkaModel));

    if (sciezkaTekstura != null)
    {
      scena.Swiat[scena.Swiat.Count - 1].Renderowanie = new Renderowanie(sciezkaTekstura, scena);
    }
    comboboxModele.AppendText(scena.Swiat[scena.Swiat.Count - 1].Nazwa ?? "Model" + (scena.Swiat.Count - 1));
    comboboxModele.Active = scena.Swiat.Count - 1;
    scena.Swiat[scena.Swiat.Count - 1].Obroc(new Vector3D(Math.PI * 100, 0, 0));
  }

  void RysujNaEkranie()
  {
    if (checkbuttonSiatka.Active == false) { scena.Renderuj(); }
    else { scena.RysujSiatke(); }
    if (checkbuttonSiatkaPodlogi.Active == true) { scena.RysujSiatkePodlogi(2000, 2000, 100, Gray, Blue, Red); }
    imageEkran.Pixbuf = new Pixbuf(scena.BackBuffer, Colorspace.Rgb, true, 8, scena.Rozmiar.Width, scena.Rozmiar.Height, 4 * scena.Rozmiar.Width);
  }

  private void OnKeyPressEvent(object? sender, KeyPressEventArgs evnt)
  {
    switch (evnt.Event.Key)
    {
      case Gdk.Key.w:
        scena.Kamera.DoPrzodu(50);
        break;

      case Gdk.Key.s:
        scena.Kamera.DoPrzodu(-50);
        break;

      case Gdk.Key.a:
        scena.Kamera.WBok(50);
        break;

      case Gdk.Key.d:
        scena.Kamera.WBok(-50);
        break;

      case Gdk.Key.q:
        scena.Kamera.WGore(50);
        break;

      case Gdk.Key.z:
        scena.Kamera.WGore(-50);
        break;

      case Gdk.Key.Key_1:
        tryb = Tryb.Przesuwanie;
        labelTrybEdycji.Text = tryb.ToString();
        break;

      case Gdk.Key.Key_2:
        tryb = Tryb.Skalowanie;
        labelTrybEdycji.Text = tryb.ToString();
        break;

      case Gdk.Key.Key_3:
        tryb = Tryb.Obracanie;
        labelTrybEdycji.Text = tryb.ToString();
        break;

      case Gdk.Key.Shift_L:
        stan |= global::State.shift;
        break;
    }
  }

  protected override bool OnKeyReleaseEvent(EventKey evnt)
  {
    if (evnt.Key == Gdk.Key.Shift_L) { stan &= ~global::State.shift; }

    return base.OnKeyReleaseEvent(evnt);
  }

  protected void OnZastapActionActivated(object sender, EventArgs e)
  {
    FileChooserDialog fc = new FileChooserDialog("Wybierz model", this, FileChooserAction.Open,
                           "Anuluj", ResponseType.Cancel, "Otwórz", ResponseType.Ok);
    fc.Filter = new FileFilter();
    fc.Filter.AddPattern("*.obj");

    if (fc.Run() == (int)ResponseType.Ok)
    {
      var model = new WavefrontObj(fc.Filename);
      model.Obroc(new Vector3D(Math.PI * 100, 0, 0));
      model.Renderowanie = scena.Swiat[comboboxModele.Active].Renderowanie;

      int tmp = comboboxModele.Active;
      scena.Swiat[comboboxModele.Active] = model;
      comboboxModele.AppendText(model.Nazwa);
      comboboxModele.Active = tmp;
    }

    fc.Destroy();
  }

  protected void OnWczytajNowyActionActivated(object sender, EventArgs e)
  {
    FileChooserDialog fc = new FileChooserDialog("Wybierz model", this, FileChooserAction.Open,
                                                 "Anuluj", ResponseType.Cancel, "Otwórz", ResponseType.Ok);
    fc.Filter = new FileFilter();
    fc.Filter.AddPattern("*.obj");

    if (fc.Run() == (int)ResponseType.Ok)
    {
      WczytajModel(fc.Filename, null);
    }

    fc.Destroy();
  }

  protected void OnWczytajActionActivated(object sender, EventArgs e)
  {
    FileChooserDialog fc = new FileChooserDialog("Wybierz model", this, FileChooserAction.Open,
                           "Anuluj", ResponseType.Cancel, "Otwórz", ResponseType.Ok);
    fc.Filter = new FileFilter();
    fc.Filter.AddPattern("*.jpg");
    fc.Filter.AddPattern("*.jpeg");
    fc.Filter.AddPattern("*.jpe");
    fc.Filter.AddPattern("*.png");
    fc.Filter.AddPattern("*.bmp");
    fc.Filter.AddPattern("*.tif");

    if (fc.Run() == (int)ResponseType.Ok)
    {
      scena.Swiat[comboboxModele.Active].Renderowanie = new Renderowanie(fc.Filename, scena);
    }

    fc.Destroy();
  }

  protected void OnSterowanieActionActivated(object sender, EventArgs e)
  {
  }

  protected void OnButtonZmienZrodloSwiatlaClicked(object sender, EventArgs e)
  {
    scena.ZrodloSwiatlaIndeks = comboboxModele.Active;
  }

  protected void OnEventboxEkranMotionNotifyEvent(object o, MotionNotifyEventArgs args)
  {
    if ((stan & global::State.lpm) != 0)
    {
      if ((stan & global::State.shift) != 0)
      {

        scena.Kamera.Obroc(new Vector3D(0, 0, -(lpm0.X - args.Event.X) / 2));
      }
      else
      {
        scena.Kamera.Obroc(new Vector3D(-(lpm0.Y - args.Event.Y) * czuloscMyszy,
          (lpm0.X - args.Event.X) * czuloscMyszy, 0));
      }

      lpm0 = new Point((int)args.Event.X, (int)args.Event.Y);
    }

    if ((stan & global::State.ppm) != 0)
    {
      Point ile = new Point(-(int)(ppm0.X - args.Event.X), -(int)(ppm0.Y - args.Event.Y));
      switch (tryb)
      {
        case Tryb.Przesuwanie:
          if ((stan & global::State.shift) != 0)
          {
            scena.Swiat[comboboxModele.Active].Przesun(new Vector3D(-ile.Y * scena.Kamera.Przod.X * 3,
  -ile.Y * scena.Kamera.Przod.Y * 3, -ile.Y * scena.Kamera.Przod.Z * 3));
          }
          else
          {
            scena.Swiat[comboboxModele.Active].Przesun(new Vector3D(ile.X * scena.Kamera.Prawo.X * 3,
              ile.X * scena.Kamera.Prawo.Y * 3, ile.X * scena.Kamera.Prawo.Z * 3));
            scena.Swiat[comboboxModele.Active].Przesun(new Vector3D(ile.Y * scena.Kamera.Gora.X * 3,
              ile.Y * scena.Kamera.Gora.Y * 3, ile.Y * scena.Kamera.Gora.Z * 3));
          }
          break;

        case Tryb.Skalowanie:
          if ((stan & global::State.shift) != 0)
          {
            double s = Math.Sqrt(Math.Pow(ile.X - ile.Y, 2)) * Math.Sign(ile.X - ile.Y) / 2;
            scena.Swiat[comboboxModele.Active].Skaluj(new Vector3D(s, s, s));
          }
          else
          {
            scena.Swiat[comboboxModele.Active].Skaluj(new Vector3D(ile.X * scena.Kamera.Prawo.X,
              ile.X * scena.Kamera.Prawo.Y, ile.X * scena.Kamera.Prawo.Z));
            scena.Swiat[comboboxModele.Active].Skaluj(new Vector3D(-ile.Y * scena.Kamera.Gora.X,
              -ile.Y * scena.Kamera.Gora.Y, -ile.Y * scena.Kamera.Gora.Z));
          }
          break;

        case Tryb.Obracanie:
          if ((stan & global::State.shift) != 0)
          {
            scena.Swiat[comboboxModele.Active].ObrocWokolOsi(ile.X, scena.Kamera.Przod,
              scena.Swiat[comboboxModele.Active].VertexCoords.ZnajdzSrodek());
          }
          else
          {
            scena.Swiat[comboboxModele.Active].ObrocWokolOsi(-ile.X, scena.Kamera.Gora,
              scena.Swiat[comboboxModele.Active].VertexCoords.ZnajdzSrodek());

            scena.Swiat[comboboxModele.Active].ObrocWokolOsi(ile.Y, scena.Kamera.Prawo,
              scena.Swiat[comboboxModele.Active].VertexCoords.ZnajdzSrodek());
          }
          break;
      }
    }

    ppm0 = new Point((int)args.Event.X, (int)args.Event.Y);
  }

  protected void OnEventboxEkranButtonPressEvent(object o, ButtonPressEventArgs args)
  {
    if (args.Event.Button == 1)
    {
      lpm0 = new Point((int)args.Event.X, (int)args.Event.Y);
      stan |= global::State.lpm;
    }

    if (args.Event.Button == 3)
    {
      ppm0 = new Point((int)args.Event.X, (int)args.Event.Y);
      stan |= global::State.ppm;
    }
  }

  protected void OnEventboxEkranButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
  {
    if (args.Event.Button == 1) { stan &= ~global::State.lpm; }
    if (args.Event.Button == 3) { stan &= ~global::State.ppm; }
  }

  protected void OnEventboxEkranScrollEvent(object o, ScrollEventArgs args)
  {
    if (args.Event.Direction == ScrollDirection.Down) { scena.Odleglosc += 100; }
    else if (args.Event.Direction == ScrollDirection.Up) { scena.Odleglosc -= 100; }
  }
}
