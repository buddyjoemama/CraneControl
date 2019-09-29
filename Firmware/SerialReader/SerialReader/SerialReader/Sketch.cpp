#include <Arduino.h>

void Shift(int, int);

//Pin connected to latch pin (ST_CP) of 74HC595
const int latchPin = 10;
//Pin connected to clock pin (SH_CP) of 74HC595
const int clockPin = 11;
////Pin connected to Data in (DS) of 74HC595
const int dataPin = 12;

uint32_t sampleRate = 500;

 bool tcIsSyncing()
 {
	 return TC5->COUNT16.STATUS.reg & TC_STATUS_SYNCBUSY;
 }
 
 void tcReset()
 {
	 TC5->COUNT16.CTRLA.reg = TC_CTRLA_SWRST;
	 while (tcIsSyncing());
	 while (TC5->COUNT16.CTRLA.bit.SWRST);
 }

 void tcConfigure(int sampleRate)
 {
	 // Enable GCLK for TCC2 and TC5 (timer counter input clock)
	 GCLK->CLKCTRL.reg = (uint16_t) (GCLK_CLKCTRL_CLKEN | GCLK_CLKCTRL_GEN_GCLK0 | GCLK_CLKCTRL_ID(GCM_TC4_TC5)) ;
	 while (GCLK->STATUS.bit.SYNCBUSY);

	 tcReset(); //reset TC5

	 // Set Timer counter Mode to 16 bits
	 TC5->COUNT16.CTRLA.reg |= TC_CTRLA_MODE_COUNT16;
	 // Set TC5 mode as match frequency
	 TC5->COUNT16.CTRLA.reg |= TC_CTRLA_WAVEGEN_MFRQ;
	 //set prescaler and enable TC5
	 TC5->COUNT16.CTRLA.reg |= TC_CTRLA_PRESCALER_DIV1024 | TC_CTRLA_ENABLE;
	 //set TC5 timer counter based off of the system clock and the user defined sample rate or waveform
	 TC5->COUNT16.CC[0].reg = (uint16_t) (SystemCoreClock / sampleRate - 1);
	 while (tcIsSyncing());
	 
	 // Configure interrupt request
	 NVIC_DisableIRQ(TC5_IRQn);
	 NVIC_ClearPendingIRQ(TC5_IRQn);
	 NVIC_SetPriority(TC5_IRQn, 0);
	 NVIC_EnableIRQ(TC5_IRQn);

	 // Enable the TC5 interrupt request
	 TC5->COUNT16.INTENSET.bit.MC0 = 1;
	 while (tcIsSyncing()); //wait until TC5 is done syncing
 }
 
 void TC5_Handler (void) {
	 //YOUR CODE HERE
	 //if(state == true) {
		 //digitalWrite(LED_PIN,HIGH);
		 //} else {
		 //digitalWrite(LED_PIN,LOW);
	 //}
	 //state = !state;
	 // END OF YOUR CODE
	 TC5->COUNT16.INTFLAG.bit.MC0 = 1; //don't change this, it's part of the timer code
 }
 
 void tcStartCounter()
 {
	 TC5->COUNT16.CTRLA.reg |= TC_CTRLA_ENABLE; //set the CTRLA register
	 while (tcIsSyncing()); //wait until snyc'd
 }

void setup() {

	tcConfigure(sampleRate);
	tcStartCounter();
	
	Serial.begin(9600);
	
	return;
	
	pinMode(latchPin, OUTPUT);
	pinMode(clockPin, OUTPUT);
	pinMode(dataPin, OUTPUT);

	Shift(255, 255);
	delay(3000);
	Shift(0, 0);
	
	for(int i = 0; i < 6; i++)
	{
		Shift(1 << i, 0);
		delay(1000);
	}

	delay(1000);
	// Reset
	Shift(0, 0);

	for(int i = 0; i < 6; i++)
	{
		Shift(0, 1 << i);
		delay(1000);
	}

	delay(1000);
	Shift(0, 0);
}

char *message = "Welcome";

void loop()
{
	// Wait until host connects.	
	while(!Serial);
	Serial.write(message, strlen_P(message));
	
	while(true) {
		
	}
	//int len = 0;
	//if((len = Serial.available()) > 0) {
		//char *buffer = new char[len];
		//
		//Serial.readBytes(buffer, len);
		//
		//int i = 2;
		//int cx = i + 3;
	//}
	//String s;
	//char* buffer;
	//
	//int count = 0;
	//if((count = Serial.available()) > 0) {
		//if(Serial.readBytes(buffer, 2) > 0) {
			//Shift(buffer[0], buffer[1]);
			//Serial.write("ok");
		//}
	//}
}

void Shift(int northChip, int southChip)
{
	digitalWrite(latchPin, LOW);
	shiftOut(dataPin, clockPin, MSBFIRST, southChip);
	shiftOut(dataPin, clockPin, MSBFIRST, northChip);
	digitalWrite(latchPin, HIGH);
}
