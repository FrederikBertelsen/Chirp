# Chirp
Repo for groupwork for the course "Analysis, Design and Software Architecture(BSANDSA1KU)" at ITU in fall 2023.

[rapport.md](docs%2Frapport.md)

## How to make _Chirp!_ work locally
### SQLite
First, clone the repository from github to local storage. Do so however you prefer.
Navigate to  */Chirp/src/Chirp.Web*, and run:
```
dotnet run
```
This starts Chirp. A localhost link will be written in the terminal. Follow it, and you are now on a locally hosted Chirp.



### SQL Server
> We ended with SQLite as we ran out of credits. For the last commit that works with SQL Server, see: [commit](https://github.com/ITU-BDSA23-GROUP24/Chirp/tree/9e05008fab7e9c4d4fc7709a31296f601a372c11).

First, clone the repository from github to local storage. Do so however you prefer.
Then start a docker container with a SQL Server database
```
 docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=%Password123' -p 1433:1433 --name sql_server_container -d mcr.microsoft.com/mssql/server
```
Next, navigate to */Chirp/src/Chirp.Web*. Here you set the user secret required to connect you to your database:
```
dotnet user-secrets set "ConnectionStrings:Chirp" "Server=tcp:localhost,1433;Initial Catalog=Chirp SQL Database;Persist Security Info=False;User ID=sa;Password=%Password123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;" --id id
```
Both of these are run to make the SQL database work. Stay in */Chirp/src/Chirp.Web*, and run:
```
dotnet run
```
This starts Chirp. A localhost link will be written in the terminal. Follow it, and you are now on a locally hosted Chirp.

### How to run test suite locally
Run command in repository root:
```
dotnet test
```
