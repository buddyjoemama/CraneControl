#include <Arduino.h>

void Shift(int, int);
void HandleRotation(int);

//Pin connected to latch pin (ST_CP) of 74HC595
const int latchPin = 10;
//Pin connected to clock pin (SH_CP) of 74HC595
const int clockPin = 11;
////Pin connected to Data in (DS) of 74HC595
const int dataPin = 12;

//
// Cycles through all LEDS and motors.
//
void powerOnSelfTest() {
	pinMode(latchPin, OUTPUT);
	pinMode(clockPin, OUTPUT);
	pinMode(dataPin, OUTPUT);

	Shift(255, 255);
	delay(3000);
	Shift(0, 0);
	
	for(int i = 0; i < 6; i++)
	{
		Shift(1 << i, 0);
		delay(3000);
	}

	delay(1000);
	// Reset
	Shift(0, 0);

	for(int i = 0; i < 6; i++)
	{
		Shift(0, 1 << i);
		delay(3000);
	}

	delay(1000);
	Shift(0, 0);
}

int rotation = 0;
void loop()
{
	if(Serial.available() > 0) {
		int available = Serial.available();
		
		char *buffer = new char[available];
		Serial.readBytes(buffer, available);		
		//Serial.println(buffer);
		 //Serial.println('buteavailable);
		
		//if(Serial.readBytes(buffer, 3) > 0) {
			//Shift(buffer[0], buffer[1]);
			////HandleRotation(buffer[2]);
			//rotation = buffer[2];
		//}
	}

	//HandleRotation(rotation);
}

void Shift(int northChip, int southChip)
{
	digitalWrite(latchPin, LOW);
	shiftOut(dataPin, clockPin, MSBFIRST, southChip);
	shiftOut(dataPin, clockPin, MSBFIRST, northChip);
	digitalWrite(latchPin, HIGH);
}

static int CW = B10000000; // 128
static int CCW = B01000000; // 64

// bit:
//MSB 7 - CW
// 6 - CCW
// 5 - value msb
// 4 -
// 3 -
// 2 -
// 1 -
//LSB 0 - value lsb
void HandleRotation(int val)
{
	// Mask out first two bits to get pulse wait time ms
	int pulseMs = val & B00111111;
	int cwVal = val & CW;
	int ccwVal = val & CCW;
	
	if((val & CW) == CW) {
		
	}
	else if((val & CCW) == CCW) {
		
	}

	delay(pulseMs);
}


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

void TC5_Handler (void) 
{
	
	TC5->COUNT16.INTFLAG.bit.MC0 = 1; //don't change this, it's part of the timer code
}

void tcStartCounter()
{
	TC5->COUNT16.CTRLA.reg |= TC_CTRLA_ENABLE; //set the CTRLA register
	while (tcIsSyncing()); //wait until snyc'd
}

uint32_t sampleRate = 500; //milliseconds
void setup()
{

	
	Serial.begin(9600);
	//powerOnSelfTest();
	
	//tcConfigure(sampleRate);
	//tcStartCounter();
}