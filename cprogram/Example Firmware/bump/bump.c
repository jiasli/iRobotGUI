/* bump.c
 * Designed to run on the Mind Control.
 *
 * Copyright 2005, Element Products, Inc.
 *
 * Roomba is started by pressing the blinking "max" button.  Roomba
 * drives around with motors off and beeps and turns when it bumps
 * into something.  It turns off with a button press or when it 
 * detects a cliff.  
 *
 * The basic architecture of this program can be re-used to easily 
 * write a wide variety of Roomba control programs.  All sensor values
 * are polled in the background (using the serial rx interrupt) and 
 * stored in the sensors array as long as the function 
 * delaySensors() is called periodically.  Users can send SCI commands
 * directly a byte at a time using byteTx() or they can use the 
 * provided functions, such as baud() and drive().
 */



// Includes
#include <avr/interrupt.h>
#include <avr/io.h>
#include <avr/signal.h>
#include <stdlib.h>
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
void delaySensors(unsigned int time_ms);
void initialize(void);
void wake(void);
void baud(uint8_t baud_code);
void drive(int16_t velocity, int16_t radius);
uint16_t randomAngle(void);
void defineSongs(void);




int main (void) 
{
  uint8_t temp = 0;
  int16_t turn_angle = 0;
  uint8_t turn_dir = 1;
  uint8_t turning = 0;


  // Set up the Mind Control and Roomba SCI
  initialize();
  wake();
  byteTx(CmdStart);
  baud(Baud28800);
  defineSongs();
  byteTx(CmdControl);
  byteTx(CmdFull);


  // Wait for a max button press to start
  while(!(sensors[SenButton] & 0x01))
  {
    // Flash the max button led at 1 Hz
    if(temp++ & 0x80)
    {
      byteTx(CmdLeds);
      byteTx(0x00);
      byteTx(0);
      byteTx(0);
    }
    else
    {
      byteTx(CmdLeds);
      byteTx(0x02);
      byteTx(0);
      byteTx(0);
    }

    delaySensors(4);
  }


  // Turn on the status and dirt leds
  byteTx(CmdLeds);
  byteTx(0x11);
  byteTx(0);
  byteTx(0);

  // Play the start song and wait while it plays
  byteTx(CmdPlay);
  byteTx(0);
  delaySensors(1500);


  // Drive around until a button or cliff is detected
  while((!(sensors[SenButton] & 0x0F))
      && (!sensors[SenCliffL])
      && (!sensors[SenCliffFL])
      && (!sensors[SenCliffFR])
      && (!sensors[SenCliffR]))
  {
    // Keep turning until the specified angle is reached
    if(turning)
    {
      if(turn_dir)
      {
        if(angle > turn_angle)
          turning = 0;
        drive(200, RadCCW);
      }
      else
      {
        if((-angle) > turn_angle)
          turning = 0;
        drive(200, RadCW);
      }

      // Turn on the side brush while turning
      byteTx(CmdMotors);
      byteTx(0x01);

      // Beep while turning
      byteTx(CmdPlay);
      byteTx(1);
    }
    // Check for a bump
    else if(sensors[SenBumpDrop] & 0x03)
    {
      // Set the turn paramters and reset the angle
      if(sensors[SenBumpDrop] & 0x02)
        turn_dir = 0;
      else
        turn_dir = 1;
      turning = 1;
      angle = 0;
      turn_angle = randomAngle();

      // Back up a little
      drive(-200, RadStraight);

      // Play the bump beeps
      byteTx(CmdPlay);
      byteTx(2);
      delaySensors(100);
    }
    else
    {
      // Otherwise, drive straight
      drive(300, RadStraight);

      // Turn off the side brush
      byteTx(CmdMotors);
      byteTx(0x00);
    }

    // Delay 1 ms so sensors can be updated
    delaySensors(1);
  }


  // Stop driving
  drive(0, RadStraight);

  // Play the end song
  delay(500);
  byteTx(CmdPlay);
  byteTx(3);
  delay(1000);

  // Turn off the leds
  byteTx(CmdLeds);
  byteTx(0x00);
  byteTx(0);
  byteTx(0);

  // Turn Roomba off
  byteTx(CmdPower);


  // Wait in an infinite loop
  for(;;)
  {
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
  uint8_t temp;

  timer_on = 1;
  timer_cnt = time_ms;
  while(timer_on)
  {
    if(!sensors_flag)
    {
      for(temp = 0; temp < Sen0Size; temp++)
        sensors[temp] = sensors_in[temp];

      // Update running totals of distance and angle
      distance += (int)((sensors[SenDist1] << 8) | sensors[SenDist0]);
      angle += (int)((sensors[SenAng1] << 8) | sensors[SenAng0]);

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




// Return an angle value in the range 150 to 405 (67 to 180 degrees)
uint16_t randomAngle(void)
{
  return (150 + (uint16_t)(random() & 0xFF));
}




// Define songs to be played later
void defineSongs(void)
{
  // Start song
  byteTx(CmdSong);
  byteTx(0);
  byteTx(9);
  byteTx(76);
  byteTx(8);
  byteTx(69);
  byteTx(8);
  byteTx(71);
  byteTx(8);
  byteTx(69);
  byteTx(8);
  byteTx(66);
  byteTx(8);
  byteTx(69);
  byteTx(8);
  byteTx(78);
  byteTx(8);
  byteTx(76);
  byteTx(8);
  byteTx(81);
  byteTx(16);

  // Turning beep
  byteTx(CmdSong);
  byteTx(1);
  byteTx(2);
  byteTx(69);
  byteTx(8);
  byteTx(0);
  byteTx(8);

  // Bump beeps
  byteTx(CmdSong);
  byteTx(2);
  byteTx(2);
  byteTx(67);
  byteTx(24);
  byteTx(71);
  byteTx(16);

  // End song
  byteTx(CmdSong);
  byteTx(3);
  byteTx(3);
  byteTx(57);
  byteTx(32);
  byteTx(49);
  byteTx(16);
  byteTx(45);
  byteTx(16);
}
