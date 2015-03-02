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
//SONG_DEF 1,64,32,64,32,65,32,67,32,67,32,65,32,64,32,62,32,60,32,60,32,62,32,64,32,64,48,62,16,62,64

byteTx(CmdSong);
byteTx(1);
byteTx(15);

byteTx(64);
byteTx(32);
byteTx(64);
byteTx(32);
byteTx(65);
byteTx(32);
byteTx(67);
byteTx(32);
byteTx(67);
byteTx(32);
byteTx(65);
byteTx(32);
byteTx(64);
byteTx(32);
byteTx(62);
byteTx(32);
byteTx(60);
byteTx(32);
byteTx(60);
byteTx(32);
byteTx(62);
byteTx(32);
byteTx(64);
byteTx(32);
byteTx(64);
byteTx(48);
byteTx(62);
byteTx(16);
byteTx(62);
byteTx(64);

//SONG_PLAY 1

byteTx(CmdPlay);
byteTx(1);


//LEFT 90

angle = 0;
drive(200, RadCCW);
while(angle < 90)
{
    delaySensors(100);
}
drive(0, RadStraight);  



}