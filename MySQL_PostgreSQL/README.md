# MySQL
```sh
mysql -u root -p
show databases;
create database asp;
use asp;
drop database asp;
exit
```

# PostgreSQL
```sh
sudo -u postgres psql
\l # show databases
create database asp;
\c asp; # connect to database
drop database asp;
exit/quit
```

# MSSQL
#### Preconditions:
* Install docker
* Install azure data studio
* Run Docker
```sh
docker pull microsoft/mssql-server-linux # install sqlserver
docker run -d --name MySQLServer -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Semafor4!' -p 1433:1433 microsoft/mssql-server-linux # run sqlserver
docker ps -a # show all containers running
docker ps -a # show all containers
docker stop CONTAINER_ID_HERE 
docker container start CONTAINER_ID_HERE 
```
#### Core
```sh
sudo npm install -g sql-cli # if you do not have it
mssql -u sa -p your_password
select name from sys.databases # show all databases
create database ASP; # create database
select * from master.INFORMATION_SCHEMA.TABLES;
USE ASP;
select * from INFORMATION_SCHEMA.TABLES;
drop database ASP;
exit
```


# PipEnv
```sh
pipenv shell # create env
pipenv --venv # locate current env
pipenv --where  # locate root of project 
ls /Users/user_name/.local/share/virtualenvs # show all environments
pipenv --rm # remove current env
rm -rf /Users/user_name/.local/share/virtualenvs/model-N-S4uBGU # remove certain env
pipenv shell # activate shell
exit # exit current env
```
