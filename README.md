# System Rezerwacji – ASP.NET Core MVC

Webowa aplikacja rezerwacyjna umożliwiająca tworzenie i zarządzanie rezerwacjami usług.  
Projekt został stworzony w celach edukacyjnych z naciskiem na logikę biznesową, pracę z bazą danych oraz wykorzystanie LINQ w aplikacji ASP.NET Core.

---

## Główne funkcjonalności

- Rejestracja i logowanie użytkowników  
- Role użytkowników (*User / Admin*)  
- Tworzenie rezerwacji  
- Anulowanie rezerwacji  
- Walidacja konfliktów terminów  
- Panel administracyjny  
- Dashboard z podstawowymi danymi o rejestracjach  

---

## Technologie

- **ASP.NET Core (MVC)**
- **Entity Framework Core**
- **SQL Server**
- **LINQ**
- **ASP.NET Identity**
- **Bootstrap**
- Minimalna ilość JavaScript

Frontend został celowo ograniczony – głównym celem projektu była implementacja logiki backendowej oraz operacji na danych.

---

## Logika biznesowa

Projekt koncentruje się na:

- Filtrowaniu i przetwarzaniu danych przy użyciu **LINQ**
- Walidacji dostępności terminów przed zapisaniem rezerwacji
- Relacjach między encjami (*Users – Reservations – Services*)
- Separacji logiki biznesowej od kontrolerów

## System wykorzystuje ASP.NET Identity oraz role użytkowników (User / Admin).

W repozytorium część atrybutów:
```bash
[Authorize]
```

Została zakomentowana, aby umożliwić szybkie uruchomienie projektu bez dodatkowej konfiguracji kont użytkowników.

Po odkomentowaniu atrybutów aplikacja działa w pełnym trybie autoryzacji.
##  Uruchomienie projektu lokalnie

### 1️ Sklonuj repozytorium
```bash
git clone https://github.com/M-Mencel/E-Shop.git
cd E-Shop

```

### 2 Skonfiguruj połączenie z bazą danych

W pliku appsettings.json uzupełnij połączenie do swojego serwera SQL:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EshopDB;Trusted_Connection=True;"
}
```
### 3 Wykonaj migracje bazy danych
```bash
dotnet ef database update
```

### 4 Uruchom aplikacje
```bash
dotnet run
```
