# AzurePizza01

AzurePizza01 är en RESTful web API för ett pizzabeställningssystem, byggt med **ASP.NET Core (.NET 9)** och **Entity Framework Core**. Projektet demonstrerar säker autentisering, rollbaserad auktorisation och moderna API-utvecklingsprinciper.

## Funktioner

- **User Authentication & Authorization**  
  - ASP.NET Core Identity med JWT tokens  
  - Roller: `Admin`, `PremiumUser`, `RegularUser`

- **Order & Menu Management**  
  - Endpoints för hantering av pizzabeställningar och menyobjekt

- **Swagger/OpenAPI**  
  - Interaktiv API-dokumentation med JWT-stöd

- **EF Core & SQL**  
  - Använder **Entity Framework Core** med **Azure SQL** eller **LocalDB**

## Åtkomst

API:t är driftsatt och tillgängligt på:  
https://azurepizzaapi-c9a9evdvf3d5gka8.swedencentral-01.azurewebsites.net/

## Användning

1. **Registrera dig och logga in** för att få en JWT token  
2. **Använd token** för autentiserade förfrågningar (via Swagger eller API-klient)  
3. **Utforska endpoints** för användare, beställningar och menyobjekt  

## Teknologier

- ASP.NET Core 9  
- Entity Framework Core  
- JWT Authentication  
- Swagger/OpenAPI  
