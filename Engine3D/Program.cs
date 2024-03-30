using System;
using Gtk;

namespace Engine3D;

class Program
{
  [STAThread]
  public static void Main()
  {
    Application.Init();

    var app = new Application("org.Engine3D.Engine3D", GLib.ApplicationFlags.None);
    app.Register(GLib.Cancellable.Current);

    var win = new MainWindow();
    app.AddWindow(win);

    win.Show();
    Application.Run();
  }
}
