/* simple.c
 * Designed to run on the Mind Control.
 *
 * Copyright 2005, Element Products, Inc.
 *
 * Demonstrates a simple use of the Roomba SCI. 
 * A "clean" button press turns the vacuum motor on.
 * A "spot" button press turns the vacuum motor off.
 */




// Included files
#include <avr/interrupt.h>
#include <avr/io.h>
#include <avr/delay.h>
#include "sci.h"




// Functions
void initialize(void);
void wake(void);
void baud28k(void);
void delay(uint8_t delay_10ms);
uint8_t byteRx(void);
void flushRx(void);
void byteTx(uint8_t value);




int main (void) 
{
  uint8_t buttons = 0;

  // Initialize the atmel
  initialize();

  // Wake the roomba using the DD pin
  wake();

  // Start the interface
  byteTx(CmdStart);

  // Change to 28800 baud
  baud28k();

  // Take full control of the Roomba
  byteTx(CmdControl);
  byteTx(CmdFull);

  // Turn on the spot and clean leds
  byteTx(CmdLeds);
  byteTx(0x0C);
  byteTx(0);
  byteTx(0);

  // Get rid of unwanted data in the serial port receiver
  flushRx();

  for(;;)
  {
    // Request the 6 middle sensor bytes
    byteTx(CmdSensors);
    byteTx(2);

    // Read the 6 bytes, only keep the buttons data byte
    byteRx();
    buttons = byteRx();
    byteRx();
    byteRx();
    byteRx();
    byteRx();

    // If the clean button is pressed
    if(buttons & 0x02)
    {
      // Turn on the vacuum motor
      byteTx(CmdMotors);
      byteTx(0x02);
    }
    // Else, if the spot button is pressed
    else if(buttons & 0x04)
    {
      // Turn off the vacuum motor
      byteTx(CmdMotors);
      byteTx(0x00);
    }
  }
}




void initialize(void)
{
  // Turn off interrupts
  cli();

  // Configure the I/O pins
  DDRB = 0x00;
  PORTB = 0xFF;
  DDRC = 0x00;
  PORTC = 0xFF;
  DDRD = 0x06;
  PORTD = 0xFD;

  // Set up the serial port for 57600 baud
  UBRR0 = Ubrr57600;
  UCSR0B = (_BV(TXEN0) | _BV(RXEN0));
  UCSR0C = (_BV(UCSZ00) | _BV(UCSZ01));
}




void wake(void)
{
  // Set the DD pin low for 100 ms
  PORTD &= ~(0x04);
  delay(10);
  PORTD |= 0x04;
}




void baud28k(void)
{
  // Send the baud change command for 28800 baud
  byteTx(CmdBaud);
  byteTx(Baud28800);

  // Wait while until the command is sent
  while(!(UCSR0A & _BV(TXC0))) ;

  // Change the atmel's baud rate
  UBRR0 = Ubrr28800;

  // Wait 100 ms
  delay(10);
}




void delay(uint8_t delay_10ms)
{
  // Delay for (delay_10ms * 10) ms
  while(delay_10ms-- > 0)
  {
    // Call a 10 ms delay loop
    _delay_loop_2(46080);
  }
}




uint8_t byteRx(void)
{
  // Receive a byte over the serial port (UART)
  while(!(UCSR0A & _BV(RXC0))) ;
  return UDR0;
}




void flushRx(void)
{
  uint8_t temp;

  // Clear the serial port
  while(UCSR0A & _BV(RXC0))
    temp = UDR0;
}




void byteTx(uint8_t value)
{
  // Send a byte over the serial port
  while(!(UCSR0A & _BV(UDRE0))) ;
  UDR0 = value;
}
