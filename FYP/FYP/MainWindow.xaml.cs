using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.IO;
using System.IO.Ports;






using System.ServiceModel;
using System.Windows.Threading;

namespace FYP
{



    public partial class MainWindow : Window
    {

        //lights
        bool light1state = false;
        bool light2state = false;
        bool light3state = false;
        bool movement = false;


        //temp
        string tempString;
        string tempSubString;
        double onlineTemp;
        double ambientTemp;
        double setTemp;
        double lowerBoundTemp = 5;
        double upperBoundTemp = 40;

        //time
        int incrementLightsSeconds = 0;
        int incrementTempSeconds = 0;
        int incrementTempMinutes = 0;
        int incrementSeconds = 0;
        double delayTime = 5;
        String portName;

        //internet
        bool hasInternet=false;

        //arduino 
        SerialPort sp = new SerialPort();
        double response;
        bool arduinoConnected;

        public MainWindow()
        {


            InitializeComponent();



        }

        void connectArduino()
        {
            
            try
            {

                sp.PortName = portName;
                sp.BaudRate = 9600;
                sp.Open();
                arduinoConnected = true;
                connectLabel.Content = "Arduino: Connected";
                connectButton.IsEnabled = false;
                
                updateLTemp.IsEnabled = true;
                sensorMovementCheckBox.IsEnabled = true;
                sp.WriteLine("");
                OutputText.AppendText("\n" + "Arduino Connected");
                OutputText.ScrollToEnd();
                getLTemp();

            }
            catch (Exception)
            {
                connectLabel.Content = "Arduino: Disconnected";
                Console.WriteLine("ERROR CONNECTING ARDUINO");
                
                OutputText.AppendText("\n" + "arduino did not connect");
                OutputText.AppendText("\n" + "Change COM port");
                OutputText.ScrollToEnd();
                
                arduinoConnected = false;
                connectButton.IsEnabled = true; ;
                updateLTemp.IsEnabled = false;
            }
        }

        void getOTemp()
        {

            string sURL;

            sURL = "http://api.openweathermap.org/data/2.5/forecast?id=2644668&APPID=90375e80d95a34c3df52b029eb02580e";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);



            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";



            sLine = objReader.ReadLine();
            tempString = sLine;
            if (tempString.Substring(78, 1) == ":")
            {
                tempSubString = tempString.Substring(79, 5);
            }
            else
            {
                tempSubString = tempString.Substring(78, 5);
            }
            Console.WriteLine(tempSubString);
            onlineTemp = Convert.ToDouble(tempSubString) - 273.2;
            onlineTemp = Math.Round(onlineTemp,2);
            OutputText.AppendText("\n" + "Online Temperature Update: " + onlineTemp);
            OutputText.ScrollToEnd();

            //Console.WriteLine(tempString);
            onlineTempLabel.Content = "Online Temperature: " + onlineTemp;

            if (onlineTemp < lowerBoundTemp)
            {

                OutputText.AppendText("\n" + "Online Temperature is lower than the predefined values. ");
                OutputText.AppendText("\n" + "Lower Bound: " + lowerBoundTemp);
                OutputText.ScrollToEnd();
                autoTempCheck.IsChecked = true;
            }
            else if (onlineTemp > upperBoundTemp)
            {

                OutputText.AppendText("\n" + "Online Temperature is higher than the predefined values. ");
                OutputText.AppendText("\n" + "Upper Bound: " + upperBoundTemp);
                OutputText.ScrollToEnd();
                autoTempCheck.IsChecked = true;
            }
            incrementTempMinutes = 0;
            incrementTempSeconds = 0;
        }

        void getLTemp()
        {
            sp.Write("7");

            ambientTemp = Convert.ToDouble(sp.ReadLine());
            ambientTempLabel.Content = "Ambient Temperature: " + ambientTemp;

            OutputText.AppendText("\n" + "Ambient Temperature Update: " + ambientTemp);
            OutputText.ScrollToEnd();

            if (ambientTemp < lowerBoundTemp)
            {

                OutputText.AppendText("\n" + "Ambient Temperature is lower than the predefined values. ");
                OutputText.AppendText("\n" + "Lower Bound: " + lowerBoundTemp);
                OutputText.ScrollToEnd();
                autoTempCheck.IsChecked = true;
            }
            else if (ambientTemp > upperBoundTemp)
            {

                OutputText.AppendText("\n" + "Ambient Temperature is higher than the predefined values. ");
                OutputText.AppendText("\n" + "Upper Bound: " + upperBoundTemp);
                OutputText.ScrollToEnd();
                autoTempCheck.IsChecked = true;
            }

           
        }
        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            switch (COMComboBox.SelectedIndex)
            {
                
                case 0:
                    portName = "COM3";
                    break;
                case 1:
                    portName = "COM4";
                    break;
                case 2:
                    portName = "COM5";
                    break;
                case 3:
                    portName = "COM6";
                    break;
                case 4:
                    portName = "COM7";
                    break;
            }


            connectArduino();
            
        } //ARDUINO CONNECTION 

        private void Light1On_Click(object sender, RoutedEventArgs e)
        {
            incrementLightsSeconds = 0;

            if (light1state)
            {
                OutputText.AppendText("\n" + "Light 1 already ON, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 1 is now ON.");
                OutputText.ScrollToEnd();
            }

            Light1StateLabel.Content = "Light 1 State: ON";
            light1state = true;
        }// LIGHT 1 ON


        private void Light1Off_Click(object sender, RoutedEventArgs e)
        {

            if (!light1state)
            {
                OutputText.AppendText("\n" + "Light 1 already OFF, no changes made.");
                OutputText.ScrollToEnd();

            }
            else
            {
                OutputText.AppendText("\n" + "Light 1 is now OFF.");
                OutputText.ScrollToEnd();
            }
            Light1StateLabel.Content = "Light 1 State: OFF";
            light1state = false;
        } // LIGHT 1 OFF
        private void Light2On_Click(object sender, RoutedEventArgs e)
        {
            incrementLightsSeconds = 0;
            if (light2state)
            {
                OutputText.AppendText("\n" + "Light 2 already ON, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 2 is now ON.");
                OutputText.ScrollToEnd();
            }
            Light2StateLabel.Content = "Light 2 State: ON";
            light2state = true;
        } //LIGHT 2 ON


        private void Light2Off_Click(object sender, RoutedEventArgs e)
        {
            if (!light2state)
            {
                OutputText.AppendText("\n" + "Light 2 already OFF, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 2 is now OFF.");
                OutputText.ScrollToEnd();
            }
            Light2StateLabel.Content = "Light 2 State: OFF";
            light2state = false;
        } //LIGHT 2 OFF
        private void Light3On_Click(object sender, RoutedEventArgs e)
        {
            incrementLightsSeconds = 0;
            if (light3state)
            {
                OutputText.AppendText("\n" + "Light 3 already ON, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 3 is now ON.");
                OutputText.ScrollToEnd();
            }
            Light3StateLabel.Content = "Light 3 State: ON";
            light3state = true;
        } //LIGHT 3 ON 


        private void Light3Off_Click(object sender, RoutedEventArgs e)
        {
            if (!light3state)
            {
                OutputText.AppendText("\n" + "Light 3 already OFF, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 3 is now OFF.");
                OutputText.ScrollToEnd();
            }
            Light3StateLabel.Content = "Light 3 State: OFF";
            light3state = false;
        } //LIGHT 3 OFF

        private void LightsOn_Click(object sender, RoutedEventArgs e)
        {
            incrementLightsSeconds = 0;
            if (light1state)
            {
                OutputText.AppendText("\n" + "Light 1 already ON, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 1 is now ON.");
                OutputText.ScrollToEnd();
            }
            Light1StateLabel.Content = "Light 1 State: ON";
            light1state = true;

            if (light2state)
            {
                OutputText.AppendText("\n" + "Light 2 already ON, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 2 is now ON.");
                OutputText.ScrollToEnd();
            }
            Light2StateLabel.Content = "Light 2 State: ON";
            light2state = true;

            if (light3state)
            {
                OutputText.AppendText("\n" + "Light 3 already ON, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 3 is now ON.");
                OutputText.ScrollToEnd();
            }
            Light3StateLabel.Content = "Light 3 State: ON";
            light3state = true;
        } //LIGHTS ON 


        private void LightsOff_Click(object sender, RoutedEventArgs e)
        {
            if (!light1state)
            {
                OutputText.AppendText("\n" + "Light 1 already OFF, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 1 is now OFF.");
                OutputText.ScrollToEnd();
            }
            Light1StateLabel.Content = "Light 1 State: OFF";
            light1state = false;

            if (!light2state)
            {
                OutputText.AppendText("\n" + "Light 2 already OFF, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 2 is now OFF.");
                OutputText.ScrollToEnd();
            }
            Light2StateLabel.Content = "Light 2 State: OFF";
            light2state = false;

            if (!light3state)
            {
                OutputText.AppendText("\n" + "Light 3 already OFF, no changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Light 3 is now OFF.");
                OutputText.ScrollToEnd();
            }
            Light3StateLabel.Content = "Light 3 State: OFF";
            light3state = false;

        } //LIGHTS OFF

        private void TempSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            TempVal.Content = Math.Round(TempSlider.Value);
            if (Math.Round(TempSlider.Value) == -10)
            {
                TempVal.Content = null;
            }


        } //UPDATE SLIDER TEXT

        private void TempSet_Click(object sender, RoutedEventArgs e)
        {
            if (setTemp == Math.Round(TempSlider.Value))
            {
                OutputText.AppendText("\n" + "Target temperature already set to: " + setTemp + ". No changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                if (Math.Round(TempSlider.Value) != -10)
                {
                    setTemp = Math.Round(TempSlider.Value);
                    OutputText.AppendText("\n" + "Target temperature set to: " + setTemp);
                    OutputText.ScrollToEnd();
                }
                else
                {
                    OutputText.AppendText("\n" + "Heater OFF");
                    OutputText.ScrollToEnd();
                }


            }

        } //SET TARGET TEMPERATURE 

        void updateOTemp_Click(object sender, RoutedEventArgs e)
        {
            getOTemp();

        } //MANUALLLY UPDATE TEMP ONLINE
        private void updateLTemp_Click(object sender, RoutedEventArgs e) //MANUALLY UPDATE TEMP LOCALLY
        {
            getLTemp();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //SET UPPER AND LOWER BOUNDS
        {
            
            if (Convert.ToDouble(lowerBoundTB.Text) == lowerBoundTemp)
            {
                OutputText.AppendText("\n" + "Lower Bound is already;" + lowerBoundTemp + ". No changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                lowerBoundTemp = Convert.ToDouble(lowerBoundTB.Text);
                OutputText.AppendText("\n" + "Lower Bound Set To;" + lowerBoundTemp);
                OutputText.ScrollToEnd();
            }
            if (Convert.ToDouble(upperBoundTB.Text) == upperBoundTemp)
            {
                OutputText.AppendText("\n" + "upper Bound is already;" + upperBoundTemp + ". No changes made.");
                OutputText.ScrollToEnd();
            }
            else
            {
                upperBoundTemp = Convert.ToDouble(upperBoundTB.Text);
                OutputText.AppendText("\n" + "Upper Bound Set To;" + upperBoundTemp);
                OutputText.ScrollToEnd();
            }

        }


        private void Window_Loaded(object sender, RoutedEventArgs e) //STARTS TIEMRS WHEN WINDOW LOADED
        {
            DispatcherTimer dtLights = new DispatcherTimer();
            DispatcherTimer dtTemp = new DispatcherTimer();
            DispatcherTimer dtTime = new DispatcherTimer();


            dtLights.Interval = TimeSpan.FromSeconds(1);
            dtLights.Tick += dtLightsInterval;
            dtLights.Start();

            dtTemp.Interval = TimeSpan.FromSeconds(1);
            dtTemp.Tick += dtTempInterval;
            dtTemp.Start();

            dtTime.Interval = TimeSpan.FromMilliseconds(1);
            dtTime.Tick += dtTimeInterval;
            dtTime.Start();

           
            

            if (CheckConnection() == true)
            {
                hasInternet = true;
                getOTemp();
            }
            else
            {
                hasInternet = false; 
            }
        }

        private void dtLightsInterval(object sender, EventArgs e) //LIGHT LOOP FOR MOTION DETECTION 
        {
            incrementLightsSeconds++;
            

            Light1StateLabel.ToolTip = "Sate: " + light1state;
            Light2StateLabel.ToolTip = "Sate: " + light2state;
            Light3StateLabel.ToolTip = "Sate: " + light3state;


            if (sensorMovementCheckBox.IsChecked == true)
            {

                sp.Write("r");
                response = Convert.ToDouble(sp.ReadLine());
                //Console.WriteLine("Sensor message sent");
                //Console.WriteLine("Read:" + response);

                if (response == 111)
                {
                    Console.WriteLine("Registered");
                    light1state = true;
                    light2state = true;
                    light3state = true;
                    if (movement == false)
                    {
                        movement = true;

                        OutputText.AppendText("\n" + "Motion Detected, Lights ON");

                        OutputText.ScrollToEnd();

                    }


                    incrementLightsSeconds = 0;

                    Light1StateLabel.Content = "Light 1 State: ON";
                    Light2StateLabel.Content = "Light 2 State: ON";
                    Light3StateLabel.Content = "Light 3 State: ON";


                }
                if (response == 101)
                {
                    Console.WriteLine("No Movement");
                    movement = false;



                }


            }
            if(sensorMovementCheckBox.IsChecked ==false&& simulatedMovementCheckBox.IsChecked == false)
            {
                movement = false;
            }

            if (incrementLightsSeconds >= delayTime)
            {
                incrementLightsSeconds = 0;
                if (simulatedMovementCheckBox.IsChecked == false)
                {
                    if (light1state)
                    {
                        Light1StateLabel.Content = "Light 1 State: OFF";
                        OutputText.AppendText("\n" + "Light 1 is now OFF. No Motion");
                        OutputText.ScrollToEnd();
                        light1state = false;
                    }
                    if (light2state)
                    {
                        Light2StateLabel.Content = "Light 2 State: OFF";
                        OutputText.AppendText("\n" + "Light 2 is now OFF. No Motion");
                        OutputText.ScrollToEnd();
                        light2state = false;
                    }
                    if (light3state)
                    {
                        Light3StateLabel.Content = "Light 3 State: OFF";
                        OutputText.AppendText("\n" + "Light 3 is now OFF. No Motion");
                        OutputText.ScrollToEnd();
                        light3state = false;
                    }
                }
            }
        }
        private void dtTempInterval(object sender, EventArgs e)
        {
            incrementTempSeconds++;
            if (incrementTempSeconds == 60)
            {
                if (CheckConnection() == true)
                {
                    hasInternet = true;
                }
                else
                {
                    hasInternet = false;
                }

                if (arduinoConnected)
                {
                    getLTemp();
                } //ARDUINO

                incrementTempSeconds = 0;
                incrementTempMinutes++;
            }
            if (incrementTempMinutes == 10)
            {
                incrementTempMinutes = 0;

                
                if (CheckConnection() == true)
                {
                    getOTemp();
                } //ONLINE
               

            }

        } //TEMP LOOP EVERY 10 MINUTES IF ARDUINO IS CONNECTED IT WILL TAKE A LOCAL VALUE IF NOT IS WILL USE ONLINE

        private void dtTimeInterval(object sender, EventArgs e) // things that should happen every millisecond
        {
            incrementSeconds++;
            TimeLabel.Content = "Light Timer: " + incrementLightsSeconds + " Timer: " + incrementSeconds + " Temp Timer: " + incrementTempMinutes + ":" + incrementTempSeconds;

            if (lowerBoundTemp > upperBoundTemp)
            {
                double tempvalue1 = upperBoundTemp;
                double tempvalue2 = lowerBoundTemp;
                lowerBoundTB.Text = Convert.ToString(tempvalue1);
                upperBoundTB.Text = Convert.ToString(tempvalue2);
                lowerBoundTemp = tempvalue1;
                upperBoundTemp = tempvalue2;

                OutputText.AppendText("\n" + "Lower Bound is larger than Upper Bound, Values Swapped");
                OutputText.ScrollToEnd();
            }
            if (ambientTemp > setTemp && setTemp!=0)
            {
                controlLabel.Content = "Temperature Control: Air Conditioning (" + setTemp + ")";
            }
            else if (setTemp != 0)
            {
                controlLabel.Content = "Temperature Control: Heating (" + setTemp + ")";
            }
            if (setTemp == 0)
            {
                controlLabel.Content = "Temperature Control: OFF";
            }

            if (incrementSeconds == 500)
            {
                incrementSeconds = 0;
            }
            

            if (hasInternet == true)
            {
                updateOTemp.IsEnabled = true;
                connectionLabel.Content = "Internet: True";
            }
            else
            {
                updateOTemp.IsEnabled = false;
                connectionLabel.Content = "Internet: False";
            }

            if (autoTempCheck.IsChecked == true)
            {
                lowerBoundTB.IsEnabled = false;
                upperBoundTB.IsEnabled = false;
                setBoundsButton.IsEnabled = false;
                if (arduinoConnected && hasInternet)
                {
                    setTemp = (ambientTemp+onlineTemp) / 2;
                }
                else
                {
                    if (arduinoConnected)
                    {
                        setTemp = (((lowerBoundTemp + upperBoundTemp) / 2) + ambientTemp) / 2;
                    }
                    else if (hasInternet)
                    {
                        setTemp = (((lowerBoundTemp + upperBoundTemp) / 2) + onlineTemp) / 2;
                    }
                    else
                    {
                        setTemp = (lowerBoundTemp + upperBoundTemp) / 2;
                    }
                }
                
                TempSlider.Value = setTemp;
                TempSlider.IsEnabled = false;
            }
            else
            {
                TempSlider.IsEnabled = true;
                lowerBoundTB.IsEnabled = true;
                upperBoundTB.IsEnabled = true;
                setBoundsButton.IsEnabled = true;
            }

            if (movement)
            {
                movementLabel.Content = "Movement: True";
                light1state = true;
                light2state = true;
                light3state = true;

                movement = true;

                incrementLightsSeconds = 0;

                Light1StateLabel.Content = "Light 1 State: ON";
                Light2StateLabel.Content = "Light 2 State: ON";
                Light3StateLabel.Content = "Light 3 State: ON";



            }
            else
            {
                movementLabel.Content = "Movement: False";

            }

            
            if (arduinoConnected)
            {


                if (light1state)
                {
                    sp.Write("1");

                }
                else
                {
                    sp.Write("q");
                }

                if (light2state)
                {
                    sp.Write("2");

                }
                else
                {
                    sp.Write("w");
                }
                if (light3state)
                {
                    sp.Write("3");

                }
                else
                {
                    sp.Write("e");
                }


            }

        }


        private void simulatedMovementCheckBox_Checked(object sender, RoutedEventArgs e) //Turns lights on and is used as a conditional statement 
        {
            light1state = true;
            light2state = true;
            light3state = true;

            movement = true;

            incrementLightsSeconds = 0;

            Light1StateLabel.Content = "Light 1 State: ON";
            Light2StateLabel.Content = "Light 2 State: ON";
            Light3StateLabel.Content = "Light 3 State: ON";

            OutputText.AppendText("\n" + "Motion Detected, Lights ON");

            OutputText.ScrollToEnd();
        }

        private void simulatedMovementCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            movement = false;

            incrementLightsSeconds = 0;
            OutputText.AppendText("\n" + "No Motion Detected, Lights OFF in " + delayTime + " Seconds.");
            OutputText.ScrollToEnd();

        } // conditional

        private void autoTempCheck_Checked(object sender, RoutedEventArgs e)
        {
            OutputText.AppendText("\n" + "Setting Temp to Auto");
            OutputText.ScrollToEnd();
        } //auto temp 

        private void delayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (delayComboBox.SelectedIndex)
            {
                case 0:
                    delayTime = 0;
                    break;
                case 1:
                    delayTime = 5;
                    break;
                case 2:
                    delayTime = 10;
                    break;
                case 3:
                    delayTime = 15;
                    break;
                case 4:
                    delayTime = 20;
                    break;
                case 5:
                    delayTime = 30;
                    break;
                case 6:
                    delayTime = 60;
                    break;
                case 7:
                    delayTime = 9999999999999;
                    break;
            }
            if(delayTime== 9999999999999)
            {
                OutputText.AppendText("\n" + "Time Delay Set To: Always On");
                OutputText.ScrollToEnd();
            }
            else
            {
                OutputText.AppendText("\n" + "Time Delay Set To: " + delayTime);
                OutputText.ScrollToEnd();
            }
            
        }
        private bool CheckConnection() //tries to open google if it cannot open it, it will produce an error and return false 
        {
            WebClient client = new WebClient();
            try
            {
                using (client.OpenRead("http://www.google.com"))
                {
                }
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        private void tempOffBtn_Click(object sender, RoutedEventArgs e)
        {
            TempSlider.Value = 0;
            setTemp = 0;
            autoTempCheck.IsChecked = false;
            OutputText.AppendText("\n" + "Temperature sytem: OFF");
            OutputText.ScrollToEnd();
        }
    }
}
