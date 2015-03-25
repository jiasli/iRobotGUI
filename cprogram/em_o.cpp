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
//SONG_DEF 0,52,32,52,32,53,32,55,32,55,32,53,32,52,32,50,32
byteTx(CmdSong);
byteTx(0);
byteTx(8);
byteTx(52);
byteTx(32);
byteTx(52);
byteTx(32);
byteTx(53);
byteTx(32);
byteTx(55);
byteTx(32);
byteTx(55);
byteTx(32);
byteTx(53);
byteTx(32);
byteTx(52);
byteTx(32);
byteTx(50);
byteTx(32);


}