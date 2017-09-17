
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;

	private global::Gtk.Action PlikAction;

	private global::Gtk.Action ModelAction;

	private global::Gtk.Action TeksturaAction;

	private global::Gtk.Action ZastpAction;

	private global::Gtk.Action WczytajNowyAction;

	private global::Gtk.Action WczytajAction;

	private global::Gtk.Action PomocAction;

	private global::Gtk.Action SterowanieAction;

	private global::Gtk.VBox vbox1;

	private global::Gtk.HBox hbox1;

	private global::Gtk.MenuBar menubarMain;

	private global::Gtk.Label labelOpoznienie;

	private global::Gtk.HBox hbox2;

	private global::Gtk.Frame frame1;

	private global::Gtk.Alignment GtkAlignment;

	private global::Gtk.EventBox eventboxEkran;

	private global::Gtk.Image imageEkran;

	private global::Gtk.VBox vbox2;

	private global::Gtk.ComboBox comboboxModele;

	private global::Gtk.Button buttonZmienZrodloSwiatla;

	private global::Gtk.HBox hbox3;

	private global::Gtk.Label label4;

	private global::Gtk.Label labelTrybEdycji;

	private global::Gtk.CheckButton checkbuttonSiatkaPodlogi;

	private global::Gtk.CheckButton checkbuttonSiatka;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup("Default");
		this.PlikAction = new global::Gtk.Action("PlikAction", global::Mono.Unix.Catalog.GetString("Plik"), null, null);
		this.PlikAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Plik");
		w1.Add(this.PlikAction, null);
		this.ModelAction = new global::Gtk.Action("ModelAction", global::Mono.Unix.Catalog.GetString("Model"), null, null);
		this.ModelAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Model");
		w1.Add(this.ModelAction, null);
		this.TeksturaAction = new global::Gtk.Action("TeksturaAction", global::Mono.Unix.Catalog.GetString("Tekstura"), null, null);
		this.TeksturaAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Tekstura");
		w1.Add(this.TeksturaAction, null);
		this.ZastpAction = new global::Gtk.Action("ZastpAction", global::Mono.Unix.Catalog.GetString("Zastąp..."), null, null);
		this.ZastpAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Zastąp...");
		w1.Add(this.ZastpAction, null);
		this.WczytajNowyAction = new global::Gtk.Action("WczytajNowyAction", global::Mono.Unix.Catalog.GetString("Wczytaj nowy..."), null, null);
		this.WczytajNowyAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Wczytaj nowy...");
		w1.Add(this.WczytajNowyAction, null);
		this.WczytajAction = new global::Gtk.Action("WczytajAction", global::Mono.Unix.Catalog.GetString("Wczytaj..."), null, null);
		this.WczytajAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Wczytaj...");
		w1.Add(this.WczytajAction, null);
		this.PomocAction = new global::Gtk.Action("PomocAction", global::Mono.Unix.Catalog.GetString("Pomoc"), null, null);
		this.PomocAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Pomoc");
		w1.Add(this.PomocAction, null);
		this.SterowanieAction = new global::Gtk.Action("SterowanieAction", global::Mono.Unix.Catalog.GetString("Sterowanie..."), null, null);
		this.SterowanieAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Sterowanie...");
		w1.Add(this.SterowanieAction, null);
		this.UIManager.InsertActionGroup(w1, 0);
		this.AddAccelGroup(this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox();
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString(@"<ui><menubar name='menubarMain'><menu name='PlikAction' action='PlikAction'><menu name='ModelAction' action='ModelAction'><menuitem name='ZastpAction' action='ZastpAction'/><menuitem name='WczytajNowyAction' action='WczytajNowyAction'/></menu><menu name='TeksturaAction' action='TeksturaAction'><menuitem name='WczytajAction' action='WczytajAction'/></menu></menu><menu name='PomocAction' action='PomocAction'><menuitem name='SterowanieAction' action='SterowanieAction'/></menu></menubar></ui>");
		this.menubarMain = ((global::Gtk.MenuBar)(this.UIManager.GetWidget("/menubarMain")));
		this.menubarMain.Name = "menubarMain";
		this.hbox1.Add(this.menubarMain);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.menubarMain]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.labelOpoznienie = new global::Gtk.Label();
		this.labelOpoznienie.Name = "labelOpoznienie";
		this.labelOpoznienie.LabelProp = global::Mono.Unix.Catalog.GetString("0 ms");
		this.hbox1.Add(this.labelOpoznienie);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.labelOpoznienie]));
		w3.PackType = ((global::Gtk.PackType)(1));
		w3.Position = 1;
		w3.Expand = false;
		w3.Fill = false;
		w3.Padding = ((uint)(15));
		this.vbox1.Add(this.hbox1);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
		w4.Position = 0;
		w4.Expand = false;
		w4.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.frame1 = new global::Gtk.Frame();
		this.frame1.Name = "frame1";
		this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
		// Container child frame1.Gtk.Container+ContainerChild
		this.GtkAlignment = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
		this.GtkAlignment.Name = "GtkAlignment";
		this.GtkAlignment.LeftPadding = ((uint)(12));
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		this.eventboxEkran = new global::Gtk.EventBox();
		this.eventboxEkran.Name = "eventboxEkran";
		// Container child eventboxEkran.Gtk.Container+ContainerChild
		this.imageEkran = new global::Gtk.Image();
		this.imageEkran.WidthRequest = 1024;
		this.imageEkran.HeightRequest = 768;
		this.imageEkran.Name = "imageEkran";
		this.eventboxEkran.Add(this.imageEkran);
		this.GtkAlignment.Add(this.eventboxEkran);
		this.frame1.Add(this.GtkAlignment);
		this.hbox2.Add(this.frame1);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.frame1]));
		w8.Position = 0;
		// Container child hbox2.Gtk.Box+BoxChild
		this.vbox2 = new global::Gtk.VBox();
		this.vbox2.WidthRequest = 200;
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		this.vbox2.BorderWidth = ((uint)(15));
		// Container child vbox2.Gtk.Box+BoxChild
		this.comboboxModele = global::Gtk.ComboBox.NewText();
		this.comboboxModele.Name = "comboboxModele";
		this.vbox2.Add(this.comboboxModele);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.comboboxModele]));
		w9.Position = 0;
		w9.Expand = false;
		w9.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.buttonZmienZrodloSwiatla = new global::Gtk.Button();
		this.buttonZmienZrodloSwiatla.CanFocus = true;
		this.buttonZmienZrodloSwiatla.Name = "buttonZmienZrodloSwiatla";
		this.buttonZmienZrodloSwiatla.UseUnderline = true;
		this.buttonZmienZrodloSwiatla.Label = global::Mono.Unix.Catalog.GetString("Ustaw jako źródło światła");
		this.vbox2.Add(this.buttonZmienZrodloSwiatla);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.buttonZmienZrodloSwiatla]));
		w10.Position = 1;
		w10.Expand = false;
		w10.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbox3 = new global::Gtk.HBox();
		this.hbox3.Name = "hbox3";
		this.hbox3.Spacing = 6;
		// Container child hbox3.Gtk.Box+BoxChild
		this.label4 = new global::Gtk.Label();
		this.label4.Name = "label4";
		this.label4.LabelProp = global::Mono.Unix.Catalog.GetString("Tryb edycji:");
		this.hbox3.Add(this.label4);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.label4]));
		w11.Position = 0;
		w11.Expand = false;
		w11.Fill = false;
		// Container child hbox3.Gtk.Box+BoxChild
		this.labelTrybEdycji = new global::Gtk.Label();
		this.labelTrybEdycji.Name = "labelTrybEdycji";
		this.labelTrybEdycji.LabelProp = global::Mono.Unix.Catalog.GetString("Przesuwanie");
		this.hbox3.Add(this.labelTrybEdycji);
		global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.labelTrybEdycji]));
		w12.PackType = ((global::Gtk.PackType)(1));
		w12.Position = 1;
		w12.Expand = false;
		w12.Fill = false;
		this.vbox2.Add(this.hbox3);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox3]));
		w13.PackType = ((global::Gtk.PackType)(1));
		w13.Position = 2;
		w13.Expand = false;
		w13.Fill = false;
		w13.Padding = ((uint)(15));
		// Container child vbox2.Gtk.Box+BoxChild
		this.checkbuttonSiatkaPodlogi = new global::Gtk.CheckButton();
		this.checkbuttonSiatkaPodlogi.CanFocus = true;
		this.checkbuttonSiatkaPodlogi.Name = "checkbuttonSiatkaPodlogi";
		this.checkbuttonSiatkaPodlogi.Label = global::Mono.Unix.Catalog.GetString("Siatka podłogi");
		this.checkbuttonSiatkaPodlogi.DrawIndicator = true;
		this.checkbuttonSiatkaPodlogi.UseUnderline = true;
		this.checkbuttonSiatkaPodlogi.FocusOnClick = false;
		this.vbox2.Add(this.checkbuttonSiatkaPodlogi);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.checkbuttonSiatkaPodlogi]));
		w14.PackType = ((global::Gtk.PackType)(1));
		w14.Position = 3;
		w14.Expand = false;
		w14.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.checkbuttonSiatka = new global::Gtk.CheckButton();
		this.checkbuttonSiatka.CanFocus = true;
		this.checkbuttonSiatka.Name = "checkbuttonSiatka";
		this.checkbuttonSiatka.Label = global::Mono.Unix.Catalog.GetString("Siatka");
		this.checkbuttonSiatka.DrawIndicator = true;
		this.checkbuttonSiatka.UseUnderline = true;
		this.checkbuttonSiatka.FocusOnClick = false;
		this.vbox2.Add(this.checkbuttonSiatka);
		global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.checkbuttonSiatka]));
		w15.PackType = ((global::Gtk.PackType)(1));
		w15.Position = 4;
		w15.Expand = false;
		w15.Fill = false;
		this.hbox2.Add(this.vbox2);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.vbox2]));
		w16.Position = 1;
		w16.Expand = false;
		w16.Fill = false;
		w16.Padding = ((uint)(5));
		this.vbox1.Add(this.hbox2);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox2]));
		w17.Position = 1;
		this.Add(this.vbox1);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 1258;
		this.DefaultHeight = 804;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.ZastpAction.Activated += new global::System.EventHandler(this.OnZastpActionActivated);
		this.WczytajNowyAction.Activated += new global::System.EventHandler(this.OnWczytajNowyActionActivated);
		this.WczytajAction.Activated += new global::System.EventHandler(this.OnWczytajActionActivated);
		this.SterowanieAction.Activated += new global::System.EventHandler(this.OnSterowanieActionActivated);
		this.eventboxEkran.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler(this.OnEventboxEkranButtonPressEvent);
		this.eventboxEkran.ScrollEvent += new global::Gtk.ScrollEventHandler(this.OnEventboxEkranScrollEvent);
		this.eventboxEkran.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler(this.OnEventboxEkranButtonReleaseEvent);
		this.eventboxEkran.MotionNotifyEvent += new global::Gtk.MotionNotifyEventHandler(this.OnEventboxEkranMotionNotifyEvent);
		this.buttonZmienZrodloSwiatla.Clicked += new global::System.EventHandler(this.OnButtonZmienZrodloSwiatlaClicked);
	}
}
