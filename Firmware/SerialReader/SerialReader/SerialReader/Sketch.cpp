/*Begining of Auto generated code by Atmel studio */
#include <Arduino.h>

/*End of auto generated code by Atmel studio */


//Beginning of Auto generated function prototypes by Atmel Studio
void Shift(int northChip, int southChip);
//End of Auto generated function prototypes by Atmel Studio


//Pin connected to latch pin (ST_CP) of 74HC595
const int latchPin = 13;
//Pin connected to clock pin (SH_CP) of 74HC595
const int clockPin = 11;
////Pin connected to Data in (DS) of 74HC595
const int dataPin = 12;

void setup() {
  Serial.begin(9600);

  pinMode(latchPin, OUTPUT);
  pinMode(clockPin, OUTPUT);
  pinMode(dataPin, OUTPUT);

  // POST
  digitalWrite(latchPin, LOW);
  shiftOut(dataPin, clockPin, MSBFIRST, 0);
  shiftOut(dataPin, clockPin, MSBFIRST, 0);
  digitalWrite(latchPin, HIGH);  
  
  int postVal = analogRead(0);
  if(postVal <= 10) 
  {
    for(int i = 0; i < 6; i++) 
    {
      Shift(1 << i, 0);
      delay(500);
    }

    // Reset
    Shift(0, 0);

    for(int i = 0; i < 6; i++) 
    {
      Shift(0, 1 << i);
      delay(500);
    }
    
    Shift(255, 255);
    delay(1000);
  }
  
  Shift(0, 0);
}

void loop() 
{
  char buffer[2];
  
  if(Serial.available() > 0) {
    if(Serial.readBytes(buffer, 2) > 0) {
      Shift(buffer[0], buffer[1]);
    }
  }
}

void Shift(int northChip, int southChip)
{
  digitalWrite(latchPin, LOW);
    shiftOut(dataPin, clockPin, MSBFIRST, southChip);
    shiftOut(dataPin, clockPin, MSBFIRST, northChip);
  digitalWrite(latchPin, HIGH);    
}
