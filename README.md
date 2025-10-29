# 🦆 Sortomatic 9000
Sortomatic 9000 är ett filsorteringsprogram som automatiskt övervakar dina mappar och flyttar filer till rätt plats utifrån regler i en inställningsfil (XML-format).
Programmet är perfekt för att hålla ordning på t.ex. Hämtade filer-mappen utan att lyfta ett finger.

## Installation

### Steg-för-steg

1.	Ladda ner programmet
   
    Klicka på den gröna knappen “Code”, välj sedan “Download ZIP”.
    Spara filen på ett ställe du lätt hittar, t.ex. på Skrivbordet.

2.	Öppna programmet

   * Öppna mappen /../../../../net25-kurs-1-sortomatic-9000-discovicke/Uppgift/dist
   * Dubbelklicka på filen Sortomatic9000.exe.
   * Programmet startar direkt och börjar läsa in Inställningar.xml.
  	
3.	Låt det arbeta i bakgrunden

    När programmet startar kommer det automatiskt att läsa Inställningar.xml och börja övervaka mapparna du angett där.
    Du kan minimera programmet — det fortsätter arbeta tills du stänger det.

#### Så fungerar det

Sortomatic 9000 använder en XML-fil för att veta vilka mappar som ska övervakas och vart filer med olika ändelser ska flyttas. Du kan lägga till fler Directories om du vill bevaka fler mappar eller flytta andra typer av filer till andra mappar.
Här är ett exempel på hur Inställningar.xml kan se ut:

```
<?xml version="1.0" encoding="UTF-8" ?>
<Settings>
  <Log>log.txt</Log>

  <Directory>
    <Name>Bilder</Name>
    <Input>C:\Users\Knatte\Downloads</Input>
    <Output>C:\Users\Knatte\Pictures</Output>
    <Type>.jpg</Type>
    <Type>.png</Type>
  </Directory>

  <Directory>
    <Name>Dokument</Name>
    <Input>C:\Users\Knatte\Downloads</Input>
    <Output>C:\Users\Knatte\Documents</Output>
    <Type>.txt</Type>
    <Type>.pdf</Type>
  </Directory>
</Settings>
```

---

## Tips och extras

* Programmet körs i bakgrunden och håller koll på nya eller ändrade filer i realtid.
* Det sorterar även filer som tillkommit under tiden programmet inte varit igång, perfekt för att rensa upp en försummad mapp.
* Ändringar i Inställningar.xml laddas automatiskt utan att du behöver starta om programmet.
* Loggfilen (log.txt) uppdateras kontinuerligt med datum, klockslag och vad som flyttats vart.
* Du kan enkelt anpassas genom att redigera konfigurationsfilen, inga kommandon eller flaggor behövs.
