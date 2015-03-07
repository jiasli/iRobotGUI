/* testall.c
 * Designed to run on the Mind Control.
 *
 * Copyright 2005, Element Products, Inc.
 *
 * Designed to demonstrate and test all of the features of the Roomba 
 * SCI.  This program can be used as a reference for adding features 
 * to other Roomba control code.  The program runs through a series of 
 * tests.  The Roomba beeps at the start and then at the end of each 
 * test.  Be sure the Roomba is on the floor before running the
 * program since it will make the Roomba move.  The tests are:
 *
 * 1) Switches through baud codes 0 to 8.  For each baud, it plays a 
 *    note and displays the baud code in binary on the LEDs.
 * 2) Turns on each of the LEDs in turn and then cycles through the 
 *    intensity and color values for the power LED.
 * 3) Plays all of the possible notes on the Roomba in order from 
 *    note number 31 to 127.
 * 4) Turns on each of the cleaning motors one at a time.
 * 5) Uses the drive command, along with the angle and distance 
 *    sensors, to drive forward 100 mm and then back to where it 
 *    started, then to turn 45 degrees and back again.
 * 6) Starts a normal Roomba spot cleaning cycle.  It lets the Roomba
 *    run for 10 seconds and then stops it.
 * 7) Starts a normal Roomba cleaning cycle.  It lets the Roomba run
 *    for 10 seconds and then stops it.
 * 8) Turns off the Roomba power for 2 seconds and then wakes it up 
 *    again.
 * 9) Starts a normal Roomba max cleaning cycle.  It lets the Roomba
 *    run for 10 seconds and then stops it.
 * 10) Plays a song corresponding to the 16-bit value of each of the 
 *     six charging sensors. Each note in a song represents a single 
 *     hexidecimal digit.  The LEDs display the index of the sensor
 *     song playing, which are listed below:
 *       1 = Charging state
 *       2 = Voltage (in mV)
 *       3 = Current (in mA)
 *       4 = Temperature (in degrees C)
 *       5 = Charge (in mAh)
 *       6 = Capacity (in mAh)
 * 11) The Roomba becomes a musical instrument, with each sensor 
 *     mapped to a different note.  If you activate the sensor, it 
 *     will play the note.  The notes and sensors are, in ascending 
 *     order: 
 *       C4 = Bump right
 *       D4 = Bump left
 *       E4 = Wall
 *       F4 = Cliff right
 *       G4 = Cliff front right
 *       A4 = Cliff front left
 *       B4 = Cliff left
 *       C5 = Wheeldrop (any wheel)
 *       D5 = Max button
 *       E5 = Clean button
 *       F5 = Spot button
 *       G5 = Power button
 *       A5 = Virtual wall
 *       B5 = Remote (any command)
 *       C6 = Dirt sensor (either side)
 *       D6 = Overcurrent (any motor)
 */




// Includes
#include <avr/interrupt.h>
#include <avr/io.h>
#include <avr/signal.h>
#include "sci.h"




// Global variables
volatile uint16_t timer_cnt = 0;
volatile uint8_t timer_on = 0;
volatile uint8_t sensors_flag = 0;
volatile uint8_t sensors_index = 0;
volatile uint8_t sensors_in[Sen0Size];
volatile uint8_t sensors[Sen0Size];
volatile int16_t distance = 0;
volatile int16_t angle = 0;




// Functions
void byteTx(uint8_t value);
void delay(uint16_t time_ms);
void delaySensors(uint16_t time_ms);
void initialize(void);
void wake(void);
void baud(uint8_t baud_code);
void drive(int16_t velocity, int16_t radius);
uint16_t randomAngle(void);
void defineSongs(void);
void playNote(uint8_t note, uint8_t dur);
void valueToSong(uint16_t value);
void valueToLeds(uint8_t value);
void testDone(void);




int main (void) 
{
  uint8_t temp = 0;


  // Set up the Mind Control and Roomba SCI
  initialize();
  wake();
  byteTx(CmdStart);
  baud(Baud28800);
  defineSongs();
  byteTx(CmdControl);
  byteTx(CmdFull);


  // Beep at startup
  testDone();


  // Test 1, switch through bauds 0 to 8
  for(temp = 0; temp <= 8; temp++)
  {
    baud(temp);
    byteTx(CmdControl);
    byteTx(CmdFull);

    valueToLeds(temp);
    playNote((32 + (temp * 4)), 32);
  }
  testDone();


  // Test 2, light all LEDs and cycle through power LED values
  for(temp = 0x01; temp < 0x41; temp <<= 1)
  {
    if(temp == 0x40)
      temp = 0x30;
    byteTx(CmdLeds);
    byteTx(temp);
    byteTx(0);
    byteTx(0);
    delay(750);
  }

  // Turn on the power LED intensity slowly
  for(temp = 0; temp < 255; temp++)
  {
    byteTx(CmdLeds);
    byteTx(0);
    byteTx(0);
    byteTx(temp);
    delay(10);
  }

  // Change the power LED color slowly
  for(temp = 0; temp < 255; temp++)
  {
    byteTx(CmdLeds);
    byteTx(0);
    byteTx(temp);
    byteTx(255);
    delay(10);
  }
  testDone();


  // Test 3, play all possible notes
  for(temp = 31; temp < 128; temp++)
    playNote(temp,12);
  testDone();
 

  // Test 4, turn on each of the motors in turn
  for(temp = 0x01; temp < 0x08; temp <<= 1)
  {
    byteTx(CmdMotors);
    byteTx(temp);
    delaySensors(1500);
    byteTx(CmdMotors);
    byteTx(0x00);
    delaySensors(750);
  }
  testDone();


  // Test 5, drive forward and back, then turn and back
  distance = 0;
  // Drive forward 100 mm
  drive(150, RadStraight);
  while(distance < 100) 
    delaySensors(1);
  drive(0, RadStraight);
  delaySensors(750);

  // Drive backward until distance is 0 again
  drive(-150, RadStraight);
  while(distance > 0)
    delaySensors(1);
  drive(0, RadStraight);
  delaySensors(750);

  // Turn counter-clockwise 45 degrees
  angle= 0;
  drive(150, RadCCW);
  while(angle < 101) 
    delaySensors(1);
  drive(0, RadStraight);
  delaySensors(750);

  // Turn clockwise until angle is 0 again
  drive(150, RadCW);
  while(angle > 0)
    delaySensors(1);
  drive(0, RadStraight);
  testDone();
  

  // Test 6, spot cleaning for 10 seconds
  byteTx(CmdSpot);
  delaySensors(10000);
  byteTx(CmdControl);
  byteTx(CmdFull);
  testDone();


  // Test 7, cleaning for 10 seconds
  byteTx(CmdClean);
  delaySensors(10000);
  byteTx(CmdControl);
  byteTx(CmdFull);
  testDone();


  // Test 8, power off for 2 seconds
  byteTx(CmdPower);
  delaySensors(2000);
  wake();
  byteTx(CmdStart);
  byteTx(CmdControl);
  byteTx(CmdFull);
  testDone();


  // Test 9, max cleaning for 10 seconds
  byteTx(CmdMax);
  delaySensors(10000);
  byteTx(CmdControl);
  byteTx(CmdFull);
  testDone();


  // Test 10, play songs for the values of the charging sensors
  valueToLeds(1);
  valueToSong((int16_t)sensors[SenState]);
  valueToLeds(2);
  valueToSong((int16_t)((sensors[SenVolt1] << 8) | sensors[SenVolt0]));
  valueToLeds(3);
  valueToSong((int16_t)((sensors[SenCurr1] << 8) | sensors[SenCurr0]));
  valueToLeds(4);
  valueToSong((int16_t)sensors[SenTemp]);
  valueToLeds(5);
  valueToSong((int16_t)((sensors[SenCharge1] << 8) | sensors[SenCharge0]));
  valueToLeds(6);
  valueToSong((int16_t)((sensors[SenCap1] << 8) | sensors[SenCap0]));
  testDone();


  // Test 11, play a different note for each sensor
  for(;;)
  {
    if(sensors[SenOverC])
      playNote(86,24);
    else if(sensors[SenDirtL] || sensors[SenDirtR])
      playNote(84,24);
    else if(sensors[SenRemote] != 255)
      playNote(83,24);
    else if(sensors[SenVWall])
      playNote(81,24);
    else if(sensors[SenButton] & 0x08)
      playNote(79,24);
    else if(sensors[SenButton] & 0x04)
      playNote(77,24);
    else if(sensors[SenButton] & 0x02)
      playNote(76,24);
    else if(sensors[SenButton] & 0x01)
      playNote(74,24);
    else if(sensors[SenBumpDrop] & 0x1C)
      playNote(72,24);
    else if(sensors[SenCliffL])
      playNote(71,24);
    else if(sensors[SenCliffFL])
      playNote(69,24);
    else if(sensors[SenCliffFR])
      playNote(67,24);
    else if(sensors[SenCliffR])
      playNote(65,24);
    else if(sensors[SenWall])
      playNote(64,24);
    else if(sensors[SenBumpDrop] & 0x02)
      playNote(62,24);
    else if(sensors[SenBumpDrop] & 0x01)
      playNote(60,24);

    // Delay in order to update sensor values
    delaySensors(1);
  }
}




// Serial receive interrupt to store sensor values
SIGNAL(SIG_USART_RECV)
{
  uint8_t temp;


  temp = UDR0;

  if(sensors_flag)
  {
    sensors_in[sensors_index++] = temp;
    if(sensors_index >= Sen0Size)
      sensors_flag = 0;
  }
}




// Timer 1 interrupt to time delays in ms
SIGNAL(SIG_OUTPUT_COMPARE1A)
{
  if(timer_cnt)
    timer_cnt--;
  else
    timer_on = 0;
}




// Transmit a byte over the serial port
void byteTx(uint8_t value)
{
  while(!(UCSR0A & _BV(UDRE0))) ;
  UDR0 = value;
}




// Delay for the specified time in ms without updating sensor values
void delay(uint16_t time_ms)
{
  timer_on = 1;
  timer_cnt = time_ms;
  while(timer_on) ;
}




// Delay for the specified time in ms and update sensor values
void delaySensors(uint16_t time_ms)
{
  uint8_t i;

  timer_on = 1;
  timer_cnt = time_ms;
  while(timer_on)
  {
    if(!sensors_flag)
    {
      for(i = 0; i < Sen0Size; i++)
        sensors[i] = sensors_in[i];

      // Update running totals of distance and angle
      distance += (int16_t)((sensors[SenDist1] << 8) | sensors[SenDist0]);
      angle += (int16_t)((sensors[SenAng1] << 8) | sensors[SenAng0]);

      byteTx(CmdSensors);
      byteTx(0);
      sensors_index = 0;
      sensors_flag = 1;
    }
  }
}




// Initialize the Mind Control's ATmega168 microcontroller
void initialize(void)
{
  cli();

  // All I/O pins are inputs except serial transmit
  DDRB = 0x00;
  PORTB = 0xFF;
  DDRC = 0x00;
  PORTC = 0xFF;
  DDRD = 0x06;
  PORTD = 0xFD;

  // Set up timer 1 to generate an interrupt every 1 ms
  TCCR1A = 0x00;
  TCCR1B = (_BV(WGM12) | _BV(CS12));
  OCR1A = 72;
  TIMSK1 = _BV(OCIE1A);

  // Set up the serial port with rx interrupt
  UBRR0 = 19;
  UCSR0B = (_BV(RXCIE0) | _BV(TXEN0) | _BV(RXEN0));
  UCSR0C = (_BV(UCSZ00) | _BV(UCSZ01));

  sei();
}




// Wake up the Roomba using the device detect line
void wake(void)
{
  delay(200);
  PORTD &= ~(0x04);
  delay(100);
  PORTD |= 0x04;
  delay(10);
}




// Switch the baud rate on both the Roomba and the Mind Control
void baud(uint8_t baud_code)
{
  if(baud_code <= 11)
  {
    byteTx(CmdBaud);
    UCSR0A |= _BV(TXC0);
    byteTx(baud_code);
    // Wait until transmit is complete
    while(!(UCSR0A & _BV(TXC0))) ;

    cli();

    // Switch the baud rate register
    if(baud_code == Baud115200)
      UBRR0 = Ubrr115200;
    else if(baud_code == Baud57600)
      UBRR0 = Ubrr57600;
    else if(baud_code == Baud38400)
      UBRR0 = Ubrr38400;
    else if(baud_code == Baud28800)
      UBRR0 = Ubrr28800;
    else if(baud_code == Baud19200)
      UBRR0 = Ubrr19200;
    else if(baud_code == Baud14400)
      UBRR0 = Ubrr14400;
    else if(baud_code == Baud9600)
      UBRR0 = Ubrr9600;
    else if(baud_code == Baud4800)
      UBRR0 = Ubrr4800;
    else if(baud_code == Baud2400)
      UBRR0 = Ubrr2400;
    else if(baud_code == Baud1200)
      UBRR0 = Ubrr1200;
    else if(baud_code == Baud600)
      UBRR0 = Ubrr600;
    else if(baud_code == Baud300)
      UBRR0 = Ubrr300;

    sei();

    delay(100);
  }
}




// Send the Roomba drive commands in terms of velocity and radius
void drive(int16_t velocity, int16_t radius)
{
  byteTx(CmdDrive);
  byteTx((uint8_t)((velocity >> 8) & 0x00FF));
  byteTx((uint8_t)(velocity & 0x00FF));
  byteTx((uint8_t)((radius >> 8) & 0x00FF));
  byteTx((uint8_t)(radius & 0x00FF));
}




// Define songs to be played later
void defineSongs(void)
{
  // Test end beep
  byteTx(CmdSong);
  byteTx(0);
  byteTx(1);
  byteTx(60);
  byteTx(16);
}




// Play a note by defining a one note song and then playing it
void playNote(uint8_t note, uint8_t dur)
{
    byteTx(CmdSong);
    byteTx(15);
    byteTx(1);
    byteTx(note);
    byteTx(dur);

    byteTx(CmdPlay);
    byteTx(15);
    delaySensors(20 * dur);
}




// Play a 4 note song corresponding to a 16-bit value
void valueToSong(uint16_t value)
{
  int8_t digit = 0;

  for(digit = 3; digit >= 0; digit--)
    playNote(72 + ((value >> (digit * 4)) & 0x0F),16);
  delaySensors(1000);
}




// Display a value in binary on the LEDs
void valueToLeds(uint8_t value)
{
  byteTx(CmdLeds);
  byteTx(value & 0x2F);
  byteTx(0);
  byteTx((value & 0x10)?255:0);
}




// Beep and turn off LEDs at the end of each test
void testDone(void)
{
  byteTx(CmdLeds);
  byteTx(0x00);
  byteTx(0);
  byteTx(0);

  delay(500);
  byteTx(CmdPlay);
  byteTx(0);
  delay(1000);
}
