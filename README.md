# AnkiCardMaker
Add list of kanji to anki for autogenerated flashcards with data from Jisho.org

![image](https://github.com/Bluskyfishing/AnkiCardMaker/assets/121456599/93200d7b-1d52-4a51-92ca-c134358a96ab)

**Example:**<br>
**FrontSide:**
![image](https://github.com/Bluskyfishing/AnkiCardMaker/assets/121456599/376f1682-e94f-4c07-91b9-3ba179fc898f)

**BackSide:**
![image](https://github.com/Bluskyfishing/AnkiCardMaker/assets/121456599/94eba8ac-47a1-4e17-8f36-334d17729495)

<h3>Features:</h3>
1. Adding one or more kanji to text file for easy import to anki. <br>
2. Pulls information from jisho.org for flashcards. <br>
3. Clickable kanji on flashcard for easy lookup on "Kanji Study"-app (Android only). <br>
4. Clickable links for context and pronunciation on flashcards. <br>
<br>

<h2>Getting started</h2>

<h3>Anki settings:</h3> 

Field Names in anki: 
Kanji, JMdictID, Furigana, Meaning

Card Type Settings: <br>
**Front Template:**

```
<div class=small>{{hint:Furigana}}</div>
<div class=big>{{Kanji}}</div>
```
**Back Template:**
```
<a href="kanjistudy://word?id={{JMdictID}}">

<div class=small>{{Furigana}}</div>
<div class=big>{{Kanji}}</div>

</a><br>{{Meaning}}</div>

<br>

<a style='font-size:18px; text-decoration: Underline; float:center'
        
	href='https://tatoeba.org/en/sentences/search?from=jpn&query={{Kanji}}&to=eng&source=lnms&tbm=isch'>(Context)</a>  

<a style='font-size:18px; text-decoration: Underline; float:center'
        
	href='https://youglish.com/pronounce/{{Kanji}}/japanese'>(Pronunciation)</a>
```
**Styling:**
```
.card {
 font-size: 25px;
 text-align: center;
 --text-color: black;
 word-wrap: break-word;
}
.card.night_mode {
 font-size: 24px;
 text-align: center;
 --text-color: white;
 word-wrap: break-word;
}

div, a {
 color: var(--text-color);
}
.card a { text-decoration-color: #A1B2BA; }

.big { font-size: 50px; }
.small { font-size: 18px;}
```
After setting up:
go to "Import file" on anki
file will be in "webScraperTest\bin\Debug\net8.0" named after date created .txt
Check that every line is filled out before importing: 

![image](https://github.com/Bluskyfishing/AnkiCardMaker/assets/121456599/2f30fd16-f31d-4c7f-8b1a-de3f36c8f242)

<h4>Additional reccomendation:</h4>
Anki addon for pitch accent:
https://ankiweb.net/shared/info/148002038
I like to apply the pitch accent on the meaning field! :)
