void Shift(int, int);
void HandleRotation(int);

//Pin connected to latch pin (ST_CP) of 74HC595
const int latchPin = 10;
//Pin connected to clock pin (SH_CP) of 74HC595
const int clockPin = 11;
////Pin connected to Data in (DS) of 74HC595
const int dataPin = 12;

const int PLATFORM_CW = 9;
const int PLATFORM_CCW = 6;
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

void setup() 
{
	Serial.begin(9600);
	powerOnSelfTest();
}

void loop()
{
	char buffer[2];

	if(Serial.available() > 0) {
		if(Serial.readBytes(buffer, 3) > 0) {
			Shift(buffer[0], buffer[1]);
			HandleRotation(buffer[2]);			
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
	
	if((val & CW) == CW) {
		
	}
	else if((val & CCW) == CCW) {
		
	}
}
