# Set up project
* Create new project
* Run project
* Optional

### Create new project
Create new project by typing in your terminal
```sh
dotnet new webapi -o api
```
### Run project
Run project by typing in your terminal
```sh
cd api
dotnet run
```
and go [here](https://localhost:5001/weatherforecast) to see results 
### Optional
Create new project with sln file by typing in your terminal
```sh
mkdir pro
cd pro
dotnet new sln
dotnet new webapi -o api
dotnet sln add api
cd api
dotnet run
```
and go [here](https://localhost:5001/weatherforecast) to see results 
