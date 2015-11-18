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
#define MUSIC_LENGTH 269
const PROGMEM uint8_t music[] = { 4, 64, 174, 0, 69, 92, 1, 69, 93, 1, 64, 92, 1, 72, 93, 1, 71, 93, 1, 67, 93, 1, 74, 92, 1, 81, 185, 2, 59, 174, 0, 62, 174, 0, 60, 174, 0, 72, 185, 2, 79, 185, 2, 76, 93, 1, 84, 92, 1, 86, 93, 1, 88, 185, 2, 79, 92, 1, 81, 93, 1, 83, 185, 2, 72, 174, 0, 62, 93, 1, 76, 175, 0, 69, 185, 2, 65, 93, 1, 60, 92, 1, 59, 92, 1, 64, 93, 1, 62, 92, 1, 74, 93, 1, 71, 92, 1, 0, 92, 1, 128, 130, 131, 133, 128, 140, 128, 130, 131, 133, 128, 149, 151, 129, 128, 130, 131, 133, 128, 140, 128, 130, 131, 133, 128, 149, 151, 129, 128, 130, 131, 133, 128, 140, 128, 130, 131, 133, 128, 149, 151, 129, 128, 130, 131, 133, 128, 140, 128, 130, 131, 133, 128, 149, 151, 129, 128, 130, 131, 133, 128, 140, 128, 130, 131, 133, 128, 149, 76, 11, 2, 138, 134, 157, 130, 138, 71, 185, 2, 138, 134, 157, 130, 138, 71, 174, 0, 74, 175, 0, 129, 128, 130, 131, 133, 128, 140, 128, 130, 131, 133, 128, 149, 76, 11, 2, 138, 134, 157, 130, 138, 71, 185, 2, 138, 134, 157, 130, 138, 71, 174, 0, 74, 11, 2, 139, 153, 154, 134, 139, 152, 139, 153, 154, 134, 139, 152, 137, 150, 155, 156, 137, 62, 185, 2, 137, 150, 155, 156, 137, 62, 185, 2, 139, 153, 154, 134, 139, 152, 139, 153, 154, 134, 139, 152, 137, 150, 155, 156, 137, 62, 185, 2, 137, 150, 155, 156, 137, 150, 129, 132, 135, 142, 141, 136, 143, 144, 145, 129, 132, 135, 142, 141, 136, 143, 144, 145, 131, 134, 129, 132, 74, 185, 2, 76, 185, 2, 146, 147, 148, 129, 132, 135, 79, 93, 1, 136, 159, 158, 146, 147, 148, 129, 132, 135, 142, 141, 136, 143, 144, 145, 129, 132, 135, 142, 141, 136, 143, 144, 145, 131, 134, 129, 132, 74, 185, 2, 76, 185, 2, 146, 147, 148, 129, 132, 135, 79, 93, 1, 136, 159, 158, 146, 147, 148, 129, 132, 135, 142, 141, 136, 143, 144, 145, 129, 132, 135, 142, 141, 136, 143, 144, 145, 131, 134, 129, 132, 74, 185, 2, 76, 185, 2, 146, 147, 148, 129, 132, 135, 79, 93, 1, 136, 159, 158, 146, 147, 148};

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

void playNumbers(const PROGMEM uint8_t *music, int musicLength, int dictionarySize)
{
    //Serial.println(dictionarySize);
    int pos = 0;
    for (int i = 0; i < musicLength; i++)
            {
                Serial.println(pos);
                uint8_t nn0 = pgm_read_byte_near(music + pos + 1 + dictionarySize);
                uint8_t ln0;
                uint16_t fr;
                uint8_t ln1;
                if (!(nn0 & B10000000))
                {
                    ln0 = pgm_read_byte_near(music + pos + 2 + dictionarySize);
                    ln1 = pgm_read_byte_near(music + pos + 3 + dictionarySize);
                    pos += 3;
                }
                else
                {
                    uint8_t dn = (nn0 & B01111111) * 3;
                    //Serial.println("dic");
                    //Serial.println(dn);
                    nn0 = pgm_read_byte_near(music + 1 + dn);
                    ln0 = pgm_read_byte_near(music + 2 + dn);
                    ln1 = pgm_read_byte_near(music + 3 + dn);
                    pos++;
                }
                if (nn0 != 0)
                    fr = noteFreqs[(nn0 - 1) % 12] / pow(2, 10 - ((nn0 - 1) / 12));
                else
                    fr = 0;
                //Serial.println(fr);
                //Serial.println((uint16_t)(ln0 | ln1 << 8));
                beep(fr, (uint16_t)(ln0 | ln1 << 8));
            }
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
            playNumbers(music, musicLength, 0);
            break;
        case 2:
            playNumbers(music, musicLength, 128 * 3);
            break;
        case 3:
            playNumbers(music, musicLength, 64 * 3);
            break;
        case 4:
            playNumbers(music, musicLength, 32 * 3);
            break;
        case 5:
            playNumbers(music, musicLength, 16 * 3);
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
