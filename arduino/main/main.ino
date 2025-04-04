#include "SD.h"
#include "SPI.h"
#include <TimeLib.h>

const int MQ2_D0 = 3;
const int muxSigPin = A0;
const int s0 = 8;
const int s1 = 9;
const int s2 = 10;
const int s3 = 11;
const int ledPin = 13;      // Onboard LED pin to indicate detection
const int chipSelect = 2;


void selectMuxChannel(int channel) {
  digitalWrite(s0, channel & 0x01);
  digitalWrite(s1, (channel >> 1) & 0x01);
  digitalWrite(s2, (channel >> 2) & 0x01);
  digitalWrite(s3, (channel >> 3) & 0x01);
}

void setup() {
  Serial.begin(9600);

  pinMode(MQ2_D0, INPUT);

  pinMode(s0, OUTPUT);
  pinMode(s1, OUTPUT);
  pinMode(s2, OUTPUT);
  pinMode(s3, OUTPUT);
  
  pinMode(ledPin, OUTPUT);

  while (!Serial) {
    ;
  }

  Serial.println("Initializing SD card...");

  if (!SD.begin(chipSelect)) {
    Serial.println("Card failed, or not present.");
    while (true);
  }
  Serial.println("Card initialized.");

  setTimeFromCompiler();
}

void loop() {
  selectMuxChannel(0);
  int flameValue = analogRead(muxSigPin);

  int flameThreshold = 315;

  Serial.println("Flame Sensor Value: " + String(flameValue));

  if (flameValue < flameThreshold) {
    Serial.println("Flame detected!");
    writeToFile("Flame detected!");
    digitalWrite(ledPin, HIGH);
  } else {
    Serial.println("No flame detected.");
    digitalWrite(ledPin, LOW);
  }

  selectMuxChannel(1);
  int irSensorValue = analogRead(muxSigPin);

  Serial.println("IR Sensor Value: " + String(irSensorValue));
  
  if (irSensorValue < 310) {
    Serial.println("Object detected by IR sensor!");
    writeToFile("Object detected by IR sensor!");
    digitalWrite(ledPin, HIGH);
  } else {
    Serial.println("No object detected by IR sensor.");
    digitalWrite(ledPin, LOW);
  }

  selectMuxChannel(2);

  int gasDetected = analogRead(muxSigPin);

  if (gasDetected < 250) {
      Serial.println("Warning! Gas detected!");
      writeToFile("Warning! Gas detected!");
  } else {
      Serial.println("No gas detected.");
  }

  delay(8000);
}

void writeToFile(char *info) {
    File myFile = SD.open("info.txt", FILE_WRITE);

    if (myFile) {
      myFile.print(hour());
      myFile.print(":");
      myFile.print(minute());
      myFile.print(":");
      myFile.print(second());
      myFile.print(" ");
      myFile.print(day());
      myFile.print("-");
      myFile.print(month());
      myFile.print("-");
      myFile.println(year());
      myFile.println(info);
      myFile.close();
      Serial.println("Done writing to file.");
    }
    else {
      Serial.println("Error opening file.");
    }

    return;
}

void setTimeFromCompiler() {
  int hh, mm, ss, DD, MM, YYYY;

  sscanf(__TIME__, "%d:%d:%d", &hh, &mm, &ss);
  char monthStr[4];
  sscanf(__DATE__, "%s %d %d", monthStr, &DD, &YYYY);

  String months = "JanFebMarAprMayJunJulAugSepOctNovDec";
  MM = (months.indexOf(monthStr) / 3) + 1;

  setTime(hh, mm, ss, DD, MM, YYYY);
}
