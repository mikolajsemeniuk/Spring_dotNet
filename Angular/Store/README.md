# Store
* init
* init store
* create reducer


### init
```sh
ng add @ngrx/store@latest
ng add @ngrx/store-devtools@latest
ng add @ngrx/schematics@latest
```
### init store
```sh
ng generate store State --root --state-path store --module app.module.ts
```
change `app.module.ts`

### create reducer
```sh
ng generate reducer store/Todo --reducers index.ts --flat false --skipTests=true
```
