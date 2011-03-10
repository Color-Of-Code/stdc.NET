/* fputc example: alphabet writer */
#include <stdio.h>

int main ()
{
  FILE * pFile;
  char c;

  pFile = fopen ("ex_fputc.txt","w");
  if (pFile!=NULL)
  {
    for (c = 'A' ; c <= 'Z' ; c++)
    {
      fputc ( (int) c , pFile );
    }
    fclose (pFile);
  }
  return 0;
}