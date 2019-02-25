// File Encryption Program in C.

/*		 WARNING : do not give the have the same sourcefile and
destinationfile, encrypting or decrypting, this bug will be worked
out as i get more interested in file encryption
*/

#include <stdio.h>

#define ENCRYPTION_FORMULA  (int) Byte + 25
#define DECRYPTION_FORMULA  (int) Byte - 25

int Encrypt(char * FILENAME, char * NEW_FILENAME)
{
	//printf("Loaded Encrypt");
	FILE *inFile;		 		 		 		 		 //Declare inFile
	FILE *outFile;		 		 		 		 		 //Declare outFile

	char Byte;
	char newByte;
	int n;
	int i=0;


	printf("1");

	inFile = fopen(FILENAME,"rb");
	outFile = fopen(NEW_FILENAME, "w");

	if(inFile==NULL)
		printf("Error: Can't Open inFile");

	if(outFile==NULL)
	{
		printf("Error: Can't open outFile.");
		return 1;
	}
	else
	{
		printf("File Opened, Encrypting");
		while(1)
		{
			printf(".");

			if(Byte!=EOF)
			{
				Byte=fgetc(inFile);
				//		 printf("%d",Byte);
				newByte=Byte+25;

				fputc(newByte,outFile);
			}
			else
			{
				printf("End of File");
					break;
			}
		}
		fclose(inFile);
		fclose(outFile);
	}
}

int Decrypt (char *FILENAME, char *NEW_FILENAME)
{
	//printf("Loaded Decrypt");
	FILE *inFile;		 		 		 		 		 //Declare inFile
	FILE *outFile;		 		 		 		 		 //Declare outFile

	char Byte;
	char newByte;
	int i=0;

	printf("2");

	inFile = fopen(FILENAME,"rb");
	outFile = fopen(NEW_FILENAME, "w");

	if(inFile==NULL)
		printf("Error: Can't Open inFile");

	if(outFile==NULL)
	{
		printf("Error: Can't open outFile.");
		return 1;
	}
	else
	{

		printf("File Opened, Decrypting");
		while(1)
		{
			printf(".");

			if(Byte!=EOF)
			{
				Byte=fgetc(inFile);
				//		 printf("%d",Byte);
				newByte=Byte-25;

				fputc(newByte,outFile);
			}
			else
			{
				printf("End of File");
					break;
			}
		}
		fclose(inFile);
		fclose(outFile);
	}
}

int main()
{
	char encFile[200];
	char newencFile[200];
	char decFile[200];
	char newdecFile[200];

	int choice;

	printf("NOTE: you must Decrypt the file with the same file extension!!!");
	printf("Enter 1 to Encrypt  / 2 to Decrypt");
	scanf("%d",&choice);

	switch(choice)
	{
	case 1:
		printf("Enter the Source Filename:  ");
		scanf("%s",&encFile);
		printf("Enter the Destination Filename:   ");
		scanf("%s",&newencFile);
		Encrypt(encFile, newencFile);
		break;
	case 2:
		printf("Enter the Source Filename:   ");
		scanf("%s",&decFile);
		printf("Enter the Destination Filename:   ");
		scanf("%s",&newdecFile);
		Decrypt(decFile, newdecFile);
		break;
	}
	return 0;
}
