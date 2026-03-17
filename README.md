# BOF App

## Suomi (FI)

### Sovelluksen tarkoitus
BOF App hakee Suomen Pankin avoimesta rajapinnasta setelidataa valitulta aikaväliltä ja näyttää:
- määrän muutoksen
- summan muutoksen (EUR)
- nimellisarvokohtaisen erittelyn
- valinnaisen valuuttamuunnoksen valittuun valuuttaan

### Keskeiset ominaisuudet
- Aikavälin valinta (`Alkupäivä` / `Loppupäivä`)
- Setelimuutosten yhteenveto kortilla
- Näytä/piilota erittely (`Näytä erittely` / `Piilota erittely`)
- Väritetty arvoesitys:
	- negatiivinen = punainen
	- positiivinen = vihreä
- Valinnainen valuuttamuunnos valitulla valuutalla

### Tekninen rakenne
- Frontend: React + Vite + MUI
- Backend: ASP.NET Core Web API (.NET 10)
- Oletus-API-osoite frontendissä: `http://localhost:5040/api`

### Vaatimukset
- .NET SDK 10
- Node.js + npm

### Käynnistysohjeet

#### 1) Backend
```bash
cd backend
dotnet restore
dotnet run
```
Backend käynnistyy oletuksena osoitteeseen `http://localhost:5040`.

#### 1b) Backend Dockerilla
```bash
docker build -t bof-backend:latest ./backend
docker run --rm -p 5040:8080 bof-backend:latest
```
Backend on käytettävissä osoitteessa `http://localhost:5040`.

#### 2) Frontend
```bash
cd frontend
npm install
npm start
```
Frontend käynnistyy oletuksena osoitteeseen `http://localhost:5173`.

### Käyttöohje (askel askeleelta)
1. Avaa frontend selaimessa.
2. Valitse `Alkupäivä` ja `Loppupäivä`.
3. (Valinnainen) Valitse valuutta pudotusvalikosta.
4. Paina `Hae Data`.
5. Tarkastele oikealla näkyvää tuloskorttia:
	 - Alkupäivä / Loppupäivä
	 - Määrän muutos
	 - Summan muutos (EUR)
	 - Summan muutos valitussa valuutassa (jos valuutta valittu)
6. Avaa nimellisarvokohtainen erittely `Show breakdown` -painikkeella.

### Testit
Backend-testit:
```bash
dotnet test
```

### Vianmääritys
- Jos frontend ei saa dataa:
	- varmista että backend on käynnissä portissa `5040`
	- varmista että `frontend/.env` sisältää oikean arvon:
		- `VITE_API_URL=http://localhost:5040/api`
- Jos saat CORS-virheen:
	- varmista että frontend ajetaan osoitteessa `http://localhost:5173`

---

## English (EN)

### App purpose
BOF App fetches Bank of Finland open API data for a selected date range and displays:
- quantity change
- amount change (EUR)
- denomination-level breakdown
- optional currency conversion to a selected currency

### Main features
- Date range selection (`Start date` / `End date`)
- Banknote summary card
- Expand/collapse breakdown (`Show breakdown` / `Hide breakdown`)
- Color-coded values:
	- negative = red
	- positive = green
- Optional currency conversion

### Technical stack
- Frontend: React + Vite + MUI
- Backend: ASP.NET Core Web API (.NET 10)
- Frontend API base URL (default): `http://localhost:5040/api`

### Requirements
- .NET SDK 10
- Node.js + npm

### How to run

#### 1) Backend
```bash
cd backend
dotnet restore
dotnet run
```
Backend runs by default at `http://localhost:5040`.

#### 1b) Backend with Docker
```bash
docker build -t bof-backend:latest ./backend
docker run --rm -p 5040:8080 bof-backend:latest
```
Backend is available at `http://localhost:5040`.

#### 2) Frontend
```bash
cd frontend
npm install
npm start
```
Frontend runs by default at `http://localhost:5173`.

### User manual (step by step)
1. Open the frontend in your browser.
2. Select `Start date` and `End date`.
3. (Optional) Pick a currency from the dropdown.
4. Click `Hae Data`.
5. Review the results card on the right:
	 - Start/End date
	 - Quantity change
	 - Amount change (EUR)
	 - Amount change in selected currency (if selected)
6. Click `Show breakdown` to view denomination-level rows.

### Tests
Run backend tests:
```bash
dotnet test
```

### Troubleshooting
- If frontend cannot load data:
	- ensure backend is running on port `5040`
	- verify `frontend/.env` contains:
		- `VITE_API_URL=http://localhost:5040/api`
- If you get a CORS error:
	- ensure frontend is served from `http://localhost:5173`
