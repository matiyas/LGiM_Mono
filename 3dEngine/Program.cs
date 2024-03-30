using System;
using Gtk;

namespace LGiM_Mono
{
  class Program
  {
    [STAThread]
    public static void Main()
    {
      Application.Init();

      var app = new Application("org.3dEngine.3dEngine", GLib.ApplicationFlags.None);
      app.Register(GLib.Cancellable.Current);

      var win = new MainWindow();
      app.AddWindow(win);

      win.Show();
      Application.Run();
    }
  }
}
