using System;
using Gtk;
using Projekt_LGiM;
using Gdk;
using MathNet.Spatial.Euclidean;
using System.Threading;
using System.Diagnostics;

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
	double dpi;
	double czuloscMyszy;
	Tryb tryb;

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();

		dpi = 96;
		czuloscMyszy = 0.3;
		labelTrybEdycji.Text = tryb.ToString();
		comboboxModele.Active = 0;
		string sciezkaTlo = @"background.jpg";
		scena = new Scena(sciezkaTlo, System.Drawing.Image.FromFile(sciezkaTlo).Size, 1000, 100)
		{
			KolorPedzla = Green,
			KolorTla = Black,
		};

		WczytajModel(@"modele/sphere.obj", @"tekstury/sun.jpg");
		WczytajModel(@"modele/smoothMonkey.obj", @"tekstury/mercury.jpg");
		scena.Swiat[1].Przesun(new Vector3D(500, 0, 0));
		scena.Swiat[1].Skaluj(new Vector3D(-50, -50, -50));
		scena.ZrodloSwiatlaIndeks = 0;

		var t = new Thread(new ParameterizedThreadStart((e) =>
		{
			var stopWatch = new Stopwatch();
			double avgRefreshTime = 0;
			int i = 1;

			while (true)
			{
				Thread.Sleep(15);
				stopWatch.Restart();
				scena.ZrodloSwiatla = scena.Swiat[scena.ZrodloSwiatlaIndeks].VertexCoords.ZnajdzSrodek();
				RysujNaEkranie();
				stopWatch.Stop();
				avgRefreshTime += stopWatch.ElapsedMilliseconds;

				if (i == 100)
				{
					labelOpoznienie.Text = avgRefreshTime / i + " ms";
					i = 0;
					avgRefreshTime = 0;
				}
				++i;
			}
		}));

		t.IsBackground = true;
		t.Start();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	void WczytajModel(string sciezkaModel, string sciezkaTekstura)
	{
		scena.Swiat.Add(new WavefrontObj(sciezkaModel));
		scena.Swiat[scena.Swiat.Count - 1].Renderowanie = new Renderowanie(sciezkaTekstura, scena);
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

	//void Ekran_MouseWheel(object sender, MouseWheelEventArgs e)
	//{
	//  if (e.Delta > 0) { scena.Odleglosc += 100; }
	//  else { scena.Odleglosc -= 100; }
	//}

	protected override bool OnKeyPressEvent(EventKey evnt)
	{
		switch (evnt.KeyValue)
		{
			case 'w':
				scena.Kamera.DoPrzodu(50);
				break;

			case 's':
				scena.Kamera.DoPrzodu(-50);
				break;

			case 'a':
				scena.Kamera.WBok(50);
				break;

			case 'd':
				scena.Kamera.WBok(-50);
				break;

			case ' ':
				scena.Kamera.WGore(50);
				break;

			case 65507: // LCtrl
				scena.Kamera.WGore(-50);
				break;

			case '1':
				tryb = Tryb.Przesuwanie;
				labelTrybEdycji.Text = tryb.ToString();
				break;

			case '2':
				tryb = Tryb.Skalowanie;
				labelTrybEdycji.Text = tryb.ToString();
				break;

			case '3':
				tryb = Tryb.Obracanie;
				labelTrybEdycji.Text = tryb.ToString();
				break;
		}

		return base.OnKeyPressEvent(evnt);
	}

    protected void OnZastpActionActivated(object sender, EventArgs e)
    {
        
    }

    protected void OnWczytajNowyActionActivated(object sender, EventArgs e)
    {
    }

    protected void OnWczytajActionActivated(object sender, EventArgs e)
    {
    }

    protected void OnSterowanieActionActivated(object sender, EventArgs e)
    {
    }
}
