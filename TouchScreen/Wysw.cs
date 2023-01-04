using System;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT;
using System.Threading;
using STM32F429I_Discovery.Netmf.Hardware;
using Microsoft.SPOT.Presentation;//weysiwelacz
using Microsoft.SPOT.Presentation.Media; //weysiwelacz




namespace TouchScreenExample
{
    public class Wysw
    {
         public static Color ColorRed = ColorUtility.ColorFromRGB(255, 0, 0);
         public static Color Colorwhite = ColorUtility.ColorFromRGB(255, 255, 255);
         public static Color ColorBlue = ColorUtility.ColorFromRGB(0, 0, 255);
         public static Color ColorGreen = ColorUtility.ColorFromRGB(0, 255, 0);

         public static Font fsmall = Resources.GetFont(Resources.FontResources.small);
         public static Font fmed = Resources.GetFont(Resources.FontResources.med);
         public static Font fbig = Resources.GetFont(Resources.FontResources.big);

         public static Font fCalibri12 = Resources.GetFont(Resources.FontResources.Calibri12);
         public static Font fCalibri12AA = Resources.GetFont(Resources.FontResources.Calibri12AA);
         public static Font Calibri12Bold = Resources.GetFont(Resources.FontResources.Calibri12Bold);

         public static Font fCalibri8 = Resources.GetFont(Resources.FontResources.Calibri8);
         public static Font fCalibri8AA = Resources.GetFont(Resources.FontResources.Calibri8AA);
         public static Font Calibri8Bold = Resources.GetFont(Resources.FontResources.Calibri8Bold);

         public static Font fCalibri16 = Resources.GetFont(Resources.FontResources.Calibri16);
         public static Font fCalibri16AA = Resources.GetFont(Resources.FontResources.Calibri16AA);
         public static Font Calibri16Bold = Resources.GetFont(Resources.FontResources.Calibri16Bold);

         public static Font fCalibri20 = Resources.GetFont(Resources.FontResources.Calibri20);
         public static Font fCalibri20AA = Resources.GetFont(Resources.FontResources.Calibri20AA);
         public static Font Calibri20Bold = Resources.GetFont(Resources.FontResources.Calibri20Bold);

         public static Font fCalibri26 = Resources.GetFont(Resources.FontResources.Calibri26);
         public static Font fCalibri26Bold = Resources.GetFont(Resources.FontResources.Calibri26Bold);

         public static Font fCalibri32 = Resources.GetFont(Resources.FontResources.Calibri32);
         public static Font fCalibri32Bold = Resources.GetFont(Resources.FontResources.Calibri32Bold);



    }
}
