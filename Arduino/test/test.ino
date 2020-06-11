#include <Adafruit_BMP085.h>



#include <Wire.h>



Adafruit_BMP085 bmp;

//clock a5
//data a4
void setup()   
{  
  
    pinMode(8, OUTPUT);  
    pinMode(9, OUTPUT);
    pinMode(10, OUTPUT);

    Serial.begin(9600);  
    if (!bmp.begin()) {
      Serial.println("Could not find a valid BMP180 sensor, check wiring!");
      while (1) {}
    }

    pinMode(4, INPUT);
    digitalWrite(4, LOW);
}  
  
void loop()   
{  
    
    
        char c = Serial.read();  
        if (c=='r'){
          if(digitalRead(4)==HIGH){
            Serial.println("111");
          }
          else{
            Serial.println("101");
          }
        }
        
        
        if (c == '1')   
        {  
           digitalWrite(8, HIGH);
           
        } 
        if (c == '2')   
        {  
            digitalWrite(9, HIGH);  
        }
        if (c == '3')   
        {  
            digitalWrite(10, HIGH);  
        }
        if (c == 'q')   
        {  
            digitalWrite(8, LOW);
            
        }  
        if (c == 'w')   
        {  
            digitalWrite(9, LOW);  
        }  
        if (c == 'e')   
        {  
            digitalWrite(10, LOW);  
        }  
        
        if (c=='7'){
          Serial.println(bmp.readTemperature());
          
          
        }
        
        
       
      
    
  
      
}  
