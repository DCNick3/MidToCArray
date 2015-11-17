#include <Arduino.h>
#include "TimerOne.h"
#include <util\delay.h>
#include <avr/pgmspace.h>
#define BEEP_PIN 10
#define SYNC_PIN 12

struct NOTE
{
    uint16_t frequency;
    uint16_t length;
};

const uint16_t noteFreqs[] = {8372, 8869, 9397, 9956, 10548, 11175, 11840, 12544, 13290, 14080, 14917, 15804};

//Music start!
#define MUSIC_LENGTH 0
const PROGMEM uint8_t music[] = { 1, 0, 86, 16, 69, 88, 65, 67, 86, 16, 69, 86, 16, 67, 86, 16, 41, 86, 16, 43, 86, 16, 41, 86, 16, 43, 87, 16, 0, 87, 65, 45, 172, 32, 40, 86, 16, 41, 43, 8, 43, 43, 8, 45, 172, 32, 40, 86, 16, 41, 43, 8, 43, 43, 8, 45, 157, 13};

//Music end!


void beep(int freq, int lng)
{
    unsigned long startTime = millis();
    if (freq != 0)
    {
        Timer1.initialize(1.0 / freq * 1000000.0);
        Timer1.pwm(BEEP_PIN, 512);
    }
    while (millis() - startTime < lng) {_delay_us(100);}
    Timer1.disablePwm(BEEP_PIN);
}
void whiteNoise(unsigned long lng)
{
    unsigned long startTime = millis();
    Timer1.initialize(10);
    Timer1.pwm(BEEP_PIN, 512);
    while (millis() - startTime < lng)
    {
        Timer1.setPwmDuty(BEEP_PIN, rand() % 1024);
        _delay_us(1);
    }
    Timer1.disablePwm(BEEP_PIN);
}

void play(const PROGMEM uint8_t *music, int musicLength)
{
    switch (pgm_read_byte_near(music))
    {
        case 0:
            for (int i = 0; i < musicLength; i++)
            {
                uint8_t fr0 = pgm_read_byte_near(music + i * 4 + 1);
                uint8_t fr1 = pgm_read_byte_near(music + i * 4 + 2);
                uint8_t ln0 = pgm_read_byte_near(music + i * 4 + 3);
                uint8_t ln1 = pgm_read_byte_near(music + i * 4 + 4);
                beep((uint16_t)(fr0 | fr1 << 8), (uint16_t)(ln0 | ln1 << 8));
            }
            break;
        case 1:
            for (int i = 0; i < musicLength; i++)
            {
                uint8_t nn0 = pgm_read_byte_near(music + i * 3 + 1);
                uint8_t ln0 = pgm_read_byte_near(music + i * 3 + 2);
                uint8_t ln1 = pgm_read_byte_near(music + i * 3 + 3);
                uint16_t fr;
                if (nn0 != 0)
                    fr = noteFreqs[(nn0 - 1) % 12] / pow(2, 10 - ((nn0 - 1) / 12));
                else
                    fr = 0;
                beep(fr, (uint16_t)(ln0 | ln1 << 8));
            }
            break;
    }
}

int main()
{
    init();
    //Serial.begin(9600);
    pinMode(BEEP_PIN, OUTPUT);
    beep(1000,1000);
    whiteNoise(100);
    while(1)
    {
        if (digitalRead(SYNC_PIN))
            play(music, MUSIC_LENGTH);
    }
    return 0;
}
