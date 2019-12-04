void Shift(int, int);
void HandleRotation(int);

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
int rotation = 0;
void loop()
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
	if(Serial.available() > 0) {
		int available = Serial.available();
		
		char *buffer = new char[available];
		Serial.readBytes(buffer, available);		
		Serial.println(buffer);
		Serial.println(available);
		
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
