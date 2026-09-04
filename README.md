# Receptenboek

Repository voor het project OOSDD Periode 1. Beheert recepten, bereidingsstappen, ingrediënten en statistieken binnen een gelaagde C# console-architectuur.

---

## Branching-strategie (Gitflow)

Binnen dit project werken we volgens Gitflow[cite: 5]. Direct pushen naar `main` of `develop` is niet toegestaan; alle codewijzigingen verlopen via pull requests[cite: 5].

### Vaste branches
* **`main`**: Bevat uitsluitend stabiele, geteste code voorzien van een release-tag volgens Semantic Versioning (SemVer)[cite: 5].
* **`develop`**: Centrale integratiebranch voor afgeronde functionaliteiten[cite: 5].

### Tijdelijke branches
* **`feature/<usecase-id>-<korte-beschrijving>`**
  * Bronbranch: `develop`[cite: 5]
  * Doelbranch: `develop` (via PR)[cite: 5]
  * Voorbeeld: `feature/uc01-recept-toevoegen`[cite: 5]
* **`release/vX.Y.Z`**
  * Bronbranch: `develop`[cite: 5]
  * Doelbranch: `main` (inclusief tag) én `develop`[cite: 5]
* **`hotfix/vX.Y.Z`**
  * Bronbranch: `main`[cite: 5]
  * Doelbranch: `main` én `develop`[cite: 5]

---

## Commit-standaarden

We volgen de Conventional Commits-specificatie:
* `feat:` Nieuwe functionaliteit (bijv. `feat: berekening gemiddelde bereidingstijd toevoegen`)
* `fix:` Bugfix (bijv. `fix: null reference bij leeg ingredient afvangen`)
* `test:` Tests toevoegen of aanpassen (bijv. `test: a3-unittests voor ReceptenStatistiekService`)[cite: 4]
* `docs:` Wijzigingen in documentatie of canvas (bijv. `docs: probleemanalyse bijwerken`)[cite: 1]
* `refactor:` Code optimaliseren zonder functionele impact

---

## Pull Request-proces

Elke bijdrage aan de codebase doorloopt de volgende stappen:

1. **Aanmaken:** Open een PR vanaf de feature-branch naar `develop`[cite: 5].
2. **Documenteren:** Koppel het betreffende issue- of usecase-nummer, vat de wijzigingen samen en verwijs naar de uitgevoerde tests[cite: 5].
3. **Pipeline-validatie:** De CI-pipeline moet foutloos bouwen en alle tests moeten slagen[cite: 4, 5].
4. **Peer review:** Minimaal één teamlid inspecteert de code op SRP, modulariteit en code smells conform de HBO-ICT richtlijnen[cite: 3]. Feedback wordt verwerkt en traceerbaar vastgelegd in de PR[cite: 3].
5. **Afronding:** Na goedkeuring wordt de PR gemerged en de feature-branch direct verwijderd[cite: 5].

---

## Lokale configuratie

### Vereisten
* .NET SDK 8.0 of hoger
* Visual Studio 2022 of JetBrains Rider[cite: 3]

### Setup en synchronisatie
```bash
# Clone de repository
git clone <URL-VAN-REPO>
cd Receptenboek

# Koppel de upstream-bron (bij een fork)
git remote add upstream <URL-VAN-UPSTREAM-REPO>
git remote -v

# Valideer build en tests
dotnet build
dotnet test