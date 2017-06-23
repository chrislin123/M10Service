using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace M10AlertLRTI
{
  static class Program
  {
    /// <summary>
    /// 應用程式的主要進入點。
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      //Application.Run(new M10AlertLRTI());
      //Application.Run(new M10kml());  
      Application.Run(new L11HighRail());
    }
  }
}
