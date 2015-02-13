#include "rs232.h"
#include "sci.h"
#include "iRobot_program.h"

static int com_port_no;

void set_com_port(int com)
{
	com_port_no = com;
}

void byteTx(unsigned char byte)
{
	RS232_SendByte(com_port_no, (unsigned char)byte);
}

void iRobot_program()
{
##main_program##
}