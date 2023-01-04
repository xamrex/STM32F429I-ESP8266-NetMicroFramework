/**
  ******************************************************************************
  * @file    TouchScreen\TouchScreen\Program.cs
  * @author  MCD
  * @version V1.0.0
  * @date    24-Sep-2013
  * @brief   Main program body
  ******************************************************************************
   * @attention
  *
  * <h2><center>&copy; COPYRIGHT 2013 STMicroelectronics</center></h2>
  *
  * Licensed under MCD-ST Liberty SW License Agreement V2, (the "License");
  * You may not use this file except in compliance with the License.
  * You may obtain a copy of the License at:
  *
  *        http://www.st.com/software_license_agreement_liberty_v2
  *
  * Unless required by applicable law or agreed to in writing, software 
  * distributed under the License is distributed on an "AS IS" BASIS, 
  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  * See the License for the specific language governing permissions and
  * limitations under the License.
  *
  * <h2><center>&copy; COPYRIGHT 2013 STMicroelectronics</center></h2>
  */

/* References ------------------------------------------------------------------*/
using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;
using System.Threading;
using STM32F429I_Discovery.Netmf.Hardware;
using System.IO.Ports;
using Microsoft.SPOT.Hardware;

namespace TouchScreenExample
{
    /// <summary>
    /// Touch screen example.
    /// </summary>
    public class Program : Application
    {
        public Bitmap lcd;
        public bool uruchomienie = false;
        public bool ladowanieDanych = false;
        public string IP; //Zmienna do do przechowywania adresu IP

        private int SzerokoscBoxa=113;
        private int WysokoscBoxa=40;
        public bool mainScreen =true;

       // public string[] miastaAdress = { "poland/slaski/gliwice--ul.-mewy-34", "poland/malopolska/krakow/kurdwanow", "shanghai", "newyork", "brazil/sao-paulo/parque-d.pedro-ii", "australia/nsw/randwick/sydney-east", "turkey/akdeniz/antalya", "norway/norway/oslo/kirkeveien" };
        public string[] miastaAdress = { "6539", "9039", "1437", "3560", "363", "3259", "4045", "2654" };
        public string[] miastaNazwa = { "Gliwice", "Krakow", "Shanghai", "New York", "Sao Paulo", "Sydney", "Antalya", "Oslo" }; 

        static SerialPortHelper serialPortHelper1 = new SerialPortHelper(SerialPorts.SerialCOM2, 115200, Parity.None, 8, StopBits.One);
        public static void Main()
        {
            Program app = new Program();
            app.Run();
        }

        public Program()
        {                                   //240                   //320
            this.lcd = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);
            Touch.Initialize(this);
            this.MainWindow = new Window();
            this.MainWindow.TouchUp += MainWindow_TouchUp;
            new Timer(new TimerCallback(RunMe), null, 1000, -1); //wlacz timer jednorazowy zeby wyswqietlic ekran glowny
        }

         void RunMe(object o)
        {
           UstawParametryPolaczenia(); 
           EkranStartowy();
         }

        private void UstawParametryPolaczenia()
        {
            serialPortHelper1.PrintLine("ATE0"); //Wylacz ECHO
            Thread.Sleep(100);
            serialPortHelper1.ClearAllBufor(); //wyczysc bufor

            serialPortHelper1.PrintLine("AT+CIPSTA?"); //wyslij zapytanie - zwraca IP, maske 
            Thread.Sleep(300); //czekaj na odpowiedz  
            string[] IPc = serialPortHelper1.ReadAllBuf().Split('"');
            IP = IPc[1]; //skopoiwanie IP
            serialPortHelper1.ClearAllBufor(); //wyczysc bufor
        }

        private void WyswietlIP()
        { //Na samej gorze pokaz adres IP urzadzenia
            this.lcd.DrawTextInRect("IP:" + IP, 0, 0, 240, 35, Bitmap.DT_AlignmentCenter, Wysw.Colorwhite, Wysw.fCalibri26Bold);
            this.lcd.DrawRectangle(Wysw.ColorBlue, 6, 3, 3, 234, 35, 0, 0, Wysw.ColorBlue, 0, 0, Wysw.ColorBlue, 0, 0, 0);
        }

        public void EkranStartowy()
        {
            WyswietlIP();

            this.lcd.DrawTextInRect("Wybierz miasto aby zobaczyć poziom zanieczyszczenia:", 2, 50, 238, 50, Bitmap.DT_AlignmentCenter, Wysw.Colorwhite, Wysw.fCalibri20AA);
            this.lcd.DrawRectangle(Wysw.ColorBlue, 4, 2, 50, 236, 50, 0, 0, Wysw.ColorBlue, 0, 0, Wysw.ColorBlue, 0, 0, 0);


            BoxMiasto(miastaNazwa[0], 2, 110, WysokoscBoxa, SzerokoscBoxa);
            BoxMiasto(miastaNazwa[1], 125, 110, WysokoscBoxa, SzerokoscBoxa);

            BoxMiasto(miastaNazwa[2], 2, 160, WysokoscBoxa, SzerokoscBoxa);
            BoxMiasto(miastaNazwa[3], 125, 160, WysokoscBoxa, SzerokoscBoxa);

            BoxMiasto(miastaNazwa[4], 2, 210, WysokoscBoxa, SzerokoscBoxa);
            BoxMiasto(miastaNazwa[5], 125, 210, WysokoscBoxa, SzerokoscBoxa);

            BoxMiasto(miastaNazwa[6], 2, 260, WysokoscBoxa, SzerokoscBoxa);
            BoxMiasto(miastaNazwa[7], 125, 260, WysokoscBoxa, SzerokoscBoxa);

            this.lcd.Flush();
            mainScreen = true;
        }
        private void EkranZanieczyszczenieMiasta(int IDMiasta)
        {
            mainScreen = false;
            ladowanieDanych = true;

            serialPortHelper1.ClearAllBufor(); //wyczysc buffor.
            ZapytajOMiasto(miastaAdress[IDMiasta]);

            this.lcd.Clear();
            this.lcd.DrawTextInRect(miastaNazwa[IDMiasta], 0, 0, 240, 35, Bitmap.DT_AlignmentCenter, Wysw.Colorwhite, Wysw.fCalibri26Bold);

            string odpowiedz = serialPortHelper1.ReadAllBuf();
            string status = Between(odpowiedz, "status\":\"", "\",\"data");
            string pm10 = Between(odpowiedz, "pm10\":{\"v\":", "},");
            string pm25 = Between(odpowiedz, "pm25\":{\"v\":", "},");
            string No2 = Between(odpowiedz, "no2\":{\"v\":", "},");
            string So2 = Between(odpowiedz, "so2\":{\"v\":", "},");
            string temp = Between(odpowiedz, "t\":{\"v\":", "}");  //sprwadzic czy okej
            string pressure = Between(odpowiedz, "p\":{\"v\":", "},");
            string humidity = Between(odpowiedz, "h\":{\"v\":", "},");

            double dpm10=0, dpm25=0, dNo2=0, dSo2=0,dtemp=0,dpressure=0,dhumidity=0;
            //Konwersja ze stringa na double
            try {  dpm10 = double.Parse(pm10); } catch{;}
            try {  dpm25 = double.Parse(pm25);} catch{;}
            try {  dNo2 = double.Parse(No2);} catch{;}
            try {  dSo2 = double.Parse(So2); } catch { ;}
            try {  dtemp = double.Parse(temp); } catch { ;}
            try { dpressure = double.Parse(pressure); } catch { ;}
            try { dhumidity = double.Parse(humidity); } catch { ;}
            Color kolor=Wysw.Colorwhite;

            //NORMY WHO
            //http://smog.imgw.pl/content/norm
            double normaPm10=50;
            double normaPm25 = 25;
            double normaNo2 = 40;
            double normaSo2 = 125;

            double normaOptymTemp =23;
            double normatemphistereza = 5;
            double normacisnienie = 1000;
            double normacisnieniehistereza = 10;
            double normawilgotnoscpow = 50;
            double normawilgotnoscpowhistereza = 10;

            this.lcd.DrawTextInRect("Status:", 0, 50, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (status == "ok") kolor = Wysw.ColorGreen; else kolor=Wysw.ColorRed;
            this.lcd.DrawTextInRect(status, 90, 50, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);
            
            this.lcd.DrawTextInRect("Pm₁₀:", 0, 75, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dpm10 > normaPm10) kolor = Wysw.ColorRed; else kolor = Wysw.ColorGreen;
            this.lcd.DrawTextInRect(pm10 + " µg/m³", 90, 75, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);

            this.lcd.DrawTextInRect("Pm₂₅:", 0, 100, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dpm25 > normaPm25) kolor = Wysw.ColorRed; else kolor = Wysw.ColorGreen;
            this.lcd.DrawTextInRect(pm25 + " µg/m³", 90, 100, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);

            this.lcd.DrawTextInRect("No₂:", 0, 125, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dNo2 > normaNo2) kolor = Wysw.ColorRed; else kolor = Wysw.ColorGreen;
            this.lcd.DrawTextInRect(No2 + " µg/m³", 90, 125, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);

            this.lcd.DrawTextInRect("So₂:", 0, 150, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dSo2 > normaSo2) kolor = Wysw.ColorRed; else kolor = Wysw.ColorGreen;
            this.lcd.DrawTextInRect(So2 + " µg/m³", 90, 150, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);

            this.lcd.DrawTextInRect("Temp:", 0, 175, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dtemp < normaOptymTemp - normatemphistereza) { kolor = Wysw.ColorBlue; }
            else if (dtemp > normaOptymTemp + normatemphistereza) { kolor = Wysw.ColorRed; }
            else { kolor = Wysw.ColorGreen; }
            this.lcd.DrawTextInRect(temp + " °C", 90, 175, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);

            this.lcd.DrawTextInRect("Pressure:", 0, 200, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dpressure < normacisnienie - normacisnieniehistereza) { kolor = Wysw.ColorBlue; }
            else if (dpressure > normacisnienie + normacisnieniehistereza) { kolor = Wysw.ColorRed; }
            else { kolor = Wysw.ColorGreen; }
            this.lcd.DrawTextInRect(pressure + " hPa", 90, 200, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);


            this.lcd.DrawTextInRect("Humidity:", 0, 225, 80, 25, Bitmap.DT_AlignmentLeft, Wysw.Colorwhite, Wysw.fCalibri20);
            if (dhumidity < normawilgotnoscpow - normawilgotnoscpowhistereza) { kolor = Wysw.ColorBlue; }
            else if (dhumidity > normawilgotnoscpow + normawilgotnoscpowhistereza) { kolor = Wysw.ColorRed; }
            else { kolor = Wysw.ColorGreen; }
            this.lcd.DrawTextInRect(humidity + " %", 90, 225, 160, 25, Bitmap.DT_AlignmentLeft, kolor, Wysw.fCalibri20);

            RysujBtnDol();
            this.lcd.Flush();
            ladowanieDanych = false;
        }

        public void BoxMiasto(string NazwaMiasta, int x, int y, int height, int width)
        {   
            this.lcd.DrawRectangle(Wysw.Colorwhite, 4, x, y, width, height, 0, 0, Wysw.Colorwhite, 0, 0, Wysw.Colorwhite, 0, 0, 0);
            this.lcd.DrawTextInRect(NazwaMiasta, x, y+6 , width, height, Bitmap.DT_AlignmentCenter, Wysw.ColorRed, Wysw.fCalibri20);
        }

        public void RysujBtnPrawo()
        {
            Bitmap icLeft = new Bitmap(Resources.GetBytes(Resources.BinaryResources.icon_left), Bitmap.BitmapImageType.Bmp);
            const int yPozycjaStrzałek = 250;
            this.lcd.RotateImage(180, SystemMetrics.ScreenWidth - icLeft.Width, yPozycjaStrzałek, icLeft, 0, 0, icLeft.Width, icLeft.Height, 0);
            this.lcd.Flush();
        }
        public void RysujBtnLewo()
        {
            Bitmap icLeft = new Bitmap(Resources.GetBytes(Resources.BinaryResources.icon_left), Bitmap.BitmapImageType.Bmp);
            const int yPozycjaStrzałek = 250;
            this.lcd.DrawImage(0, yPozycjaStrzałek, icLeft, 0, 0, icLeft.Width, icLeft.Height);
            this.lcd.Flush();
        }
        public void RysujBtnDol()
        {
            Bitmap icLeft = new Bitmap(Resources.GetBytes(Resources.BinaryResources.icon_left), Bitmap.BitmapImageType.Bmp);
            const int yPozycjaStrzałek = 250;
            this.lcd.RotateImage(270, (SystemMetrics.ScreenWidth / 2) - (icLeft.Width / 2), yPozycjaStrzałek, icLeft, 0, 0, icLeft.Width, icLeft.Height, 0);
            this.lcd.Flush();
        }

        private void MainWindow_TouchUp(object sender, TouchEventArgs e)
        {
            Debug.Print("Touch up at (" + e.Touches[0].X.ToString() + ", " + e.Touches[0].Y.ToString() + ")");
            if (ladowanieDanych == true) { return;} //jesli laduje dane nie reaguj na przyciskania

            //sprawdzenie czy klikneto strzalke w dol 
            if (mainScreen == false) //czyli jesli nie jestesmy na ekranie glownym
            {
                if ((e.Touches[0].X < SystemMetrics.ScreenWidth / 2 + 32) && (e.Touches[0].X > SystemMetrics.ScreenWidth / 2 - 32) && (e.Touches[0].Y > 250))
                {
                    this.lcd.Clear();
                    Debug.Print("Kliknieto strzalke w dol");
                    EkranStartowy();
                    return;
                }
            }

            if (mainScreen == true) //czyli jesli nie jestesmy na ekranie glownym
            {
                /************** GLIWICE **************/
                if (((e.Touches[0].X < SzerokoscBoxa) && (e.Touches[0].Y < 110 + WysokoscBoxa) && (e.Touches[0].Y > 110)))
                {
                    Debug.Print("Kliknieto gliwice");
                    EkranZanieczyszczenieMiasta(0);
                }

                /************** Krakow **************/
                else if (((e.Touches[0].X > 125) && (e.Touches[0].Y < 110 + WysokoscBoxa) && (e.Touches[0].Y > 110)))
                {
                    Debug.Print("Kliknieto Krakow");
                    EkranZanieczyszczenieMiasta(1);
                }
                /************** Shangahi **************/
                else if (((e.Touches[0].X < SzerokoscBoxa) && (e.Touches[0].Y < 160 + WysokoscBoxa) && (e.Touches[0].Y > 160)))
                {
                    Debug.Print("Kliknieto Shangahi");
                    EkranZanieczyszczenieMiasta(2);
                }
                /************** NY **************/
                else if (((e.Touches[0].X > 125) && (e.Touches[0].Y < 160 + WysokoscBoxa) && (e.Touches[0].Y > 160)))
                {
                    Debug.Print("Kliknieto NY");
                    EkranZanieczyszczenieMiasta(3);
                }

               /************** Sao Paulo **************/
                else if (((e.Touches[0].X < SzerokoscBoxa) && (e.Touches[0].Y < 210 + WysokoscBoxa) && (e.Touches[0].Y > 210)))
                {
                    Debug.Print("Kliknieto Paulo");
                    EkranZanieczyszczenieMiasta(4);
                }
                /************** Sydney **************/
                else if (((e.Touches[0].X > 125) && (e.Touches[0].Y < 210 + WysokoscBoxa) && (e.Touches[0].Y > 210)))
                {
                    Debug.Print("Kliknieto Sydney");
                    EkranZanieczyszczenieMiasta(5);
                }
                /************** Antalya **************/
                else if (((e.Touches[0].X < SzerokoscBoxa) && (e.Touches[0].Y < 260 + WysokoscBoxa) && (e.Touches[0].Y > 260)))
                {
                    Debug.Print("Kliknieto Antalya");
                    EkranZanieczyszczenieMiasta(6);
                }
                /************** Oslo **************/
                else if (((e.Touches[0].X > 125) && (e.Touches[0].Y < 260 + WysokoscBoxa) && (e.Touches[0].Y > 260)))
                {
                    Debug.Print("Kliknieto Oslo");
                    EkranZanieczyszczenieMiasta(7);
                }
            }

        }

        public void ZapytajOMiasto(string miasto)
        {   /***LADOWANIE**/
            this.lcd.Clear();
            this.lcd.DrawText("ŁADUJĘ", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
            this.lcd.Flush(); 
            /*************/
            string zadanie = "GET /feed/@" + miasto + "/?token=74dbfdb75b51b11bfdfd7c4bb6716ea1ccac22f2 HTTP/1.1\r\nHost: api.waqi.info\r\n\r\n";
           
            for (int i = 1; i <= 7; i++)
            {
                switch (i)
                {
                    case 1:
                        serialPortHelper1.PrintLine("AT+CIPSTART=\"TCP\",\"api.waqi.info\",80");
                        Thread.Sleep(500);
                        this.lcd.DrawText("ŁADUJĘ.", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush(); 
                        break;
                    case 2:
                        Thread.Sleep(500);
                        this.lcd.DrawText("ŁADUJĘ..", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush();
                        break;
                    case 3:
                        //obliczanie dlugosc srtringa
                        int dlugosc = zadanie.Length;
                        serialPortHelper1.PrintLine("AT+CIPSEND=" + dlugosc.ToString());
                        Thread.Sleep(500);
                        this.lcd.DrawText("ŁADUJĘ...", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush(); 
                        break;
                    case 4:
                        serialPortHelper1.PrintLine(zadanie);
                        Thread.Sleep(500);
                        this.lcd.DrawText("ŁADUJĘ....", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush(); 
                        break;
                    case 5:
                        Thread.Sleep(500);
                        this.lcd.DrawText("ŁADUJĘ.....", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush(); 
                        break;

                    case 6:
                        Thread.Sleep(500);
                        this.lcd.DrawText("ŁADUJĘ......", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush(); 
                        break;
                    case 7:
                        ZAMKNIJ();
                        Thread.Sleep(500);
                        Debug.Print("bufor to: ");
                        Debug.Print(serialPortHelper1.ReadAllBuf());
                        this.lcd.DrawText("ŁADUJĘ.......", Wysw.fCalibri32Bold, Wysw.Colorwhite, 10, 120);
                        this.lcd.Flush(); 
                        break;
              
                }
            }

        }
        private void ZAMKNIJ()
        {
            serialPortHelper1.PrintLine("AT+CIPCLOSE");
        }

        public static string Between(string STR, string FirstString, string LastString)
        {
            try
            {

                string FinalString;
                if (STR.IndexOf(FirstString) < 0) return "N/A";
                int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;

                string kopia = STR.Substring(Pos1);
                int Pos2 = kopia.IndexOf(LastString);

                FinalString = STR.Substring(Pos1, Pos2);
                return FinalString;
            }
            catch
            {
                return "N/A";
            }
        }

    }
}
/******************* (C) COPYRIGHT STMicroelectronics *****END OF FILE****/