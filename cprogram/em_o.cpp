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
//LED 0,13,128

byteTx(CmdLeds);
byteTx(0);
byteTx(13);
byteTx(128);


//SONG_DEF 1,84,32,84,32,91,32,91,32,93,32,93,32,91,32,89,32,89,32,88,32,88,32,86,32,86,32,84,32

byteTx(CmdSong);
byteTx(1);
byteTx(14);

byteTx(84);
byteTx(32);
byteTx(84);
byteTx(32);
byteTx(91);
byteTx(32);
byteTx(91);
byteTx(32);
byteTx(93);
byteTx(32);
byteTx(93);
byteTx(32);
byteTx(91);
byteTx(32);
byteTx(89);
byteTx(32);
byteTx(89);
byteTx(32);
byteTx(88);
byteTx(32);
byteTx(88);
byteTx(32);
byteTx(86);
byteTx(32);
byteTx(86);
byteTx(32);
byteTx(84);
byteTx(32);

//SONG_PLAY 1

byteTx(CmdPlay);
byteTx(1);



}