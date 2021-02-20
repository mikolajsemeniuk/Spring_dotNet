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
